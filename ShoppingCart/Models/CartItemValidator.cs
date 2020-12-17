using FluentValidation;

namespace ShoppingCart.Models
{
    public class CartItemValidator : AbstractValidator<CartItem>
    {
        public CartItemValidator()
        {
            RuleFor(item => item.UnitQuantity)
                .GreaterThan(-1)
                .WithMessage(ValidationMessages.PositiveQuantities);
        }
    }
}