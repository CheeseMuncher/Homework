using ShoppingCart.Interfaces;

namespace ShoppingCart.Implementation
{
    public class ShippingCalculator : IShippingCalculator
    {
        // TODO this should come from config or be provided by a ShippingThresholdRepository
        private (decimal value, decimal price)[] _shippingThresholds = new[] { (20.0m, 7.0m), (40.0m, 5.0m) };

        public decimal CalcShipping(decimal cartTotal)
        {
            foreach (var threshold in _shippingThresholds)
                if (cartTotal < threshold.value)
                    return threshold.price;

            return 0;
        }
    }
}