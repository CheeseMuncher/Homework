using System.Collections.Generic;

namespace ShoppingCart.Models
{
    public class ShoppingCartRequest
    {
        public string DiscountCode { get; set; }
        public IEnumerable<CartItem> CartItems { get; set; }
    }
}