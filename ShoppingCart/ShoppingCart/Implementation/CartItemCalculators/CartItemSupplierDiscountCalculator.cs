using ShoppingCart.Models;
using System;

namespace ShoppingCart.Interfaces
{
    public class CartItemSupplierDiscountCalculator : ICartItemDiscountCalculator
    {
        public decimal CalculateLineTotal(Product item, int quantity, Discount discount)
            => (this as ICartItemDiscountCalculator).CalculateLineTotal(
                quantity,
                item.Price,
                discount.DiscountPercentage,
                ApplySupplierDiscount(item.Supplier, discount.DiscountedSupplier));

        private bool ApplySupplierDiscount(string itemSupplier, string discountSupplier)
            => itemSupplier.Equals(discountSupplier, StringComparison.InvariantCultureIgnoreCase);
    }
}