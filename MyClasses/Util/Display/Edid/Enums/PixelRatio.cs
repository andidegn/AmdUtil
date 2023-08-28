using System.ComponentModel;

namespace AMD.Util.Display.Edid.Enums
{
  /// <summary>
  ///     Contains the possible pixel ratios
  /// </summary>
  public enum PixelRatio
  {
    /// <summary>
    ///     1:1 Ratio
    /// </summary>
    [Description("1:1")]
    Ratio1To1 = -1,

    /// <summary>
    ///     16:10 Ratio
    /// </summary>
    [Description("16:10")]
    Ratio16To10 = 0,

    /// <summary>
    ///     4:3 Ratio
    /// </summary>
    [Description("4:3")]
    Ratio4To3 = 1,

    /// <summary>
    ///     5:4 Ratio
    /// </summary>
    [Description("5:4")]
    Ratio5To4 = 2,

    /// <summary>
    ///     16:9 Ratio
    /// </summary>
    [Description("16:9")]
    Ratio16To9 = 3
  }
}