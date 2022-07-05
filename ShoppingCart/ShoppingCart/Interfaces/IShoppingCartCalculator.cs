using ShoppingCart.Models;

namespace ShoppingCart.Interfaces
{
    public interface IShoppingCartCalculator
    {
        double Total(ShoppingCartRequest request);
    }
}