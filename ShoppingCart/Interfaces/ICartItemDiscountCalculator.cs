using ShoppingCart.Models;

namespace ShoppingCart.Interfaces
{
    public interface ICartItemDiscountCalculator
    {
        double CalculateLineTotal(Product item, int quantity, Discount discount);
    }
}