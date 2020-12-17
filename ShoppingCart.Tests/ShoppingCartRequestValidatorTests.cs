﻿using NUnit.Framework;
using ShoppingCart.Models;
using Shouldly;
using System.Linq;

namespace ShoppingCart.Tests
{
    public class ShoppingCartRequestValidatorTests
    {
        private ShoppingCartRequestValidator _sut = new ShoppingCartRequestValidator();

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

        private ShoppingCartRequest GetValidRequest()
        {
            return new ShoppingCartRequest
            {
                CouponCode = "FREESHIPPING",
                CartItems = new[]
                {
                    new CartItem  { ProductId = 123, UnitQuantity = 1 }
                }
            };
        }
    }
}