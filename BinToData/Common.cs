using AMD.Util.Log;
using AMD.Util.MyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinToData
{
  internal static class Out
  {
    internal static void Print(string text, ConsoleColor? color = null)
    {
      LogWriter.Instance.PrintNotification(null, "{0}", text);
      if (color is null)
      {
        ConsoleHelper.PrintLine(text);
      }
      else
      {
        ConsoleHelper.PrintLine(text, color.Value);
      }
    }

    internal static void Error(string text)
    {
      LogWriter.Instance.PrintError(text);
      ConsoleHelper.PrintErrorLine(text);
    }

    internal static void Exception(Exception ex)
    {
      LogWriter.Instance.PrintException(ex);
      ConsoleHelper.PrintErrorLine(ex.ToString());
    }
  }
}
