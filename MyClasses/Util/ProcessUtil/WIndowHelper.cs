using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace AMD.Util.ProcessUtil
{
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
  }

  public static class ProcessHelper
  {
    private const uint SWP_ASYNCWINDOWPOS = 0x4000;
    private const uint SWP_DEFERERASE     = 0x2000;
    private const uint SWP_DRAWFRAME      = 0x0020;
    private const uint SWP_FRAMECHANGED   = 0x0020;
    private const uint SWP_HIDEWINDOW     = 0x0080;
    private const uint SWP_NOACTIVATE     = 0x0010;
    private const uint SWP_NOCOPYBITS     = 0x0100;
    private const uint SWP_NOMOVE         = 0x0002;
    private const uint SWP_NOOWNERZORDER  = 0x0200;
    private const uint SWP_NOREDRAW       = 0x0008;
    private const uint SWP_NOREPOSITION   = 0x0200;
    private const uint SWP_NOSENDCHANGING = 0x0400;
    private const uint SWP_NOSIZE         = 0x0001;
    private const uint SWP_NOZORDER       = 0x0004;
    private const uint SWP_SHOWWINDOW     = 0x0040;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string className, string windowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowRect(IntPtr hwnd, out RECT rect);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    public static Rectangle GetFirstWindowRect(string processName)
    {
      var process = FindWindows(processName).DefaultIfEmpty(null).FirstOrDefault();
      return GetWindowRect(process);
      //IntPtr hwnd = FindWindow("cmd", null);
      //GetWindowRect(wd.MainWindowHandle, out rect);
      //return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
    }

    public static Rectangle GetWindowRect(Process process)
    {
      Rectangle retVal = new Rectangle();
      if (null != process?.MainWindowHandle)
      {
        RECT rect;
        GetWindowRect(process.MainWindowHandle, out rect);
        retVal = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
      }
      return retVal;
    }

    public static void SetWindowPosition(Process process, Rectangle rectangle)
    {
      if (null != process?.MainWindowHandle)
      {
        SetWindowPos(process.MainWindowHandle, IntPtr.Zero, rectangle.X, rectangle.Y, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_SHOWWINDOW);
      }
    }

    public static void SetWindowSize(Process process, Rectangle rectangle)
    {
      if (null != process?.MainWindowHandle)
      {
        SetWindowPos(process.MainWindowHandle, IntPtr.Zero, 0, 0, rectangle.Width, rectangle.Height, SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_SHOWWINDOW);
      }
    }

    public static IEnumerable<Process> FindWindows(string processName)
    {
      Process[] processes = Process.GetProcesses();
      List<Process> retVal = new List<Process>();

      for (int i = 0; i < processes.Length; i++)
      {
        if (processes[i].ToString().Contains(processName))
        {
          retVal.Add(processes[i]);
        }
      }
      return retVal;
    }
  }
}
