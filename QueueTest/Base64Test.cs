using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AMD.Util.Extensions;

namespace QueueTest
{

  public class RandomStringGenerator
  {
    // Characters that are allowed in Base64 encoding (A-Z, a-z, 0-9, +, /, =)
    private static readonly string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    private static readonly Random random = new Random();

    // Generate a list of 10000 random strings with 50% valid Base64
    public static List<string> GenerateRandomStrings(int totalCount, int maxLength)
    {
      var randomStrings = new List<string>();

      for (int i = 0; i < totalCount; i++)
      {
        bool isBase64 = i < totalCount / 2; // First 50% will be valid Base64

        string randomString = isBase64 ? GenerateValidBase64String(random.Next(1, maxLength + 1)) : GenerateInvalidBase64String(random.Next(1, maxLength + 1));
        randomStrings.Add(randomString);
      }

      return randomStrings;
    }

    // Generate a valid Base64 string
    private static string GenerateValidBase64String(int length)
    {
      // Base64 encoded strings are usually generated from bytes, so we'll first create random bytes
      byte[] bytes = new byte[(length * 3) / 4]; // Base64 encodes 3 bytes into 4 characters
      random.NextBytes(bytes);
      if (bytes.Length == 0)
      {
        bytes = new byte[] { (byte)random.Next(byte.MaxValue) };
      }

      // Convert the byte array into a valid Base64 string
      string base64String = Convert.ToBase64String(bytes);
      if (string.IsNullOrWhiteSpace(base64String))
      {

      }
      return base64String;
      // Ensure the string does not exceed the maximum length
      return base64String.Substring(0, Math.Min(base64String.Length, length));
    }

    // Generate an invalid Base64 string by randomly choosing characters that don't fit the Base64 character set
    private static string GenerateInvalidBase64String(int length)
    {
      // Generate random characters that don't belong to Base64 set (Base64 allows A-Z, a-z, 0-9, +, /, and =)
      var validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      var realInvalidChars = "!@#$%^&*()_[]{}|;:,.<>?~`";
      var sb = new StringBuilder(length);
      int randomInsertIndex = random.Next(length);

      for (int i = 0; i < length; i++)
      {
        if (i == randomInsertIndex)
        {
          sb.Append(realInvalidChars[random.Next(realInvalidChars.Length)]);
        }
        else
        {
          sb.Append(validChars[random.Next(validChars.Length)]);
        }
      }

      return sb.ToString();
    }
  }
  internal class Base64Test
  {

    public static void TestBase64(string[] args)
    {
      var randomStrings = RandomStringGenerator.GenerateRandomStrings(1000000, 70).ToArray();
      bool[] valid1 = new bool[randomStrings.Length];
      bool[] valid2 = new bool[randomStrings.Length];
      bool isEqual = false;

      TimeSpan time1 = new TimeSpan();
      TimeSpan time2 = new TimeSpan();

      Stopwatch sw = new Stopwatch();

      for (int j = 0; j < 100; j++)
      {
        GC.Collect();
        sw.Restart();
        for (int i = 0; i < randomStrings.Length; i++)
        {
          valid1[i] = randomStrings[i].IsBase64();
        }
        sw.Stop();
        time1 = sw.Elapsed;

        GC.Collect();
        sw.Restart();
        for (int i = 0; i < randomStrings.Length; i++)
        {
          valid2[i] = randomStrings[i].IsBase64();
        }
        sw.Stop();
        time2 = sw.Elapsed;

        isEqual = valid1.SequenceEqual(valid2);
        Console.WriteLine($"Time1 = {time1} - Time2 = {time2} - IsEqual = {isEqual} - count = {valid1.Where(x => x == true).Count()}");
        if (!isEqual)
        {
          for (int i = 0; i < valid1.Length; i++)
          {
            //if (valid1[i] != valid2[i])
            {
              Console.WriteLine($"{i} {valid1[i]} {valid2[i]} - Data: {randomStrings[i]}");
            }
          }
        }
      }
      Console.ReadKey();
    }

  }
}
