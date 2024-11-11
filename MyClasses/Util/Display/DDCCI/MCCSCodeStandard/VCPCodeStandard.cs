using AMD.Util.AttributeHelper;
using AMD.Util.Display.DDCCI.Util;
using AMD.Util.Extensions;
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
    [Name("VCP Code Page")]
    VCPCodePage,

    [Name("Degauss")]
    Degauss,

    [Name("New Control Value")]
    NewControlValue,

    [Name("Soft Controls")]
    SoftControls,

    [Name("Restore Factory Defaults")]
    RestoreFactoryDefaults,

    [Name("Restore Factory Luminance/ Contrast Defaults")]
    RestoreFactoryLuminanceContrastDefaults,

    [Name("Restore Factory Geometry Defaults")]
    RestoreFactoryGeometryDefaults,

    [Name("")]
    Reserved0x07,

    [Name("Restore Factory Color Defaults")]
    RestoreFactoryColorDefaults,

    [Name("")]
    Reserved0x09,

    [Name("Restore Factory TV Defaults")]
    RestoreFactoryTVDefaults,

    [Name("Color Temperature Increment")]
    ColorTemperatureIncrement,

    [Name("Color Temperature Request")]
    ColorTemperatureRequest,

    [Name("")]
    Reserved0x0D,

    [Name("Clock")]
    Clock,

    [Name("")]
    Reserved0x0F,

    [Name("Luminance")]
    Luminance,

    [Name("Flesh Tone Enhancement")]
    FleshToneEnhancement,

    [Name("Contrast")]
    Contrast,

    [Name("Backlight Control")]
    BacklightControl,

    [Name("Select Color Preset")]
    SelectColorPreset,

    [Name("")]
    Reserved0x15,

    [Name("Video Gain (Drive): Red")]
    VideoGainDriveRed,

    [Name("User Color Vision Compensation")]
    UserColorVisionCompensation,

    [Name("Video Gain (Drive): Green")]
    VideoGainDriveGreen,

    [Name("")]
    Reserved0x19,

    [Name("Video Gain (Drive): Blue")]
    VideoGainDriveBlue,

    [Name("")]
    Reserved0x1B,

    [Name("Focus")]
    Focus,

    [Name("")]
    Reserved0x1D,

    [Name("Auto Setup")]
    AutoSetup,

    [Name("Auto Color Setup")]
    AutoColorSetup,

    [Name("Horizontal Position (Phase)")]
    HorizontalPositionPhase,

    [Name("")]
    Reserved0x21,

    [Name("Horizontal Size")]
    HorizontalSize,

    [Name("")]
    Reserved0x23,

    [Name("Horizontal Pincushion")]
    HorizontalPincushion,

    [Name("")]
    Reserved0x25,

    [Name("Horizontal Pincushion Balance")]
    HorizontalPincushionBalance,

    [Name("")]
    Reserved0x27,

    [Name("Horizontal Convergence R / B")]
    HorizontalConvergenceRB,

    [Name("Horizontal Convergence M / G")]
    HorizontalConvergenceMG,

    [Name("Horizontal Linearity")]
    HorizontalLinearity,

    [Name("")]
    Reserved0x2B,

    [Name("Horizontal Linearity Balance")]
    HorizontalLinearityBalance,

    [Name("")]
    Reserved0x2D,

    [Name("Gray Scale Expansion")]
    GrayScaleExpansion,

    [Name("")]
    Reserved0x2F,

    [Name("Vertical Position (Phase)")]
    VerticalPositionPhase,

    [Name("")]
    Reserved0x31,

    [Name("Vertical Size")]
    VerticalSize,

    [Name("")]
    Reserved0x33,

    [Name("Vertical Pincushion")]
    VerticalPincushion,

    [Name("")]
    Reserved0x35,

    [Name("Vertical Pincushion Balance")]
    VerticalPincushionBalance,

    [Name("")]
    Reserved0x37,

    [Name("Vertical Convergence R/B")]
    VerticalConvergenceRB,

    [Name("Vertical Convergence M/G")]
    VerticalConvergenceMG,

    [Name("Vertical Linearity")]
    VerticalLinearity,

    [Name("")]
    Reserved0x3B,

    [Name("Vertical Linearity Balance")]
    VerticalLinearityBalance,

    [Name("")]
    Reserved0x3D,

    [Name("Clock Phase")]
    ClockPhase,

    [Name("")]
    Reserved0x3F,

    [Name("Horizontal Parallelogram")]
    HorizontalParallelogram,

    [Name("Vertical Parallelogram")]
    VerticalParallelogram,

    [Name("Horizontal Keystone")]
    HorizontalKeystone,

    [Name("Vertical Keystone")]
    VerticalKeystone,

    [Name("Rotation")]
    Rotation,

    [Name("")]
    Reserved0x45,

    [Name("Top Corner Flare")]
    TopCornerFlare,

    [Name("")]
    Reserved0x47,

    [Name("Top Corner Hook")]
    TopCornerHook,

    [Name("")]
    Reserved0x49,

    [Name("Bottom Corner Flare")]
    BottomCornerFlare,

    [Name("")]
    Reserved0x4B,

    [Name("Bottom Corner Hook")]
    BottomCornerHook,

    [Name("")]
    Reserved0x4D,

    [Name("")]
    Reserved0x4E,

    [Name("")]
    Reserved0x4F,

    [Name("")]
    Reserved0x50,

    [Name("")]
    Reserved0x51,

    [Name("Active Control")]
    ActiveControl,

    [Name("")]
    Reserved0x53,

    [Name("Performance Preservation")]
    PerformancePreservation,

    [Name("")]
    Reserved0x55,

    [Name("H Moiré")]
    HMoiré,

    [Name("")]
    Reserved0x57,

    [Name("V Moiré")]
    VMoiré,

    [Name("6 Axis Saturation Control: Red")]
    SixAxisSaturationControlRed,

    [Name("6 Axis Saturation Control: Yellow")]
    SixAxisSaturationControlYellow,

    [Name("6 Axis Saturation Control: Green")]
    SixAxisSaturationControlGreen,

    [Name("6 Axis Saturation Control: Cyan")]
    SixAxisSaturationControlCyan,

    [Name("6 Axis Saturation Control: Blue")]
    SixAxisSaturationControlBlue,

    [Name("6 Axis Saturation Control: Magenta")]
    SixAxisSaturationControlMagenta,

    [Name("")]
    Reserved0x5F,

    [Name("Input Select")]
    InputSelect,

    [Name("")]
    Reserved0x61,

    [Name("Audio: Speaker Volume")]
    AudioSpeakerVolume,

    [Name("Audio: Speaker Pair Select")]
    AudioSpeakerPairSelect,

    [Name("Audio: Microphone Volume")]
    AudioMicrophoneVolume,

    [Name("Audio: Jack Connection Status")]
    AudioJackConnectionStatus,

    [Name("Ambient Light Sensor")]
    AmbientLightSensor,

    [Name("")]
    Reserved0x67,

    [Name("")]
    Reserved0x68,

    [Name("")]
    Reserved0x69,

    [Name("")]
    Reserved0x6A,

    [Name("Backlight Level: White")]
    BacklightLevelWhite,

    [Name("Video Black Level: Red")]
    VideoBlackLevelRed,

    [Name("Backlight Level: Red")]
    BacklightLevelRed,

    [Name("Video Black Level: Green")]
    VideoBlackLevelGreen,

    [Name("Backlight Level: Green")]
    BacklightLevelGreen,

    [Name("Video Black Level: Blue")]
    VideoBlackLevelBlue,

    [Name("Backlight Level: Blue")]
    BacklightLevelBlue,

    [Name("Gamma")]
    Gamma,

    [Name("LUT Size")]
    LUTSize,

    [Name("Single Point LUT Operation")]
    SinglePointLUTOperation,

    [Name("Block LUT Operation")]
    BlockLUTOperation,

    [Name("Remote Procedure Call")]
    RemoteProcedureCall,

    [Name("")]
    Reserved0x77,

    [Name("Display Identification Data Operation")]
    DisplayIdentificationDataOperation,

    [Name("")]
    Reserved0x79,

    [Name("")]
    Reserved0x7A,

    [Name("")]
    Reserved0x7B,

    [Name("Adjust Zoom")]
    AdjustZoom,

    [Name("")]
    Reserved0x7D,

    [Name("")]
    Reserved0x7E,

    [Name("")]
    Reserved0x7F,

    [Name("")]
    Reserved0x80,

    [Name("")]
    Reserved0x81,

    [Name("Horizontal Mirror (Flip)")]
    HorizontalMirrorFlip,

    [Name("")]
    Reserved0x83,

    [Name("Vertical Mirror (Flip)")]
    VerticalMirrorFlip,

    [Name("")]
    Reserved0x85,

    [Name("Display Scaling")]
    DisplayScaling,

    [Name("Sharpness")]
    Sharpness,

    [Name("Velocity Scan Modulation")]
    VelocityScanModulation,

    [Name("")]
    Reserved0x89,

    [Name("Color Saturation")]
    ColorSaturation,

    [Name("TV Channel Up / Down")]
    TVChannelUpDown,

    [Name("TV Sharpness")]
    TVSharpness,

    [Name("Audio Mute / Screen Blank")]
    AudioMuteScreenBlank,

    [Name("TV Contrast")]
    TVContrast,

    [Name("Audio Treble")]
    AudioTreble,

    [Name("Hue")]
    Hue,

    [Name("Audio Bass")]
    AudioBass,

    [Name("TV Black Level / Luminance")]
    TVBlackLevelLuminance,

    [Name("Audio Balance L / R")]
    AudioBalanceLR,

    [Name("Audio Processor Mode:")]
    AudioProcessorMode,

    [Name("Window Position (TL_X)")]
    WindowPositionTL_X,

    [Name("Window Position (TL_Y)")]
    WindowPositionTL_Y,

    [Name("Window Position (BR_X)")]
    WindowPositionBR_X,

    [Name("Window Position (BR_Y)")]
    WindowPositionBR_Y,

    [Name("")]
    Reserved0x99,

    [Name("Window Background")]
    WindowBackground,

    [Name("6 Axis Color Control: Red")]
    SixAxisColorControlRed,

    [Name("6 Axis Color Control: Yellow")]
    SixAxisColorControlYellow,

    [Name("6 Axis Color Control: Green")]
    SixAxisColorControlGreen,

    [Name("6 Axis Color Control: Cyan")]
    SixAxisColorControlCyan,

    [Name("6 Axis Color Control: Blue")]
    SixAxisColorControlBlue,

    [Name("6 Axis Color Control: Magenta")]
    SixAxisColorControlMagenta,

    [Name("")]
    Reserved0xA1,

    [Name("Auto Setup On / Off")]
    AutoSetupOnOff,

    [Name("")]
    Reserved0xA3,

    [Name("Window Mask Control")]
    WindowMaskControl,

    [Name("Window Select")]
    WindowSelect,

    [Name("Window Size")]
    WindowSize,

    [Name("Window Transparency")]
    WindowTransparency,

    [Name("")]
    Reserved0xA8,

    [Name("")]
    Reserved0xA9,

    [Name("Screen Orientation")]
    ScreenOrientation,

    [Name("")]
    Reserved0xAB,

    [Name("Horizontal Frequency")]
    HorizontalFrequency,

    [Name("")]
    Reserved0xAD,

    [Name("Vertical Frequency")]
    VerticalFrequency,

    [Name("")]
    Reserved0xAF,

    [Name("Settings")]
    Settings,

    [Name("")]
    Reserved0xB1,

    [Name("Flat Panel Sub-Pixel Layout")]
    FlatPanelSubPixelLayout,

    [Name("")]
    Reserved0xB3,

    [Name("Source Timing Mode")]
    SourceTimingMode,

    [Name("Source Color Coding")]
    SourceColorCoding,

    [Name("Display Technology Type")]
    DisplayTechnologyType,

    [Name("DPVL : Display status")]
    DPVLDisplaystatus,

    [Name("DPVL : Packet count")]
    DPVLPacketcount,

    [Name("DPVL : Display X origin")]
    DPVLDisplayXorigin,

    [Name("DPVL : Display Y origin")]
    DPVLDisplayYorigin,

    [Name("DPVL : Header CRC error count")]
    DPVLHeaderCRCErrorCount,

    [Name("DPVL : Body CRC error count")]
    DPVLBodyCRCErrorCount,

    [Name("DPVL : Client ID")]
    DPVLClientID,

    [Name("DPVL : Link control")]
    DPVLLinkcontrol,

    [Name("")]
    Reserved0xBF,

    [Name("Display Usage Time")]
    DisplayUsageTime,

    [Name("")]
    Reserved0xC1,

    [Name("Display Descriptor Length")]
    DisplayDescriptorLength,

    [Name("Transmit Display Descriptor")]
    TransmitDisplayDescriptor,

    [Name("Enable Display of ‘Display Descriptor’")]
    EnableDisplayofDisplayDescriptor,

    [Name("")]
    Reserved0xC5,

    [Name("Application Enable Key")]
    ApplicationEnableKey,

    [Name("Reserved")]
    Reserved0xC7,

    [Name("Display Controller ID")]
    DisplayControllerID,

    [Name("Display Firmware Level")]
    DisplayFirmwareLevel,

    [Name("OSD")]
    OSD,

    [Name("")]
    Reserved0xCB,

    [Name("OSD Language")]
    OSDLanguage,

    [Name("Status Indicators")]
    StatusIndicators,

    [Name("Auxiliary Display Size")]
    AuxiliaryDisplaySize,

    [Name("Auxiliary Display Data")]
    AuxiliaryDisplayData,

    [Name("Output Selection")]
    OutputSelection,

    [Name("")]
    Reserved0xD1,

    [Name("Asset Tag")]
    AssetTag,

    [Name("")]
    Reserved0xD3,

    [Name("Stereo Video Mode")]
    StereoVideoMode,

    [Name("")]
    Reserved0xD5,

    [Name("Power Mode")]
    PowerMode,

    [Name("Auxiliary Power Output")]
    AuxiliaryPowerOutput,

    [Name("")]
    Reserved0xD8,

    [Name("")]
    Reserved0xD9,

    [Name("Scan Mode")]
    ScanMode,

    [Name("Image Mode")]
    ImageMode,

    [Name("Display Application")]
    DisplayApplication,

    [Name("")]
    Reserved0xDD,

    [Name("Scratch Pad")]
    ScratchPad,

    [Name("VCP Version")]
    VCPVersion,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE0,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE1,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE2,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE3,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE4,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE5,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE6,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE7,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE8,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xE9,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xEA,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xEB,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xEC,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xED,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xEE,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xEF,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF0,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF1,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF2,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF3,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF4,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF5,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF6,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF7,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF8,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xF9,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xFA,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xFB,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xFC,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xFD,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xFE,

    [Name("Manufacturer Specific")]
    ManufacturerSpecific0xFF
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
      return code.GetAttribute<NameAttribute>().Name;
    }

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
