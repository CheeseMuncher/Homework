using NUnit.Framework;
using ShoppingCart.Data;
using ShoppingCart.Implementation;
using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Tests
{
    public class ShoppingCartCalculatorTests
    {
        private ProductRepository _productRepository;

        [SetUp]
        public void Init()
        {
            _productRepository = new ProductRepository();
        }

        [Test]
        public void WithSmallCart_CheckCalculation()
        {
            var carItem1 = new CartItem { ProductId = 2, UnitQuantity = 1 };
            var carItem2 = new CartItem { ProductId = 4, UnitQuantity = 1 };

            var cart = new List<CartItem> { carItem1, carItem2 };

            var calc = new ShoppingCartCalculator(new ShippingCalculator(), _productRepository);
            double total = calc.Total(cart);

            Assert.AreEqual(1004, total);
        }

        [Test]
        public void WithOneItem_CheckCalculation()
        {
            var carItem1 = new CartItem { ProductId = 2, UnitQuantity = 1 };

            var cart = new List<CartItem> { carItem1 };

            var calc = new ShoppingCartCalculator(new ShippingCalculator(), _productRepository);
            double total = calc.Total(cart);

            Assert.AreEqual(11, total);
        }
    }
}