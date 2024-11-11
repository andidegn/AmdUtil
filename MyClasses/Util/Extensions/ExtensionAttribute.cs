using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Extensions
{
  public static class ExtensionAttribute
  {
    public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
    {
      Type enumType = value.GetType();
      string name = Enum.GetName(enumType, value);
      return enumType.GetField(name)?.GetCustomAttributes(false)?.OfType<TAttribute>()?.SingleOrDefault();
    }

    public static TAttribute GetAttribute<TAttribute>(this Action value) where TAttribute : Attribute
    {
      Type type = value.GetType();
      string name = Enum.GetName(type, value);
      return type.GetField(name)?.GetCustomAttributes(false)?.OfType<TAttribute>()?.SingleOrDefault();
    }
  }
}
