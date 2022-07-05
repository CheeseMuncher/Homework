using System.Collections.Generic;
using ShoppingCart.Interfaces;
using ShoppingCart.Pocos;

namespace ShoppingCart.Implementation
{
    public class Calculator
    {
        private readonly IRepository<Product> _productStore;
        private readonly IShippingCalculator _shippingCalculator;

        public Calculator(IShippingCalculator shippingCalculator, IRepository<Product> productStore)
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