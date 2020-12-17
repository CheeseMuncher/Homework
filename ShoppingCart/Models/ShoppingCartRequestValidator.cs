using FluentValidation;

namespace ShoppingCart.Models
{
    public class ShoppingCartRequestValidator : AbstractValidator<ShoppingCartRequest>
    {
        public ShoppingCartRequestValidator()
        {
            RuleFor(request => request.CouponCode)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(ValidationMessages.ExactlyOneCode)
                .NotEmpty()
                .WithMessage(ValidationMessages.ExactlyOneCode)
                .Must(code => code.Split(' ').Length == 1)
                .WithMessage(ValidationMessages.ExactlyOneCode);
        }
    }
}