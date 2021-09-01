using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AMD.Util.Display
{
  public static class ScreenUtil
  {
    public static List<ScreenInfo> AllScreens
    {
      get
      {
        List<ScreenInfo> infoList = new List<ScreenInfo>();
        foreach (var screen in Screen.AllScreens)
        {
          infoList.Add(new ScreenInfo(screen));
        }
        return infoList;
      }
    }

    public static bool IsWithinScreenArea(double left, double top)
    {
      bool retVal = false;
      foreach (ScreenInfo screen in AllScreens)
      {
        if (top > screen.WorkingArea.Top &&
            top < screen.WorkingArea.Bottom &&
            left > screen.WorkingArea.Left &&
            left < screen.WorkingArea.Right)
        {
          retVal = true;
        }
      }
      return retVal;
    }

    public static bool IsWithinScreenArea(Rectangle rectangle)
    {
      return IsWithinScreenArea(rectangle.Left, rectangle.Top);
    }

    public static ScreenInfo GetContainedScreen(double left, double top)
    {
      ScreenInfo retVal = null;
      foreach (ScreenInfo screen in AllScreens)
      {
        if (top > screen.WorkingArea.Top &&
            top < screen.WorkingArea.Bottom &&
            left > screen.WorkingArea.Left &&
            left < screen.WorkingArea.Right)
        {
          retVal = screen;
          break;
        }
      }
      return retVal;
    }

    public static ScreenInfo GetContainedScreen(Rectangle rectangle)
    {
      return GetContainedScreen(rectangle.Left, rectangle.Top);
    }


    public class ScreenInfo
    {
      public int BitsPerPixel { get; set; }
      public Rectangle Bounds { get; set; }
      public String DeviceName { get; set; }
      public bool Primary { get; set; }
      public Rectangle WorkingArea { get; set; }

      public ScreenInfo()
      {

      }

      public ScreenInfo(Screen screen)
      {
        this.BitsPerPixel = screen.BitsPerPixel;
        this.Bounds = screen.Bounds;
        this.DeviceName = screen.DeviceName;
        this.Primary = screen.Primary;
        this.WorkingArea = screen.WorkingArea;
      }
    }

  }
}
