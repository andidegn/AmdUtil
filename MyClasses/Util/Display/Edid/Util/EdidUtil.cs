using AMD.Util.Extensions;
using AMD.Util.Log;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AMD.Util.Display.Edid;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using AMD.Util.Display.DDCCI.Util;

namespace AMD.Util.Display.Edid.Util
{
  public static class EdidUtil
  {
    private static readonly byte[] DescripterHeaderDeviceName = new byte[] { 0x00, 0x00, 0x00, 0xFC };

    /// <summary>
    /// Gets the Edid stored in Windows Registry by searching for DeviceFriendlyName in the Edid VESA descripter blocks
    /// </summary>
    /// <param name="deviceFriendlyName"></param>
    /// <returns></returns>
    public static byte[] GetRegEdidFromNameInEdid(string deviceFriendlyName)
    {
      byte[] retVal = null;
      int subIndex, indexOfFirstDescripterBlock = 0x48;
      foreach (byte[] edid in GetAllRegEdid())
      {
        if (edid != null && 0x80 <= edid.Length && 0x100 >= edid.Length)
        {
          try
          {
            // 0x48 is the first configurable VESA descripter block address. 0x12 is the length of the blocks.
            for (int i = 0; i < 3; i++)
            {
              subIndex = indexOfFirstDescripterBlock + (i * 18);
              if (edid.SubArray(subIndex, DescripterHeaderDeviceName.Length).SequenceEqual(DescripterHeaderDeviceName))
              {
                if (edid.SubArray(subIndex, 18).GetString().Contains(deviceFriendlyName))
                {
                  retVal = edid;
                  break;
                }
              }
            }
            if (null != retVal)
            {
              break;
            }
          }
          catch (Exception ex)
          {
            LogWriter.Instance.WriteToLog(ex, $"Exception in GetRegEdidFromNameInEdid. Edid length: {edid.Length}");
          }
        }
        //else
        //{
        //  LogWriter.Instance.WriteToLog(LogMsgType.Error, $"Edid array is not long enough: {edid?.Length}");
        //}
      }
      return retVal;
    }

    /// <summary>
    /// Gets all Edids stored in Windows Registry
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<byte[]> GetAllRegEdid()
    {
      //Open the Display Reg-Key
      RegistryKey regDisplay = Registry.LocalMachine;
      bool found = false;
      byte[] edid = null;
      try
      {
        regDisplay = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\DISPLAY");
        found = true;
      }
      catch (Exception ex)
      {
        LogWriter.Instance.WriteToLog(ex);
      }
      if (found && null != regDisplay)
      {

        //Get all MonitorIDss
        foreach (string monitorId in regDisplay.GetSubKeyNames())
        {
          RegistryKey regMonitorId = regDisplay.OpenSubKey(monitorId);

          if (null != regMonitorId)
          {
            //Get all Plug&Play ID's
            foreach (string pnpId in regMonitorId.GetSubKeyNames())
            {
              RegistryKey regPnpId = regMonitorId.OpenSubKey(pnpId);
              if (regPnpId != null)
              {
                string[] subKeys = regPnpId.GetSubKeyNames();

                //Check if Monitor is active
                if (subKeys.Contains("Device Parameters"))
                {
                  string driverId = regPnpId.GetValue("Driver", null) as string;
                  RegistryKey regDeviceParameters = regPnpId.OpenSubKey("Device Parameters");

                  //Get the EDID code
                  yield return regDeviceParameters.GetValue("EDID", null) as byte[];

                }
              }
            }
          }
        }
      }
    }

    #region Another method which seems better
    #region Windows API stuff
    private static Guid GUID_CLASS_MONITOR = new Guid(0x4d36e96e, 0xe325, 0x11ce, 0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18);
    private const int DIGCF_PRESENT = 0x00000002;
    private const int ERROR_NO_MORE_ITEMS = 259;
    #endregion // Windows API stuff

    #region Windows REG stuff??
    const int DICS_FLAG_GLOBAL = 0x00000001;
    const int DIREG_DEV = 0x00000001;
    const int KEY_READ = 0x20019;
    #endregion // Windows REG stuff??

    private static List<EDID> edidList;

