using AMD.Util.Data;
using AMD.Util.Display.DDCCI.MCCSCodeStandard;
using AMD.Util.Display.DDCCI.Util;
using AMD.Util.Display.Edid;
using AMD.Util.Display.Edid.Util;
using AMD.Util.Extensions;
using AMD.Util.GraphicsCard;
using AMD.Util.Log;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AMD.Util.Display
{
  public struct MonitorParameter
  {
    public bool Supported;
    public uint Minimum;
    public uint Maximum;
    public uint Value;
    public uint Original;
  }

  public enum FeatureType
  {
    Brightness,
    Contrast,
    RedDrive,
    GreenDrive,
    BlueDrive,
    RedGain,
    GreenGain,
    BlueGain
  }

  public class Monitor : INotifyPropertyChanged
  {
    #region Interface OnPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion // Interface OnPropertyChanged

    #region Progress
    public IProgress<ProgressChangedEventArgs> Progress { get; set; }
    private int currentProgressPercentage;
    private void Report(string status, int percent)
    {
      currentProgressPercentage = percent;
      Progress?.Report(new ProgressChangedEventArgs(percent, status));
    }
    #endregion // Progress

    public NativeStructures.PHYSICAL_MONITOR physicalMonitor;
    public HandleRef physicalMonitorPtr { get; set; }
    public HandleRef hMonitor { get; set; }
    public NativeStructures.MonitorInfoEx mInfo { get; set; }
    public int Index { get; set; }

    private string name;
    public string Name
    {
      get
      {
        return name;
      }
      set
      {
        name = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(NameOnAdapter));
        OnPropertyChanged(nameof(InfoString));
      }
    }

    private string deviceName;
    public string DeviceName
    {
      get
      { 
        return deviceName; 
      }
      set
      {
        deviceName = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(InfoString));
      }
    }

    private string description;
    public string Description
    {
      get
      {
        return description;
      }
      set
      {
        description = value;
        OnPropertyChanged();
      }
    }

    private string adapterName;
    public string AdapterName
    {
      get
      {
        if (string.IsNullOrWhiteSpace(adapterName))
        {
          AdapterName = ScreenInterrogatory.GetAdapterNameFromDeviceName(mInfo.szDeviceName);
        }
        return adapterName;
      }
      set
      {
        adapterName = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(NameOnAdapter));
        OnPropertyChanged(nameof(InfoString));
      }
    }

    public string NameOnAdapter
    {
      get
      {
        return $"{Name} on {AdapterName}";
      }
    }

    private EDID edid;
    public EDID Edid
    {
      get
      {
        return edid;
      }
      set
      {
        edid = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(InfoString));
      }
    }

    private EDID edidFromReg;
    public EDID EdidFromReg
    {
      get
      {
        return edidFromReg;
      }
      set
      {
        edidFromReg = value;
        OnPropertyChanged();
      }
    }

    private string capabilityString;
    public string CapabilityString
    {
      get
      {
        return capabilityString;
      }
      set
      {
        capabilityString = value;
        OnPropertyChanged();
      }
    }

    private string capabilityStringFormatted;
    public string CapabilityStringFormatted
    {
      get
      {
        return VCPCodes?.CapabilityStringFormatted;
      }
      set
      {
        capabilityStringFormatted = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(InfoString));
      }
    }

    public string InfoString
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{Name} ({DeviceName}) on {AdapterName}");
        sb.AppendLine();
        sb.AppendLine("EDID:");
        sb.AppendLine(StringFormatHelper.GetFormattedMemoryString(0, Edid.RawData.GetNullableUIntArray()));
        sb.AppendLine();
        sb.AppendLine("CapabilityString:");
        sb.AppendLine(CapabilityStringFormatted);
        sb.AppendLine();
        foreach (VCPCode vcp in VCPCodes)
        {
          sb.AppendLine(vcp.ToString());
        }
        return sb.ToString();
      }
    }
    public VCPCodeList VCPCodes { get; set; }

    public bool SupportsHighLevelDDC { get; set; }
    public bool SupportsLowLevelDDC { get; set; }


    private MonitorParameter Brightness;
    private MonitorParameter Contrast;
    private MonitorParameter RedDrive;
    private MonitorParameter GreenDrive;
    private MonitorParameter BlueDrive;
    private MonitorParameter RedGain;
    private MonitorParameter GreenGain;
    private MonitorParameter BlueGain;

    private uint _monitorCapabilities = 0u;
    private uint _supportedColorTemperatures = 0u;

    private LogWriter log;

    public Monitor(NativeStructures.PHYSICAL_MONITOR physicalMonitor, HandleRef hMonitor, NativeStructures.MonitorInfoEx mInfo, bool skipCapabilityCheck, IProgress<ProgressChangedEventArgs> progress = null)
    {
      log = LogWriter.Instance;
      VCPCodes = new VCPCodeList();
      this.Progress = progress;
      this.physicalMonitor = physicalMonitor;
      this.physicalMonitorPtr = new HandleRef(this, physicalMonitor.hPhysicalMonitor);
      this.DeviceName = $"{physicalMonitor.szPhysicalMonitorDescription} [{mInfo.szDeviceName}]";
      this.Description = physicalMonitor.szPhysicalMonitorDescription;
      this.Name = ScreenUtil.GetDeviceFriendlyNameFromDeviceName(mInfo.szDeviceName);
      this.hMonitor = hMonitor;
      this.mInfo = mInfo;
      Report($"Monitor discovered: {Name}, checking capabilities", 0);
      log.WriteToLog(LogMsgType.Notification, "Monitor discovered: {0}", Name);

      Edid = EdidHelper.GetEDID().Where(x => x.FirstMonitorNameFromDescriptor.Equals(Name)).FirstOrDefault();

      byte[] edidFromRegRaw = EdidUtil.GetRegEdidFromNameInEdid(Name);
      if (null != edidFromRegRaw)
      {
        try
        {
          EdidFromReg = new EDID(edidFromRegRaw);
        }
        catch (Exception ex)
        {
          log.WriteToLog(ex, "Exception while trying to parse Edid");
        }
      }

      if (!skipCapabilityCheck)
      {
        CheckCapabilities();
      }
    }
    #region DDC
    #region Get HighLevel
    public uint GetBrightness()
    {
      NativeMethods.GetMonitorBrightness(physicalMonitorPtr, ref Brightness.Minimum, ref Brightness.Value, ref Brightness.Maximum);
      return Brightness.Value;
    }

    public uint GetContrast()
    {
      NativeMethods.GetMonitorContrast(physicalMonitorPtr, ref Contrast.Minimum, ref Contrast.Value, ref Contrast.Maximum);
      return Contrast.Value;
    }

    public uint GetRedGain()
    {
      NativeMethods.GetMonitorRedGreenOrBlueGain(physicalMonitorPtr, NativeStructures.MC_GAIN_TYPE.MC_RED_GAIN, ref RedGain.Minimum, ref RedGain.Value, ref RedGain.Maximum);
      return RedGain.Value;
    }

    public uint GetGreenGain()
    {
      NativeMethods.GetMonitorRedGreenOrBlueGain(physicalMonitorPtr, NativeStructures.MC_GAIN_TYPE.MC_GREEN_GAIN, ref GreenGain.Minimum, ref GreenGain.Value, ref GreenGain.Maximum);
      return GreenGain.Value;
    }

    public uint GetBlueGain()
    {
      NativeMethods.GetMonitorRedGreenOrBlueGain(physicalMonitorPtr, NativeStructures.MC_GAIN_TYPE.MC_BLUE_GAIN, ref BlueGain.Minimum, ref BlueGain.Value, ref BlueGain.Maximum);
      return BlueGain.Value;
    }

    public uint GetRedDrive()
    {
      NativeMethods.GetMonitorRedGreenOrBlueDrive(physicalMonitorPtr, NativeStructures.MC_DRIVE_TYPE.MC_RED_DRIVE, ref RedDrive.Minimum, ref RedDrive.Value, ref RedDrive.Maximum);
      return RedDrive.Value;
    }

    public uint GetGreenDrive()
    {
      NativeMethods.GetMonitorRedGreenOrBlueDrive(physicalMonitorPtr, NativeStructures.MC_DRIVE_TYPE.MC_GREEN_DRIVE, ref GreenDrive.Minimum, ref GreenDrive.Value, ref GreenDrive.Maximum);
      return GreenDrive.Value;
    }

    public uint GetBlueDrive()
    {
      NativeMethods.GetMonitorRedGreenOrBlueDrive(physicalMonitorPtr, NativeStructures.MC_DRIVE_TYPE.MC_BLUE_DRIVE, ref BlueDrive.Minimum, ref BlueDrive.Value, ref BlueDrive.Maximum);
      return BlueDrive.Value;
    }

    public uint GetCurrent(FeatureType ft)
    {
      uint cur = 0;
      switch (ft)
      {
        case FeatureType.Brightness:
          cur = GetBrightness();
          break;
        case FeatureType.Contrast:
          cur = GetContrast();
          break;
        case FeatureType.RedDrive:
          cur = GetRedDrive();
          break;
        case FeatureType.GreenDrive:
          cur = GetGreenDrive();
          break;
        case FeatureType.BlueDrive:
          cur = GetBlueDrive();
          break;
        case FeatureType.RedGain:
          cur = GetRedGain();
          break;
        case FeatureType.GreenGain:
          cur = GetGreenGain();
          break;
        case FeatureType.BlueGain:
          cur = GetBlueGain();
          break;
        default:
          break;
      }
      return cur;
    }

    public uint GetMax(FeatureType ft)
    {
      uint max = 0;
      switch (ft)
      {
        case FeatureType.Brightness:
          max = Brightness.Maximum;
          break;
        case FeatureType.Contrast:
          max = Contrast.Maximum;
          break;
        case FeatureType.RedDrive:
          max = RedDrive.Maximum;
          break;
        case FeatureType.GreenDrive:
          max = GreenDrive.Maximum;
          break;
        case FeatureType.BlueDrive:
          max = BlueDrive.Maximum;
          break;
        case FeatureType.RedGain:
          max = RedDrive.Maximum;
          break;
        case FeatureType.GreenGain:
          max = GreenDrive.Maximum;
          break;
        case FeatureType.BlueGain:
          max = BlueDrive.Maximum;
          break;
        default:
          break;
      }
      return max;
    }

    public uint GetMin(FeatureType ft)
    {
      uint min = 0;
      switch (ft)
      {
        case FeatureType.Brightness:
          min = Brightness.Minimum;
          break;
        case FeatureType.Contrast:
          min = Contrast.Minimum;
          break;
        case FeatureType.RedDrive:
          min = RedDrive.Minimum;
          break;
        case FeatureType.GreenDrive:
          min = GreenDrive.Minimum;
          break;
        case FeatureType.BlueDrive:
          min = BlueDrive.Minimum;
          break;
        case FeatureType.RedGain:
          min = RedDrive.Minimum;
          break;
        case FeatureType.GreenGain:
          min = GreenDrive.Minimum;
          break;
        case FeatureType.BlueGain:
          min = BlueDrive.Minimum;
          break;
        default:
          break;
      }
      return min;
    }

    public uint GetOriginal(FeatureType ft)
    {
      uint org = 0;
      switch (ft)
      {
        case FeatureType.Brightness:
          org = Brightness.Original;
          break;
        case FeatureType.Contrast:
          org = Contrast.Original;
          break;
        case FeatureType.RedDrive:
          org = RedDrive.Original;
          break;
        case FeatureType.GreenDrive:
          org = GreenDrive.Original;
          break;
        case FeatureType.BlueDrive:
          org = BlueDrive.Original;
          break;
        case FeatureType.RedGain:
          org = RedGain.Original;
          break;
        case FeatureType.GreenGain:
          org = GreenGain.Original;
          break;
        case FeatureType.BlueGain:
          org = BlueGain.Original;
          break;
        default:
          break;
      }
      return org;
    }

    public bool IsSupported(FeatureType ft)
    {
      bool res = false;
      switch (ft)
      {
        case FeatureType.Brightness:
          res = Brightness.Supported;
          break;
        case FeatureType.Contrast:
          res = Contrast.Supported;
          break;
        case FeatureType.RedDrive:
        case FeatureType.GreenDrive:
        case FeatureType.BlueDrive:
          res = BlueDrive.Supported;
          break;
        case FeatureType.RedGain:
        case FeatureType.GreenGain:
        case FeatureType.BlueGain:
          res = BlueGain.Supported;
          break;
        default:
          break;
      }
      return res;
    }
    #endregion // Get

    #region Set HighLevel
    public void SetBrightness(uint value)
    {
      Brightness.Value = value;
      NativeMethods.SetMonitorBrightness(physicalMonitorPtr, value);
    }

    public void SetContrast(uint value)
    {
      Contrast.Value = value;
      NativeMethods.SetMonitorContrast(physicalMonitorPtr, value);
    }

    public void SetRedGain(uint value)
    {
      NativeMethods.SetMonitorRedGreenOrBlueGain(physicalMonitorPtr, NativeStructures.MC_GAIN_TYPE.MC_RED_GAIN, value);
    }

    public void SetGreenGain(uint value)
    {
      NativeMethods.SetMonitorRedGreenOrBlueGain(physicalMonitorPtr, NativeStructures.MC_GAIN_TYPE.MC_GREEN_GAIN, value);
    }

    public void SetBlueGain(uint value)
    {
      NativeMethods.SetMonitorRedGreenOrBlueGain(physicalMonitorPtr, NativeStructures.MC_GAIN_TYPE.MC_BLUE_GAIN, value);
    }

    public void SetRedDrive(uint value)
    {
      NativeMethods.SetMonitorRedGreenOrBlueDrive(physicalMonitorPtr, NativeStructures.MC_DRIVE_TYPE.MC_RED_DRIVE, value);
    }

    public void SetGreenDrive(uint value)
    {
      NativeMethods.SetMonitorRedGreenOrBlueDrive(physicalMonitorPtr, NativeStructures.MC_DRIVE_TYPE.MC_GREEN_DRIVE, value);
    }

    public void SetBlueDrive(uint value)
    {
      NativeMethods.SetMonitorRedGreenOrBlueDrive(physicalMonitorPtr, NativeStructures.MC_DRIVE_TYPE.MC_BLUE_DRIVE, value);
    }

    public void SetCurrent(FeatureType ft, uint value)
    {
      switch (ft)
      {
        case FeatureType.Brightness:
          SetBrightness(value);
          break;
        case FeatureType.Contrast:
          SetContrast(value);
          break;
        case FeatureType.RedDrive:
          SetRedDrive(value);
          break;
        case FeatureType.GreenDrive:
          SetGreenDrive(value);
          break;
        case FeatureType.BlueDrive:
          SetBlueDrive(value);
          break;
        case FeatureType.RedGain:
          SetRedGain(value);
          break;
        case FeatureType.GreenGain:
          SetGreenGain(value);
          break;
        case FeatureType.BlueGain:
          SetBlueGain(value);
          break;
        default:
          break;
      }
    }
    #endregion // Set


    public void CheckCapabilities()
    {
      Report("Checking high level capabilities", 25);
      log.WriteToLog(LogMsgType.Notification, "Checking high level capabilities");
      CheckHighLevelCapabilities();
      Report("Checking low level capabilities", 50);
      log.WriteToLog(LogMsgType.Notification, "Checking low level capabilities");
      CheckLowLevelCapabilities();
      Report("Querying VCP codes", 75);
      log.WriteToLog(LogMsgType.Notification, "Querying VCP codes");
      GetVCPCodeValues(VCPCodes, true);
    }

    public void CheckLowLevelCapabilities()
    {
      try
      {
        if (NativeMethods.GetCapabilitiesStringLength(physicalMonitorPtr, out uint pdwCapabilitiesStringLengthInCharacters))
        {
          StringBuilder pszASCIICapabilitiesString = new StringBuilder((int)pdwCapabilitiesStringLengthInCharacters);
          if (NativeMethods.CapabilitiesRequestAndCapabilitiesReply(physicalMonitorPtr, pszASCIICapabilitiesString, pdwCapabilitiesStringLengthInCharacters))
          {
            if (0 < VCPCodes.Count)
            {
              VCPCodes.Clear();
            }
            VCPCodes.Populate(pszASCIICapabilitiesString.ToString());

            SupportsLowLevelDDC = 0 < VCPCodes.Count;
          }
          //var vcpCodes = DDCHelper.GetVcpCodes(pszASCIICapabilitiesString.ToString()).ToArray();
        }

      }
      catch (Exception ex)
      {
        LogWriter.Instance.WriteToLog(ex);
        throw;
      }
    }

    public void CheckHighLevelCapabilities()
    {
      if (SupportsHighLevelDDC = NativeMethods.GetMonitorCapabilities(physicalMonitorPtr, ref _monitorCapabilities, ref _supportedColorTemperatures))
      {
        CheckBrightness();
        CheckContrast();
        CheckRgbDrive();
        CheckRgbGain();

        Brightness.Original = Brightness.Value;
        Contrast.Original = Contrast.Value;
        RedDrive.Original = RedDrive.Value;
        GreenDrive.Original = GreenDrive.Value;
        BlueDrive.Original = BlueDrive.Value;
        RedGain.Original = RedGain.Value;
        GreenGain.Original = GreenGain.Value;
        BlueGain.Original = BlueGain.Value;
      }
      else
      {
        log.WriteToLog(LogMsgType.Error, $"Error calling GetMonitorCapabilities({physicalMonitorPtr.Handle:X8}). \nLastErrorCode: {Marshal.GetLastWin32Error():X8} - Msg: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
      }
    }

    public bool GetAllVCPFeatures(bool refreshOriginalValues = false)
    {
      return GetVCPCodeValues(VCPCodes, refreshOriginalValues);
    }

    public bool GetVCPFeature(VCPCode code)
    {
      return GetVCPFeature(code.Code);
    }

    public bool GetVCPFeature(eVCPCode code)
    {
      bool retVal = false;
      if (VCPCodes.Contains(code))
      {
        VCPCode vcpCode = VCPCodes.Get(code);
        retVal = GetVCPFeature(vcpCode, false);
      }
      return retVal;
    }

    public (bool success, uint currentValue, uint maxValue, eVCPCodeType type) GetVCPFeature(byte code)
    {
      uint currentValue = 0;
      uint maxValue = 0;
      bool retVal = false;
      int retries = 5;
      NativeStructures.MC_VCP_CODE_TYPE codeType = NativeStructures.MC_VCP_CODE_TYPE.MC_MOMENTARY;

      while (!(retVal = NativeMethods.GetVCPFeatureAndVCPFeatureReply(physicalMonitorPtr, code, ref codeType, ref currentValue, ref maxValue)) && 0 < --retries)
      {
        Thread.Sleep(50);
      }
      if (!retVal)
      {
        log.WriteToLog(LogMsgType.Error, $"Error calling GetVCPFeatureAndVCPFeatureReply({physicalMonitorPtr.Handle:X8}, {code:X2}, {codeType}, {currentValue:X4}, {maxValue:X4}). \nLastErrorCode: {Marshal.GetLastWin32Error():X8} - Msg: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
      }
      //LogWriter.Instance.PrintNotification($"{Name} [{(code).ToString("X2")}] {codeType} max: {maxValue:X4}, cur: {currentValue:X4}");
      return (retVal, currentValue, maxValue, NativeStructures.MC_VCP_CODE_TYPE.MC_MOMENTARY == codeType ? eVCPCodeType.ReadOnly : eVCPCodeType.ReadWrite);
    }

    private bool GetVCPCodeValues(VCPCodeList list, bool refreshOriginalValues)
    {
      bool retVal = true;
      int i = 0, len = list.Count;
      foreach (VCPCode code in list)
      {
        Report($"Code found: {code.ToString()}", (100 - currentProgressPercentage) / len * ++i);
        retVal &= GetVCPFeature(code, refreshOriginalValues);
      }
      return retVal;
    }

    private bool GetVCPFeature(VCPCode code, bool refreshOriginalValues)
    {
      bool retVal = false;
      (bool success, uint currentValue, uint maxValue, eVCPCodeType type) result;
      if ((result = GetVCPFeature((byte)code.Code)).success)
      {
        code.Type = result.type;
        code.CurrentValue = result.currentValue;
        code.MaximumValue = result.maxValue;
        if (refreshOriginalValues)
        {
          code.OriginalValue = result.currentValue;
        }
        LogWriter.Instance.WriteToLog(LogMsgType.Notification, code.ToString());

        //if (code.Code == eVCPCode.ManufacturerSpecific0xEA && maxValue == 0xFFFE)
        //{
        //  Stopwatch sw = Stopwatch.StartNew();
        //  List<byte> l = new List<byte>();
        //  for (uint i = 0; i < 55; i++)
        //  {
        //    NativeMethods.SetVCPFeature(physicalMonitorPtr, (byte)code.Code, i);
        //    NativeMethods.GetVCPFeatureAndVCPFeatureReply(physicalMonitorPtr, (byte)code.Code, ref codeType, ref currentValue, ref maxValue);
        //    l.Add((byte)((maxValue >> 8) & 0xFF));
        //    l.Add((byte)(maxValue & 0xFF));
        //    l.Add((byte)((currentValue >> 8) & 0xFF));
        //    l.Add((byte)(currentValue & 0xFF));
        //  }
        //  sw.Stop();
        //  Log.LogWriter.Instance.WriteToLog(Log.LogMsgType.Measurement, $"500 words took {sw.Elapsed.TotalMilliseconds}ms to query. Total bytes stored: {l.Count}");
        //}
        retVal = true;
      }
      return retVal;
    }

    public bool SetVCPFeature(VCPCode code, UInt16 value)
    {
      return SetVCPFeature(code.Code, value);
    }

    public bool SetVCPFeature(eVCPCode code, UInt16 value)
    {
      return SetVCPFeature((byte)code, value);
    }

    public bool SetVCPFeature(byte code, UInt16 value)
    {
      return NativeMethods.SetVCPFeature(physicalMonitorPtr, code, value);
    }

    private void CheckBrightness()
    {
      Brightness.Supported = ((int)NativeStructures.MC_MONITOR_CAPABILITIES.MC_CAPS_BRIGHTNESS & _monitorCapabilities) >= 0;
      if (Brightness.Supported)
      {
        GetBrightness();
      }
    }
    private void CheckContrast()
    {
      Contrast.Supported = ((int)NativeStructures.MC_MONITOR_CAPABILITIES.MC_CAPS_CONTRAST & _monitorCapabilities) >= 0;
      if (Contrast.Supported)
      {
        GetContrast();
      }
    }

    private void CheckRgbDrive()
    {
      RedDrive.Supported =
          GreenDrive.Supported =
          BlueDrive.Supported =
          ((int)NativeStructures.MC_MONITOR_CAPABILITIES.MC_CAPS_RED_GREEN_BLUE_DRIVE) > 0;
      if (RedDrive.Supported)
      {
        GetRedDrive();
        GetGreenDrive();
        GetBlueDrive();
      }
    }

    private void CheckRgbGain()
    {
      RedGain.Supported =
          GreenGain.Supported =
          BlueGain.Supported = ((int)NativeStructures.MC_MONITOR_CAPABILITIES.MC_CAPS_RED_GREEN_BLUE_GAIN) > 0;
      if (RedGain.Supported)
      {
        GetRedGain();
        GetGreenGain();
        GetBlueGain();
      }
    }

    public void Reset()
    {
      foreach (FeatureType ft in EnumUtil.GetValues<FeatureType>())
      {
        if (GetCurrent(ft) != GetOriginal(ft))
        {
          SetCurrent(ft, GetOriginal(ft));
          Thread.Sleep(100);
        }
      }
    }
    #endregion // DDC

    #region EDID
    //private void GetMonitorDetails1()
    //{
    //  Guid MonitorGUID = new Guid(Win32.GUID_DEVINTERFACE_MONITOR);

    //  // We start at the "root" of the device tree and look for all
    //  // devices that match the interface GUID of a monitor
    //  IntPtr h = Win32.SetupDiGetClassDevs(ref MonitorGUID, IntPtr.Zero, IntPtr.Zero, (uint)(Win32.DIGCF_PRESENT | Win32.DIGCF_DEVICEINTERFACE));
    //  if (h.ToInt64() != Win32.INVALID_HANDLE_VALUE)
    //  {
    //    bool Success = true;
    //    uint i = 0;
    //    while (Success)
    //    {
    //      // create a Device Interface Data structure
    //      Win32.SP_DEVICE_INTERFACE_DATA dia = new Win32.SP_DEVICE_INTERFACE_DATA();
    //      dia.cbSize = (uint)Marshal.SizeOf(dia);

    //      // start the enumeration 
    //      Success = Win32.SetupDiEnumDeviceInterfaces(h, IntPtr.Zero, ref MonitorGUID, i, ref dia);
    //      if (Success)
    //      {
    //        // build a DevInfo Data structure
    //        Win32.SP_DEVINFO_DATA da = new Win32.SP_DEVINFO_DATA();
    //        da.cbSize = (uint)Marshal.SizeOf(da);

    //        // build a Device Interface Detail Data structure
    //        Win32.SP_DEVICE_INTERFACE_DETAIL_DATA didd = new Win32.SP_DEVICE_INTERFACE_DETAIL_DATA();
    //        didd.cbSize = (uint)(4 + Marshal.SystemDefaultCharSize); // trust me :)

    //        // now we can get some more detailed information
    //        uint nRequiredSize = 0;
    //        uint nBytes = Win32.BUFFER_SIZE;
    //        if (Win32.SetupDiGetDeviceInterfaceDetail(h, ref dia, ref didd, nBytes, out nRequiredSize, ref da))
    //        {
    //          // Now we get the InstanceID
    //          IntPtr ptrInstanceBuf = Marshal.AllocHGlobal((int)nBytes);
    //          Win32.CM_Get_Device_ID(da.DevInst, ptrInstanceBuf, (int)nBytes, 0);
    //          string InstanceID = Marshal.PtrToStringAuto(ptrInstanceBuf);
    //          Console.WriteLine("InstanceID: {0}", InstanceID);
    //          Marshal.FreeHGlobal(ptrInstanceBuf);

    //          Console.WriteLine("DevicePath: {0}", didd.DevicePath);
    //        }
    //        i++;
    //      }
    //    }
    //  }
    //  Win32.SetupDiDestroyDeviceInfoList(h);
    //}
    #endregion // EDID

    public override string ToString()
    {
      return Name;
    }
  }
}
