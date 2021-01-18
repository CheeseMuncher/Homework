using NUnit.Framework;
using ShoppingCart.Implementation;
using ShoppingCart.Interfaces;
using Shouldly;

namespace ShoppingCart.Tests
{
    public class ShippingCalculatorTests
    {
        private readonly IShippingCalculator _sut = new ShippingCalculator();

        [TestCase(0, 7)]
        [TestCase(1, 7)]
        [TestCase(19.99, 7)]
        [TestCase(20, 5)]
        [TestCase(39.99, 5)]
        [TestCase(40, 0)]
        [TestCase(10000000, 0)]
        public void CalcShipping_AppliesCorrectShippingBelow(decimal cartTotal, decimal expected)
        {
            var result = _sut.CalcShipping(cartTotal);

            result.ShouldBe(expected);
        }
    }
}