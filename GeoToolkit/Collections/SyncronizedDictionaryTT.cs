using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GeoToolkit.Collections
{
    [Obsolete("Use System.Collections.Concurrent.ConcurrentDictionary instead", true)]
	public class SyncronizedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		public readonly object SyncRoot = new object();

		#region Constructors

		public SyncronizedDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public SyncronizedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer)
		{
		}

		public SyncronizedDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
		{
		}

		public SyncronizedDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
		{
		}

		public SyncronizedDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
		{
		}

		public SyncronizedDictionary(int capacity) : base(capacity)
		{
		}

		public SyncronizedDictionary()
		{
		}

		#endregion

		public new TValue this[TKey key]
		{
			get
			{
				lock (SyncRoot)
					return base[key];
			}
			set
			{
				lock (SyncRoot)
					base[key] = value;
			}
		}

		public new IEqualityComparer<TKey> Comparer
		{
			get
			{
				lock (SyncRoot)
					return base.Comparer;
			}
		}

		public new int Count
		{
			get
			{
				lock (SyncRoot)
					return base.Count;
			}
		}

		public new KeyCollection Keys
		{
			get
			{
				lock (SyncRoot)
					return base.Keys;
			}
		}

		public new ValueCollection Values
		{
			get
			{
				lock (SyncRoot)
					return base.Values;
			}
		}

		public new void Add(TKey key, TValue value)
		{
			lock (SyncRoot)
				base.Add(key, value);
		}

		public new void Clear()
		{
			lock (SyncRoot)
				base.Clear();
		}

		public new bool ContainsKey(TKey key)
		{
			lock (SyncRoot)
				return base.ContainsKey(key);
		}

		public new bool ContainsValue(TValue value)
		{
			lock (SyncRoot)
				return base.ContainsValue(value);
		}

		public new Enumerator GetEnumerator()
		{
			lock (SyncRoot)
				return base.GetEnumerator();
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			lock (SyncRoot)
				base.GetObjectData(info, context);
		}

		public override void OnDeserialization(object sender)
		{
			lock (SyncRoot)
				base.OnDeserialization(sender);
		}

		public new bool Remove(TKey key)
		{
			lock (SyncRoot)
				return base.Remove(key);
		}

		public new bool TryGetValue(TKey key, out TValue value)
		{
			lock (SyncRoot)
				return base.TryGetValue(key, out value);
		}
	}
}