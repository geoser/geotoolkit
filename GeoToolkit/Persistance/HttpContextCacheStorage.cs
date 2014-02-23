using System;
using System.Web;
using System.Web.Caching;

namespace GeoToolkit.Persistance
{
	public class HttpContextCacheStorage : IStorage
	{
		private Cache _cache;

		public HttpContextCacheStorage(Cache cache)
		{
			if (cache == null)
				throw new ArgumentNullException("cache");

			_cache = cache;
		}

		public HttpContextCacheStorage()
		{
			if (HttpContext.Current == null)
				return;

			_cache = HttpContext.Current.Cache;
		}

		public void Save(string key, object obj)
		{
			_cache.Add(key, obj, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), CacheItemPriority.Normal, null);
		}

		public T Get<T>(string key)
		{
			return (T)_cache[key];
		}

		public void Remove(string key)
		{
			_cache.Remove(key);
		}

		public bool Contains(string key)
		{
			return _cache[key] != null;
		}

		#region IStorage Members

		public void Clear()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}