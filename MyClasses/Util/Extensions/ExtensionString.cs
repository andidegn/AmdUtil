using AMD.Util.Log;
using AMD.Util.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AMD.Util.Extensions
{
  public static class ExtensionString
  {
    private static Regex regexWhitespace;

    /// <summary>
    /// Constant for fixing ToString rounding issues
    /// </summary>
    public const string DoubleFixedPoint = "0.###################################################################################################################################################################################################################################################################################################################################################";

    public static bool IsNumber(this string s)
    {
      return StringValidater.ValidateNumber(s);
    }

    public static bool IsHexNumber(this string s)
    {
      return StringValidater.ValidateHex(s);
    }

    public static bool IsValidPropertyName(this string s)
    {
      return StringValidater.ValidatePropertyName(s);
    }

    public static bool IsValidFunctionName(this string s)
    {
      return StringValidater.ValidatePropertyName(s);
    }

    /// <summary>
    /// Returns a byte array with the char ASCII values of the string letters
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static byte[] GetBytes(this string s)
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
    public static byte[] GetBytesFromHex(this string s)
    {
      if (null == regexWhitespace)
      {
        regexWhitespace = new Regex(@"\s+");
      }
      s = regexWhitespace.Replace(s, string.Empty);

      if (0 != s.Length % 2)
      {
        s.Insert(0, "0");
      }
      string[] parts = s.Split(2).ToArray();
      //string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
    public static string ExcludeAll(this string s, char toExclude)
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

    public static string Replace(this string s, char[] separators, string newVal)
    {
      string[] temp;

      temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
      return string.Join(newVal, temp);
    }

    public static string[] Split(this string s, int chunkSize, bool strict)
    {

      EnsureValid(s, chunkSize, strict);

      if (s.Length == 0)
      {
        return new string[0];
      }

      //int numberOfItems = s.Length / chunkSize;

      //int remaining = (s.Length > numberOfItems * chunkSize) ? 1 : 0;

      //IList<string> splitted = new List<string>(numberOfItems + remaining);

      //for (int i = 0; i < numberOfItems; i++)
      //{
      //  splitted.Add(s.Substring(i * chunkSize, chunkSize));
      //}

      //if (remaining != 0)
      //{
      //  splitted.Add(s.Substring(numberOfItems * chunkSize));
      //}

      return s.Split(chunkSize).ToArray();
    }

    public static IEnumerable<string> Split(this string str, int chunkSize)
    {
      return Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));
    }

    private static void EnsureValid(string value, int desiredLength, bool strict)
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

    public static bool AreAllNullOrWhiteSpace(this IEnumerable<string> strings)
    {
      bool retValue = true;
      foreach (string str in strings)
      {
        if (!string.IsNullOrWhiteSpace(str))
        {
          retValue = false;
          break;
        }
      }
      return retValue;
    }
  }
}
