using FluentValidation;
using ShoppingCart.Interfaces;

namespace ShoppingCart.Models
{
    public class CartItemValidator : AbstractValidator<CartItem>
    {
        public CartItemValidator(IRepository<Product> productRepository)
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