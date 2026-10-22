using AnimalMart.Interfaces;
namespace AnimalMart.Strategies
{
    // Proof of concept: stands in for a real MobilePay redirect/app-switch.
    public class MobilePayPaymentStrategy : IPaymentStrategy
    {
        public string Pay(decimal amount)
        {
            return $"You paid {amount:0.00} kr. with MobilePay.";
        }
    }
}