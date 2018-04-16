using System;
using System.Linq;
using System.Reflection;

namespace AMD.Util.Types
{
  public static class TypeHelper
  {
    public static Type[] GetTypesInNamespace(Assembly assembly, String nameSpace)
    {
      return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
    }
  }
}
