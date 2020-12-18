namespace ShoppingCart.Interfaces
{
    public interface IRepository<TKey, TValue>
    {
        void Add(TKey key, TValue value);

        TValue Get(TKey key);
    }
}