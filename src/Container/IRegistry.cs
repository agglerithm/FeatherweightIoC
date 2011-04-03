namespace Container
{
    public interface IRegistry
    {
        void Add(string key, Registration reg);
        bool Contains(string key);
        Registration Get(string key);
    }
}