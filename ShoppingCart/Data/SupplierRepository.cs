using ShoppingCart.Interfaces;
using System.Collections.Generic;

namespace ShoppingCart.Data
{
    public class SupplierRepository : IRepository
    {
        private readonly HashSet<string> _suppliers;

        public SupplierRepository()
        {
            // TODO this should come from config or be provided by the ProductRepository
            _suppliers = new HashSet<string> { "HP", "Dell", "Apple" };
        }

        public void Add(string item)
        {
            _suppliers.Add(item);
        }

        public bool Contains(string item)
        {
            return _suppliers.Contains(item);
        }
    }
}