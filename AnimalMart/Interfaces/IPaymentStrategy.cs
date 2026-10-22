namespace AnimalMart.Interfaces;
public interface IPaymentStrategy
{
    // One operation, many interchangeable implementations.
    public string Pay(decimal amount);
}