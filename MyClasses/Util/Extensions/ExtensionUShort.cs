using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Extensions
{

  public static class ExtensionUShort
  {
    public static byte[] GetBytes(this ushort[] data, Endian endian = Endian.Big)
    {
      List<byte> byteData = new List<byte>();
      foreach (ushort us in data)
      {
        if (Endian.Big == endian)
        {
          byteData.Add((byte)((us >> 8) & 0xFF));
          byteData.Add((byte)(us & 0xFF));
        }
        else
        {
          byteData.Add((byte)(us & 0xFF));
          byteData.Add((byte)((us >> 8) & 0xFF));
        }
      }
      return byteData.ToArray();
    }
  }
}
