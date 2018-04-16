using AMD.Util.Collections.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary3DTest {
	class Program {
		static void Main(string[] args) {
			Dictionary3D<int, double, String> dict = new Dictionary3D<int, double, string>();

			dict.Add(1, 1.0, "1-1.0");
			dict.Add(1, 1.1, "1-1.1");
			dict.Add(1, 1.2, "1-1.2");
			dict.Add(1, 1.3, "1-1.3");
			dict.Add(1, 1.4, "1-1.4");
			dict.Add(1, 1.5, "1-1.5");

			dict.Add(2, 1.0, "2-1.0");
			dict.Add(2, 1.1, "2-1.1");
			dict.Add(2, 1.2, "2-1.2");
			dict.Add(2, 1.3, "2-1.3");
			dict.Add(2, 1.4, "2-1.4");
			dict.Add(2, 1.5, "2-1.5");

			Console.WriteLine("contains <1, 1.0>: {0}", dict.ContainsKeys(1, 1.0));
			Console.WriteLine("contains <1, 1.3>: {0}", dict.ContainsKeys(1, 1.3));
			Console.WriteLine("contains <1, 1.6>: {0}", dict.ContainsKeys(1, 1.6));
			Console.WriteLine("contains <2, 1.0>: {0}", dict.ContainsKeys(2, 1.0));
			Console.WriteLine("contains <2, 1.3>: {0}", dict.ContainsKeys(2, 1.3));
			Console.WriteLine("contains <2, 1.6>: {0}", dict.ContainsKeys(2, 1.6));

			Console.WriteLine("1, 1.0: {0}", dict[1, 1.0]);
			Console.WriteLine("Overriding with Add():");
			dict.Add(1, 1.0, "1, 1.0-override Add");
			Console.WriteLine("1, 1.0 override Add: {0}", dict[1, 1.0]);
			dict[1, 1.1] = "1, 1.0-override Index";
			Console.WriteLine("1, 1.0 override Index: {0}", dict[1, 1.0]);

			Console.ReadKey();
		}
	}
}
