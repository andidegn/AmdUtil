using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Extensions
{
  public static class ExtensionObject
  {
    public static bool IsNumber(this object obj)
    {
      return obj is sbyte
              || obj is byte
              || obj is short
              || obj is ushort
              || obj is int
              || obj is uint
              || obj is long
              || obj is ulong
              || obj is float
              || obj is double
              || obj is decimal;
    }

  }
}
