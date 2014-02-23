using GeoToolkit.Serialization;

namespace GeoToolkit.Persistance
{
    public class DbStorage : IStorage
    {
        private readonly PersistanceObjectDataAccessor _accessor = PersistanceObjectDataAccessor.CreateInstance();

        #region IStorage Members

        public void Save(string key, object obj)
        {
            _accessor.Save(key, BinaryFormatterHelper.Serialize(obj));
        }

        public T Get<T>(string key)
        {
            var obj = _accessor.SelectByKey(key);

            return obj != null 
                ? BinaryFormatterHelper.Deserialize<T>(obj.Data.Value) 
                : default(T);
        }

        public void Remove(string key)
        {
            _accessor.DeleteByKey(key);
        }

        public bool Contains(string key)
        {
            return _accessor.Contains(key);
        }

        public void Clear()
        {
            _accessor.Clear();
        }

        #endregion
    }
}