using System;
using System.Collections.Concurrent;

namespace AMD.Util.Memoize
{
	public static class MemoizeTemplet
	{
		public static Func<Arg, Ret> Memoize<Arg, Ret>(this Func<Arg, Ret> functor)
		{
			ConcurrentDictionary<Arg, Ret> memoTable = new ConcurrentDictionary<Arg, Ret>();

			return (arg0) =>
			{
				Ret funct_ret_value;
				if (!memoTable.TryGetValue(arg0, out funct_ret_value))
				{
					funct_ret_value = functor(arg0);
					memoTable.TryAdd(arg0, funct_ret_value);
				}
				return funct_ret_value;
			};
		}
	}
}
