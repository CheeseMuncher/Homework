using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System;
using System.Linq;

namespace ShoppingCart.Implementation
{
    public class ShoppingCartCalculator : IShoppingCartCalculator
    {
        private readonly IShippingCalculator _shippingCalculator;
        private readonly IRepository<int, Product> _productRepository;
        private readonly IRepository<string, Discount> _discountRepository;
        private readonly CartItemDiscountCalculator cartItemCalculator = new CartItemDiscountCalculator();

        public ShoppingCartCalculator(
            IShippingCalculator shippingCalculator,
            IRepository<int, Product> productRepository,
            IRepository<string, Discount> discountRepository)
        {
            _shippingCalculator = shippingCalculator ?? throw new ArgumentException(nameof(shippingCalculator));
            _productRepository = productRepository ?? throw new ArgumentException(nameof(productRepository));
            _discountRepository = discountRepository ?? throw new ArgumentException(nameof(discountRepository));
        }

        public double Total(ShoppingCartRequest request)
        {
            if (request.CartItems == null || !request.CartItems.Any()) return 0;
            var discount = _discountRepository.Get(request.CouponCode ?? "");

            var total = request.CartItems
                .Select(item => (Product: _productRepository.Get(item.ProductId), item.UnitQuantity))
                .Sum((item) => cartItemCalculator.CalculateLineTotal(item.Product, item.UnitQuantity, discount));

            return discount?.DiscountType == DiscountType.Shipping ? total : total + _shippingCalculator.CalcShipping(total);
        }
    }
}