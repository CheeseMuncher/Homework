using ShoppingCart.Models;

namespace ShoppingCart.Interfaces
{
    public interface ICartItemDiscountCalculator
    {
        decimal CalculateLineTotal(Product item, int quantity, Discount discount);

        decimal CalculateWithoutDiscount(int quantity, decimal price) => quantity * price;

        decimal CalculateWithDiscount(int quantity, decimal price, decimal percentage) => quantity * price * (1 - percentage / 100);

        decimal CalculateLineTotal(int quantity, decimal price, decimal percentage, bool applyDiscount) => applyDiscount
            ? CalculateWithDiscount(quantity, price, percentage)
            : CalculateWithoutDiscount(quantity, price);
    }
}