using System.ComponentModel;

namespace AMD.Util.Display.Edid.Enums
{
  /// <summary>
  ///     Contains possible values for the detailed timing descriptor block's sync type feature field
  /// </summary>
  public enum SyncType : uint
  {
    /// <summary>
    ///     Analog display composite sync
    /// </summary>
    [Description("Analog Composite")]
    AnalogComposite = 0,

    /// <summary>
    ///     Analog display bipolar composite sync
    /// </summary>
    [Description("Bipolar Analog Composite")]
    BipolarAnalogComposite = 1,

    /// <summary>
    ///     Digital display composite sync on HSync
    /// </summary>
    [Description("Digital Composite")]
    DigitalCompositeOnHSync = 2,

    /// <summary>
    ///     Digital display separate sync
    /// </summary>
    [Description("Digital Separate")]
    DigitalSeparate = 3
  }
}