using AMD.Util.Extensions.WinForms;
using AMD.Util.Log;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace AMD.Util.Display
{
  public static class ScreenUtil
  {
    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    public enum DeviceCap
    {
      VERTRES = 10,
      DESKTOPVERTRES = 117,

      // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
    }

    public static System.Drawing.Size GetAspectRatio(System.Drawing.Size resolution)
    {
      System.Drawing.Size retVal = new System.Drawing.Size
      {
        Width = resolution.Width / GCD(resolution),
        Height = resolution.Height / GCD(resolution)
      };

      return retVal;
    }

    public static int GCD(System.Drawing.Size ab)
    {
      int Remainder;

      while (ab.Width != 0)
      {
        Remainder = ab.Height % ab.Width;
        ab.Height = ab.Width;
        ab.Width = Remainder;
      }

      return ab.Height;
    }


    public static string GetDeviceFriendlyNameFromDeviceName(string deviceName)
    {
      string friendlyName = string.Empty;
      return ScreenInterrogatory.GetDeviceFriendlyNameFromDeviceName(deviceName);
      //foreach (Screen screen in Screen.AllScreens)
      //{
      //  if (screen.DeviceName.Equals(deviceName, StringComparison.InvariantCultureIgnoreCase))
      //  {
      //    friendlyName = screen.DeviceFriendlyName();
      //    break;
      //  }
      //}
      //return friendlyName;
    }

    public static bool IsWithinScreenArea(double left, double top)
    {
      bool retVal = false;
      foreach (Screen screen in Screen.AllScreens)
      {
        if (top >= screen.WorkingArea.Top &&
            top < screen.WorkingArea.Bottom &&
            left >= screen.WorkingArea.Left &&
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

    public static Screen GetPrimaryScreen()
    {
      return (from s in Screen.AllScreens
              where s.Primary
              select s).DefaultIfEmpty(Screen.AllScreens.FirstOrDefault()).FirstOrDefault();
    }

    public static Screen GetContainedScreen(double left, double top)
    {
      Screen retVal = null;
      foreach (Screen screen in Screen.AllScreens)
      {
        if (top >= screen.WorkingArea.Top &&
            top < screen.WorkingArea.Bottom &&
            left >= screen.WorkingArea.Left &&
            left < screen.WorkingArea.Right)
        {
          retVal = screen;
          break;
        }
      }
      return retVal;
    }

    public static Screen GetContainedScreen(Rectangle rectangle)
    {
      return GetContainedScreen(rectangle.Left, rectangle.Top);
    }

    /// <summary>
    /// Gets the scaling factor set in Windows
    /// This is detected at load of the application, not if the scaling factor is changed while the application is running
    /// To make this work as live updating as well, it is required to add a Manifest to the application and apply the following XML:
    /// <![CDATA[
    /// <application xmlns="urn:schemas-microsoft-com:asm.v3">
    ///   <windowsSettings>
    ///     <dpiAware xmlns = "http://schemas.microsoft.com/SMI/2005/WindowsSettings" > true / PM </ dpiAware >
    ///     < dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitor</dpiAwareness>
    ///   </windowsSettings>
    /// </application>
    /// ]]>
    /// <para>&lt;application xmlns="urn:schemas-microsoft-com:asm.v3"></para>
    /// <para>   &lt;windowsSettings></para>
    /// <para>     &lt;dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/PM&lt;/dpiAware></para>
    /// <para>     &lt;dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitor&lt;/dpiAwareness></para>
    /// <para>   &lt;/windowsSettings></para>
    /// <para>&lt;/application></para>
    /// </summary>
    /// <returns>1 = 100%, 1.25 = 125% etc.</returns>
    public static double GetScalingFactorAtLoad()
    {
      return Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;
    }

    /// <summary>
    /// Gets the scaling factor set in Windows
    /// This always starts with a scaling factor at 1 (regardless of the windows setting) but changes if the windows scaling factor is changed while the application is running
    /// </summary>
    /// <returns>1 = 100%, 1.25 = 125% etc.</returns>
    public static double GetScalingFactorLive()
    {
      Graphics g = Graphics.FromHwnd(IntPtr.Zero);
      IntPtr desktop = g.GetHdc();
      float LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
      float PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

      float ScreenScalingFactorLive = PhysicalScreenHeight / LogicalScreenHeight;
      return ScreenScalingFactorLive;
      double ScreenScalingFactorLoad = Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;

      return ScreenScalingFactorLoad;
      // This is a hack as:
      // The "ScreenScalingFactor always starts at 1 and updated live while the program is running.
      // The "ScreenFactorLoad has the correct scale on load but does not update live.
      return ScreenScalingFactorLoad * ScreenScalingFactorLive; // 1.25 = 125%
    }

    /// <summary>
    /// This combines the solution from both GetScalingFactorAtLoad and GetScalingFactorLive to attempt to get the scaling factor both at load and live without having to create a Manifest
    /// </summary>
    /// <returns></returns>
    public static double GetScalingFactor()
    {
      return Math.Round(GetScalingFactorAtLoad() * GetScalingFactorLive(), 3);
    }
  }





















  public class DisplayDetails
  {
    public string PnPID { get; set; }

    public string SerialNumber { get; set; }

    public string Model { get; set; }

    public string MonitorID { get; set; }

    public string DriverID { get; set; }

    /// <summary>
    /// The Constructor to create a new instances of the DisplayDetails class...
    /// </summary>
    public DisplayDetails(string sPnPID, string sSerialNumber, string sModel, string sMonitorID, string sDriverID)
    {
      PnPID = sPnPID;
      SerialNumber = sSerialNumber;
      Model = sModel;
      MonitorID = sMonitorID;
      DriverID = sDriverID;
    }

    /// <summary>
    /// This Function returns all Monitor Details
    /// </summary>
    /// <returns></returns>
    static public IEnumerable<DisplayDetails> GetMonitorDetails()
    {
      //Open the Display Reg-Key
      RegistryKey Display = Registry.LocalMachine;
      Boolean bFailed = false;
      try
      {
        Display = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\DISPLAY");
      }
      catch
      {
        bFailed = true;
      }

      if (!bFailed & (Display != null))
      {

        //Get all MonitorIDss
        foreach (string sMonitorID in Display.GetSubKeyNames())
        {
          RegistryKey MonitorID = Display.OpenSubKey(sMonitorID);

          if (MonitorID != null)
          {
            //Get all Plug&Play ID's
            foreach (string sPNPID in MonitorID.GetSubKeyNames())
            {
              RegistryKey PnPID = MonitorID.OpenSubKey(sPNPID);
              if (PnPID != null)
              {
                string[] sSubkeys = PnPID.GetSubKeyNames();

                //Check if Monitor is active
                if (sSubkeys.Contains("Device Parameters"))
                {
                  string DriverID = PnPID.GetValue("Driver", null) as string;
                  RegistryKey DevParam = PnPID.OpenSubKey("Device Parameters");
                  string sSerial = "";
                  string sModel = "";

                  //Define Search Keys
                  string sSerFind = new string(new char[] { (char)00, (char)00, (char)00, (char)0xff });
                  string sModFind = new string(new char[] { (char)00, (char)00, (char)00, (char)0xfc });

                  //Get the EDID code
                  byte[] bObj = DevParam.GetValue("EDID", null) as byte[];
                  if (bObj != null)
                  {
                    //Get the 4 Vesa descriptor blocks
                    string[] sDescriptor = new string[4];
                    sDescriptor[0] = Encoding.Default.GetString(bObj, 0x36, 18);
                    sDescriptor[1] = Encoding.Default.GetString(bObj, 0x48, 18);
                    sDescriptor[2] = Encoding.Default.GetString(bObj, 0x5A, 18);
                    sDescriptor[3] = Encoding.Default.GetString(bObj, 0x6C, 18);

                    //Search the Keys
                    foreach (string sDesc in sDescriptor)
                    {
                      if (sDesc.Contains(sSerFind))
                      {
                        sSerial = sDesc.Substring(4).Replace("\0", "").Trim();
                      }
                      if (sDesc.Contains(sModFind))
                      {
                        sModel = sDesc.Substring(4).Replace("\0", "").Trim();
                      }
                    }
                  }
                  if (!string.IsNullOrEmpty(sPNPID + sSerFind + sModel + sMonitorID + DriverID))
                  {
                    yield return new DisplayDetails(sPNPID, sSerial, sModel, sMonitorID, DriverID);
                  }
                }
              }
            }
          }
        }
      }
    }
  }

  public static class ScreenInterrogatory
  {
    public const int ERROR_SUCCESS = 0;

    #region enums

    public enum QUERY_DEVICE_CONFIG_FLAGS : uint
    {
      QDC_ALL_PATHS                                               = 0x00000001,
      QDC_ONLY_ACTIVE_PATHS                                       = 0x00000002,
      QDC_DATABASE_CURRENT                                        = 0x00000004
    }

    public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
    {
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER                       = 0xFFFFFFFF,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15                        = 0,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO                      = 1,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO             = 2,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO             = 3,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI                         = 4,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI                        = 5,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS                        = 6,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN                       = 8,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI                         = 9,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL        = 10,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED        = 11,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL                = 12,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED                = 13,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE                  = 14,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST                    = 15,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL                    = 0x80000000,
      DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32                = 0xFFFFFFFF
    }

    public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
    {
      DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED                 = 0,
      DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE                 = 1,
      DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED                  = 2,
      DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST  = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
      DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST  = 3,
      DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32                = 0xFFFFFFFF
    }

    public enum DISPLAYCONFIG_ROTATION : uint
    {
      DISPLAYCONFIG_ROTATION_IDENTITY                             = 1,
      DISPLAYCONFIG_ROTATION_ROTATE90                             = 2,
      DISPLAYCONFIG_ROTATION_ROTATE180                            = 3,
      DISPLAYCONFIG_ROTATION_ROTATE270                            = 4,
      DISPLAYCONFIG_ROTATION_FORCE_UINT32                         = 0xFFFFFFFF
    }

    public enum DISPLAYCONFIG_SCALING : uint
    {
      DISPLAYCONFIG_SCALING_IDENTITY                              = 1,
      DISPLAYCONFIG_SCALING_CENTERED                              = 2,
      DISPLAYCONFIG_SCALING_STRETCHED                             = 3,
      DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX                = 4,
      DISPLAYCONFIG_SCALING_CUSTOM                                = 5,
      DISPLAYCONFIG_SCALING_PREFERRED                             = 128,
      DISPLAYCONFIG_SCALING_FORCE_UINT32                          = 0xFFFFFFFF
    }

    public enum DISPLAYCONFIG_PIXELFORMAT : uint
    {
      DISPLAYCONFIG_PIXELFORMAT_8BPP                              = 1,
      DISPLAYCONFIG_PIXELFORMAT_16BPP                             = 2,
      DISPLAYCONFIG_PIXELFORMAT_24BPP                             = 3,
      DISPLAYCONFIG_PIXELFORMAT_32BPP                             = 4,
      DISPLAYCONFIG_PIXELFORMAT_NONGDI                            = 5,
      DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32                      = 0xffffffff
    }

    public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
    {
      DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE                         = 1,
      DISPLAYCONFIG_MODE_INFO_TYPE_TARGET                         = 2,
      DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32                   = 0xFFFFFFFF
    }

    public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
    {
      DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME                   = 1,
      DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME                   = 2,
      DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE         = 3,
      DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME                  = 4,
      DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE            = 5,
      DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE              = 6,
      DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32                      = 0xFFFFFFFF
    }

    public enum DISPLAYCONFIG_TOPOLOGY_ID : uint
    {
      DISPLAYCONFIG_TOPOLOGY_INTERNAL                             = 0x00000001,
      DISPLAYCONFIG_TOPOLOGY_CLONE                                = 0x00000002,
      DISPLAYCONFIG_TOPOLOGY_EXTEND                               = 0x00000004,
      DISPLAYCONFIG_TOPOLOGY_EXTERNAL                             = 0x00000008,
      DISPLAYCONFIG_TOPOLOGY_FORCE_UINT32                         = 0xFFFFFFFF
    }

    #endregion

    #region structs

    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
      public uint LowPart;
      public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_SOURCE_INFO
    {
      public LUID adapterId;
      public uint id;
      public uint modeInfoIdx;
      public uint statusFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_TARGET_INFO
    {
      public LUID adapterId;
      public uint id;
      public uint modeInfoIdx;
      private DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
      private DISPLAYCONFIG_ROTATION rotation;
      private DISPLAYCONFIG_SCALING scaling;
      private DISPLAYCONFIG_RATIONAL refreshRate;
      private DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
      public bool targetAvailable;
      public uint statusFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_RATIONAL
    {
      public uint Numerator;
      public uint Denominator;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_INFO
    {
      public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
      public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
      public uint flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_2DREGION
    {
      public uint cx;
      public uint cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
    {
      public ulong pixelRate;
      public DISPLAYCONFIG_RATIONAL hSyncFreq;
      public DISPLAYCONFIG_RATIONAL vSyncFreq;
      public DISPLAYCONFIG_2DREGION activeSize;
      public DISPLAYCONFIG_2DREGION totalSize;
      public uint videoStandard;
      public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_TARGET_MODE
    {
      public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
      private int x;
      private int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_SOURCE_MODE
    {
      public uint width;
      public uint height;
      public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
      public POINTL position;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DISPLAYCONFIG_MODE_INFO_UNION
    {
      [FieldOffset(0)]
      public DISPLAYCONFIG_TARGET_MODE targetMode;

      [FieldOffset(0)]
      public DISPLAYCONFIG_SOURCE_MODE sourceMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_MODE_INFO
    {
      public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
      public uint id;
      public LUID adapterId;
      public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
    {
      public uint value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
    {
      public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
      public uint size;
      public LUID adapterId;
      public uint id;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
    {
      public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
      public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
      public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
      public ushort edidManufactureId;
      public ushort edidProductCodeId;
      public uint connectorInstance;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string monitorFriendlyDeviceName;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string monitorDevicePath;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_SOURCE_DEVICE_NAME
    {
      public DISPLAYCONFIG_DEVICE_INFO_HEADER header;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string viewGdiDeviceName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_ADAPTER_NAME
    {
      public DISPLAYCONFIG_DEVICE_INFO_HEADER header;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string adapterDevicePath;
    }
    #endregion

    #region DLL-Imports

    [DllImport("user32.dll", EntryPoint = "GetDisplayConfigBufferSizes", SetLastError = true)]
    public static extern int GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

    [DllImport("user32.dll", EntryPoint = "QueryDisplayConfig", SetLastError = true)]
    public static extern int QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS flags, ref uint numPathArrayElements, [Out] DISPLAYCONFIG_PATH_INFO[] PathInfoArray,
                                                ref uint numModeInfoArrayElements, [Out] DISPLAYCONFIG_MODE_INFO[] ModeInfoArray, IntPtr currentTopologyId);

    [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo", SetLastError = true)]
    public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME targetName);

    [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo", SetLastError = true)]
    public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceName);

    [DllImport("user32.dll", EntryPoint = "DisplayConfigGetDeviceInfo", SetLastError = true)]
    public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_ADAPTER_NAME adapterName);

    #endregion

    private static string MonitorFriendlyName(LUID adapterId, uint targetId)
    {
      DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
      {
        header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                    adapterId = adapterId,
                    id = targetId,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
      };
      var error = DisplayConfigGetDeviceInfo(ref deviceName);
      if (error != ERROR_SUCCESS)
      {
        throw new Win32Exception(error);
      }
      //try
      //{
      //  /*
      //   "\\\\?\\DISPLAY#SAM0F71#4&47ce0f2&0&UID200195#{e6f07b5f-ee97-4a90-b076-33f57bf4eaa7}"
      //   "\\\\?\\DISPLAY#AOC2795#4&47ce0f2&0&UID208387#{e6f07b5f-ee97-4a90-b076-33f57bf4eaa7}"
      //   "\\\\?\\DISPLAY#IIC1560#4&47ce0f2&0&UID216579#{e6f07b5f-ee97-4a90-b076-33f57bf4eaa7}"
      //  */
      //  string mdp = deviceName.monitorDevicePath;
      //  return mdp.Substring(mdp.IndexOf('#'), 7);
      //}
      //catch (Exception)
      //{
        return deviceName.monitorFriendlyDeviceName;
      //}
    }

    public static string GetAdapterNameFromDeviceName(string name)
    {
      uint pathCount, modeCount;
      string retVal = string.Empty;
      var error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
      if (error != ERROR_SUCCESS)
      {
        LogWriter.Instance.WriteToLog(LogMsgType.Error, error.ToString());
        //throw new Win32Exception(error);
      }

      var displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
      var displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];
      error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, ref pathCount, displayPaths, ref modeCount, displayModes, IntPtr.Zero);
      if (error != ERROR_SUCCESS)
      {
        LogWriter.Instance.WriteToLog(LogMsgType.Error, error.ToString());
        //throw new Win32Exception(error);
      }

      foreach (DISPLAYCONFIG_PATH_INFO p in displayPaths)
      {
        DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceName = new DISPLAYCONFIG_SOURCE_DEVICE_NAME()
        {
          header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_SOURCE_DEVICE_NAME)),
                    adapterId = p.sourceInfo.adapterId,
                    id = p.sourceInfo.id,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME
                }
        };
        DisplayConfigGetDeviceInfo(ref sourceName);
        if (name.Equals(sourceName.viewGdiDeviceName))
        {
          DISPLAYCONFIG_ADAPTER_NAME adapterName = new DISPLAYCONFIG_ADAPTER_NAME()
          {
            header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_ADAPTER_NAME)),
                    adapterId = p.targetInfo.adapterId,
                    id = p.targetInfo.id,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME
                }
          };
          DisplayConfigGetDeviceInfo(ref adapterName);
          retVal = adapterName.adapterDevicePath;

          if (!string.IsNullOrWhiteSpace(retVal) && retVal.Contains("#"))
          {
            string[] parts = retVal.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            StringBuilder sb = new StringBuilder();
            bool found = false;
            retVal = string.Empty;
            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
            {
              foreach (PropertyData property in managementObject.Properties)
              {
                if (!(property.Value is null))
                {
                  switch (property.Name)
                  {
                    case "PNPDeviceID":
                      if (property.Value is string pnpdid && pnpdid.Contains(parts[1]))
                      {
                        found = true;
                        break;
                      }
                      break;

                    case "Description":
                      retVal = property.Value as string;
                      break;

                    default:
                      break;
                  }
                  //if (property.Value != null && property.Name.Equals("PNPDeviceID") && property.Value is string val && val.Contains(parts[1]))
                  //{
                  //  found = true;
                  //  break;
                  //    //sb.AppendLine($"{property.Name}: {property.Value.ToString()}, Origin: {property.Origin}, Qualifiers: {property.Qualifiers}, Type: {property.Type}");
                  //}
                  //else
                  //{
                  //  sb.AppendLine($"{property.Name}: [UNSUPPORTED], Origin: {property.Origin}, Qualifiers: {property.Qualifiers}, Type: {property.Type}");
                  //}
                }
              }
              if (found && !string.IsNullOrWhiteSpace(retVal))
              {
                break;
              }
            }
            if (!found)
            {
              retVal = "[Undetected]";
            }
          }
        }
      }
      return retVal;
    }

    public static string GetDeviceFriendlyNameFromDeviceName(string name)
    {
      uint pathCount, modeCount;
      string retVal = name;
      var error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
      if (error != ERROR_SUCCESS)
      {
        LogWriter.Instance.WriteToLog(LogMsgType.Error, error.ToString());
        //throw new Win32Exception(error);
      }

      var displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
      var displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];
      error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, ref pathCount, displayPaths, ref modeCount, displayModes, IntPtr.Zero);
      if (error != ERROR_SUCCESS)
      {
        LogWriter.Instance.WriteToLog(LogMsgType.Error, error.ToString());
        //throw new Win32Exception(error);
      }

      foreach (DISPLAYCONFIG_PATH_INFO p in displayPaths)
      {
        DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceName = new DISPLAYCONFIG_SOURCE_DEVICE_NAME()
        {
          header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_SOURCE_DEVICE_NAME)),
                    adapterId = p.sourceInfo.adapterId,
                    id = p.sourceInfo.id,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME
                }
        };
        DisplayConfigGetDeviceInfo(ref sourceName);
        if (name.Equals(sourceName.viewGdiDeviceName))
        {
          DISPLAYCONFIG_TARGET_DEVICE_NAME targetName = new DISPLAYCONFIG_TARGET_DEVICE_NAME()
          {
            header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                    adapterId = p.targetInfo.adapterId,
                    id = p.targetInfo.id,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
          };
          DisplayConfigGetDeviceInfo(ref targetName);
          retVal = targetName.monitorFriendlyDeviceName;
        }
      }
      return retVal;
    }

    public static IEnumerable<string> GetAllMonitorsFriendlyNames()
    {
      uint pathCount, modeCount;
      var error = GetDisplayConfigBufferSizes(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
      if (error != ERROR_SUCCESS)
      {
        LogWriter.Instance.WriteToLog(LogMsgType.Error, error.ToString());
        //throw new Win32Exception(error);
      }

      var displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
      var displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];
      error = QueryDisplayConfig(QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, ref pathCount, displayPaths, ref modeCount, displayModes, IntPtr.Zero);
      if (error != ERROR_SUCCESS)
      {
        LogWriter.Instance.WriteToLog(LogMsgType.Error, error.ToString());
        //throw new Win32Exception(error);
      }

      for (var i = 0; i < modeCount; i++)
      {
        if (displayModes[i].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
        {
          yield return MonitorFriendlyName(displayModes[i].adapterId, displayModes[i].id);
        }
      }
    }
  }
}