using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System;
using System.Linq;

namespace ShoppingCart.Implementation
{
    public class CartItemCategoryDiscountCalculator : ICartItemDiscountCalculator
    {
        public decimal CalculateLineTotal(Product item, int quantity, Discount discount)
            => (this as ICartItemDiscountCalculator).CalculateLineTotal(
                quantity,
                item.Price,
                discount.DiscountPercentage,
                ApplyCategoryDiscount(item.Categories, discount.DiscountedCategory));

        private bool ApplyCategoryDiscount(string[] itemCategories, string discountCategory)
            => itemCategories.Any(cat => cat.Equals(discountCategory, StringComparison.InvariantCultureIgnoreCase));
    }
}