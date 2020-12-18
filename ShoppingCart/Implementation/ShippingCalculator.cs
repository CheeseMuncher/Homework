using ShoppingCart.Interfaces;

namespace ShoppingCart.Implementation
{
    public class ShippingCalculator : IShippingCalculator
    {
        // TODO this should come from config or be provided by a ShippingThresholdRepository
        private (double value, double price)[] _shippingThresholds = new[] { (20.0, 7.0), (40.0, 5.0) };

        public double CalcShipping(double cartTotal)
        {
            foreach (var threshold in _shippingThresholds)
                if (cartTotal < threshold.value)
                    return threshold.price;

            return 0;
        }
    }
}