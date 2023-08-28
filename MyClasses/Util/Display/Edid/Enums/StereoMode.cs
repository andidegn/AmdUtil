using System.ComponentModel;

namespace AMD.Util.Display.Edid.Enums
{
  /// <summary>
  ///     Contains possible stereo modes for a display
  /// </summary>
  public enum StereoMode : uint
  {
    /// <summary>
    ///     No stereo is supported
    /// </summary>
    [Description("No Stereo")]
    NoStereo = 0,

    /// <summary>
    ///     Stereo lines fields are sequential and sync during right eye signal
    /// </summary>
    [Description("Field sequential stereo, right image when stereo sync = 1")]
    FieldSequentialSyncDuringRight = 1,

    /// <summary>
    ///     Stereo lines fields are sequential and sync during left eye signal
    /// </summary>
    [Description("Field sequential stereo, left image when stereo sync = 1")]
    FieldSequentialSyncDuringLeft = 2,

    /// <summary>
    ///     4-Way interleaved stereo
    /// </summary>
    [Description("4-way interleaved stereo")]
    Stereo4WayInterleaved = 3,

    /// <summary>
    ///     2-Way interleaved stereo
    /// </summary>
    //Stereo2WayInterleaved = 4, // Invalid state

    /// <summary>
    ///     Right eye image is on the even lines
    /// </summary>
    [Description("2-way interleaved stereo, right image on even lines")]
    RightImageOnEvenLines = 5,

    /// <summary>
    ///     Left eye image is on the even lines
    /// </summary>
    [Description("2-way interleaved stereo, left image on even lines")]
    LeftImageOnEvenLines = 6,

    /// <summary>
    ///     Right and left eye images are side by side
    /// </summary>
    [Description("Side-by-Side interleaved stereo")]
    SideBySide = 7
  }
}