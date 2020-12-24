namespace ShoppingCart.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string[] Categories { get; set; }
        public string Supplier { get; set; }
        public double WholesalePrice { get; set; }
        public double Price { get; set; }
    }
}