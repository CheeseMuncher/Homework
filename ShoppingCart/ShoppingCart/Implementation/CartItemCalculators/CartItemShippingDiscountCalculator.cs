using ShoppingCart.Interfaces;
using ShoppingCart.Models;

namespace ShoppingCart.Implementation
{
    public class CartItemShippingDiscountCalculator : ICartItemDiscountCalculator
    {
        public decimal CalculateLineTotal(Product item, int quantity, Discount discount)
            => (this as ICartItemDiscountCalculator).CalculateWithoutDiscount(quantity, item.Price);
    }
}