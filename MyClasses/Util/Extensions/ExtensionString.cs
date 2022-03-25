using AMD.Util.Log;
using AMD.Util.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AMD.Util.Extensions
{
  public static class ExtensionString
  {
    /// <summary>
    /// Constant for fixing ToString rounding issues
    /// </summary>
    public const String DoubleFixedPoint = "0.###################################################################################################################################################################################################################################################################################################################################################";

    public static bool IsNumber(this String s)
    {
      return StringValidater.ValidateNumber(s);
    }

    public static bool IsHexNumber(this String s)
    {
      return StringValidater.ValidateHex(s);
    }

    public static bool IsValidPropertyName(this String s)
    {
      return StringValidater.ValidatePropertyName(s);
    }

    public static bool IsValidFunctionName(this String s)
    {
      return StringValidater.ValidatePropertyName(s);
    }

    /// <summary>
    /// Returns a byte array with the char ASCII values of the string letters
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static byte[] GetBytes(this String s)
    {
      byte[] bArr = new byte[s.Length];
      for (int i = 0; i < s.Length; i++)
      {
        bArr[i] = (byte)s[i];
      }
      return bArr;
    }

    /// <summary>
    /// Returns a byte array with the parsed hex string value
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static byte[] GetBytesFromHex(this String s)
    {
      string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      byte[] bytes = new byte[parts.Length];
      for (int i = 0; i < parts.Length; i++)
      {
        bytes[i] = Convert.ToByte(parts[i], 16);
      }
      return bytes;

      return (from h in Enumerable.Range(0, s.Length)
              where (h % 2 == 0)
              select Convert.ToByte(s.Substring(h, 2), 16)).ToArray<byte>();
    }

    /// <summary>
    /// Returns a string where all toExclude chars have been removed
    /// </summary>
    /// <param name="s"></param>
    /// <param name="toExclude"></param>
    /// <returns></returns>
    public static String ExcludeAll(this String s, char toExclude)
    {
      StringBuilder sb = new StringBuilder(s.Length);
      for (int i = 0; i < s.Length; i++)
      {
        char c = s[i];
        if (c != toExclude)
        {
          sb.Append(c);
        }
      }
      return sb.ToString();
    }
    public static String Replace(this string s, char[] separators, String newVal)
    {
      String[] temp;

      temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
      return String.Join(newVal, temp);
    }

    public static String[] Split(this String s, int desiredLength, bool strict = false)
    {

      EnsureValid(s, desiredLength, strict);

      if (s.Length == 0)
      {
        return new String[0];
      }

      int numberOfItems = s.Length / desiredLength;

      int remaining = (s.Length > numberOfItems * desiredLength) ? 1 : 0;

      IList<String> splitted = new List<String>(numberOfItems + remaining);

      for (int i = 0; i < numberOfItems; i++)
      {
        splitted.Add(s.Substring(i * desiredLength, desiredLength));
      }

      if (remaining != 0)
      {
        splitted.Add(s.Substring(numberOfItems * desiredLength));
      }

      return splitted.ToArray();
    }

    private static void EnsureValid(String value, int desiredLength, bool strict)
    {
      if (value == null) { throw new ArgumentNullException(nameof(value)); }

      if (value.Length == 0 && desiredLength != 0)
      {
        throw new ArgumentException($"The passed {nameof(value)} may not be empty if the {nameof(desiredLength)} <> 0");
      }

      if (value.Length != 0 && desiredLength < 1) { throw new ArgumentException($"The value of {nameof(desiredLength)} needs to be > 0"); }

      if (strict && (value.Length % desiredLength != 0))
      {
        throw new ArgumentException($"The passed {nameof(value)}'s length can't be split by the {nameof(desiredLength)}");
      }
    }

    public static bool AreAllNullOrWhiteSpace(this IEnumerable<String> strings)
    {
      bool retValue = true;
      foreach (String str in strings)
      {
        if (!String.IsNullOrWhiteSpace(str))
        {
          retValue = false;
          break;
        }
      }
      return retValue;
    }
  }
}
