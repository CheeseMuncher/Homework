using ShoppingCart.Interfaces;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Data
{
    public class SupplierRepository : IRepository<string, string>
    {
        private readonly HashSet<string> _suppliers;

        public SupplierRepository()
        {
            // TODO this should come from config or be provided by the ProductRepository
            _suppliers = new HashSet<string> { "HP", "Dell", "Apple" };
        }

        public void Add(string item, string value)
        {
            if (item == value)
                _suppliers.Add(item);

            throw new ArgumentException("Key and value don't match");
        }

        public string Get(string item)
        {
            return _suppliers.Contains(item) ? item : null;
        }
    }
}