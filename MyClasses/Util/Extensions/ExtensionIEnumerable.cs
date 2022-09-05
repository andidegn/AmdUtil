using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Extensions
{
	public static class ExtensionIEnumerable
	{
		public static IEnumerable<List<T>> Split<T>(this List<T> source, int itemsPerList)
		{
			int index = 0;

			while (index < source.Count)
			{
				yield return source.GetRange(index, Math.Min(itemsPerList, source.Count - index));
				index += itemsPerList;
			}







      //int rangeSize = source.Count / count;
      //int firstRangeSize = rangeSize + source.Count % count;
      //int index = 0;

      //yield return source.GetRange(index, firstRangeSize);
      //index += firstRangeSize;

      //while (index < source.Count)
      //{
      //	yield return source.GetRange(index, rangeSize);
      //	index += rangeSize;
      //}
    }
    public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
    {
      if (source == null) throw new ArgumentNullException("source");
      bool first = true;
      T maxObj = default(T);
      U maxKey = default(U);
      foreach (var item in source)
      {
        if (first)
        {
          maxObj = item;
          maxKey = selector(maxObj);
          first = false;
        }
        else
        {
          U currentKey = selector(item);
          if (currentKey.CompareTo(maxKey) > 0)
          {
            maxKey = currentKey;
            maxObj = item;
          }
        }
      }
      if (first) throw new InvalidOperationException("Sequence is empty.");
      return maxObj;
    }
  }
}
