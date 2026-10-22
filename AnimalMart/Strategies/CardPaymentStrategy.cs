using AnimalMart.Interfaces;
namespace AnimalMart.Strategies
{
    // Proof of concept: no real PSP call and no card data is ever collected.
    public class CardPaymentStrategy : IPaymentStrategy
    {
        public string Pay(decimal amount)
        {
            return $"You paid {amount:0.00} kr. with Card.";
        }
    }
}