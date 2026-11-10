namespace AnimalMart.Interfaces
{
    public interface IOrderRepo
    {
        /// <summary>
        /// Persists a completed order (header + lines) in one transaction
        /// and returns it with the database-generated id set.
        /// </summary>
        Order Create(Order order);

        /// <summary>
        /// All orders for one user, newest first, with their lines.
        /// </summary>
        List<Order> GetByUserId(int userId);
    }
}