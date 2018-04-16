using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Sort
{
  public class BubbleSort<T> where T : IComparable
  {

    /**
		 * int array
		 */
    public static void Sort(int[] arr)
    {
      if (!IsSorted(arr))
        for (int i = 0; i < arr.Length; i++)
          bubbleDown(arr, i);
    }

    private static void bubbleDown(int[] arr, int i)
    {
      int temp;
      while (i > 0 && arr[i] < arr[i - 1])
      {
        temp = arr[i];
        arr[i] = arr[i - 1];
        arr[i - 1] = temp;
        i--;
      }
    }

    public static bool IsSorted(int[] arr)
    {
      for (int i = 1; i < arr.Length; i++)
        if (arr[i - 1] > arr[i])
          return false;
      return true;
    }

    /**
		 * Object array
		 */
    public static void Sort(T[] arr)
    {
      if (!IsSorted(arr))
        for (int i = 0; i < arr.Length; i++)
          bubbleDown(arr, i);
    }

    private static void bubbleDown(T[] arr, int i)
    {
      T temp;
      while (i > 0 && arr[i].CompareTo(arr[i - 1]) < 0)
      {
        temp = arr[i];
        arr[i] = arr[i - 1];
        arr[i - 1] = temp;
        i--;
      }
    }

    public static bool IsSorted(T[] arr)
    {
      for (int i = 1; i < arr.Length; i++)
        if (arr[i - 1].CompareTo(arr[i]) > 0)
          return false;
      return true;
    }
  }
}
