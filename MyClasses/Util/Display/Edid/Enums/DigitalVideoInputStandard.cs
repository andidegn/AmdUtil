using System.ComponentModel;

namespace AMD.Util.Display.Edid.Enums
{
  public enum DigitalVideoInputStandard : uint
  {
    [Description("Not Defined")]
    NotDefined      = 0,

    [Description("DVI is supported")]
    DVI             = 1,

    [Description("HDMI-a is supported")]
    HDMIa           = 2,

    [Description("HDMI-b is supported")]
    HDMIb           = 3,

    [Description("MDDI is supported")]
    MDDI            = 4,

    [Description("DisplayPort is supported")]
    DP              = 5,

    [Description("Reserved. Only values 0-5 are supported")]
    Reserved
  }
}
