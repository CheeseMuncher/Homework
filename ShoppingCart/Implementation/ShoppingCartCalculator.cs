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

        public ShoppingCartCalculator(
            IShippingCalculator shippingCalculator,
            IRepository<int, Product> productRepository)
        {
            _shippingCalculator = shippingCalculator ?? throw new ArgumentException(nameof(shippingCalculator));
            _productRepository = productRepository ?? throw new ArgumentException(nameof(productRepository));
        }

        public double Total(ShoppingCartRequest request)
        {
            if (request == null || !request.CartItems.Any()) return 0;

            double runningTotal = 0;
            foreach (var item in request.CartItems)
            {
                var product = _productRepository.Get(item.ProductId);
                if (product != null)
                {
                    runningTotal += item.UnitQuantity * product.Price;
                }
            }

            return runningTotal + _shippingCalculator.CalcShipping(runningTotal);
        }
    }
}