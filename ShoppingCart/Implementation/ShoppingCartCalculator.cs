using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Implementation
{
    public class ShoppingCartCalculator : IShoppingCartCalculator
    {
        private readonly IRepository<int, Product> _productStore;
        private readonly IShippingCalculator _shippingCalculator;

        public ShoppingCartCalculator(IShippingCalculator shippingCalculator, IRepository<int, Product> productStore)
        {
            _productStore = productStore;
            _shippingCalculator = shippingCalculator;
        }

        public double Total(IList<CartItem> cart)
        {
            if (cart == null || cart.Count == 0) return 0;

            double runningTotal = 0;
            foreach (var item in cart)
            {
                var product = _productStore.Get(item.ProductId);
                if (product != null)
                {
                    runningTotal += item.UnitQuantity * product.Price;
                }
            }

            return runningTotal + _shippingCalculator.CalcShipping(runningTotal);
        }
    }
}