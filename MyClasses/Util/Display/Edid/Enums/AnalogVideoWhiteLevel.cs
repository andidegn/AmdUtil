using System.ComponentModel;

namespace AMD.Util.Display.Edid.Enums
{
  /// <summary>
  ///     Contains possible values of the analog display's white and sync levels relative to blank
  /// </summary>
  public enum AnalogVideoWhiteLevel : uint
  {
    /// <summary>
    ///     +0.7/−0.3 V
    /// </summary>
    [Description("+0.700, -0.300 (1.000 Vpp)")]
    White07OnMinus03V = 0,

    /// <summary>
    ///     +0.714/−0.286 V
    /// </summary>
    [Description("+0.714, -0.286 (1.000 Vpp)")]
    White0714OnMinus0286V = 1,

    /// <summary>
    ///     +1.0/−0.4 V
    /// </summary>
    [Description("+1.000, -0.400 (1.400 Vpp)")]
    White1OnMinus04V = 2,

    /// <summary>
    ///     +0.7/0 V
    /// </summary>
    [Description("+0.700,  0.000 (0.700 Vpp)")]
    White07On0V = 3
  }
}