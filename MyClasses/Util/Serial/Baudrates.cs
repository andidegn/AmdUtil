using System;
using System.Collections;

namespace AMD.Util.Serial
{
	public class BaudRateList : IEnumerable
	{
		private static BaudRate[] rates;
		private static BaudRateList list;

		private BaudRateList()
		{
			rates = new BaudRate[] {
                new BaudRate("2400", 2400),
                new BaudRate("4800", 4800),
                new BaudRate("7200", 7200),
                new BaudRate("9600", 9600),
                new BaudRate("14K4", 14400),
                new BaudRate("19K2", 19200),
                new BaudRate("28K8", 28800),
                new BaudRate("38K4", 38400),
                new BaudRate("57K6", 57600),
                new BaudRate("115K2", 115200),
                new BaudRate("128K0", 128000),
                new BaudRate("230K4", 230400),
                new BaudRate("460K8", 460800),
                new BaudRate("921K6", 921600)
            };
		}

		//public int this[int index] {
		//	get { return rates[index].Value; }
		//}

		public BaudRate this[int index]
		{
			get { return rates[index].Copy(); }
		}

		//public int this[String key] {
		//	get {
		//		foreach (BaudRate br in rates) {
		//			if (br.Name.Equals(key.ToUpper())) {
		//				return br.Value;
		//			}
		//		}
		//		return -1;
		//	}
		//}

		public BaudRate this[String key]
		{
			get
			{
				foreach (BaudRate br in rates)
				{
					if (br.Name.Equals(key.ToUpper()))
					{
						return br.Copy();
					}
				}
				return null;
			}
		}

		public int Length
		{
			get
			{
				return rates.Length;
			}
		}

		public static BaudRateList Instance
		{
			get
			{
				if (list == null)
					list = new BaudRateList();
				return list;
			}
		}

		public BaudRate[] GetArray
		{
			get
			{
				int length = rates.Length;
				BaudRate[] ret = new BaudRate[length];
				Array.Copy(rates, ret, length);
				return ret;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return rates.GetEnumerator();
		}
	}



	public class BaudRate
	{
		public String Name { get; private set; }
		public int Value { get; private set; }

		public BaudRate(String name, int value)
		{
			Name = name;
			Value = value;
		}

		public BaudRate Copy()
		{
			return new BaudRate(Name, Value);
		}

		//public override bool Equals(object obj) {
		//    BaudRate o = obj as BaudRate;
		//    return _value == o.Value;
		//}

		public override String ToString()
		{
			return Name;
		}
	}
}
