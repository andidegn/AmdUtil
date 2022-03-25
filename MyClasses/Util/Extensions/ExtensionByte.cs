using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Extensions
{
  public enum Endian
  {
    Big,
    Little
  }
  public static class ExtensionByte
	{
		public static String GetString(this byte[] bArr)
		{
			return Encoding.Default.GetString(bArr);
		}

    /// <summary>
    /// Returns a string with bytes formatted as hex
    /// </summary>
    /// <param name="bArr">The data</param>
    /// <param name="separator">The separator</param>
    /// <param name="prefix">The prefix before the byte. ex. 0x</param>
    /// <param name="newLineAfter">Insert new line after how many bytes</param>
    /// <returns></returns>
		public static String GetHexString(this byte[] bArr, char separator = ' ', string prefix = "", long newLineAfter = -1)
		{
			StringBuilder sb = new StringBuilder();
			if (bArr == null)
			{
				return String.Empty;
			}
			//if (seperator == '\0')
   //   {
   //     for (int i = 0; i < bArr.Length; i++)
   //     {
			//		sb.Append($"{prefix}{bArr[i]:X2}");
			//	}
			//}
			//else
			{
        for (int i = 0; i < bArr.Length; i++)
        {
          sb.Append($"{prefix}{bArr[i]:X2}");
          if ('\0' != separator)
          {
            sb.Append(((i < bArr.Length - 1) ? separator.ToString() : ""));
          }
          if (0 < newLineAfter && 0 == (i + 1) % newLineAfter)
          {
            sb.AppendLine();
          }
        }
			}
			return sb.ToString();
    }

    public static uint[] GetUIntArray(this byte[] bArr, Endian endian = Endian.Big)
    {
      uint[] uArr = new uint[(bArr.Length + 3) / 4];

      for (int i = 0; i < bArr.Length; i++)
      {
        if (Endian.Little == endian)
        {
          uArr[i / 4] |= (uint)(bArr[i] << (8 * (i % 4)));
        }
        else
        {
          uArr[i / 4] |= (uint)(bArr[i] << (8 * (3 - (i % 4))));
        }
      }
      return uArr;
    }

    public static ushort[] GetUShortArray(this byte[] bArr, Endian endian = Endian.Big)
    {
      ushort[] uArr = new ushort[(bArr.Length + 1) / 2];

      for (int i = 0; i < bArr.Length; i++)
      {
        if (Endian.Little == endian)
        {
          uArr[i / 2] |= (ushort)(bArr[i] << (8 * (i % 2)));
        }
        else
        {
          uArr[i / 2] |= (ushort)(bArr[i] << (8 * (1 - (i % 2))));
        }
      }
      return uArr;
    }

    public static uint?[] GetNullableUIntArray(this byte[] bArr, Endian endian = Endian.Big)
    {
      uint?[] uArr = new uint?[(bArr.Length + 3) / 4];
      int index = 0;

      for (int i = 0; i < bArr.Length; i++)
      {
        index = i / 4;
        if (Endian.Little == endian)
        {
          uArr[index] = uArr[index].GetValueOrDefault(0) | (uint)(bArr[i] << (8 * (i % 4)));
        }
        else
        {
          uArr[index] = uArr[index].GetValueOrDefault(0) | (uint)(bArr[i] << (8 * (3 - (i % 4))));
        }
      }
      return uArr;
    }

    public static ushort?[] GetNullableUShortArray(this byte[] bArr, Endian endian = Endian.Big)
    {
      ushort?[] uArr = new ushort?[(bArr.Length + 1) / 2];
      int index = 0;

      for (int i = 0; i < bArr.Length; i++)
      {
        if (Endian.Little == endian)
        {
          uArr[index] = (ushort)(uArr[index].GetValueOrDefault(0) | (ushort)(bArr[i] << (8 * (i % 2))));
        }
        else
        {
          uArr[index] = (ushort)(uArr[index].GetValueOrDefault(0) | (ushort)(bArr[i] << (8 * (1 - (i % 2)))));
        }
      }
      return uArr;
    }

    public static byte[] Replace(this byte[] b, byte[] separators, byte newVal)
    {
      for (int i = 0; i < b.Length; i++)
      {
        if (separators.Contains(b[i]))
        {
          b[i] = newVal;
        }
      }
      return b;
    }
  }
}
