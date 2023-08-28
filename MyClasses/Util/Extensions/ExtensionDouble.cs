using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Extensions
{
  public static class ExtensionDouble
  {
    public static bool IsFinite(this double d)
    {
      return !double.IsNaN(d) && !double.IsInfinity(d);
    }
  }
}
