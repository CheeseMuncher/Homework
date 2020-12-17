using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System;

namespace ShoppingCart.Implementation
{
    public class CartItemDiscountCalculator : ICartItemDiscountCalculator
    {
        public double CalculateLineTotal(Product item, int quantity, Discount discount)
        {
            switch (discount.DiscountType)
            {
                case DiscountType.Supplier:
                    return discount.SupplierDiscount.Equals(item.Supplier, StringComparison.InvariantCultureIgnoreCase)
                        ? quantity * item.Price * (1 - discount.DiscountPercentage) / 100
                        : quantity * item.Price;

                case DiscountType.Category:
                    return discount.CategoryDiscount.Equals(item.Category, StringComparison.InvariantCultureIgnoreCase)
                        ? quantity * item.Price * (1 - discount.DiscountPercentage) / 100
                        : quantity * item.Price;

                case DiscountType.Shipping:
                default:
                    return quantity * item.Price;
            }
        }
    }
}