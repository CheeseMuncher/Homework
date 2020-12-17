namespace ShoppingCart.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T item);

        T Get(int id);
    }

    public interface IRepository
    {
        void Add(string item);

        bool Contains(string item);
    }
}