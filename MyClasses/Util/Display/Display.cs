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
