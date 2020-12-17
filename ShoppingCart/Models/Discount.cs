namespace ShoppingCart.Models
{
    public class Discount
    {
        public DiscountType DiscountType { get; set; }
        public string CategoryDiscount { get; set; }
        public string SupplierDiscount { get; set; }
        public double DiscountPercentage { get; set; }
    }
}