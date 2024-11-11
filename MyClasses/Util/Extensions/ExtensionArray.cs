using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Extensions
{
  public static class ExtensionArray
  {
    public static T[] SubArray<T>(this T[] data, int index)
    {
      return data.SubArray(index, data.Length - index);
    }

    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
      T[] result = new T[length];
      Array.Copy(data, index, result, 0, length);
      return result;
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(this T[] arr, int size)
    {
      for (var i = 0; i < arr.Length / size; i++)
      {
        yield return arr.Skip(i * size).Take(size);
      }
    }
  }
}
