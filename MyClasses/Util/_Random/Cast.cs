using System;

namespace AMD.Util
{
  /// <summary>
  /// Cast helper
  /// </summary>
  public static class Cast
  {
    /// <summary>
    /// Directly casts the input the type of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public static T DirectCastTo<T>(object input)
    {
      return (T)input;
    }

    /// <summary>
    /// Converts input to the type of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public static T ConvertTo<T>(object input)
    {
      return (T)Convert.ChangeType(input, typeof(T));
    }
  }
}
