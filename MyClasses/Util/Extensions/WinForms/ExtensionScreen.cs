using AMD.Util.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMD.Util.Extensions.WinForms
{
  public static class ExtensionScreen
  {
    public static string DeviceFriendlyName(this Screen screen)
    {
      IEnumerable<string> allFriendlyNames = ScreenInterrogatory.GetAllMonitorsFriendlyNames();
      string friendlyName = string.Empty;
      for (var index = 0; index < Screen.AllScreens.Length; index++)
      {
        if (Equals(screen, Screen.AllScreens[index]))
        {
          friendlyName = allFriendlyNames.ToArray()[index];
          break;
        }
      }
      return friendlyName;
    }
  }
}
