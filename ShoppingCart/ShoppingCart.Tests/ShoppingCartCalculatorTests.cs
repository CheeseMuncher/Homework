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

        private Dictionary<string, Discount> _testDiscounts = new Dictionary<string, Discount>
        {
            ["BACKTOSCHOOL2019"] = new Discount { DiscountType = DiscountType.Supplier, DiscountedSupplier = "Apple", DiscountPercentage = 15 },
            ["APPLE5"] = new Discount { DiscountType = DiscountType.Supplier, DiscountedSupplier = "Apple", DiscountPercentage = 5 },
            ["LOVEMYMUSIC"] = new Discount { DiscountType = DiscountType.Category, DiscountedCategory = "Audio", DiscountPercentage = 20 },
            ["AUDIO10"] = new Discount { DiscountType = DiscountType.Category, DiscountedCategory = "Audio", DiscountPercentage = 10 },
            ["FREESHIPPING"] = new Discount { DiscountType = DiscountType.Shipping }
        };

        private Mock<IRepository<int, Product>> _mockProductRepository = new Mock<IRepository<int, Product>>();
        private Mock<IRepository<string, Discount>> _mockDiscountRepository = new Mock<IRepository<string, Discount>>();
        private ShoppingCartCalculator _sut;

        [SetUp]
        public void Init()
        {
            _mockProductRepository
                .Setup(repo => repo.Get(It.IsAny<int>()))
                .Returns((int id) => _testProducts.FirstOrDefault(p => p.Id == id));

            _mockDiscountRepository
                .Setup(repo => repo.Get(It.IsAny<string>()))
                .Returns((string id) => _testDiscounts.TryGetValue(id, out var value) ? value : null);

            _sut = new ShoppingCartCalculator(new ShippingCalculator(), _mockProductRepository.Object, _mockDiscountRepository.Object);
        }

        [Test]
        public void CorrectResult_WithSingleItemAndShipping()
        {
            var cart = new List<CartItem> { new CartItem { ProductId = 2, UnitQuantity = 1 } };
            var request = new ShoppingCartRequest { CartItems = cart };

            var total = _sut.Total(request);

            total.ShouldBe(11);
        }

        [Test]
        public void CorrectResult_WithMultipleQuantity()
        {
            var cart = new List<CartItem> { new CartItem { ProductId = 3, UnitQuantity = 2 } };
            var request = new ShoppingCartRequest { CartItems = cart };

            var total = _sut.Total(request);

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
            var request = new ShoppingCartRequest { CartItems = cart };

            var total = _sut.Total(request);

            total.ShouldBe(1004);
        }

        [Test]
        public void CorrectResult_WithSingleShippingDiscount()
        {
            var cart = new List<CartItem> { new CartItem { ProductId = 2, UnitQuantity = 1 } };
            var request = new ShoppingCartRequest { CouponCode = "FREESHIPPING", CartItems = cart };

            var total = _sut.Total(request);

            total.ShouldBe(4);
        }

        [TestCase("AUDIO10", 1122)]
        [TestCase("APPLE5", 1122.8)]
        [TestCase("FREESHIPPING", 1124)]
        [TestCase("TRYINGMYLUCK", 1124)]
        public void CorrectResult_WithTestCaseCart(string couponCode, decimal expectedTotal)
        {
            var cart = GetTestCaseCart(couponCode);

            var result = _sut.Total(cart);

            result.ShouldBe(expectedTotal);
        }

        private ShoppingCartRequest GetTestCaseCart(string couponCode)
        {
            return new ShoppingCartRequest
            {
                CouponCode = couponCode,
                CartItems = new List<CartItem>
                {
                    new CartItem { ProductId = 1, UnitQuantity = 2 },
                    new CartItem { ProductId = 2, UnitQuantity = 1 },
                    new CartItem { ProductId = 3, UnitQuantity = 1 },
                    new CartItem { ProductId = 4, UnitQuantity = 1 }
                }
            };
        }
    }
}