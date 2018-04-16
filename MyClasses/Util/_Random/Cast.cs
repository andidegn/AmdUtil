using System;

namespace AMD.Util
{
  public static class Cast<T>
  {
    public static T DirectCastTo<T>(object input)
    {
      return (T)input;
    }

    public static T ConvertTo<T>(object input)
    {
      return (T)Convert.ChangeType(input, typeof(T));
    }
  }
}
