using ShoppingCart.Interfaces;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Data
{
    public class CategoryRepository : IRepository<string, string>
    {
        private readonly HashSet<string> _categories;

        public CategoryRepository()
        {
            // TODO this should come from config or be provided by the ProductRepository
            _categories = new HashSet<string> { "Electronic", "Accessory", "Audio" };
        }

        public void Add(string item, string value)
        {
            if (item == value)
                _categories.Add(item);

            throw new ArgumentException("Key and value don't match");
        }

        public string Get(string item)
        {
            return _categories.Contains(item) ? item : null;
        }
    }
}