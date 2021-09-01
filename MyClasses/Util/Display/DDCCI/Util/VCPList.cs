using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Display.DDCCI.Util
{
  public class VCPCodeList : List<VCPCode>
  {
    public VCPCodeList()
    {
    }

    public VCPCode this[eVCPCode code]
    {
      get
      {
        return this.Get(code);
      }
      set
      {
        this.Add(value);
      }
    }

    public void Populate(String capabilityString)
    {
      DDCHelper.PopulateVcpCodes(capabilityString, this);
      DDCHelper.PopulateVcpCodeNames(capabilityString, this);
    }

    public bool Add(VCPCode vcpCode)
    {
      if (this.Contains(vcpCode))
      {
        this.Remove(vcpCode);
      }
      return this.Add(vcpCode);
    } 

    public bool Remove(eVCPCode code)
    {
      return this.Remove(this.Get(code));
    }

    public VCPCode Get(eVCPCode code)
    {
      return (from v in this
              where v.Code == code
              select v).SingleOrDefault();
    }

    public bool Contains(eVCPCode code)
    {
      return 0 == (from v in this
                   where v.Code == code
                   select v).Count();
    }
  }

  public class VCPCode
  {
    public eVCPCode Code { get; set; }
    public eVCPCodeType Type { get; set; }
    public eVCPCodeFunction Function { get; set; }
    public List<byte> Presets { get; set; }
    public byte OriginalValue { get; set; }
    public byte CurrentValue { get; set; }
    public byte MaximumValue { get; set; }

    private string _name;
    public String Name
    {
      get
      {
        return _name ?? DDCHelper.GetVCPName(Code);
      }
      set
      {
        _name = value;
      }
    }
    public String Description { get; set; }
    public bool HasPresets
    {
      get
      {
        return 0 < Presets?.Count();
      }
    }

    public VCPCode()
    {
      Presets = new List<byte>();
    }

    public VCPCode(byte code, String name = "", String description = "", params byte[] presets) : this((eVCPCode)code, name, description, presets) { }

    public VCPCode(eVCPCode code, String name = "", String description = "", params byte[] presets)
      : this()
    {
      this.Code = code;
      foreach (byte preset in presets)
      {
        Presets.Add(preset);
      }
      this.Name = name;
      this.Description = description;
    }

    public override bool Equals(object obj)
    {
      bool retVal = false;
      if (obj is VCPCode)
      {
        retVal = Code.Equals((obj as VCPCode).Code);
      }
      return retVal;
    }
  }

  #region eVCPCode
  public enum eVCPCode : byte
  {
    VCPCodePage,
    Degauss,
    NewControlValue,
    SoftControls,
    RestoreFactoryDefaults,
    RestoreFactoryLuminanceContrastDefaults,
    RestoreFactoryGeometryDefaults,
    Reserved0x07,
    RestoreFactoryColorDefaults,
    Reserved0x09,
    RestoreFactoryTVDefaults,
    ColorTemperatureIncrement,
    ColorTemperatureRequest,
    Reserved0x0D,
    Clock,
    Reserved0x0F,
    Luminance,
    FleshToneEnhancement,
    Contrast,
    BacklightControl,
    SelectColorPreset,
    Reserved0x15,
    VideoGainDriveRed,
    UserColorVisionCompensation,
    VideoGainDriveGreen,
    Reserved0x19,
    VideoGainDriveBlue,
    Reserved0x1B,
    Focus,
    Reserved0x1D,
    AutoSetup,
    AutoColorSetup,
    HorizontalPositionPhase,
    Reserved0x21,
    HorizontalSize,
    Reserved0x23,
    HorizontalPincushion,
    Reserved0x25,
    HorizontalPincushionBalance,
    Reserved0x27,
    HorizontalConvergenceRB,
    HorizontalConvergenceMG,
    HorizontalLinearity,
    Reserved0x2B,
    HorizontalLinearityBalance,
    Reserved0x2D,
    GrayScaleExpansion,
    Reserved0x2F,
    VerticalPositionPhase,
    Reserved0x31,
    VerticalSize,
    Reserved0x33,
    VerticalPincushion,
    Reserved0x35,
    VerticalPincushionBalance,
    Reserved0x37,
    VerticalConvergenceRB,
    VerticalConvergenceMG,
    VerticalLinearity,
    Reserved0x3B,
    VerticalLinearityBalance,
    Reserved0x3D,
    ClockPhase,
    Reserved0x3F,
    HorizontalParallelogram,
    VerticalParallelogram,
    HorizontalKeystone,
    VerticalKeystone,
    Rotation,
    Reserved0x45,
    TopCornerFlare,
    Reserved0x47,
    TopCornerHook,
    Reserved0x49,
    BottomCornerFlare,
    Reserved0x4B,
    BottomCornerHook,
    Reserved0x4D,
    Reserved0x4E,
    Reserved0x4F,
    Reserved0x50,
    Reserved0x51,
    ActiveControl,
    Reserved0x53,
    PerformancePreservation,
    Reserved0x55,
    HMoiré,
    Reserved0x57,
    VMoiré,
    SixAxisSaturationControlRed,
    SixAxisSaturationControlYellow,
    SixAxisSaturationControlGreen,
    SixAxisSaturationControlCyan,
    SixAxisSaturationControlBlue,
    SixAxisSaturationControlMagenta,
    Reserved0x5F,
    InputSelect,
    Reserved0x61,
    AudioSpeakerVolume,
    AudioSpeakerPairSelect,
    AudioMicrophoneVolume,
    AudioJackConnectionStatus,
    AmbientLightSensor,
    Reserved0x67,
    Reserved0x68,
    Reserved0x69,
    Reserved0x6A,
    BacklightLevelWhite,
    VideoBlackLevelRed,
    BacklightLevelRed,
    VideoBlackLevelGreen,
    BacklightLevelGreen,
    VideoBlackLevelBlue,
    BacklightLevelBlue,
    Gamma,
    LUTSize,
    SinglePointLUTOperation,
    BlockLUTOperation,
    RemoteProcedureCall,
    Reserved0x77,
    DisplayIdentificationDataOperation,
    Reserved0x79,
    Reserved0x7A,
    Reserved0x7B,
    AdjustZoom,
    Reserved0x7D,
    Reserved0x7E,
    Reserved0x7F,
    Reserved0x80,
    Reserved0x81,
    HorizontalMirrorFlip,
    Reserved0x83,
    VerticalMirrorFlip,
    Reserved0x85,
    DisplayScaling,
    Sharpness,
    VelocityScanModulation,
    Reserved0x89,
    ColorSaturation,
    TVChannelUpDown,
    TVSharpness,
    AudioMuteScreenBlank,
    TVContrast,
    AudioTreble,
    Hue,
    AudioBass,
    TVBlackLevelLuminance,
    AudioBalanceLR,
    AudioProcessorMode,
    WindowPositionTL_X,
    WindowPositionTL_Y,
    WindowPositionBR_X,
    WindowPositionBR_Y,
    Reserved0x99,
    WindowBackground,
    SixAxisColorControlRed,
    SixAxisColorControlYellow,
    SixAxisColorControlGreen,
    SixAxisColorControlCyan,
    SixAxisColorControlBlue,
    SixAxisColorControlMagenta,
    Reserved0xA1,
    AutoSetupOnOff,
    Reserved0xA3,
    WindowMaskControl,
    WindowSelect,
    WindowSize,
    WindowTransparency,
    Reserved0xA8,
    Reserved0xA9,
    ScreenOrientation,
    Reserved0xAB,
    HorizontalFrequency,
    Reserved0xAD,
    VerticalFrequency,
    Reserved0xAF,
    Settings,
    Reserved0xB1,
    FlatPanelSubPixelLayout,
    Reserved0xB3,
    SourceTimingMode,
    SourceColorCoding,
    DisplayTechnologyType,
    DPVLDisplaystatus,
    DPVLPacketcount,
    DPVLDisplayXorigin,
    DPVLDisplayYorigin,
    DPVLHeaderCRCerrorcount,
    DPVLBodyCRCerrorcount,
    DPVLClientID,
    DPVLLinkcontrol,
    Reserved0xBF,
    DisplayUsageTime,
    Reserved0xC1,
    DisplayDescriptorLength,
    TransmitDisplayDescriptor,
    EnableDisplayofDisplayDescriptor,
    Reserved0xC5,
    ApplicationEnableKey,
    Reserved0xC7,
    DisplayControllerID,
    DisplayFirmwareLevel,
    OSD,
    Reserved0xCB,
    OSDLanguage,
    StatusIndicators,
    AuxiliaryDisplaySize,
    AuxiliaryDisplayData,
    OutputSelection,
    Reserved0xD1,
    AssetTag,
    Reserved0xD3,
    StereoVideoMode,
    Reserved0xD5,
    PowerMode,
    AuxiliaryPowerOutput,
    Reserved0xD8,
    Reserved0xD9,
    ScanMode,
    ImageMode,
    DisplayApplication,
    Reserved0xDD,
    ScratchPad,
    VCPVersion,
    ManufacturerSpecific0xE0,
    ManufacturerSpecific0xE1,
    ManufacturerSpecific0xE2,
    ManufacturerSpecific0xE3,
    ManufacturerSpecific0xE4,
    ManufacturerSpecific0xE5,
    ManufacturerSpecific0xE6,
    ManufacturerSpecific0xE7,
    ManufacturerSpecific0xE8,
    ManufacturerSpecific0xE9,
    ManufacturerSpecific0xEA,
    ManufacturerSpecific0xEB,
    ManufacturerSpecific0xEC,
    ManufacturerSpecific0xED,
    ManufacturerSpecific0xEE,
    ManufacturerSpecific0xEF,
    ManufacturerSpecific0xF0,
    ManufacturerSpecific0xF1,
    ManufacturerSpecific0xF2,
    ManufacturerSpecific0xF3,
    ManufacturerSpecific0xF4,
    ManufacturerSpecific0xF5,
    ManufacturerSpecific0xF6,
    ManufacturerSpecific0xF7,
    ManufacturerSpecific0xF8,
    ManufacturerSpecific0xF9,
    ManufacturerSpecific0xFA,
    ManufacturerSpecific0xFB,
    ManufacturerSpecific0xFC,
    ManufacturerSpecific0xFD,
    ManufacturerSpecific0xFE,
    ManufacturerSpecific0xFF,
  }
  #endregion // eVCPCode

  #region eVCPCodeType
  public enum eVCPCodeType
  {
    WriteOnly,
    ReadOnly,
    ReadWrite
  }
  #endregion // eVCPCodeType

  #region eVCPCodeFunction
  public enum eVCPCodeFunction
  {
    Continuous,
    NonContinuous,
    Table
  }
  #endregion // eVCPCodeFunction

  /// <summary>
  /// Based on the VESA MCCS Standard Version 2.2a
  /// </summary>
  public static class VCPCodeStandard
  {
    #region VCPNameLUT

    public static VCPCodeList VCPStandardMCCS = new VCPCodeList()
    {
      //{ new VCPCode() {Code, } }
    };
    
    public static Dictionary<byte, string> VCPNameLUT = new Dictionary<byte, string>()
    {
      { 0x00, "VCP Code Page" },
      { 0x01, "Degauss" },
      { 0x02, "New Control Value" },
      { 0x03, "Soft Controls" },
      { 0x04, "Restore Factory Defaults"  },
      { 0x05, "Restore Factory Luminance/ Contrast Defaults"  },
      { 0x06, "Restore Factory Geometry Defaults" },
      { 0x07, ""  },
      { 0x08, "Restore Factory Color Defaults"  },
      { 0x09, ""  },
      { 0x0A, "Restore Factory TV Defaults" },
      { 0x0B, "Color Temperature Increment" },
      { 0x0C, "Color Temperature Request" },
      { 0x0D, ""  },
      { 0x0E, "Clock" },
      { 0x0F, ""  },
      { 0x10, "Luminance" },
      { 0x11, "Flesh Tone Enhancement"  },
      { 0x12, "Contrast"  },
      { 0x13, "Backlight Control" },
      { 0x14, "Select Color Preset" },
      { 0x15, ""  },
      { 0x16, "Video Gain (Drive): Red" },
      { 0x17, "User Color Vision Compensation"  },
      { 0x18, "Video Gain (Drive): Green" },
      { 0x19, ""  },
      { 0x1A, "Video Gain (Drive): Blue"  },
      { 0x1B, ""  },
      { 0x1C, "Focus" },
      { 0x1D, ""  },
      { 0x1E, "Auto Setup"  },
      { 0x1F, "Auto Color Setup"  },
      { 0x20, "Horizontal Position (Phase)" },
      { 0x21, ""  },
      { 0x22, "Horizontal Size" },
      { 0x23, ""  },
      { 0x24, "Horizontal Pincushion" },
      { 0x25, ""  },
      { 0x26, "Horizontal Pincushion Balance" },
      { 0x27, ""  },
      { 0x28, "Horizontal Convergence R / B"  },
      { 0x29, "Horizontal Convergence M / G"  },
      { 0x2A, "Horizontal Linearity"  },
      { 0x2B, ""  },
      { 0x2C, "Horizontal Linearity Balance"  },
      { 0x2D, ""  },
      { 0x2E, "Gray Scale Expansion"  },
      { 0x2F, ""  },
      { 0x30, "Vertical Position (Phase)" },
      { 0x31, ""  },
      { 0x32, "Vertical Size" },
      { 0x33, ""  },
      { 0x34, "Vertical Pincushion" },
      { 0x35, ""  },
      { 0x36, "Vertical Pincushion Balance" },
      { 0x37, ""  },
      { 0x38, "Vertical Convergence R/B"  },
      { 0x39, "Vertical Convergence M/G"  },
      { 0x3A, "Vertical Linearity"  },
      { 0x3B, ""  },
      { 0x3C, "Vertical Linearity Balance"  },
      { 0x3D, ""  },
      { 0x3E, "Clock Phase" },
      { 0x3F, ""  },
      { 0x40, "Horizontal Parallelogram"  },
      { 0x41, "Vertical Parallelogram"  },
      { 0x42, "Horizontal Keystone" },
      { 0x43, "Vertical Keystone" },
      { 0x44, "Rotation"  },
      { 0x45, ""  },
      { 0x46, "Top Corner Flare"  },
      { 0x47, ""  },
      { 0x48, "Top Corner Hook" },
      { 0x49, ""  },
      { 0x4A, "Bottom Corner Flare" },
      { 0x4B, ""  },
      { 0x4C, "Bottom Corner Hook"  },
      { 0x4D, ""  },
      { 0x4E, ""  },
      { 0x4F, ""  },
      { 0x50, ""  },
      { 0x51, ""  },
      { 0x52, "Active Control"  },
      { 0x53, ""  },
      { 0x54, "Performance Preservation"  },
      { 0x55, ""  },
      { 0x56, "H Moiré" },
      { 0x57, ""  },
      { 0x58, "V Moiré" },
      { 0x59, "6 Axis Saturation Control: Red"  },
      { 0x5A, "6 Axis Saturation Control: Yellow" },
      { 0x5B, "6 Axis Saturation Control: Green"  },
      { 0x5C, "6 Axis Saturation Control: Cyan" },
      { 0x5D, "6 Axis Saturation Control: Blue" },
      { 0x5E, "6 Axis Saturation Control: Magenta"  },
      { 0x5F, ""  },
      { 0x60, "Input Select"  },
      { 0x61, ""  },
      { 0x62, "Audio: Speaker Volume" },
      { 0x63, "Audio: Speaker Pair Select"  },
      { 0x64, "Audio: Microphone Volume"  },
      { 0x65, "Audio: Jack Connection Status" },
      { 0x66, "Ambient Light Sensor"  },
      { 0x67, ""  },
      { 0x68, ""  },
      { 0x69, ""  },
      { 0x6A, ""  },
      { 0x6B, "Backlight Level: White"  },
      { 0x6C, "Video Black Level: Red"  },
      { 0x6D, "Backlight Level: Red"  },
      { 0x6E, "Video Black Level: Green"  },
      { 0x6F, "Backlight Level: Green"  },
      { 0x70, "Video Black Level: Blue" },
      { 0x71, "Backlight Level: Blue" },
      { 0x72, "Gamma" },
      { 0x73, "LUT Size"  },
      { 0x74, "Single Point LUT Operation"  },
      { 0x75, "Block LUT Operation" },
      { 0x76, "Remote Procedure Call" },
      { 0x77, ""  },
      { 0x78, "Display Identification Data Operation" },
      { 0x79, ""  },
      { 0x7A, ""  },
      { 0x7B, ""  },
      { 0x7C, "Adjust Zoom" },
      { 0x7D, ""  },
      { 0x7E, ""  },
      { 0x7F, ""  },
      { 0x80, ""  },
      { 0x81, ""  },
      { 0x82, "Horizontal Mirror (Flip)"  },
      { 0x83, ""  },
      { 0x84, "Vertical Mirror (Flip)"  },
      { 0x85, ""  },
      { 0x86, "Display Scaling" },
      { 0x87, "Sharpness" },
      { 0x88, "Velocity Scan Modulation"  },
      { 0x89, ""  },
      { 0x8A, "Color Saturation"  },
      { 0x8B, "TV Channel Up / Down"  },
      { 0x8C, "TV Sharpness"  },
      { 0x8D, "Audio Mute / Screen Blank" },
      { 0x8E, "TV Contrast" },
      { 0x8F, "Audio Treble"  },
      { 0x90, "Hue" },
      { 0x91, "Audio Bass"  },
      { 0x92, "TV Black Level / Luminance"  },
      { 0x93, "Audio Balance L / R" },
      { 0x94, "Audio Processor Mode:" },
      { 0x95, "Window Position (TL_X)"  },
      { 0x96, "Window Position (TL_Y)"  },
      { 0x97, "Window Position (BR_X)"  },
      { 0x98, "Window Position (BR_X)"  },
      { 0x99, ""  },
      { 0x9A, "Window Background" },
      { 0x9B, "6 Axis Color Control: Red" },
      { 0x9C, "6 Axis Color Control: Yellow"  },
      { 0x9D, "6 Axis Color Control: Green" },
      { 0x9E, "6 Axis Color Control: Cyan"  },
      { 0x9F, "6 Axis Color Control: Blue"  },
      { 0xA0, "6 Axis Color Control: Magenta" },
      { 0xA1, ""  },
      { 0xA2, "Auto Setup On / Off" },
      { 0xA3, ""  },
      { 0xA4, "Window Mask Control" },
      { 0xA5, "Window Select" },
      { 0xA6, "Window Size" },
      { 0xA7, "Window Transparency" },
      { 0xA8, ""  },
      { 0xA9, ""  },
      { 0xAA, "Screen Orientation"  },
      { 0xAB, ""  },
      { 0xAC, "Horizontal Frequency"  },
      { 0xAD, ""  },
      { 0xAE, "Vertical Frequency"  },
      { 0xAF, ""  },
      { 0xB0, "Settings"  },
      { 0xB1, ""  },
      { 0xB2, "Flat Panel Sub-Pixel Layout" },
      { 0xB3, ""  },
      { 0xB4, "Source Timing Mode"  },
      { 0xB5, "Source Color Coding" },
      { 0xB6, "Display Technology Type" },
      { 0xB7, "DPVL : Display status" },
      { 0xB8, "DPVL : Packet count" },
      { 0xB9, "DPVL : Display X origin" },
      { 0xBA, "DPVL : Display Y origin" },
      { 0xBB, "DPVL : Header CRC error count" },
      { 0xBC, "DPVL : Body CRC error count" },
      { 0xBD, "DPVL : Client ID"  },
      { 0xBE, "DPVL : Link control" },
      { 0xBF, ""  },
      { 0xC0, "Display Usage Time"  },
      { 0xC1, ""  },
      { 0xC2, "Display Descriptor Length" },
      { 0xC3, "Transmit Display Descriptor" },
      { 0xC4, "Enable Display of ‘Display Descriptor’"  },
      { 0xC5, ""  },
      { 0xC6, "Application Enable Key"  },
      { 0xC7, "Reserved"  },
      { 0xC8, "Display Controller ID" },
      { 0xC9, "Display Firmware Level"  },
      { 0xCA, "OSD" },
      { 0xCB, ""  },
      { 0xCC, "OSD Language"  },
      { 0xCD, "Status Indicators" },
      { 0xCE, "Auxiliary Display Size"  },
      { 0xCF, "Auxiliary Display Data"  },
      { 0xD0, "Output Selection"  },
      { 0xD1, ""  },
      { 0xD2, "Asset Tag" },
      { 0xD3, ""  },
      { 0xD4, "Stereo Video Mode" },
      { 0xD5, ""  },
      { 0xD6, "Power Mode"  },
      { 0xD7, "Auxiliary Power Output"  },
      { 0xD8, ""  },
      { 0xD9, ""  },
      { 0xDA, "Scan Mode" },
      { 0xDB, "Image Mode"  },
      { 0xDC, "Display Application" },
      { 0xDE, "Scratch Pad" },
      { 0xDF, "VCP Version" },
      { 0xE0, "Manufacturer Specific" },
      { 0xE1, "Manufacturer Specific" },
      { 0xE2, "Manufacturer Specific" },
      { 0xE3, "Manufacturer Specific" },
      { 0xE4, "Manufacturer Specific" },
      { 0xE5, "Manufacturer Specific" },
      { 0xE6, "Manufacturer Specific" },
      { 0xE7, "Manufacturer Specific" },
      { 0xE8, "Manufacturer Specific" },
      { 0xE9, "Manufacturer Specific" },
      { 0xEA, "Manufacturer Specific" },
      { 0xEB, "Manufacturer Specific" },
      { 0xEC, "Manufacturer Specific" },
      { 0xED, "Manufacturer Specific" },
      { 0xEE, "Manufacturer Specific" },
      { 0xEF, "Manufacturer Specific" },
      { 0xF0, "Manufacturer Specific" },
      { 0xF1, "Manufacturer Specific" },
      { 0xF2, "Manufacturer Specific" },
      { 0xF3, "Manufacturer Specific" },
      { 0xF4, "Manufacturer Specific" },
      { 0xF5, "Manufacturer Specific" },
      { 0xF6, "Manufacturer Specific" },
      { 0xF7, "Manufacturer Specific" },
      { 0xF8, "Manufacturer Specific" },
      { 0xF9, "Manufacturer Specific" },
      { 0xFA, "Manufacturer Specific" },
      { 0xFB, "Manufacturer Specific" },
      { 0xFC, "Manufacturer Specific" },
      { 0xFD, "Manufacturer Specific" },
      { 0xFE, "Manufacturer Specific" },
      { 0xFF, "Manufacturer Specific" }
    };
    #endregion // VCPNameLUT
  }
}

//(prot(monitor) type(lcd)model(rtk)cmds(01 02 03 07 0c e3 f3)vcp(04 10 12 16 18 1a 60(0f 10) d6(01 04) e0 e1 e2 e3 e5 e6 eb f0 f1 f2 f3 f4 f5 f7 f8 f9 fd fe)mswhql(1)asset_eep(40)mccs_ver(2.2))vcpname(e0 (color temperature), e1 (power off), e2 (backlight), e3 (keypad lock) e5(buzzer) e6(usb link select) eb(port select)f0(temperature) f1(min temperature) f2(max temperature) f3(unit runt-time) f4(backlight run-time) f5(service run-time)f7(backlight adjustment control) f8(buzzer output 60-85 db level) f9(buzzer output level)fd(software version) fe(serial number)))