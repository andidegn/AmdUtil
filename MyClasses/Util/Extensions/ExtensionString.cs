﻿using AMD.Util.Log;
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

    public static bool IsBase64(this string s)
    {
      if (string.IsNullOrEmpty(s))
      {
        return false;
      }

      if (s.Length % 4 != 0)
      {
        return false;
      }

      for (int i = 0; i < s.Length; i++)
      {
        char c = s[i];

        if (!(
            (c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z') ||
            (c >= '0' && c <= '9') ||
            c == '+' || c == '/' || c == '='
        ))
        {
          return false;
        }
      }
      return true;
      // This is practically redundant.
      try
      {
        Convert.FromBase64String(s);
        return true;
      }
      catch (FormatException)
      {
        return false;
      }
    }

    public static string ToBase64String(this string s)
    {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(s);
      return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string FromBase64String(this string s)
    {
      var base64EncodedBytes = System.Convert.FromBase64String(s);
      return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static string MyPadLeft(this string str, int length, char? pad = null)
    {
      return string.Concat(new string(Enumerable.Repeat(pad ?? ' ', length - str.Length).ToArray()), str);
    }

    private static readonly Regex sWhitespace = new Regex(@"\s+");
    public static string ReplaceWhitespace(this string input, string replacement)
    {
      return sWhitespace.Replace(input, replacement);
    }

    public static string FastPadLeft(this string str, int length, char? pad = null)
    {
      if (pad is null)
      {
        pad = ' ';
      }
      return MyPadHelper(str, length, pad.Value, true);
    }

    public static string FastPadRight(this string str, int length, char? pad = null)
    {
      if (pad is null)
      {
        pad = ' ';
      }
      return MyPadHelper(str, length, pad.Value, false);
    }

    private enum ePadDirection { Left, Right }

    private static string MyPadHelper(string str, int length, char pad, bool isLeftPad)
    {
      if (str.Length >= length)
      {
        return str;
      }

      length -= str.Length;

      char[] padding = new char[length];
      for (int i = 0; i < length; i++)
      {
        padding[i] = pad;
      }

      return isLeftPad ? string.Concat(new string(padding), str) : string.Concat(str, new string(padding));
    }

    public static string PadBoth(this string str, int length)
    {
      int spaces = length - str.Length;
      int padRight = spaces / 2 + str.Length;
      return str.FastPadRight(padRight).FastPadLeft(length);
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