    public static List<EDID> GetEDID(bool forceRecheck = false)
    {
      if (forceRecheck || null == edidList)
      {
        edidList = new List<EDID>();
        IntPtr pGuid = Marshal.AllocHGlobal(Marshal.SizeOf(GUID_CLASS_MONITOR));
        Marshal.StructureToPtr(GUID_CLASS_MONITOR, pGuid, false);
        IntPtr hDevInfo = NativeMethods.SetupDiGetClassDevsEx(pGuid, null, IntPtr.Zero, DIGCF_PRESENT, IntPtr.Zero, null, IntPtr.Zero);

        NativeStructures.DISPLAY_DEVICE dd = new NativeStructures.DISPLAY_DEVICE();
        dd.cb = Marshal.SizeOf(typeof(NativeStructures.DISPLAY_DEVICE));

        UInt32 dev = 0;
        string DeviceID;
        bool foundDevice = false;

        while (NativeMethods.EnumDisplayDevices(null, dev, ref dd, 0) && !foundDevice)
        {
          NativeStructures.DISPLAY_DEVICE ddMon = new NativeStructures.DISPLAY_DEVICE();
          ddMon.cb = Marshal.SizeOf(typeof(NativeStructures.DISPLAY_DEVICE));
          UInt32 devMon = 0;

          while (NativeMethods.EnumDisplayDevices(dd.DeviceName, devMon, ref ddMon, 0) && !foundDevice)
          {
            if ((ddMon.StateFlags & NativeStructures.DisplayDeviceStateFlags.AttachedToDesktop) != 0 && (ddMon.StateFlags & NativeStructures.DisplayDeviceStateFlags.MirroringDriver) == 0)
            {
              (foundDevice, DeviceID) = GetActualEDID(edidList);
            }
            devMon++;

            ddMon = new NativeStructures.DISPLAY_DEVICE();
            ddMon.cb = Marshal.SizeOf(typeof(NativeStructures.DISPLAY_DEVICE));
          }

          dd = new NativeStructures.DISPLAY_DEVICE();
          dd.cb = Marshal.SizeOf(typeof(NativeStructures.DISPLAY_DEVICE));
          dev++;
        }
      }

      return edidList;
    }

    private static (bool success, string deviceId) GetActualEDID(List<EDID> lsi)
    {
      bool success = false;
      string deviceId = string.Empty;
      IntPtr pGuid = Marshal.AllocHGlobal(Marshal.SizeOf(GUID_CLASS_MONITOR));
      Marshal.StructureToPtr(GUID_CLASS_MONITOR, pGuid, false);
      IntPtr hDevInfo = NativeMethods.SetupDiGetClassDevsEx(pGuid, null, IntPtr.Zero, DIGCF_PRESENT, IntPtr.Zero, null, IntPtr.Zero);

      if (null != hDevInfo)
      {
        success = true;
        for (int i = 0; Marshal.GetLastWin32Error() != ERROR_NO_MORE_ITEMS; ++i)
        {
          NativeStructures.SP_DEVINFO_DATA devInfoData = new NativeStructures.SP_DEVINFO_DATA();
          devInfoData.cbSize = Marshal.SizeOf(typeof(NativeStructures.SP_DEVINFO_DATA));

          if (NativeMethods.SetupDiEnumDeviceInfo(hDevInfo, i, ref devInfoData) > 0)
          {
            UIntPtr hDevRegKey = NativeMethods.SetupDiOpenDevRegKey(hDevInfo, ref devInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);

            if (hDevRegKey == null)
            {
              continue;
            }

            EDID edid = PullEDID(hDevRegKey);
            if (null != edid)
            {
              lsi.Add(edid);
            }
            NativeMethods.RegCloseKey(hDevRegKey);
          }
        }
      }

      Marshal.FreeHGlobal(pGuid);

      return (success, deviceId);
    }

    private static EDID PullEDID(UIntPtr hDevRegKey)
    {
      //ScreenInformation si = null;
      EDID edid = null;
      StringBuilder valueName = new StringBuilder(128);
      uint ActualValueNameLength = 128;

      byte[] EDIdata = new byte[1024];
      IntPtr pEDIdata = Marshal.AllocHGlobal(EDIdata.Length);
      Marshal.Copy(EDIdata, 0, pEDIdata, EDIdata.Length);

      int size = 1024;
      for (uint i = 0, retValue = ScreenInterrogatory.ERROR_SUCCESS; retValue != ERROR_NO_MORE_ITEMS; i++)
      {
        retValue = NativeMethods.RegEnumValue(hDevRegKey, i, valueName, ref ActualValueNameLength, IntPtr.Zero, IntPtr.Zero, pEDIdata, ref size);

        string data = valueName.ToString();
        if (retValue != ScreenInterrogatory.ERROR_SUCCESS || !data.Contains("EDID"))
        {
          continue;
        }

        if (1 > size)
        {
          continue;
        }

        byte[] actualData = new byte[size];
        Marshal.Copy(pEDIdata, actualData, 0, size);
        string hex = System.Text.Encoding.ASCII.GetString(actualData);
        edid = new EDID(actualData);
      }

      Marshal.FreeHGlobal(pEDIdata);
      return edid;
    }
    #endregion // Another method which seems better
  }




