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
	}
}
