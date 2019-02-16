namespace Pi.Utils
{
    internal interface IKeyed<T>
    {
        T this[string key] { get; set; }
        T Get(string key);
        bool Get(string key, out T value);
        void Set(string key, T value);
    }
}
