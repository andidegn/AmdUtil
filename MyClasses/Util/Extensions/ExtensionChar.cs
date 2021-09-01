using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Extensions
{
  public static class ExtensionChar
  {
    public static bool IsHexNumber(this char c)
    {
      return ('0' <= c && c <= '9') ||
             ('A' <= c && c <= 'F') ||
             ('a' <= c && c <= 'f');
    }
  }
}
