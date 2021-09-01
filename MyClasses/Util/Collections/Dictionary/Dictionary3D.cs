using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Collections.Dictionary
{
	public class Dictionary3D<tKey1, tKey2, tValue>
	{
		private IDictionary<tKey1, IDictionary<tKey2, tValue>> _dict;

		public Dictionary3D()
		{
			_dict = new Dictionary<tKey1, IDictionary<tKey2, tValue>>();
		}

		public tValue this[tKey1 key1, tKey2 key2]
		{
			get
			{
				if (_dict.ContainsKey(key1))
				{
					return _dict[key1][key2];
				}
				throw new KeyNotFoundException(String.Format("Key not found: {0}", key1));
			}
			set
			{
				Add(key1, key2, value);
			}
		}

		public void Add(tKey1 key1, tKey2 key2, tValue value)
		{
			IDictionary<tKey2, tValue> inner = _dict.ContainsKey(key1) ? _dict[key1] : new Dictionary<tKey2, tValue>();
			//IDictionary<tKey2, tValue> inner = _dict[key1] ?? new Dictionary<tKey2, tValue>();

			inner[key2] = value;
			_dict[key1] = inner;
		}

		public bool ContainsKeys(tKey1 key1, tKey2 key2)
		{
			return _dict.ContainsKey(key1) && _dict[key1].ContainsKey(key2);
		}
	}
}
