using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMD.Util.Collections;
using AMD.Util.Collections.QueueArray;
using AMD.Util;

namespace QueueTest {
	class Program {
		static int length = 100000;
		static double[] arr = new double[length];
        static void Main(string[] args) {
            IQueue<int> q = new ArrayQueue<int>(10);
			for (int i = 0; i < 10; i++) {
				q.Enqueue(i);
			}
            print(q);
            q.Enqueue(10);
            print(q);
            q.Enqueue(11);
            print(q);
            for (int i = 12; i < 100; i++)
                q.Enqueue(i);
            print(q);
            Console.WriteLine();

			Random r = new Random();

			for (int i = 0; i < length; i++) {
				arr[i] = r.NextDouble();
			}
			Console.WriteLine("testing measurement functions");
			Diagnostics.MeasureAndPrintToConsole("Random for loop", 10, MeasureMerge);
			Diagnostics.MeasureAndPrintToConsole("Random for loop", 10, MeasureBubble);

            Console.ReadKey();
        }

		static void MeasureBubble() {
			double[] arrCopy = new double[length];
			Array.Copy(arr, arrCopy, length);
			AMD.Util.Sort.BubbleSort<double>.Sort(arrCopy);
		}

		static void MeasureMerge() {
			double[] arrCopy = new double[length];
			Array.Copy(arr, arrCopy, length);
			AMD.Util.Sort.MergeSort<double>.Sort(arrCopy);
		}

        static void print(IQueue<int> q) {
            Console.WriteLine("First: \n" + q.First());
            Console.WriteLine(q.ToString());
        }
    }
}
