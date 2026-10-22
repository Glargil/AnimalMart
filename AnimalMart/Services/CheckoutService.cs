using AnimalMart.Interfaces;
namespace AnimalMart.Services
{
    // The Strategy pattern's context. It works against the
    // IPaymentStrategy interface only and never knows which
    // concrete payment method is behind it.
    public class CheckoutService
    {
        private IPaymentStrategy? _strategy;

        public void SetStrategy(IPaymentStrategy strategy)
        {
            _strategy = strategy;
        }

        public string Checkout(ShoppingCart cart)
        {
            if (cart.Lines.Count == 0)
                throw new InvalidOperationException("Cannot check out an empty cart.");

            if (_strategy is null)
                throw new InvalidOperationException("No payment method selected.");

            // The amount is computed on the server from the cart.
            // The client never sends a price, so it cannot be tampered with.
            return _strategy.Pay(cart.TotalPrice);
        }
    }
}
