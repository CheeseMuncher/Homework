namespace ShoppingCart.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string[] Categories { get; set; }
        public string Supplier { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal Price { get; set; }
    }
}