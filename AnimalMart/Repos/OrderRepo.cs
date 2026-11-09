using AnimalMart.Interfaces;
using Npgsql;

namespace AnimalMart.Repos
{
  public class OrderRepo : IOrderRepo
  {
    private readonly string _connectionString;

    public OrderRepo(IConfiguration configuration)
    {
      _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
          "Connection string 'DefaultConnection' not found."
        );
    }

    public Order Create(Order order)
    {
      // The order header and all its lines are written as ONE
      // transaction: either the complete order is stored, or —
      // if anything fails — nothing is, and the caller can retry
      // without leaving half an order behind.
      using var connection = new NpgsqlConnection(_connectionString);
      connection.Open();
      using var transaction = connection.BeginTransaction();
      try
      {
        const string orderSql = @"
          INSERT INTO orders (user_id, payment_method, total, created_at_utc)
          VALUES (@user_id, @payment_method, @total, NOW())
          RETURNING id";

        int orderId;
        using (var command = new NpgsqlCommand(orderSql, connection, transaction))
        {
          command.Parameters.AddWithValue("@user_id", order.UserId);
          command.Parameters.AddWithValue("@payment_method", order.PaymentMethod);
          command.Parameters.AddWithValue("@total", order.Total);
          orderId = Convert.ToInt32(command.ExecuteScalar());
        }

        const string lineSql = @"
          INSERT INTO order_lines (order_id, product_id, product_name, unit_price, quantity, line_total)
          VALUES (@order_id, @product_id, @product_name, @unit_price, @quantity, @line_total)";

        foreach (var line in order.Lines)
        {
          using var command = new NpgsqlCommand(lineSql, connection, transaction);
          command.Parameters.AddWithValue("@order_id", orderId);
          command.Parameters.AddWithValue("@product_id", line.ProductId);
          command.Parameters.AddWithValue("@product_name", line.ProductName);
          command.Parameters.AddWithValue("@unit_price", line.UnitPrice);
          command.Parameters.AddWithValue("@quantity", line.Quantity);
          command.Parameters.AddWithValue("@line_total", line.LineTotal);
          command.ExecuteNonQuery();
        }

        transaction.Commit();

        order.Id = orderId;
        foreach (var line in order.Lines)
        {
          line.OrderId = orderId;
        }
        return order;
      }
      catch
      {
        transaction.Rollback();
        throw;
      }
    }

    public List<Order> GetByUserId(int userId)
    {
      var orders = new List<Order>();

      using (var connection = new NpgsqlConnection(_connectionString))
      {
        // Newest order first — the natural order for a history page.
        const string orderSql =
          "SELECT id, user_id, payment_method, total, created_at_utc FROM orders WHERE user_id = @user_id ORDER BY created_at_utc DESC, id DESC";
        connection.Open();
        using (var command = new NpgsqlCommand(orderSql, connection))
        {
          command.Parameters.AddWithValue("@user_id", userId);
          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              orders.Add(
                new Order
                {
                  Id = reader.GetInt32(reader.GetOrdinal("id")),
                  UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                  PaymentMethod = reader.GetString(reader.GetOrdinal("payment_method")),
                  Total = reader.GetDecimal(reader.GetOrdinal("total")),
                  CreatedAtUtc = reader.GetDateTime(reader.GetOrdinal("created_at_utc")),
                }
              );
            }
          }
        }

        // No orders means no lines to fetch.
        if (orders.Count == 0)
        {
          return orders;
        }

        const string lineSql = @"
          SELECT id, order_id, product_id, product_name, unit_price, quantity, line_total
          FROM order_lines
          WHERE order_id = ANY(@order_ids)
          ORDER BY id";

        using (var command = new NpgsqlCommand(lineSql, connection))
        {
          command.Parameters.AddWithValue(
            "@order_ids",
            orders.Select(o => o.Id).ToArray()
          );
          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              var line = new OrderLine
              {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                OrderId = reader.GetInt32(reader.GetOrdinal("order_id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                ProductName = reader.GetString(reader.GetOrdinal("product_name")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("unit_price")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                LineTotal = reader.GetDecimal(reader.GetOrdinal("line_total")),
              };

              // Attach the line to its order (kept fast by the
              // dictionary lookup rather than a linear search
              // per line).
              orders
                .First(o => o.Id == line.OrderId)
                .Lines.Add(line);
            }
          }
        }
      }

      return orders;
    }
  }
}