  /*



  #region Windows API stuff
  static Guid GUID_CLASS_MONITOR = new Guid(0x4d36e96e, 0xe325, 0x11ce, 0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18);
  const int DIGCF_PRESENT = 0x00000002;
  const int ERROR_NO_MORE_ITEMS = 259;

  [DllImport("advapi32.dll", SetLastError = true)]
  static extern uint RegEnumValue(
        UIntPtr hKey,
        uint dwIndex,
        StringBuilder lpValueName,
        ref uint lpcValueName,
        IntPtr lpReserved,
        IntPtr lpType,
        IntPtr lpData,
        ref int lpcbData);

  [Flags()]
  public enum DisplayDeviceStateFlags : int
  {
    /// <summary>The device is part of the desktop.</summary>
    AttachedToDesktop = 0x1,
    MultiDriver = 0x2,
    /// <summary>The device is part of the desktop.</summary>
    PrimaryDevice = 0x4,
    /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
    MirroringDriver = 0x8,
    /// <summary>The device is VGA compatible.</summary>
    VGACompatible = 0x10,
    /// <summary>The device is removable; it cannot be the primary display.</summary>
    Removable = 0x20,
    /// <summary>The device has more display modes than its output devices support.</summary>
    ModesPruned = 0x8000000,
    Remote = 0x4000000,
    Disconnect = 0x2000000
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
  public struct DISPLAY_DEVICE
  {
    [MarshalAs(UnmanagedType.U4)]
    public int cb;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string DeviceName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceString;
    [MarshalAs(UnmanagedType.U4)]
    public DisplayDeviceStateFlags StateFlags;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceKey;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct SP_DEVINFO_DATA
  {
    public int cbSize;
    public Guid ClassGuid;
    public uint DevInst;
    public IntPtr Reserved;
  }

  [DllImport("setupapi.dll")]
  internal static extern IntPtr SetupDiGetClassDevsEx(IntPtr ClassGuid,
      [MarshalAs(UnmanagedType.LPStr)]String enumerator,
      IntPtr hwndParent, Int32 Flags, IntPtr DeviceInfoSet,
      [MarshalAs(UnmanagedType.LPStr)]String MachineName, IntPtr Reserved);

  [DllImport("setupapi.dll", SetLastError = true)]
  internal static extern Int32 SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet,
      Int32 MemberIndex, ref SP_DEVINFO_DATA DeviceInterfaceData);

  [DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
  public static extern UIntPtr SetupDiOpenDevRegKey(
      IntPtr hDeviceInfoSet,
      ref SP_DEVINFO_DATA deviceInfoData,
      int scope,
      int hwProfile,
      int parameterRegistryValueKind,
      int samDesired);

  [DllImport("user32.dll")]
  public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

  [DllImport("advapi32.dll", SetLastError = true)]
  public static extern int RegCloseKey(
      UIntPtr hKey);
  #endregion

  public static List<ScreenInformation> GetEDID()
  {
    List<ScreenInformation> lsi = new List<ScreenInformation>();
    IntPtr pGuid = Marshal.AllocHGlobal(Marshal.SizeOf(GUID_CLASS_MONITOR));
    Marshal.StructureToPtr(GUID_CLASS_MONITOR, pGuid, false);
    IntPtr hDevInfo = SetupDiGetClassDevsEx(
        pGuid,
        null,
        IntPtr.Zero,
        DIGCF_PRESENT,
        IntPtr.Zero,
        null,
        IntPtr.Zero);

    DISPLAY_DEVICE dd = new DISPLAY_DEVICE();
    dd.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
    UInt32 dev = 0;

    string DeviceID;
    bool bFoundDevice = false;
    while (EnumDisplayDevices(null, dev, ref dd, 0) && !bFoundDevice)
    {
      DISPLAY_DEVICE ddMon = new DISPLAY_DEVICE();
      ddMon.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
      UInt32 devMon = 0;

      while (EnumDisplayDevices(dd.DeviceName, devMon, ref ddMon, 0) && !bFoundDevice)
      {
        if ((ddMon.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) != 0 && (ddMon.StateFlags & DisplayDeviceStateFlags.MirroringDriver) == 0)
        {
          bFoundDevice = GetActualEDID(out DeviceID, lsi);
        }
        devMon++;

        ddMon = new DISPLAY_DEVICE();
        ddMon.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
      }

      dd = new DISPLAY_DEVICE();
      dd.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
      dev++;
    }

    return lsi;
  }

  const int DICS_FLAG_GLOBAL = 0x00000001;
  const int DIREG_DEV = 0x00000001;
  const int KEY_READ = 0x20019;
  const int KEY_QUERY_VALUE = 0x0001;
  private static bool GetActualEDID(out string DeviceID, List<ScreenInformation> lsi)
  {
    IntPtr pGuid = Marshal.AllocHGlobal(Marshal.SizeOf(GUID_CLASS_MONITOR));
    Marshal.StructureToPtr(GUID_CLASS_MONITOR, pGuid, false);
    IntPtr hDevInfo = SetupDiGetClassDevsEx(
        pGuid,
        null,
        IntPtr.Zero,
        DIGCF_PRESENT,
        IntPtr.Zero,
        null,
        IntPtr.Zero);

    DeviceID = string.Empty;

    if (null == hDevInfo)
    {
      Marshal.FreeHGlobal(pGuid);
      return false;
    }

    for (int i = 0; Marshal.GetLastWin32Error() != ERROR_NO_MORE_ITEMS; ++i)
    {
      SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();
      devInfoData.cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));

      if (SetupDiEnumDeviceInfo(hDevInfo, i, ref devInfoData) > 0)
      {
        UIntPtr hDevRegKey = SetupDiOpenDevRegKey(
            hDevInfo,
            ref devInfoData,
            DICS_FLAG_GLOBAL,
            0,
            DIREG_DEV,
            KEY_READ);

        if (hDevRegKey == null)
          continue;

        ScreenInformation si = PullEDID(hDevRegKey);
        if (si != null)
        {
          lsi.Add(si);
        }
        RegCloseKey(hDevRegKey);
      }
    }

    Marshal.FreeHGlobal(pGuid);

    return true;
  }

  public const int ERROR_SUCCESS = 0;
  private static ScreenInformation PullEDID(UIntPtr hDevRegKey)
  {
    ScreenInformation si = null;
    StringBuilder valueName = new StringBuilder(128);
    uint ActualValueNameLength = 128;

    byte[] EDIdata = new byte[1024];
    IntPtr pEDIdata = Marshal.AllocHGlobal(EDIdata.Length);
    Marshal.Copy(EDIdata, 0, pEDIdata, EDIdata.Length);

    int size = 1024;
    for (uint i = 0, retValue = ERROR_SUCCESS; retValue != ERROR_NO_MORE_ITEMS; i++)
    {
      retValue = RegEnumValue(
          hDevRegKey, i,
          valueName, ref ActualValueNameLength,
          IntPtr.Zero, IntPtr.Zero, pEDIdata, ref size); // EDIdata, pSize);

      string data = valueName.ToString();
      if (retValue != ERROR_SUCCESS || !data.Contains("EDID"))
        continue;

      if (size < 1)
        continue;

      byte[] actualData = new byte[size];
      Marshal.Copy(pEDIdata, actualData, 0, size);
      string hex = System.Text.Encoding.ASCII.GetString(actualData);
      si = new ScreenInformation
      {
        Manufacturer = hex.Substring(90, 17).Trim().Replace("\0", string.Empty).Replace("?", string.Empty),
        Model = hex.Substring(108, 17).Trim().Replace("\0", string.Empty).Replace("?", string.Empty),
        RawEdid = actualData
      };
    }

    Marshal.FreeHGlobal(pEDIdata);
    return si;
  }
  //private static byte[] GetMonitorEDID(IntPtr pDevInfoSet, SP_DEVINFO_DATA deviceInfoData)
  //{
  //  UIntPtr hDeviceRegistryKey = SetupDiOpenDevRegKey(pDevInfoSet, ref deviceInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_QUERY_VALUE);
  //  if (hDeviceRegistryKey == UIntPtr.Zero)
  //  {
  //    throw new Exception("Failed to open a registry key for device-specific configuration information");
  //  }

  //  IntPtr ptrBuff = Marshal.AllocHGlobal((int)256);
  //  try
  //  {
  //    RegistryValueKind lpRegKeyType = RegistryValueKind.Binary;
  //    int length = 256;
  //    uint result = RegQueryValueEx(hDeviceRegistryKey, "EDID", 0, ref lpRegKeyType, ptrBuff, ref length);
  //    if (result != 0)
  //    {
  //      throw new Exception("Can not read registry value EDID for device " + deviceInfoData.ClassGuid);
  //    }
  //  }
  //  finally
  //  {
  //    RegCloseKey(hDeviceRegistryKey);
  //  }
  //  byte[] edidBlock = new byte[256];
  //  Marshal.Copy(ptrBuff, edidBlock, 0, 256);
  //  Marshal.FreeHGlobal(ptrBuff);
  //  return edidBlock;
  //}





  public class ScreenInformation
  {
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public byte[] RawEdid { get; set; }
    private string _edidString;
    public string EdidString
    {
      get
      {
        if (string.IsNullOrWhiteSpace(_edidString) && 0 < RawEdid.Length)
        {
          _edidString = BitConverter.ToString(RawEdid);
        }
        return _edidString;
      }
    }
  }*/
}
