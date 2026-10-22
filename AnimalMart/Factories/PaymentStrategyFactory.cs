using AnimalMart.Interfaces;
using AnimalMart.Models;
using AnimalMart.Strategies;

namespace AnimalMart.Factories
{
    // Maps the user's choice to a concrete strategy, so no page or
    // service needs "new CardPaymentStrategy()" scattered around.
    public class PaymentStrategyFactory
    {
        public IPaymentStrategy Create(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Card => new CardPaymentStrategy(),
                PaymentMethod.MobilePay => new MobilePayPaymentStrategy(),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(method), $"Unknown payment method: {method}"),
            };
        }
    }
}