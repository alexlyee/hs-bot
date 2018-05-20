namespace HSBot.Persistent
{
    public interface IDataStorage
    {
        void StoreObject(object obj, string file, string folder = "");

        T RestoreObject<T>(string file);
    }
}