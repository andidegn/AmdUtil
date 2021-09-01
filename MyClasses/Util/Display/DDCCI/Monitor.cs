using AMD.Util.Display.DDCCI.Util;
using System;
using System.Collections.Generic;
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

  public class Monitor
  {
    #region Declarations

    public IntPtr physicalMonitorPtr { get; set; }
    public int Index { get; set; }
    public string Name { get; set; }
    public NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE Type { get; set; }
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

    #endregion


    public Monitor(NativeStructures.PHYSICAL_MONITOR physicalMonitor)
    {
      physicalMonitorPtr = physicalMonitor.hPhysicalMonitor;
      Name = physicalMonitor.szPhysicalMonitorDescription;
      CheckCapabilities();
    }

    #region Get
    // THis is currently only for test. THis is not working yet!
    //public String GetCapabilitiesString() {
    //	GetVCPFeatureAndVCPFeatureReply();
    //	uint capabilityStringLength = 0;
    //	NativeMethods.GetCapabilitiesStringLength(MonitorPtr, ref capabilityStringLength);
    //	//char[] capabilityString = new char[capabilityStringLength];
    //	//StringBuilder capabilityString = new StringBuilder();
    //	String capabilityString = "";
    //	bool succeed = NativeMethods.CapabilitiesRequestAndCapabilitiesReply(MonitorPtr, ref capabilityString, capabilityStringLength - 1);
    //	//return new String(capabilityString);
    //	//return capabilityString.ToString();
    //	return capabilityString;
    //}

    //public void GetVCPFeatureAndVCPFeatureReply() {
    //	byte bVCPCode = 0;
    //	for (bVCPCode = 1; bVCPCode < 255; bVCPCode++) {
    //		NativeStructures.MC_VCP_CODE_TYPE pvct = 0;
    //		uint pdwCurrentValue = 0;
    //		uint pdwMaximumValue = 0;
    //		bool b = NativeMethods.GetVCPFeatureAndVCPFeatureReply(MonitorPtr, bVCPCode, ref pvct, ref pdwCurrentValue, ref pdwMaximumValue);
    //	}
    //}

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




    #region Set
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


    private void CheckCapabilities()
    {
      CheckHighLevelCapabilities();
      CheckLowLevelCapabilities();

      Type = NativeStructures.MC_DISPLAY_TECHNOLOGY_TYPE.MC_SHADOW_MASK_CATHODE_RAY_TUBE;
    }

    private void CheckLowLevelCapabilities()
    {
      try
      {
        if (NativeMethods.GetCapabilitiesStringLength(physicalMonitorPtr, out uint pdwCapabilitiesStringLengthInCharacters))
        {
          StringBuilder pszASCIICapabilitiesString = new StringBuilder((int)pdwCapabilitiesStringLengthInCharacters);
          if (NativeMethods.CapabilitiesRequestAndCapabilitiesReply(physicalMonitorPtr, pszASCIICapabilitiesString, pdwCapabilitiesStringLengthInCharacters))
          {

            VCPCodes = new VCPCodeList();
            VCPCodes.Populate(pszASCIICapabilitiesString.ToString());

            SupportsLowLevelDDC = VCPCodes.Contains(eVCPCode.Luminance);
          }
          //var vcpCodes = DDCHelper.GetVcpCodes(pszASCIICapabilitiesString.ToString()).ToArray();
        }

      }
      catch (Exception ex)
      {

        throw;
      }
    }

    private void CheckHighLevelCapabilities()
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

    public override string ToString()
    {
      return Name;
    }
  }
}
