using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMD.Util.Collections;
using AMD.Util.Collections.QueueArray;
using AMD.Util;
using AMD.Util.Compression;
using AMD.Util.Compression.SevenZip.Compression.LZMA;
using AMD.Util.Search;
using System.Data.SqlTypes;
using System.Reflection;
using System.Diagnostics;
using AMD.Util.MyConsole;

namespace QueueTest
{
  class TestContainer : IComparable
  {
    public int id { get; set; }

    public bool IsNull => throw new NotImplementedException();

    public int CompareTo(object obj)
    {
      return id.CompareTo((int)obj);
    }
  }

  class Program
  {
    static int length = 100000;
    static double[] arr = new double[length];

    static void Main(string[] args)
    {
      int size = 128;

      BitArray ba = new BitArray(size);

      for (int i = 1; i < size; i += 3)
      {
        ba.SetHigh(i);
      }
      var dArr1 = ba.GetByteArray();
      var dArr2 = ba.GetUShortArray();
      var dArr3 = ba.GetUIntArray();
      var dArr4 = ba.GetULongArray();

      var dArr5 = ba.GetSByteArray();
      var dArr6 = ba.GetShortArray();
      var dArr7 = ba.GetIntArray();
      var dArr8 = ba.GetLongArray();

      int index = 0;
      foreach (var item in ba)
      {
        Console.WriteLine($"ba{index++} = {(item ? "1" : "0")}");
      }
      Console.ReadKey();
      return;




      RunCodeTestExample();
      return;

      Random random = new Random();
      int randomCount = 100;
      int totalValuesCount = 10000;
      int numberOfTests = 100;
      int bsearchIsFasterCnt = 0, bsearch1IsFasterCnt = 0;

      List<int> list = new List<int>();
      List<int> excludeValues = new List<int>();
      List<long> timeKeeping = new List<long>(totalValuesCount);

      Stopwatch sw = Stopwatch.StartNew();

      for (int i = 0; i < randomCount; i++)
      {
        int rand = -1;
        do
        {
          rand = random.Next(totalValuesCount);
        } while (excludeValues.Contains(rand));
        excludeValues.Add(rand);
      }

      for (int i = 0; i < totalValuesCount + randomCount; i++)
      {
        if (excludeValues.Contains(i))
        {
          continue;
        }
        list.Add(i);
      }

      Console.WriteLine($"List created: {sw.Elapsed}");

      //Console.WriteLine(string.Join(", ", excludeValues.ToArray()));

      for (int j = 0; j < numberOfTests; j++)
      {
        sw.Restart();
        Stopwatch sw2 = Stopwatch.StartNew();
        double avgContains = 0, avgBSearch = 0;

        for (int i = 0; i < totalValuesCount; i++)
        {
          BSearch<int> bSearch = new BSearch<int>(list);

          bool expected = !excludeValues.Contains(i);
          GC.Collect();
          sw2.Restart();
          bool result = bSearch.Exists(i);
          timeKeeping.Add(sw.ElapsedMilliseconds);

          if (expected != result)
          {
            result = bSearch.Exists(i);
            Console.WriteLine($"{i:000} {result} - expected: {expected}");
          }
        }
        sw.Stop();
        avgContains = timeKeeping.Average();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"({j}/{numberOfTests}) Contains on {list.Count} items took {sw.Elapsed}. Average pr: {timeKeeping.Average()}ms");
        Console.ResetColor();

        timeKeeping = new List<long>(totalValuesCount);
        sw.Restart();
        for (int i = 0; i < totalValuesCount; i++)
        {
          BSearch<int> bSearch = new BSearch<int>(list);

          bool expected = !excludeValues.Contains(i);
          GC.Collect();
          sw2.Restart();
          bool result = bSearch.Exists(i);
          timeKeeping.Add(sw.ElapsedMilliseconds);

          if (expected != result)
          {
            result = bSearch.Exists(i);
            Console.WriteLine($"{i:000} {result} - expected: {expected}");
          }
        }
        avgBSearch = timeKeeping.Average();
        if (timeKeeping.Average() < avgContains)
        {
          bsearchIsFasterCnt++;
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"({j}/{numberOfTests}) BSearch  on {list.Count} items took {sw.Elapsed}. Average pr: {timeKeeping.Average()}ms {(timeKeeping.Average() < avgContains ? "Faster!!!" : "")}");
        Console.ResetColor();

        //timeKeeping = new List<long>(totalValuesCount);
        //sw.Restart();
        //for (int i = 0; i < totalValuesCount; i++)
        //{
        //  BSearch<int> bSearch = new BSearch<int>(list);

        //  bool expected = !excludeValues.Contains(i);
        //  GC.Collect();
        //  sw2.Restart();
        //  bool result = bSearch.Exists1(i);
        //  timeKeeping.Add(sw.ElapsedMilliseconds);

        //  if (expected != result)
        //  {
        //    result = bSearch.Exists1(i);
        //    Console.WriteLine($"{i:000} {result} - expected: {expected}");
        //  }
        //}
        //if (timeKeeping.Average() < avgBSearch)
        //{
        //  bsearch1IsFasterCnt++;
        //}
        //Console.ForegroundColor = ConsoleColor.DarkGreen;
        //Console.WriteLine($"({j}/{numberOfTests}) BSearch1  on {list.Count} items took {sw.Elapsed}. Average pr: {timeKeeping.Average()}ms {(timeKeeping.Average() < avgBSearch ? "Faster!!!" : "")}");
        //Console.ResetColor();
      }

