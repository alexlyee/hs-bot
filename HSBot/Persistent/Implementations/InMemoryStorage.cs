using System;
using System.Collections.Generic;

namespace HSBot.Persistent.Implementations
{
    public class InMemoryStorage : IDataStorage
    {
        private object _object;
        private string _key;
        private string _folder;
        private string _file;

        public void Initialize(object obj, string key, string folder = "")
        {
            _object = obj;
            _key = key;
            _folder = folder;
            _file = $"{folder}/{key}.json";
        }

        public void StoreObject(object obj, string key, string folder = "")
        {
            _object = obj;
            _key = key;
            _folder = folder;
            _file = $"{folder}/{key}.json";
            DataStorage.StoreObject(_object, _file);
        }

        public void StoreObject() => DataStorage.StoreObject(_object, _file);

        public T RestoreObject<T>(string key)
        {
            return DataStorage.RestoreObject<T>(_file);
        }

    }
}