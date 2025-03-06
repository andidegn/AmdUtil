using AMD.Util.Display.DDCCI.Util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DisplaySettingsTest
{
  public class DisplaySettings
  {
    private const int ENUM_CURRENT_SETTINGS = -1;
    private const int CDS_UPDATEREGISTRY = 0x01;
    private const int CDS_TEST = 0x02;
    private const int CDS_GLOBAL = 0x08; // Allow unsafe display settings
    private const int DISP_CHANGE_SUCCESSFUL = 0;
    private const int DISPLAY_DEVICE_ATTACHED_TO_DESKTOP = 0x00000001;

    [DllImport("user32.dll", EntryPoint = "EnumDisplaySettings", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumDisplaySettings(string deviceName, int modeNum, [In, Out] ref DEVMODE devMode);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int ChangeDisplaySettingsEx(string deviceName, ref DEVMODE devMode, IntPtr hwnd, int flags, IntPtr param);

    [DllImport("user32.dll")]
    private static extern int ChangeDisplaySettings(IntPtr devMode, int flags);

    public static IEnumerable<string> ListDisplays()
    {
      NativeStructures.DISPLAY_DEVICE d = new NativeStructures.DISPLAY_DEVICE();
      d.cb = Marshal.SizeOf(d);
      uint devNum = 0;

      while (NativeMethods.EnumDisplayDevices(null, devNum, ref d, 0))
      {
        if ((d.StateFlags & NativeStructures.DisplayDeviceStateFlags.AttachedToDesktop) != 0)
        {
          yield return d.DeviceName;
          //yield return $"Display {devNum}: {d.DeviceName} - {d.DeviceString}";
        }
        devNum++;
      }
    }

    /// <summary>
    /// Changes the resolution and refresh rate for a specific display.
    /// </summary>
    public static bool SetDisplayResolution(string displayName, int width, int height, int frequency)
    {
      DEVMODE dm = new DEVMODE();
      dm.dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE));

      var res = EnumDisplaySettings(displayName, ENUM_CURRENT_SETTINGS, ref dm);
      if (res)// != 0)
      {
        dm.dmPelsWidth = (uint)width;
        dm.dmPelsHeight = (uint)height;
        dm.dmDisplayFrequency = (uint)frequency;
        dm.dmFields = 0x80000 | 0x100000 | 0x400000; // DM_PELSWIDTH | DM_PELSHEIGHT | DM_DISPLAYFREQUENCY

        // Test if the new settings are valid
        int testResult = ChangeDisplaySettingsEx(displayName, ref dm, IntPtr.Zero, CDS_TEST, IntPtr.Zero);
        if (testResult == DISP_CHANGE_SUCCESSFUL)
        {
          // Apply new settings
          bool retVal = ChangeDisplaySettingsEx(displayName, ref dm, IntPtr.Zero, CDS_UPDATEREGISTRY | CDS_GLOBAL, IntPtr.Zero) == DISP_CHANGE_SUCCESSFUL;
          DEVMODE nullMode = new DEVMODE();
          ChangeDisplaySettingsEx(null, ref nullMode, IntPtr.Zero, 0, IntPtr.Zero);
          //ChangeDisplaySettings(IntPtr.Zero, 0);
          return retVal;
        }
      }
      return false;
    }
  }

}
