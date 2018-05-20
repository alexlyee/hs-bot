namespace HSBot.Persistent
{
    public interface IDataStorage
    {
        void StoreObject(object obj, string file);

        T RestoreObject<T>(string file);
    }
}