using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Extensions
{
  public static class ExtensionObject
  {
    public static T DeepClone<T>(this T obj)
    {
      using (var ms = new MemoryStream())
      {
        var formatter = new BinaryFormatter();
        formatter.Serialize(ms, obj);
        ms.Position = 0;

        return (T)formatter.Deserialize(ms);
      }
    }

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
