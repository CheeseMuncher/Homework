using ShoppingCart.Interfaces;
using System.Collections.Generic;

namespace ShoppingCart.Data
{
    public class CategoryRepository : IRepository
    {
        private readonly HashSet<string> _categories;

        public CategoryRepository()
        {
            // TODO this should come from config or be provided by the ProductRepository
            _categories = new HashSet<string> { "Electronic", "Accessory", "Audio" };
        }

        public void Add(string item)
        {
            _categories.Add(item);
        }

        public bool Contains(string item)
        {
            return _categories.Contains(item);
        }
    }
}