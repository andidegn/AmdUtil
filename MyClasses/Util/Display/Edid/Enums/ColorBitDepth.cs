using System.ComponentModel;

namespace AMD.Util.Display.Edid.Enums
{
  public enum ColorBitDepth : uint
  {
    [Description("Color Bit Depth is undefined")]
    Undefined = 0,

    [Description("6 Bits per Primary Color")]
    _6Bit     = 1,

    [Description("8 Bits per Primary Color")]
    _8Bit     = 2,

    [Description("10 Bits per Primary Color")]
    _10Bit    = 3,

    [Description("12 Bits per Primary Color")]
    _12Bit    = 4,

    [Description("14 Bits per Primary Color")]
    _14Bit    = 5,

    [Description("16 Bits per Primary Color")]
    _16Bit    = 6,
    [Description("Reserved (Do Not Use")]
    Reserved
  }
}
