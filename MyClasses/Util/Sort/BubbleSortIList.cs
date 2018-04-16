using System;
using System.Collections.Generic;

namespace AMD.Util.Sort
{
  public static class BubbleSortIList<T> where T : IComparable
  {
    public static void Sort(IList<T> list)
    {
      if (!IsSorted(list))
      {
        for (int i = 0; i < list.Count; i++)
        {
          bubbleDown(list, i);
        }
      }
    }

    private static void bubbleDown(IList<T> list, int i)
    {
      T temp;
      while (i > 0 && list[i].CompareTo(list[i - 1]) < 0)
      {
        temp = list[i];
        list[i] = list[i - 1];
        list[i - 1] = temp;
        i--;
      }
    }

    public static bool IsSorted(IList<T> col)
    {
      for (int i = 1; i < col.Count; i++)
      {
        if (col[i - 1].CompareTo(col[i]) > 0)
        {
          return false;
        }
      }
      return true;
    }
  }
}
