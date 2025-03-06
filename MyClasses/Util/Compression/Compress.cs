using AMD.Util.Compression.SevenZip;
using AMD.Util.Compression.SevenZip.Compression.LZMA;
using AMD.Util.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace AMD.Util.Compression
{

  public static class Compress
  {
    public static byte[] Zip(string str)
    {
      return Zip(str.GetBytes());
    }

    public static byte[] GZipHeaderBytes = { 0x1f, 0x8b, 8, 0, 0, 0, 0, 0, 4, 0 };
    public static byte[] GZipLevel10HeaderBytes = { 0x1f, 0x8b, 8, 0, 0, 0, 0, 0, 2, 0 };

    public static bool IsPossiblyGZippedBytes(this byte[] a)
    {
      var yes = a.Length > 10;

      if (!yes)
      {
        return false;
      }

      var header = a.SubArray(0, 10);

      return header.SequenceEqual(GZipHeaderBytes) || header.SequenceEqual(GZipLevel10HeaderBytes);
    }

    public static MemoryStream SevenZip(MemoryStream inStream)
    {
      inStream.Position = 0;

      CoderPropID[] propIDs =
      {
        CoderPropID.DictionarySize,
        CoderPropID.PosStateBits,
        CoderPropID.LitContextBits,
        CoderPropID.LitPosBits,
        CoderPropID.Algorithm
      };

      object[] properties =
      {
        (1 << 16),
        2,
        3,
        0,
        2
      };

      var outStream = new MemoryStream();
      var encoder = new Encoder();
      encoder.SetCoderProperties(propIDs, properties);
      encoder.WriteCoderProperties(outStream);
      for (var i = 0; i < 8; i++)
        outStream.WriteByte((byte)(inStream.Length >> (8 * i)));
      encoder.Code(inStream, outStream, -1, -1, null);
      outStream.Flush();
      outStream.Position = 0;

      return outStream;
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
