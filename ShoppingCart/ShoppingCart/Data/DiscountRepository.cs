using ShoppingCart.Interfaces;
using ShoppingCart.Models;
using System.Collections.Generic;

namespace ShoppingCart.Data
{
    public class DiscountRepository : IRepository<string, Discount>
    {
        private readonly Dictionary<string, Discount> _discounts;

        public DiscountRepository()
        {
            // TODO this should come from config or be backed by a database or a dedicated discount service
            _discounts = new Dictionary<string, Discount>
            {
                ["BACKTOSCHOOL2019"] = new Discount { DiscountType = DiscountType.Supplier, DiscountedSupplier = "Apple", DiscountPercentage = 15 },
                ["APPLE5"] = new Discount { DiscountType = DiscountType.Supplier, DiscountedSupplier = "Apple", DiscountPercentage = 5 },
                ["LOVEMYMUSIC"] = new Discount { DiscountType = DiscountType.Category, DiscountedCategory = "Audio", DiscountPercentage = 20 },
                ["AUDIO10"] = new Discount { DiscountType = DiscountType.Category, DiscountedCategory = "Audio", DiscountPercentage = 10 },
                ["FREESHIPPING"] = new Discount { DiscountType = DiscountType.Shipping }
            };
        }

        public void Add(string key, Discount value)
        {
            _discounts[key] = value;
        }

        public Discount Get(string key)
        {
            return _discounts.TryGetValue(key, out var result) ? result : null;
        }
    }
}