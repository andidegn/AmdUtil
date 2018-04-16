using AMD.Util;
using AMD.Util.Memoize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoizeTest {
	class Program {
		static void Main(string[] args) {
		//	Diagnostics.MeasureAndPrintToConsole("Fib test", 1, () => {
		//		for (int i = 0; i < 37; i++) {
		//			Console.WriteLine("Fib of {0} = {1}", i, Fib(i));
		//		}
		//	});
		//	var memoFunct = MemoizeTemplet.Memoize<long, decimal>(Fib);
		//	Diagnostics.MeasureAndPrintToConsole("Fib enh test", 1, () => {
		//		for (int i = 0; i < 37; i++) {
		//			Console.WriteLine("Fib enh of {0} = {1}", i, memoFunct(i));
		//		}
		//	});

		
			Diagnostics.MeasureAndPrintToConsole("Golden test", 1, () => {
				for (int i = 0; i < 1000; i++) {
					Golden(i);
				}
			});
			var memoFunct = MemoizeTemplet.Memoize<long, decimal>(Golden);
			Diagnostics.MeasureAndPrintToConsole("Golden enh test", 1, () => {
				for (int i = 0; i < 1000; i++) {
					memoFunct(i);
				}
			});
		}

		//static decimal Fib(int n) {
		//	counter++;
		//	if (n < 2)
		//		return n;
		//	return Fib(n - 1) + Fib(n - 2);
		//}

		static decimal Fib(long n) {
			if (n < 2) {
				return n;
			}
			return Fib(n - 1) + Fib(n - 2);
		}

		static Decimal Golden(long n) {
			if (n == 0)
				return 1;
			return 1 + 1 / Golden(n - 1);
		}
	}
}
