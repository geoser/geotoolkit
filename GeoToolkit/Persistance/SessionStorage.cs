using System.Web;

namespace GeoToolkit.Persistance
{
	public class SessionStorage : IStorage
	{
		public void Save(string key, object obj)
		{
			HttpContext.Current.Session[key] = obj;
		}

		public T Get<T>(string key)
		{
			return (T)HttpContext.Current.Session[key];
		}

		public void Remove(string key)
		{
			HttpContext.Current.Session.Remove(key);
		}

		public bool Contains(string key)
		{
			return HttpContext.Current.Session[key] != null;
		}

		#region IStorage Members

		public void Clear()
		{
			HttpContext.Current.Session.Clear();
		}

		#endregion
	}
}