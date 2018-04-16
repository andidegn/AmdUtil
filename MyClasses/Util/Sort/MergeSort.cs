using System;

namespace AMD.Util.Sort
{
  public class MergeSort<T> where T : IComparable
  {
    /**
		 * int array
		 */
    public static void Sort(int[] arr)
    {
      if (!IsSorted(arr))
        sortRec(arr, 0, arr.Length - 1);
    }

    private static void sortRec(int[] arr, int first, int last)
    {

      int middle = first + (last - first) / 2;

      if (last - first <= 1)
      {
        int temp;
        if (arr[first] > arr[last])
        {
          temp = arr[last];
          arr[last] = arr[first];
          arr[first] = temp;
        }
        return;
      }
      sortRec(arr, first, middle - 1);
      sortRec(arr, middle, last);
      merge(arr, first, middle, last);
    }

    private static void merge(int[] arr, int first, int middle, int last)
    {
      int[] arr1 = new int[middle - first];
      int[] arr2 = new int[last - middle + 1];
      Array.Copy(arr, first, arr1, 0, arr1.Length);
      Array.Copy(arr, middle, arr2, 0, arr2.Length);

      int a1index = 0, a2index = 0;

      do
      {
        if ((a2index >= arr2.Length) || (arr1[a1index] <= arr2[a2index]))
          arr[first++] = arr1[a1index++];
        else
          arr[first++] = arr2[a2index++];
      } while (a1index < arr1.Length);
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
        sortRec(arr, 0, arr.Length - 1);
    }

    private static void sortRec(T[] arr, int first, int last)
    {

      int middle = first + (last - first) / 2;

      if (last - first <= 1)
      {
        T temp;
        if (arr[first].CompareTo(arr[last]) > 0)
        {
          temp = arr[last];
          arr[last] = arr[first];
          arr[first] = temp;
        }
        return;
      }
      sortRec(arr, first, middle - 1);
      sortRec(arr, middle, last);
      merge(arr, first, middle, last);
    }
    private static void merge(T[] arr, int first, int middle, int last)
    {
      T[] arr1 = new T[middle - first];
      T[] arr2 = new T[last - middle + 1];
      Array.Copy(arr, first, arr1, 0, arr1.Length);
      Array.Copy(arr, middle, arr2, 0, arr2.Length);

      int a1index = 0, a2index = 0;

      do
      {
        if ((a2index >= arr2.Length) || (arr1[a1index].CompareTo(arr2[a2index])) <= 0)
          arr[first++] = arr1[a1index++];
        else
          arr[first++] = arr2[a2index++];
      } while (a1index < arr1.Length);
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
