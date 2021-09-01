using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Extensions
{
	public static class ExtensionByte
	{
		public static String GetString(this byte[] bArr)
		{
			return Encoding.Default.GetString(bArr);
		}

		public static String GetHexString(this byte[] bArr, char seperator = ' ')
		{
			StringBuilder sb = new StringBuilder();
			if (bArr == null)
			{
				return String.Empty;
			}
			if (seperator == '\0')
			{
				foreach (byte b in bArr)
				{
					sb.AppendFormat("{0:X2}", b);
				}
			}
			else
			{
        for (int i = 0; i < bArr.Length; i++)
        {
          sb.AppendFormat("{0:X2}{1}", bArr[i], (i < bArr.Length - 1) ? seperator.ToString() : "");
        }
			}
			return sb.ToString();
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
