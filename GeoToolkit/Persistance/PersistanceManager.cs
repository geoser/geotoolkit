using System;
using System.Collections.Generic;
using BLToolkit.Reflection;
using NUnit.Framework;

namespace GeoToolkit.Persistance
{
	public class PersistanceManager
	{
		private readonly IStorage _storage;

		private PersistanceManager(IStorage storage)
		{
			_storage = storage;
		}

		public static readonly PersistanceManager HttpContextCache = new PersistanceManager(new HttpContextCacheStorage());
		public static readonly PersistanceManager Session = new PersistanceManager(new SessionStorage());
		public static readonly PersistanceManager Database = new PersistanceManager(new DbStorage());
		public static readonly PersistanceManager File = new PersistanceManager(FileStorage.Default);
        public static readonly PersistanceManager IsolatedStorage = new PersistanceManager(IsolatedStorageStorage.Instance);

		private static readonly Dictionary<string, PersistanceManager> _fileManagers = new Dictionary<string, PersistanceManager>();

		public static PersistanceManager GetFileManager(string path)
		{
			if (_fileManagers.ContainsKey(path))
				return _fileManagers[path];

			var manager = new PersistanceManager(new FileStorage(path));
			_fileManagers.Add(path, manager);
			return manager;
		}

		public T GetSaved<T>(string key, Func<T> valueFactory) where T : class
		{
			string uniqueKey = string.IsNullOrEmpty(key) ? GetUniqueKey(typeof(T)) : GetUniqueKey(typeof(T), key);

			var obj = _storage.Get<T>(uniqueKey);

			if (obj != null)
				return obj;

			if (valueFactory == null)
				valueFactory = () => TypeAccessor<T>.CreateInstanceEx();

			_storage.Save(uniqueKey, valueFactory());

			return _storage.Get<T>(uniqueKey);
		}

		public T GetSaved<T>(Func<T> creation) where T : class
		{
			return GetSaved(string.Empty, creation);
		}

		public T GetSaved<T>(string key) where T : class
		{
			return GetSaved<T>(key, null);
		}

		public T GetSaved<T>() where T : class
		{
			return GetSaved<T>(string.Empty, null);
		}

		private static string GetUniqueKey(Type type, string localKey)
		{
			return string.Format("{0}_{1}", type.FullName, localKey);
		}

		private static string GetUniqueKey(Type type)
		{
            if (type == null)
                throw new ArgumentNullException("type");

			return GetUniqueKey(type, string.Empty);
		}

		public void Save(object obj, string key)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			_storage.Save(GetUniqueKey(obj.GetType(), key), obj);
		}

		public void Save(object obj)
		{
			Save(obj, string.Empty);
		}
	}

    [TestFixture]
    public class PersistanceManagerTest
    {
        [Serializable]
        public class TestObject
        {
            public string TestProp { get; set; }
        }

        [Test]
        public void IsolatedStorageTest()
        {
            var obj = new TestObject {TestProp = "Test1"};

            PersistanceManager.IsolatedStorage.Save(obj, "key1");

            var testObject = PersistanceManager.IsolatedStorage.GetSaved<TestObject>("key1");

            Assert.AreEqual(obj.TestProp, testObject.TestProp);
        }
    }
}