namespace ShoppingCart.Models
{
    public class Discount
    {
        public DiscountType DiscountType { get; set; }
        public string DiscountedCategory { get; set; }
        public string DiscountedSupplier { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}