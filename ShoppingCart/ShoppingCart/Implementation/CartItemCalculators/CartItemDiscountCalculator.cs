using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Implementation
{
    public class CartItemDiscountCalculator : ICartItemDiscountCalculator
    {
        private readonly Dictionary<DiscountType, ICartItemDiscountCalculator> _calculators;

        public CartItemDiscountCalculator(Dictionary<DiscountType, ICartItemDiscountCalculator> calculators)
        {
            _calculators = calculators;
        }

        public decimal CalculateLineTotal(Product item, int quantity, Discount discount)
            => discount is null || !_calculators.TryGetValue(discount.DiscountType, out var calculator)
            ? (this as ICartItemDiscountCalculator).CalculateWithoutDiscount(quantity, item.Price)
            : calculator.CalculateLineTotal(item, quantity, discount);
    }
}