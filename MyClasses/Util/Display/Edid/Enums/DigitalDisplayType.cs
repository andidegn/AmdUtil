using AMD.Util.Compression.SevenZip;
using System.ComponentModel;

namespace AMD.Util.Display.Edid.Enums
{
  /// <summary>
  ///     Contains possible digital display types
  /// </summary>
  public enum DigitalDisplayType : uint
  {
    /// <summary>
    ///     RGB 4:4:4
    /// </summary>
    [Description("RGB 4:4:4")]
    RGB444 = 0,

    /// <summary>
    ///     RGB 4:4:4 + YCrCb 4:4:4
    /// </summary>
    [Description("RGB 4:4:4 + YCrCb 4:4:4")]
    RGB444YCrCb444 = 1,

    /// <summary>
    ///     RGB 4:4:4 + YCrCb 4:2:2
    /// </summary>
    [Description("RGB 4:4:4 + YCrCb 4:2:2")]
    RGB444CrCb422 = 2,

    /// <summary>
    ///     RGB 4:4:4 + YCrCb 4:4:4 + YCrCb 4:2:2
    /// </summary>
    [Description("RGB 4:4:4 + YCrCb 4:4:4 + YCrCb 4:2:2")]
    RGB444YCrCb444YCrCb422 = 3
  }
}