using ShoppingCart.Models;

namespace ShoppingCart.Interfaces
{
    public interface IShoppingCartCalculator
    {
        decimal Total(ShoppingCartRequest request);
    }
}