namespace ShoppingCart.Interfaces
{

    public interface IRepository<T>
    {
        void Add(T item);
        T Get(int id);
    }
}