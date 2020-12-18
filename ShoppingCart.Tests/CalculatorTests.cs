using NUnit.Framework;
using ShoppingCart.Data;
using ShoppingCart.Implementation;
using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Tests
{
    public class Tests
    {
        private ProductRepository _productRepository;

        [SetUp]
        public void Init()
        {
            var usbCable = new Product { Id = 1, Name = "USB Cable", WholesalePrice = 2, Price = 4 };
            var laptop = new Product { Id = 2, Name = "Laptop", WholesalePrice = 800, Price = 1000 };
            var iPad = new Product { Id = 3, Name = "iPad", WholesalePrice = 180, Price = 220 };

            _productRepository = new ProductRepository();
            _productRepository.Add(usbCable.Id, usbCable);
            _productRepository.Add(laptop.Id, laptop);
            _productRepository.Add(iPad.Id, iPad);
        }

        [Test]
        public void WithSmallCart_CheckCalculation()
        {
            var carItem1 = new CartItem { ProductId = 1, UnitQuantity = 1 };
            var carItem2 = new CartItem { ProductId = 2, UnitQuantity = 1 };

            var cart = new List<CartItem> { carItem1, carItem2 };

            var calc = new Calculator(new ShippingCalculator(), _productRepository);
            double total = calc.Total(cart);

            Assert.AreEqual(1004, total);
        }

        [Test]
        public void WithOneItem_CheckCalculation()
        {
            var carItem1 = new CartItem { ProductId = 1, UnitQuantity = 1 };

            var cart = new List<CartItem> { carItem1 };

            var calc = new Calculator(new ShippingCalculator(), _productRepository);
            double total = calc.Total(cart);

            Assert.AreEqual(11, total);
        }
    }
}