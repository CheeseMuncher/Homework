using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System;
using System.Linq;

namespace ShoppingCart.Implementation
{
    public class CartItemDiscountCalculator : ICartItemDiscountCalculator
    {
        public decimal CalculateLineTotal(Product item, int quantity, Discount discount)
        {
            switch (discount?.DiscountType)
            {
                case DiscountType.Supplier:
                    return CalculateLineTotal(quantity, item.Price, discount.DiscountPercentage, ApplySupplierDiscount(item.Supplier, discount.DiscountedSupplier));

                case DiscountType.Category:
                    return CalculateLineTotal(quantity, item.Price, discount.DiscountPercentage, ApplyCategoryDiscount(item.Categories, discount.DiscountedCategory));

                case DiscountType.Shipping:
                default:
                    return CalculateWithoutDiscount(quantity, item.Price);
            }
        }

        private bool ApplySupplierDiscount(string itemSupplier, string discountSupplier) => itemSupplier.Equals(discountSupplier, StringComparison.InvariantCultureIgnoreCase);

        private bool ApplyCategoryDiscount(string[] itemCategories, string discountCategory) => itemCategories.Any(cat => cat.Equals(discountCategory, StringComparison.InvariantCultureIgnoreCase));

        private decimal CalculateWithoutDiscount(int quantity, decimal price) => quantity * price;

        private decimal CalculateWithDiscount(int quantity, decimal price, decimal percentage) => quantity * price * (1 - percentage / 100);

        private decimal CalculateLineTotal(int quantity, decimal price, decimal percentage, bool applyDiscount)
            => applyDiscount ? CalculateWithDiscount(quantity, price, percentage) : CalculateWithoutDiscount(quantity, price);
    }
}