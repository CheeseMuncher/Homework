using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Interfaces
{
    public interface IShoppingCartCalculator
    {
        double Total(IList<CartItem> cart);
    }
}