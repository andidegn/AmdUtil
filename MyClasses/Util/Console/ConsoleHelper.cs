using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.MyConsole
{
  public static class ConsoleHelper
  {
    public static void ClearLine()
    {
      try
      {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
      }
      catch { } // This will throw exception if run from Keil and Run Independant is not ticked
    }
  }
}
