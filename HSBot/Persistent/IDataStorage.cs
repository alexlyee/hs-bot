namespace HSBot.Persistent
{
    public interface InMemoryStorage
    {
        void StoreObject(object obj, string file);

        T RestoreObject<T>(string file);
    }
}