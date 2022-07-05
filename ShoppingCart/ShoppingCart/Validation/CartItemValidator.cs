using FluentValidation;
using ShoppingCart.Interfaces;
using ShoppingCart.Models;

namespace ShoppingCart.Validation
{
    public class CartItemValidator : AbstractValidator<CartItem>
    {
        public CartItemValidator(IRepository<int, Product> productRepository)
        {
            RuleFor(item => item.UnitQuantity)
                .GreaterThan(-1)
                .WithMessage(ValidationMessages.PositiveQuantities);

            // Might want to rethink validation strategy if IRepository needs to become async
            RuleFor(item => item.ProductId)
                .Must(id => productRepository.Get(id) != null)
                .WithMessage(ValidationMessages.ProductNotFound);
        }
    }
}