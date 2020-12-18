using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Data
{
    public class ProductRepository : IRepository<int, Product>
    {
        private readonly Dictionary<int, Product> _products = new Dictionary<int, Product>();

        public void Add(int key, Product value)
        {
            _products[key] = value;
        }

        public Product Get(int key)
        {
            return _products.TryGetValue(key, out var result) ? result : null;
        }
    }
}