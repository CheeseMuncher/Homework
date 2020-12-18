using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Data
{
    public class ProductRepository : IRepository<int, Product>
    {
        private readonly Dictionary<int, Product> _products;

        public ProductRepository()
        {
            // TODO this should come from config or be backed by a database or a dedicated product service
            _products = new Dictionary<int, Product>
            {
                [1] = new Product { Id = 1, Name = "Headphones", Price = 10, Supplier = "Apple", Categories = new[] { "Accessory", "Electronic", "Audio" } },
                [2] = new Product { Id = 2, Name = "USB Cable", Price = 4, Supplier = "Apple", Categories = new[] { "Accessory" } },
                [3] = new Product { Id = 3, Name = "Monitor", Price = 100, Supplier = "HP", Categories = new[] { "Electronic" } },
                [4] = new Product { Id = 4, Name = "Laptop", Price = 1000, Supplier = "Dell", Categories = new[] { "Electronic" } },
                [5] = new Product { Id = 5, Name = "iPad", Price = 220, Supplier = "Apple", Categories = new[] { "Electronic" } }
            };
        }

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