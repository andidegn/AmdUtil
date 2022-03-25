using AMD.Util.Display.DDCCI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Display.DDCCI.MCCSCodeStandard
{
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
    DPVLHeaderCRCErrorCount,
    DPVLBodyCRCErrorCount,
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
  public class VCPCodeStandard
  {
    private static VCPCodeStandard instance;
    public static VCPCodeStandard Instance
    {
      get
      {
        if (null == instance)
        {
          instance = new VCPCodeStandard();
        }
        return instance;
      }
    }

    private VCPCodeStandard()
    {

    }

    public string GetName(eVCPCode code)
    {
      return VCPNameLUT[code];
    }

    #region VCPNameLUT    
    public static Dictionary<eVCPCode, string> VCPNameLUT = new Dictionary<eVCPCode, string>()
    {
      { eVCPCode.VCPCodePage, "VCP Code Page" },
      { eVCPCode.Degauss, "Degauss" },
      { eVCPCode.NewControlValue, "New Control Value" },
      { eVCPCode.SoftControls, "Soft Controls" },
      { eVCPCode.RestoreFactoryDefaults, "Restore Factory Defaults"  },
      { eVCPCode.RestoreFactoryLuminanceContrastDefaults, "Restore Factory Luminance/ Contrast Defaults"  },
      { eVCPCode.RestoreFactoryGeometryDefaults, "Restore Factory Geometry Defaults" },
      { eVCPCode.Reserved0x07, ""  },
      { eVCPCode.RestoreFactoryColorDefaults, "Restore Factory Color Defaults"  },
      { eVCPCode.Reserved0x09, ""  },
      { eVCPCode.RestoreFactoryTVDefaults, "Restore Factory TV Defaults" },
      { eVCPCode.ColorTemperatureIncrement, "Color Temperature Increment" },
      { eVCPCode.ColorTemperatureRequest, "Color Temperature Request" },
      { eVCPCode.Reserved0x0D, ""  },
      { eVCPCode.Clock, "Clock" },
      { eVCPCode.Reserved0x0F, ""  },
      { eVCPCode.Luminance, "Luminance" },
      { eVCPCode.FleshToneEnhancement, "Flesh Tone Enhancement"  },
      { eVCPCode.Contrast, "Contrast"  },
      { eVCPCode.BacklightControl, "Backlight Control" },
      { eVCPCode.SelectColorPreset, "Select Color Preset" },
      { eVCPCode.Reserved0x15, ""  },
      { eVCPCode.VideoGainDriveRed, "Video Gain (Drive): Red" },
      { eVCPCode.UserColorVisionCompensation, "User Color Vision Compensation"  },
      { eVCPCode.VideoGainDriveGreen, "Video Gain (Drive): Green" },
      { eVCPCode.Reserved0x19, ""  },
      { eVCPCode.VideoGainDriveBlue, "Video Gain (Drive): Blue"  },
      { eVCPCode.Reserved0x1B, ""  },
      { eVCPCode.Focus, "Focus" },
      { eVCPCode.Reserved0x1D, ""  },
      { eVCPCode.AutoSetup, "Auto Setup"  },
      { eVCPCode.AutoColorSetup, "Auto Color Setup"  },
      { eVCPCode.HorizontalPositionPhase, "Horizontal Position (Phase)" },
      { eVCPCode.Reserved0x21, ""  },
      { eVCPCode.HorizontalSize, "Horizontal Size" },
      { eVCPCode.Reserved0x23, ""  },
      { eVCPCode.HorizontalPincushion, "Horizontal Pincushion" },
      { eVCPCode.Reserved0x25, ""  },
      { eVCPCode.HorizontalPincushionBalance, "Horizontal Pincushion Balance" },
      { eVCPCode.Reserved0x27, ""  },
      { eVCPCode.HorizontalConvergenceRB, "Horizontal Convergence R / B"  },
      { eVCPCode.HorizontalConvergenceMG, "Horizontal Convergence M / G"  },
      { eVCPCode.HorizontalLinearity, "Horizontal Linearity"  },
      { eVCPCode.Reserved0x2B, ""  },
      { eVCPCode.HorizontalLinearityBalance, "Horizontal Linearity Balance"  },
      { eVCPCode.Reserved0x2D, ""  },
      { eVCPCode.GrayScaleExpansion, "Gray Scale Expansion"  },
      { eVCPCode.Reserved0x2F, ""  },
      { eVCPCode.VerticalPositionPhase, "Vertical Position (Phase)" },
      { eVCPCode.Reserved0x31, ""  },
      { eVCPCode.VerticalSize, "Vertical Size" },
      { eVCPCode.Reserved0x33, ""  },
      { eVCPCode.VerticalPincushion, "Vertical Pincushion" },
      { eVCPCode.Reserved0x35, ""  },
      { eVCPCode.VerticalPincushionBalance, "Vertical Pincushion Balance" },
      { eVCPCode.Reserved0x37, ""  },
      { eVCPCode.VerticalConvergenceRB, "Vertical Convergence R/B"  },
      { eVCPCode.VerticalConvergenceMG, "Vertical Convergence M/G"  },
      { eVCPCode.VerticalLinearity, "Vertical Linearity"  },
      { eVCPCode.Reserved0x3B, ""  },
      { eVCPCode.VerticalLinearityBalance, "Vertical Linearity Balance"  },
      { eVCPCode.Reserved0x3D, ""  },
      { eVCPCode.ClockPhase, "Clock Phase" },
      { eVCPCode.Reserved0x3F, ""  },
      { eVCPCode.HorizontalParallelogram, "Horizontal Parallelogram"  },
      { eVCPCode.VerticalParallelogram, "Vertical Parallelogram"  },
      { eVCPCode.HorizontalKeystone, "Horizontal Keystone" },
      { eVCPCode.VerticalKeystone, "Vertical Keystone" },
      { eVCPCode.Rotation, "Rotation"  },
      { eVCPCode.Reserved0x45, ""  },
      { eVCPCode.TopCornerFlare, "Top Corner Flare"  },
      { eVCPCode.Reserved0x47, ""  },
      { eVCPCode.TopCornerHook, "Top Corner Hook" },
      { eVCPCode.Reserved0x49, ""  },
      { eVCPCode.BottomCornerFlare, "Bottom Corner Flare" },
      { eVCPCode.Reserved0x4B, ""  },
      { eVCPCode.BottomCornerHook, "Bottom Corner Hook"  },
      { eVCPCode.Reserved0x4D, ""  },
      { eVCPCode.Reserved0x4E, ""  },
      { eVCPCode.Reserved0x4F, ""  },
      { eVCPCode.Reserved0x50, ""  },
      { eVCPCode.Reserved0x51, ""  },
      { eVCPCode.ActiveControl, "Active Control"  },
      { eVCPCode.Reserved0x53, ""  },
      { eVCPCode.PerformancePreservation, "Performance Preservation"  },
      { eVCPCode.Reserved0x55, ""  },
      { eVCPCode.HMoiré, "H Moiré" },
      { eVCPCode.Reserved0x57, ""  },
      { eVCPCode.VMoiré, "V Moiré" },
      { eVCPCode.SixAxisSaturationControlRed, "6 Axis Saturation Control: Red"  },
      { eVCPCode.SixAxisSaturationControlYellow, "6 Axis Saturation Control: Yellow" },
      { eVCPCode.SixAxisSaturationControlGreen, "6 Axis Saturation Control: Green"  },
      { eVCPCode.SixAxisSaturationControlCyan, "6 Axis Saturation Control: Cyan" },
      { eVCPCode.SixAxisSaturationControlBlue, "6 Axis Saturation Control: Blue" },
      { eVCPCode.SixAxisSaturationControlMagenta, "6 Axis Saturation Control: Magenta"  },
      { eVCPCode.Reserved0x5F, ""  },
      { eVCPCode.InputSelect, "Input Select"  },
      { eVCPCode.Reserved0x61, ""  },
      { eVCPCode.AudioSpeakerVolume, "Audio: Speaker Volume" },
      { eVCPCode.AudioSpeakerPairSelect, "Audio: Speaker Pair Select"  },
      { eVCPCode.AudioMicrophoneVolume, "Audio: Microphone Volume"  },
      { eVCPCode.AudioJackConnectionStatus, "Audio: Jack Connection Status" },
      { eVCPCode.AmbientLightSensor, "Ambient Light Sensor"  },
      { eVCPCode.Reserved0x67, ""  },
      { eVCPCode.Reserved0x68, ""  },
      { eVCPCode.Reserved0x69, ""  },
      { eVCPCode.Reserved0x6A, ""  },
      { eVCPCode.BacklightLevelWhite, "Backlight Level: White"  },
      { eVCPCode.VideoBlackLevelRed, "Video Black Level: Red"  },
      { eVCPCode.BacklightLevelRed, "Backlight Level: Red"  },
      { eVCPCode.VideoBlackLevelGreen, "Video Black Level: Green"  },
      { eVCPCode.BacklightLevelGreen, "Backlight Level: Green"  },
      { eVCPCode.VideoBlackLevelBlue, "Video Black Level: Blue" },
      { eVCPCode.BacklightLevelBlue, "Backlight Level: Blue" },
      { eVCPCode.Gamma, "Gamma" },
      { eVCPCode.LUTSize, "LUT Size"  },
      { eVCPCode.SinglePointLUTOperation, "Single Point LUT Operation"  },
      { eVCPCode.BlockLUTOperation, "Block LUT Operation" },
      { eVCPCode.RemoteProcedureCall, "Remote Procedure Call" },
      { eVCPCode.Reserved0x77, ""  },
      { eVCPCode.DisplayIdentificationDataOperation, "Display Identification Data Operation" },
      { eVCPCode.Reserved0x79, ""  },
      { eVCPCode.Reserved0x7A, ""  },
      { eVCPCode.Reserved0x7B, ""  },
      { eVCPCode.AdjustZoom, "Adjust Zoom" },
      { eVCPCode.Reserved0x7D, ""  },
      { eVCPCode.Reserved0x7E, ""  },
      { eVCPCode.Reserved0x7F, ""  },
      { eVCPCode.Reserved0x80, ""  },
      { eVCPCode.Reserved0x81, ""  },
      { eVCPCode.HorizontalMirrorFlip, "Horizontal Mirror (Flip)"  },
      { eVCPCode.Reserved0x83, ""  },
      { eVCPCode.VerticalMirrorFlip, "Vertical Mirror (Flip)"  },
      { eVCPCode.Reserved0x85, ""  },
      { eVCPCode.DisplayScaling, "Display Scaling" },
      { eVCPCode.Sharpness, "Sharpness" },
      { eVCPCode.VelocityScanModulation, "Velocity Scan Modulation"  },
      { eVCPCode.Reserved0x89, ""  },
      { eVCPCode.ColorSaturation, "Color Saturation"  },
      { eVCPCode.TVChannelUpDown, "TV Channel Up / Down"  },
      { eVCPCode.TVSharpness, "TV Sharpness"  },
      { eVCPCode.AudioMuteScreenBlank, "Audio Mute / Screen Blank" },
      { eVCPCode.TVContrast, "TV Contrast" },
      { eVCPCode.AudioTreble, "Audio Treble"  },
      { eVCPCode.Hue, "Hue" },
      { eVCPCode.AudioBass, "Audio Bass"  },
      { eVCPCode.TVBlackLevelLuminance, "TV Black Level / Luminance"  },
      { eVCPCode.AudioBalanceLR, "Audio Balance L / R" },
      { eVCPCode.AudioProcessorMode, "Audio Processor Mode:" },
      { eVCPCode.WindowPositionTL_X, "Window Position (TL_X)"  },
      { eVCPCode.WindowPositionTL_Y, "Window Position (TL_Y)"  },
      { eVCPCode.WindowPositionBR_X, "Window Position (BR_X)"  },
      { eVCPCode.WindowPositionBR_Y, "Window Position (BR_Y)"  },
      { eVCPCode.Reserved0x99, ""  },
      { eVCPCode.WindowBackground, "Window Background" },
      { eVCPCode.SixAxisColorControlRed, "6 Axis Color Control: Red" },
      { eVCPCode.SixAxisColorControlYellow, "6 Axis Color Control: Yellow"  },
      { eVCPCode.SixAxisColorControlGreen, "6 Axis Color Control: Green" },
      { eVCPCode.SixAxisColorControlCyan, "6 Axis Color Control: Cyan"  },
      { eVCPCode.SixAxisColorControlBlue, "6 Axis Color Control: Blue"  },
      { eVCPCode.SixAxisColorControlMagenta, "6 Axis Color Control: Magenta" },
      { eVCPCode.Reserved0xA1, ""  },
      { eVCPCode.AutoSetupOnOff, "Auto Setup On / Off" },
      { eVCPCode.Reserved0xA3, ""  },
      { eVCPCode.WindowMaskControl, "Window Mask Control" },
      { eVCPCode.WindowSelect, "Window Select" },
      { eVCPCode.WindowSize, "Window Size" },
      { eVCPCode.WindowTransparency, "Window Transparency" },
      { eVCPCode.Reserved0xA8, ""  },
      { eVCPCode.Reserved0xA9, ""  },
      { eVCPCode.ScreenOrientation, "Screen Orientation"  },
      { eVCPCode.Reserved0xAB, ""  },
      { eVCPCode.HorizontalFrequency, "Horizontal Frequency"  },
      { eVCPCode.Reserved0xAD, ""  },
      { eVCPCode.VerticalFrequency, "Vertical Frequency"  },
      { eVCPCode.Reserved0xAF, ""  },
      { eVCPCode.Settings, "Settings"  },
      { eVCPCode.Reserved0xB1, ""  },
      { eVCPCode.FlatPanelSubPixelLayout, "Flat Panel Sub-Pixel Layout" },
      { eVCPCode.Reserved0xB3, ""  },
      { eVCPCode.SourceTimingMode, "Source Timing Mode"  },
      { eVCPCode.SourceColorCoding, "Source Color Coding" },
      { eVCPCode.DisplayTechnologyType, "Display Technology Type" },
      { eVCPCode.DPVLDisplaystatus, "DPVL : Display status" },
      { eVCPCode.DPVLPacketcount, "DPVL : Packet count" },
      { eVCPCode.DPVLDisplayXorigin, "DPVL : Display X origin" },
      { eVCPCode.DPVLDisplayYorigin, "DPVL : Display Y origin" },
      { eVCPCode.DPVLHeaderCRCErrorCount, "DPVL : Header CRC error count" },
      { eVCPCode.DPVLBodyCRCErrorCount, "DPVL : Body CRC error count" },
      { eVCPCode.DPVLClientID, "DPVL : Client ID"  },
      { eVCPCode.DPVLLinkcontrol, "DPVL : Link control" },
      { eVCPCode.Reserved0xBF, ""  },
      { eVCPCode.DisplayUsageTime, "Display Usage Time"  },
      { eVCPCode.Reserved0xC1, ""  },
      { eVCPCode.DisplayDescriptorLength, "Display Descriptor Length" },
      { eVCPCode.TransmitDisplayDescriptor, "Transmit Display Descriptor" },
      { eVCPCode.EnableDisplayofDisplayDescriptor, "Enable Display of ‘Display Descriptor’"  },
      { eVCPCode.Reserved0xC5, ""  },
      { eVCPCode.ApplicationEnableKey, "Application Enable Key"  },
      { eVCPCode.Reserved0xC7, "Reserved"  },
      { eVCPCode.DisplayControllerID, "Display Controller ID" },
      { eVCPCode.DisplayFirmwareLevel, "Display Firmware Level"  },
      { eVCPCode.OSD, "OSD" },
      { eVCPCode.Reserved0xCB, ""  },
      { eVCPCode.OSDLanguage, "OSD Language"  },
      { eVCPCode.StatusIndicators, "Status Indicators" },
      { eVCPCode.AuxiliaryDisplaySize, "Auxiliary Display Size"  },
      { eVCPCode.AuxiliaryDisplayData, "Auxiliary Display Data"  },
      { eVCPCode.OutputSelection, "Output Selection"  },
      { eVCPCode.Reserved0xD1, ""  },
      { eVCPCode.AssetTag, "Asset Tag" },
      { eVCPCode.Reserved0xD3, ""  },
      { eVCPCode.StereoVideoMode, "Stereo Video Mode" },
      { eVCPCode.Reserved0xD5, ""  },
      { eVCPCode.PowerMode, "Power Mode"  },
      { eVCPCode.AuxiliaryPowerOutput, "Auxiliary Power Output"  },
      { eVCPCode.Reserved0xD8, ""  },
      { eVCPCode.Reserved0xD9, ""  },
      { eVCPCode.ScanMode, "Scan Mode" },
      { eVCPCode.ImageMode, "Image Mode"  },
      { eVCPCode.DisplayApplication, "Display Application" },
      { eVCPCode.ScratchPad, "Scratch Pad" },
      { eVCPCode.VCPVersion, "VCP Version" },
      { eVCPCode.ManufacturerSpecific0xE0, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE1, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE2, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE3, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE4, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE5, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE6, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE7, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE8, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xE9, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xEA, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xEB, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xEC, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xED, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xEE, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xEF, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF0, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF1, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF2, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF3, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF4, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF5, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF6, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF7, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF8, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xF9, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xFA, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xFB, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xFC, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xFD, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xFE, "Manufacturer Specific" },
      { eVCPCode.ManufacturerSpecific0xFF, "Manufacturer Specific" }
    };
    public static Dictionary<byte, string> VCPNameByteLUT = new Dictionary<byte, string>()
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

    public static IEnumerable<VCPCodePreset> GetStandardPresets(eVCPCode code)
    {
      List<VCPCodePreset> retVal = new List<VCPCodePreset>();
      switch (code)
      {
        case eVCPCode.VCPCodePage:
          break;
        case eVCPCode.Degauss:
          break;
        case eVCPCode.NewControlValue:
          break;
        case eVCPCode.SoftControls:
          break;
        case eVCPCode.RestoreFactoryDefaults:
          break;
        case eVCPCode.RestoreFactoryLuminanceContrastDefaults:
          break;
        case eVCPCode.RestoreFactoryGeometryDefaults:
          break;
        case eVCPCode.Reserved0x07:
          break;
        case eVCPCode.RestoreFactoryColorDefaults:
          break;
        case eVCPCode.Reserved0x09:
          break;
        case eVCPCode.RestoreFactoryTVDefaults:
          break;
        case eVCPCode.ColorTemperatureIncrement:
          break;
        case eVCPCode.ColorTemperatureRequest:
          break;
        case eVCPCode.Reserved0x0D:
          break;
        case eVCPCode.Clock:
          break;
        case eVCPCode.Reserved0x0F:
          break;
        case eVCPCode.Luminance:
          break;
        case eVCPCode.FleshToneEnhancement:
          break;
        case eVCPCode.Contrast:
          break;
        case eVCPCode.BacklightControl:
          break;
        case eVCPCode.SelectColorPreset:
          retVal.Add(new VCPCodePreset(null, 0x01, "sRGB"));
          retVal.Add(new VCPCodePreset(null, 0x02, "Display Native"));
          retVal.Add(new VCPCodePreset(null, 0x03, "4000K"));
          retVal.Add(new VCPCodePreset(null, 0x04, "5000K"));
          retVal.Add(new VCPCodePreset(null, 0x05, "6500K"));
          retVal.Add(new VCPCodePreset(null, 0x06, "7500K"));
          retVal.Add(new VCPCodePreset(null, 0x07, "8200K"));
          retVal.Add(new VCPCodePreset(null, 0x08, "9300K"));
          retVal.Add(new VCPCodePreset(null, 0x09, "10000K"));
          retVal.Add(new VCPCodePreset(null, 0x0A, "11500K"));
          retVal.Add(new VCPCodePreset(null, 0x0B, "User 1"));
          retVal.Add(new VCPCodePreset(null, 0x0C, "User 2"));
          retVal.Add(new VCPCodePreset(null, 0x0D, "User 3"));
          break;
        case eVCPCode.Reserved0x15:
          break;
        case eVCPCode.VideoGainDriveRed:
          break;
        case eVCPCode.UserColorVisionCompensation:
          break;
        case eVCPCode.VideoGainDriveGreen:
          break;
        case eVCPCode.Reserved0x19:
          break;
        case eVCPCode.VideoGainDriveBlue:
          break;
        case eVCPCode.Reserved0x1B:
          break;
        case eVCPCode.Focus:
          break;
        case eVCPCode.Reserved0x1D:
          break;
        case eVCPCode.AutoSetup:
          break;
        case eVCPCode.AutoColorSetup:
          break;
        case eVCPCode.HorizontalPositionPhase:
          break;
        case eVCPCode.Reserved0x21:
          break;
        case eVCPCode.HorizontalSize:
          break;
        case eVCPCode.Reserved0x23:
          break;
        case eVCPCode.HorizontalPincushion:
          break;
        case eVCPCode.Reserved0x25:
          break;
        case eVCPCode.HorizontalPincushionBalance:
          break;
        case eVCPCode.Reserved0x27:
          break;
        case eVCPCode.HorizontalConvergenceRB:
          break;
        case eVCPCode.HorizontalConvergenceMG:
          break;
        case eVCPCode.HorizontalLinearity:
          break;
        case eVCPCode.Reserved0x2B:
          break;
        case eVCPCode.HorizontalLinearityBalance:
          break;
        case eVCPCode.Reserved0x2D:
          break;
        case eVCPCode.GrayScaleExpansion:
          break;
        case eVCPCode.Reserved0x2F:
          break;
        case eVCPCode.VerticalPositionPhase:
          break;
        case eVCPCode.Reserved0x31:
          break;
        case eVCPCode.VerticalSize:
          break;
        case eVCPCode.Reserved0x33:
          break;
        case eVCPCode.VerticalPincushion:
          break;
        case eVCPCode.Reserved0x35:
          break;
        case eVCPCode.VerticalPincushionBalance:
          break;
        case eVCPCode.Reserved0x37:
          break;
        case eVCPCode.VerticalConvergenceRB:
          break;
        case eVCPCode.VerticalConvergenceMG:
          break;
        case eVCPCode.VerticalLinearity:
          break;
        case eVCPCode.Reserved0x3B:
          break;
        case eVCPCode.VerticalLinearityBalance:
          break;
        case eVCPCode.Reserved0x3D:
          break;
        case eVCPCode.ClockPhase:
          break;
        case eVCPCode.Reserved0x3F:
          break;
        case eVCPCode.HorizontalParallelogram:
          break;
        case eVCPCode.VerticalParallelogram:
          break;
        case eVCPCode.HorizontalKeystone:
          break;
        case eVCPCode.VerticalKeystone:
          break;
        case eVCPCode.Rotation:
          break;
        case eVCPCode.Reserved0x45:
          break;
        case eVCPCode.TopCornerFlare:
          break;
        case eVCPCode.Reserved0x47:
          break;
        case eVCPCode.TopCornerHook:
          break;
        case eVCPCode.Reserved0x49:
          break;
        case eVCPCode.BottomCornerFlare:
          break;
        case eVCPCode.Reserved0x4B:
          break;
        case eVCPCode.BottomCornerHook:
          break;
        case eVCPCode.Reserved0x4D:
          break;
        case eVCPCode.Reserved0x4E:
          break;
        case eVCPCode.Reserved0x4F:
          break;
        case eVCPCode.Reserved0x50:
          break;
        case eVCPCode.Reserved0x51:
          break;
        case eVCPCode.ActiveControl:
          break;
        case eVCPCode.Reserved0x53:
          break;
        case eVCPCode.PerformancePreservation:
          break;
        case eVCPCode.Reserved0x55:
          break;
        case eVCPCode.HMoiré:
          break;
        case eVCPCode.Reserved0x57:
          break;
        case eVCPCode.VMoiré:
          break;
        case eVCPCode.SixAxisSaturationControlRed:
          break;
        case eVCPCode.SixAxisSaturationControlYellow:
          break;
        case eVCPCode.SixAxisSaturationControlGreen:
          break;
        case eVCPCode.SixAxisSaturationControlCyan:
          break;
        case eVCPCode.SixAxisSaturationControlBlue:
          break;
        case eVCPCode.SixAxisSaturationControlMagenta:
          break;
        case eVCPCode.Reserved0x5F:
          break;
        case eVCPCode.InputSelect:
          retVal.Add(new VCPCodePreset(null, 0x01, "VGA 1 (RGB) 1"));
          retVal.Add(new VCPCodePreset(null, 0x02, "VGA 2 (RGB) 2"));
          retVal.Add(new VCPCodePreset(null, 0x03, "DVI 1 (TMDS) 1"));
          retVal.Add(new VCPCodePreset(null, 0x04, "DVI 2 (TMDS) 2"));
          retVal.Add(new VCPCodePreset(null, 0x05, "Composite Video 1"));
          retVal.Add(new VCPCodePreset(null, 0x06, "Composite Video 2"));
          retVal.Add(new VCPCodePreset(null, 0x07, "S-Video 1"));
          retVal.Add(new VCPCodePreset(null, 0x08, "S-Video 2"));
          retVal.Add(new VCPCodePreset(null, 0x09, "Tuner 1"));
          retVal.Add(new VCPCodePreset(null, 0x0A, "Tuner 2"));
          retVal.Add(new VCPCodePreset(null, 0x0B, "Tuner 3"));
          retVal.Add(new VCPCodePreset(null, 0x0C, "Component Video (YPbPr / YCbCr) 1"));
          retVal.Add(new VCPCodePreset(null, 0x0D, "Component Video (YPbPr / YCbCr) 2"));
          retVal.Add(new VCPCodePreset(null, 0x0E, "Component Video (YPbPr / YCbCr) 3"));
          retVal.Add(new VCPCodePreset(null, 0x0F, "DisplayPort 1"));
          retVal.Add(new VCPCodePreset(null, 0x10, "DisplayPort 2"));
          retVal.Add(new VCPCodePreset(null, 0x11, "HDMI 1 (TMDS) 3"));
          retVal.Add(new VCPCodePreset(null, 0x12, "HDMI 2 (TMDS) 4"));
          break;
        case eVCPCode.Reserved0x61:
          break;
        case eVCPCode.AudioSpeakerVolume:
          break;
        case eVCPCode.AudioSpeakerPairSelect:
          break;
        case eVCPCode.AudioMicrophoneVolume:
          break;
        case eVCPCode.AudioJackConnectionStatus:
          break;
        case eVCPCode.AmbientLightSensor:
          break;
        case eVCPCode.Reserved0x67:
          break;
        case eVCPCode.Reserved0x68:
          break;
        case eVCPCode.Reserved0x69:
          break;
        case eVCPCode.Reserved0x6A:
          break;
        case eVCPCode.BacklightLevelWhite:
          break;
        case eVCPCode.VideoBlackLevelRed:
          break;
        case eVCPCode.BacklightLevelRed:
          break;
        case eVCPCode.VideoBlackLevelGreen:
          break;
        case eVCPCode.BacklightLevelGreen:
          break;
        case eVCPCode.VideoBlackLevelBlue:
          break;
        case eVCPCode.BacklightLevelBlue:
          break;
        case eVCPCode.Gamma:
          break;
        case eVCPCode.LUTSize:
          break;
        case eVCPCode.SinglePointLUTOperation:
          break;
        case eVCPCode.BlockLUTOperation:
          break;
        case eVCPCode.RemoteProcedureCall:
          break;
        case eVCPCode.Reserved0x77:
          break;
        case eVCPCode.DisplayIdentificationDataOperation:
          break;
        case eVCPCode.Reserved0x79:
          break;
        case eVCPCode.Reserved0x7A:
          break;
        case eVCPCode.Reserved0x7B:
          break;
        case eVCPCode.AdjustZoom:
          break;
        case eVCPCode.Reserved0x7D:
          break;
        case eVCPCode.Reserved0x7E:
          break;
        case eVCPCode.Reserved0x7F:
          break;
        case eVCPCode.Reserved0x80:
          break;
        case eVCPCode.Reserved0x81:
          break;
        case eVCPCode.HorizontalMirrorFlip:
          break;
        case eVCPCode.Reserved0x83:
          break;
        case eVCPCode.VerticalMirrorFlip:
          break;
        case eVCPCode.Reserved0x85:
          break;
        case eVCPCode.DisplayScaling:
          break;
        case eVCPCode.Sharpness:
          break;
        case eVCPCode.VelocityScanModulation:
          break;
        case eVCPCode.Reserved0x89:
          break;
        case eVCPCode.ColorSaturation:
          break;
        case eVCPCode.TVChannelUpDown:
          break;
        case eVCPCode.TVSharpness:
          break;
        case eVCPCode.AudioMuteScreenBlank:
          break;
        case eVCPCode.TVContrast:
          break;
        case eVCPCode.AudioTreble:
          break;
        case eVCPCode.Hue:
          break;
        case eVCPCode.AudioBass:
          break;
        case eVCPCode.TVBlackLevelLuminance:
          break;
        case eVCPCode.AudioBalanceLR:
          break;
        case eVCPCode.AudioProcessorMode:
          break;
        case eVCPCode.WindowPositionTL_X:
          break;
        case eVCPCode.WindowPositionTL_Y:
          break;
        case eVCPCode.WindowPositionBR_X:
          break;
        case eVCPCode.WindowPositionBR_Y:
          break;
        case eVCPCode.Reserved0x99:
          break;
        case eVCPCode.WindowBackground:
          break;
        case eVCPCode.SixAxisColorControlRed:
          break;
        case eVCPCode.SixAxisColorControlYellow:
          break;
        case eVCPCode.SixAxisColorControlGreen:
          break;
        case eVCPCode.SixAxisColorControlCyan:
          break;
        case eVCPCode.SixAxisColorControlBlue:
          break;
        case eVCPCode.SixAxisColorControlMagenta:
          break;
        case eVCPCode.Reserved0xA1:
          break;
        case eVCPCode.AutoSetupOnOff:
          break;
        case eVCPCode.Reserved0xA3:
          break;
        case eVCPCode.WindowMaskControl:
          break;
        case eVCPCode.WindowSelect:
          break;
        case eVCPCode.WindowSize:
          break;
        case eVCPCode.WindowTransparency:
          break;
        case eVCPCode.Reserved0xA8:
          break;
        case eVCPCode.Reserved0xA9:
          break;
        case eVCPCode.ScreenOrientation:
          break;
        case eVCPCode.Reserved0xAB:
          break;
        case eVCPCode.HorizontalFrequency:
          break;
        case eVCPCode.Reserved0xAD:
          break;
        case eVCPCode.VerticalFrequency:
          break;
        case eVCPCode.Reserved0xAF:
          break;
        case eVCPCode.Settings:
          break;
        case eVCPCode.Reserved0xB1:
          break;
        case eVCPCode.FlatPanelSubPixelLayout:
          break;
        case eVCPCode.Reserved0xB3:
          break;
        case eVCPCode.SourceTimingMode:
          break;
        case eVCPCode.SourceColorCoding:
          break;
        case eVCPCode.DisplayTechnologyType:
          break;
        case eVCPCode.DPVLDisplaystatus:
          break;
        case eVCPCode.DPVLPacketcount:
          break;
        case eVCPCode.DPVLDisplayXorigin:
          break;
        case eVCPCode.DPVLDisplayYorigin:
          break;
        case eVCPCode.DPVLHeaderCRCErrorCount:
          break;
        case eVCPCode.DPVLBodyCRCErrorCount:
          break;
        case eVCPCode.DPVLClientID:
          break;
        case eVCPCode.DPVLLinkcontrol:
          break;
        case eVCPCode.Reserved0xBF:
          break;
        case eVCPCode.DisplayUsageTime:
          break;
        case eVCPCode.Reserved0xC1:
          break;
        case eVCPCode.DisplayDescriptorLength:
          break;
        case eVCPCode.TransmitDisplayDescriptor:
          break;
        case eVCPCode.EnableDisplayofDisplayDescriptor:
          break;
        case eVCPCode.Reserved0xC5:
          break;
        case eVCPCode.ApplicationEnableKey:
          break;
        case eVCPCode.Reserved0xC7:
          break;
        case eVCPCode.DisplayControllerID:
          break;
        case eVCPCode.DisplayFirmwareLevel:
          break;
        case eVCPCode.OSD:
          break;
        case eVCPCode.Reserved0xCB:
          break;
        case eVCPCode.OSDLanguage:
          break;
        case eVCPCode.StatusIndicators:
          break;
        case eVCPCode.AuxiliaryDisplaySize:
          break;
        case eVCPCode.AuxiliaryDisplayData:
          break;
        case eVCPCode.OutputSelection:
          break;
        case eVCPCode.Reserved0xD1:
          break;
        case eVCPCode.AssetTag:
          break;
        case eVCPCode.Reserved0xD3:
          break;
        case eVCPCode.StereoVideoMode:
          break;
        case eVCPCode.Reserved0xD5:
          break;
        case eVCPCode.PowerMode:
          retVal.Add(new VCPCodePreset(null, 0x01, "On"));
          retVal.Add(new VCPCodePreset(null, 0x02, "Standby"));
          retVal.Add(new VCPCodePreset(null, 0x03, "Suspend"));
          retVal.Add(new VCPCodePreset(null, 0x04, "Off"));
          break;
        case eVCPCode.AuxiliaryPowerOutput:
          break;
        case eVCPCode.Reserved0xD8:
          break;
        case eVCPCode.Reserved0xD9:
          break;
        case eVCPCode.ScanMode:
          break;
        case eVCPCode.ImageMode:
          break;
        case eVCPCode.DisplayApplication:
          break;
        case eVCPCode.Reserved0xDD:
          break;
        case eVCPCode.ScratchPad:
          break;
        case eVCPCode.VCPVersion:
          break;
        default:
          break;
      }
      return retVal;
    }
  }
}
