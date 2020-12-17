using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Data
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly Dictionary<int, Product> _products = new Dictionary<int, Product>();

        public void Add(Product item)
        {
            _products[item.Id] = item;
        }

        public Product Get(int id)
        {
            return _products.TryGetValue(id, out var result) ? result : null;
        }
    }
}