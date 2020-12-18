using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System;
using System.Linq;

namespace ShoppingCart.Implementation
{
    public class CartItemDiscountCalculator : ICartItemDiscountCalculator
    {
        public double CalculateLineTotal(Product item, int quantity, Discount discount)
        {
            switch (discount?.DiscountType)
            {
                case DiscountType.Supplier:
                    return item.Supplier.Equals(discount.DiscountedSupplier, StringComparison.InvariantCultureIgnoreCase)
                        ? quantity * item.Price * (1 - discount.DiscountPercentage / 100)
                        : quantity * item.Price;

                case DiscountType.Category:
                    return item.Categories.Any(cat => cat.Equals(discount.DiscountedCategory, StringComparison.InvariantCultureIgnoreCase))
                        ? quantity * item.Price * (1 - discount.DiscountPercentage / 100)
                        : quantity * item.Price;

                case DiscountType.Shipping:
                default:
                    return quantity * item.Price;
            }
        }
    }
}