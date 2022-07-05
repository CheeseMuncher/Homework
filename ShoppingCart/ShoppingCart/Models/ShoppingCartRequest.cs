using System.Collections.Generic;

namespace ShoppingCart.Models
{
    public class ShoppingCartRequest
    {
        public string CouponCode { get; set; }
        public IEnumerable<CartItem> CartItems { get; set; }
    }
}