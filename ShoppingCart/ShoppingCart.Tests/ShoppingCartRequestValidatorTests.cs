using Moq;
using NUnit.Framework;
using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using ShoppingCart.Validation;
using Shouldly;
using System.Linq;

namespace ShoppingCart.Tests
{
    public class ShoppingCartRequestValidatorTests
    {
        private readonly Mock<IRepository<int, Product>> _mockProductRepository = new Mock<IRepository<int, Product>>();
        private readonly Mock<IRepository<string, Discount>> _mockDiscountRepository = new Mock<IRepository<string, Discount>>();
        private ShoppingCartRequestValidator _sut;

        [SetUp]
        public void Setup()
        {
            _mockDiscountRepository.Setup(repo => repo.Get("FREESHIPPING")).Returns(new Discount { DiscountType = DiscountType.Shipping });
            _sut = new ShoppingCartRequestValidator(_mockProductRepository.Object, _mockDiscountRepository.Object);
        }

        [Test]
        public void Validate_ReturnsValid_IfValidRequestSupplied()
        {
            var request = GetValidRequest();

            var result = _sut.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [Test]
        public void Validate_ReturnsValid_IfZeroQuantitySupplied()
        {
            var request = GetValidRequest();
            request.CartItems = new[] { new CartItem { ProductId = 123, UnitQuantity = 0 } };

            var result = _sut.Validate(request);

            result.IsValid.ShouldBeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("AUDIO10 FREESHIPPING")]
        public void Validate_ReturnsInvalid_IfCouponCodeNullOrEmptyOrMultiple(string input)
        {
            var request = GetValidRequest();
            request.CouponCode = input;

            var result = _sut.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldBe(1);
            result.Errors.Single().PropertyName.ShouldBe(nameof(ShoppingCartRequest.CouponCode));
            result.Errors.Single().ErrorMessage.ShouldBe("API takes exactly one Coupon Code");
        }

        [TestCase("!")]
        [TestCase("abc#")]
        [TestCase("123;")]
        public void Validate_ReturnsInvalid_IfCouponCodeInvalidRegex(string input)
        {
            var request = GetValidRequest();
            request.CouponCode = input;

            var result = _sut.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldBe(1);
            result.Errors.Single().PropertyName.ShouldBe(nameof(ShoppingCartRequest.CouponCode));
            result.Errors.Single().ErrorMessage.ShouldBe("API takes exactly one alphanumeric Coupon Code");
        }

        [TestCase(-1)]
        [TestCase(-10)]
        public void Validate_ReturnsInvalid_IfProductQuantityNoSupplied(int input)
        {
            var request = GetValidRequest();
            request.CartItems = new[] { new CartItem { ProductId = 123, UnitQuantity = input } };

            var result = _sut.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldBe(1);
            result.Errors.Single().PropertyName.ShouldContain(nameof(CartItem.UnitQuantity));
            result.Errors.Single().ErrorMessage.ShouldBe("API takes positive quantities");
        }

        [Test]
        public void Validate_ReturnsInvalid_IfProductNotFound()
        {
            var request = GetValidRequest();
            request.CartItems = new[] { new CartItem { ProductId = 456, UnitQuantity = 1 } };

            var result = _sut.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldBe(1);
            result.Errors.Single().PropertyName.ShouldContain(nameof(CartItem.ProductId));
            result.Errors.Single().ErrorMessage.ShouldBe("Product not found");
        }

        [Test]
        public void Validate_ReturnsInvalid_IfCouponCodeNotFound()
        {
            var request = GetValidRequest();
            request.CouponCode = "FakeCode";

            var result = _sut.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldBe(1);
            result.Errors.Single().PropertyName.ShouldContain(nameof(ShoppingCartRequest.CouponCode));
            result.Errors.Single().ErrorMessage.ShouldBe("Coupon code not found");
        }

        [Test]
        public void Validate_ReturnsMultipleErrorMessages()
        {
            var id1 = 123;
            var id2 = 456;
            _mockProductRepository
                .Setup(repo => repo.Get(id1))
                .Returns(new Product { Id = id1, Name = $"Product {id1}" });
            _mockProductRepository
                .Setup(repo => repo.Get(id2))
                .Returns(new Product { Id = id2, Name = $"Product {id2}" });

            var request = new ShoppingCartRequest
            {
                CouponCode = "***",
                CartItems = new[]
                {
                    new CartItem { ProductId = id1, UnitQuantity = 1 },
                    new CartItem { ProductId = id2, UnitQuantity = -1 },
                    new CartItem { ProductId = 789, UnitQuantity = 1 }
                }
            };

            var result = _sut.Validate(request);

            result.IsValid.ShouldBeFalse();
            result.Errors.Count.ShouldBe(3);
            result.Errors.ShouldContain(e => e.ErrorMessage == ValidationMessages.ExactlyOneCodeAlphanumeric);
            result.Errors.ShouldContain(e => e.ErrorMessage == ValidationMessages.PositiveQuantities);
            result.Errors.ShouldContain(e => e.ErrorMessage == ValidationMessages.ProductNotFound);
        }

        private ShoppingCartRequest GetValidRequest()
        {
            var id = 123;
            _mockProductRepository
                .Setup(repo => repo.Get(id))
                .Returns(new Product { Id = id, Name = $"Product {id}" });
            return new ShoppingCartRequest
            {
                CouponCode = "FREESHIPPING",
                CartItems = new[]
                {
                    new CartItem  { ProductId = id, UnitQuantity = 1 }
                }
            };
        }
    }
}