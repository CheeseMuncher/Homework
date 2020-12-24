namespace ShoppingCart.Interfaces
{
    public interface IShippingCalculator
    {
        double CalcShipping(double cartTotal);
    }
}