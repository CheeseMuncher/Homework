using ShoppingCart.Models;

namespace ShoppingCart.Interfaces
{
    public interface ICartItemDiscountCalculator
    {
        decimal CalculateLineTotal(Product item, int quantity, Discount discount);
    }
}