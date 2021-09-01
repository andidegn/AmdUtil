using AMD.Util;
using AMD.Util.Memoize;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoizeTest
{
  class Program
  {
    static void Main(string[] args)
    {
      int fibTests = 42;
      int goldenTests = 8000;
      decimal res = 0;

      Diagnostics.MeasureAndPrintToConsole($"Fib memo tests ({fibTests})\t", 1, () =>
      {
        for (int i = 1; i <= fibTests; i++)
        {
          res = FibMemo(i);
          Console.Write("Fib enh of {0} = {1}\r", i, res);
          if (i == fibTests)
          {
            Console.WriteLine();
          }
        }
      });
      Diagnostics.MeasureAndPrintToConsole($"Fib tests ({fibTests})\t\t", 1, () =>
      {
        for (int i = 1; i <= fibTests; i++)
        {
          res = Fib(i);
          Console.Write("Fib of {0} = {1}\r", i, res);
          if (i == fibTests)
          {
            Console.WriteLine();
          }
        }
      });

      memoDict = new ConcurrentDictionary<long, decimal>();

      Diagnostics.MeasureAndPrintToConsole($"Golden memo tests ({goldenTests})\t", 1, () =>
      {
        for (int i = 1; i <= goldenTests; i++)
        {
          res = GoldenMemo(i);
          Console.Write("Golden enh of {0} = {1}\r", i, res);
          if (i == goldenTests)
          {
            Console.WriteLine();
          }
        }
      });

      Diagnostics.MeasureAndPrintToConsole($"Golden tests ({goldenTests})\t\t", 1, () =>
      {
        for (int i = 1; i <= goldenTests; i++)
        {
          res = Golden(i);
          Console.Write("Golden of {0} = {1}\r", i, res);
          if (i == goldenTests)
          {
            Console.WriteLine();
          }
        }
      });
      Console.ReadKey();
    }

    //static decimal Fib(int n) {
    //	counter++;
    //	if (n < 2)
    //		return n;
    //	return Fib(n - 1) + Fib(n - 2);
    //}


    private static ConcurrentDictionary<long, decimal> memoDict;
    private static decimal FibMemo(long n)
    {
      if (null == memoDict)
      {
        memoDict = new ConcurrentDictionary<long, decimal>();
      }
      if (memoDict.ContainsKey(n))
      {
        return memoDict[n];
      }

      if (n < 2)
      {
        memoDict[n] = n;
        return n;
      }
      decimal res = FibMemo(n - 1) + FibMemo(n - 2);
      memoDict[n] = res;
      return res;
    }

    private static decimal Fib(long n)
    {
      if (n < 2)
      {
        return n;
      }
      return Fib(n - 1) + Fib(n - 2);
    }

    private static decimal GoldenMemo(long n)
    {
      if (null == memoDict)
      {
        memoDict = new ConcurrentDictionary<long, decimal>();
      }
      if (memoDict.ContainsKey(n))
      {
        return memoDict[n];
      }
      if (n == 0)
      {
        memoDict[n] = 1;
        return 1;
      }
      decimal res = 1 + 1 / GoldenMemo(n - 1);
      memoDict[n] = res;
      return res;
    }

    private static decimal Golden(long n)
    {
      if (n == 0)
      {
        return 1;
      }
      return 1 + 1 / Golden(n - 1);
    }
  }
}
