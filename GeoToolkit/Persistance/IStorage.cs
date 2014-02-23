namespace GeoToolkit.Persistance
{
	public interface IStorage
	{
		void Save(string key, object obj);

		T Get<T>(string key);

		void Remove(string key);

		bool Contains(string key);

		void Clear();
	}
}