      Console.WriteLine($"Done. BSearch is faster {100.0 * bsearchIsFasterCnt / numberOfTests}% of the time");
      Console.WriteLine($"Done. BSearch1 is faster {100.0 * bsearch1IsFasterCnt / numberOfTests}% of the time");

      Console.ReadKey();

      return;
      IQueue<int> q = new ArrayQueue<int>(10);
      for (int i = 0; i < 10; i++)
      {
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

      for (int i = 0; i < length; i++)
      {
        arr[i] = r.NextDouble();
      }
      Console.WriteLine("testing measurement functions");
      Diagnostics.MeasureAndPrintToConsole("Random for loop", 10, MeasureMerge);
      Diagnostics.MeasureAndPrintToConsole("Random for loop", 10, MeasureBubble);

      Console.ReadKey();
    }

    private static void RunCodeTestExample()
    {
      string inp = "ajs0dfjoaspøkfpas0åionsf";

      int size = (int)Math.Pow(10, 5);

      Random r = new Random();
      char[] chars = new char[size];
      char charToSetOnlyOnce = 't';
      int charToSetOnlyOncePosition = 99200;
      for (int i = 0; i < size; i++)
      {
        char c;
        if (i == charToSetOnlyOncePosition)
        {
          c = charToSetOnlyOnce;
        }
        else
        {
          do
          {
            c = (char)r.Next(0x61, 0x7A);
          } while (c == charToSetOnlyOnce);
        }
        chars[i] = c;
      }

      inp = new string(chars);

      Stopwatch sw = new Stopwatch();
      GC.Collect();
      sw.Start();
      CodeTestSolution1(inp);
      sw.Stop();
      Console.WriteLine($"1 Done in {sw.ElapsedMilliseconds}ms");

      GC.Collect();
      sw.Restart();
      CodeTestSolution2(inp);
      sw.Stop();
      Console.WriteLine($"2 Done in {sw.ElapsedMilliseconds}ms");

      Console.ReadKey();
    }

    private static void CodeTestSolution1(string inp)
    {
      for (int i = 0; i < inp.Length - 1; i++)
      {
        char c = inp[i];
        string subStr = inp.Substring(i, inp.Length - i);
        if (inp.Substring(i + 1).Contains(c))
        {
          continue;
        }
        Console.WriteLine($"char = {c}");
        break;
      }
    }

    private static void CodeTestSolution2(string inp)
    {
      char[] chars = inp.ToCharArray();
      AMD.Util.Sort.MergeSort<char>.Sort(chars);

      int charCnt = 0;
      char lastChar = 'z';

      foreach (char c in chars)
      {
        if (lastChar != c)
        {
          if (1 == charCnt)
          {
            Console.WriteLine($"char = {c}");
            break;
          }
          charCnt = 0;
        }
        lastChar++;
      }
    }

    static void MeasureBubble()
    {
      double[] arrCopy = new double[length];
      Array.Copy(arr, arrCopy, length);
      AMD.Util.Sort.BubbleSort<double>.Sort(arrCopy);
    }

    static void MeasureMerge()
    {
      double[] arrCopy = new double[length];
      Array.Copy(arr, arrCopy, length);
      AMD.Util.Sort.MergeSort<double>.Sort(arrCopy);
    }

    static void print(IQueue<int> q)
    {
      Console.WriteLine("First: \n" + q.First());
      Console.WriteLine(q.ToString());
    }
  }
}
