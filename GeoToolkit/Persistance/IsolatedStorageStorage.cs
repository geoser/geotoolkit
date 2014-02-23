using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;

namespace GeoToolkit.Persistance
{
    public class IsolatedStorageStorage : IStorage
    {
        private static readonly Guid _id = new Guid("{A5663682-EB8A-4249-9755-0ACE2DC79620}");
        private static readonly IsolatedStorageFile _storage;
        private static IsolatedStorageStorage _instance;

        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        static IsolatedStorageStorage()
        {
            _storage = IsolatedStorageFile.GetMachineStoreForAssembly();
        }

        public static IsolatedStorageStorage Instance
        {
            get { return _instance ?? (_instance = new IsolatedStorageStorage()); }
        }

        #region IStorage Members

        public void Save(string key, object obj)
        {
            string fileName = GetFileName(key);

            if (_storage.FileExists(fileName))
                _storage.DeleteFile(fileName);

            using (var file = _storage.OpenFile(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                _formatter.Serialize(file, obj);
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            string fileName = GetFileName(key);

            if (!_storage.FileExists(fileName))
                return default(T);

            using (var file = _storage.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                return (T) _formatter.Deserialize(file);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            string fileName = GetFileName(key);

            if (_storage.FileExists(fileName))
                _storage.DeleteFile(fileName);
        }

        public bool Contains(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            return _storage.FileExists(GetFileName(key));
        }

        public void Clear()
        {
            string[] files = _storage.GetFileNames("*.fso");

            foreach (string file in files)
                _storage.DeleteFile(file);
        }

        #endregion

        private static string GetFileName(string key)
        {
            string clearName = string.Concat(key.Split(Path.GetInvalidFileNameChars()));
            clearName = clearName.Length <= 100 ? clearName : clearName.Substring(0, 99);

            return string.Format("{0}_{1}_{2}.fso", _id, clearName, key.GetHashCode());
        }
    }
}