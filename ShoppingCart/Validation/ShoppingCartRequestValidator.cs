using FluentValidation;
using ShoppingCart.Interfaces;

using ShoppingCart.Models;

namespace ShoppingCart.Validation
{
    public class ShoppingCartRequestValidator : AbstractValidator<ShoppingCartRequest>
    {
        private const string _alpahnumericRegex = "^[a-zA-Z0-9_]*$";

        public ShoppingCartRequestValidator(IRepository<Product> productRepository)
        {
            RuleFor(request => request.CouponCode)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(ValidationMessages.ExactlyOneCode)
                .NotEmpty()
                .WithMessage(ValidationMessages.ExactlyOneCode)
                .Must(code => code.Split(' ').Length == 1)
                .WithMessage(ValidationMessages.ExactlyOneCode)
                .Matches(_alpahnumericRegex)
                .WithMessage(ValidationMessages.ExactlyOneCodeAlphanumeric);

            RuleForEach(request => request.CartItems)
                .SetValidator(new CartItemValidator(productRepository));
        }
    }
}