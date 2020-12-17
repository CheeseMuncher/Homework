using NUnit.Framework;
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