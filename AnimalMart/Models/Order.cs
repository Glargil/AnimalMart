public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<OrderLine> Lines { get; set; } = new();
}

public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}