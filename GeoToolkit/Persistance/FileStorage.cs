using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GeoToolkit.Persistance
{
	public class FileStorage : IStorage
	{
		private static readonly Guid _id = new Guid("30ee8532-bbe6-4ca8-9af6-70f3e2570370");

		public static readonly FileStorage Default;
		private readonly string _filePath;
		private readonly BinaryFormatter _formatter = new BinaryFormatter();

		static FileStorage()
		{
			Default = FileStorageSettings.Default.FilePath != null
			          	? new FileStorage(FileStorageSettings.Default.FilePath)
			          	: new FileStorage(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
		}

		public FileStorage(string filePath)
		{
			if (!Directory.Exists(filePath))
				Directory.CreateDirectory(filePath);

			_filePath = filePath;
		}

		#region IStorage Members

		public void Save(string key, object obj)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");

            if (obj == null)
                throw new ArgumentNullException("obj");

			string fullName = GetFullPath(key);

			if (File.Exists(fullName))
				File.Delete(fullName);

		    using (FileStream file = File.Open(fullName, FileMode.OpenOrCreate, FileAccess.Write))
		        _formatter.Serialize(file, obj);
		}

		private string GetFullPath(string key)
		{
			return Path.Combine(_filePath, GetFileName(key));
		}

		public T Get<T>(string key)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");

			string fullPath = GetFullPath(key);

			if (!File.Exists(fullPath))
				return default(T);

			using (FileStream file = File.Open(fullPath, FileMode.Open, FileAccess.Read))
				return (T)_formatter.Deserialize(file);
		}

		public void Remove(string key)
		{
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

			string fullPath = GetFullPath(key);
			if (File.Exists(fullPath))
				File.Delete(fullPath);
		}

		public bool Contains(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return File.Exists(GetFullPath(key));
		}

		public void Clear()
		{
			string[] files = Directory.GetFiles(_filePath, "*.fso");
			foreach(string file in files)
				File.Delete(Path.Combine(_filePath, file));
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