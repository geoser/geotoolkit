using System.Data.SqlTypes;
using BLToolkit.DataAccess;

namespace GeoToolkit.Persistance
{
	public abstract class PersistanceObjectDataAccessor 
        : DataAccessor<PersistanceManagerObject, PersistanceObjectDataAccessor>
	{
		public abstract PersistanceManagerObject SelectByKey(long id);
		public abstract void Save(long? key, SqlBinary data);

		public bool Contains(string key)
		{
			return Contains(key.GetHashCode());
		}

		public abstract bool Contains(long id);

		public void DeleteByKey(string key)
		{
			DeleteByKey(key.GetHashCode());
		}

		public abstract void DeleteByKey(long id);

		public void Save(string key, byte[] data)
		{
			Save(key.GetHashCode(), data);
		}

		public PersistanceManagerObject SelectByKey(string key)
		{
			return SelectByKey(key.GetHashCode());
		}

		public abstract void Clear();
	}
}