using System.Collections.Generic;
using System.Linq;
using ShoppingCart.Interfaces;
using ShoppingCart.Pocos;

namespace ShoppingCart.Implementation
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly List<Product> _products = new List<Product>();
        public void Add(Product item)
        {
            _products.Add(item);
        }

        public Product Get(int id)
        {
            return _products.FirstOrDefault(x => x.Id == id);
        }
    }
}