using System.Data.SqlTypes;
using BLToolkit.DataAccess;

namespace GeoToolkit.Persistance
{
	[TableName("PersistanceObject")]
	public class PersistanceManagerObject
	{
		[PrimaryKey]
		public long? Key;

		public SqlBinary Data;

		public PersistanceManagerObject()
		{
			
		}

		public PersistanceManagerObject(long key, byte[] data)
		{
			Key = key;
			Data = data;
		}
	}
}