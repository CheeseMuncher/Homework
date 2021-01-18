namespace ShoppingCart.Interfaces
{
    public interface IShippingCalculator
    {
        decimal CalcShipping(decimal cartTotal);
    }
}