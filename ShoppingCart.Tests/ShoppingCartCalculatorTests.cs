using Moq;
using NUnit.Framework;
using ShoppingCart.Implementation;
using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Tests
{
    public class ShoppingCartCalculatorTests
    {
        private Product[] _testProducts = new[]
        {
            new Product { Id = 1, Name = "Headphones", Price = 10, Supplier = "Apple", Categories = new[] { "Accessory", "Electronic", "Audio" } },
            new Product { Id = 2, Name = "USB Cable", Price = 4, Supplier = "Apple", Categories = new[] { "Accessory" } },
            new Product { Id = 3, Name = "Monitor", Price = 100, Supplier = "HP", Categories = new[] { "Electronic" } },
            new Product { Id = 4, Name = "Laptop", Price = 1000, Supplier = "Dell", Categories = new[] { "Electronic" } },
            new Product { Id = 5, Name = "iPad", Price = 220, Supplier = "Apple", Categories = new[] { "Electronic" } }
        };

        private Mock<IRepository<int, Product>> _mockProductRepository = new Mock<IRepository<int, Product>>();
        private ShoppingCartCalculator _sut;

        [SetUp]
        public void Init()
        {
            _mockProductRepository
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns((int id) => _testProducts.FirstOrDefault(p => p.Id == id));

            _sut = new ShoppingCartCalculator(new ShippingCalculator(), _mockProductRepository.Object);
        }

        [Test]
        public void CorrectResult_WithSingleItemAndShipping()
        {
            var cart = new List<CartItem> { new CartItem { ProductId = 2, UnitQuantity = 1 } };

            var total = _sut.Total(cart);

            total.ShouldBe(11);
        }

        [Test]
        public void CorrectResult_WithMultipleQuantity()
        {
            var cart = new List<CartItem> { new CartItem { ProductId = 3, UnitQuantity = 2 } };

            var total = _sut.Total(cart);

            total.ShouldBe(200);
        }

        [Test]
        public void CorrectResult_WithMultipleCartItems()
        {
            var cart = new List<CartItem>
            {
                new CartItem { ProductId = 2, UnitQuantity = 1 },
                new CartItem { ProductId = 4, UnitQuantity = 1 }
            };

            var total = _sut.Total(cart);

            total.ShouldBe(1004);
        }
    }
}