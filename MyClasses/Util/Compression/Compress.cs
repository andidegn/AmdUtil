using AMD.Util.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Compression
{
  public static class Compress
  {
    public static byte[] Zip(string str)
    {
      return Zip(str.GetBytes());
    }

    public static byte[] Zip(byte[] bytes)
    {

      using (var msi = new MemoryStream(bytes))
      {
        using (var mso = new MemoryStream())
        {
          using (var gs = new GZipStream(mso, CompressionMode.Compress))
          {
            msi.CopyTo(gs);
          }

          return mso.ToArray();
        }
      }
    }

    public static string UnzipToString(byte[] bytes)
    {
      return Unzip(bytes).GetString();
    }

    public static byte[] Unzip(byte[] bytes)
    {
      using (var msi = new MemoryStream(bytes))
      {
        using (var mso = new MemoryStream())
        {
          using (var gs = new GZipStream(msi, CompressionMode.Decompress))
          {
            gs.CopyTo(mso);
          }
          return mso.ToArray();
        }
      }
    }

    private static void CopyTo(Stream src, Stream dest)
    {
      byte[] bytes = new byte[4096];

      int cnt;

      while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
      {
        dest.Write(bytes, 0, cnt);
      }
    }
  }
}
