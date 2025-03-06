﻿using AMD.Util.Display.Edid.Enums;
using AMD.Util.Display.Edid.Exceptions;
using System;
using System.Linq;

namespace AMD.Util.Display.Edid
{
  /// <summary>
  ///     Represents the basic display parameters of a device
  /// </summary>
  public class DisplayParameters : IEquatable<DisplayParameters>
  {
    private readonly EDID _edid;
    private readonly BitAwareReader _reader;

    internal DisplayParameters(EDID edid, BitAwareReader reader)
    {
      _edid = edid;
      _reader = reader;
    }

    /// <summary>
    ///     Gets the display type of this analog device
    /// </summary>
    /// <exception cref="DigitalDisplayException">The device is not analog.</exception>
    public AnalogDisplayType AnalogDisplayType
    {
      get
      {
        if (IsDigital)
          throw new DigitalDisplayException("The device is not analog.");
        return (AnalogDisplayType)_reader.ReadInt(24, 3, 2);
      }
    }

    /// <summary>
    ///     Gets an instance of ChromaticityCoordinates type containing the CIE Chromaticity xy coordinates for red, green,
    ///     blue, and white
    /// </summary>
    public ChromaticityCoordinates ChromaticityCoordinates => new ChromaticityCoordinates(_reader);

    /// <summary>
    ///     Gets the display type of this digital device
    /// </summary>
    /// <exception cref="AnalogDisplayException">The device is not digital.</exception>
    public DigitalDisplayType DigitalDisplayType
    {
      get
      {
        if (!IsDigital)
          throw new AnalogDisplayException("The device is not digital.");
        return (DigitalDisplayType)_reader.ReadInt(24, 3, 2);
      }
    }

    /// <summary>
    ///     Gets the display gamma value (1.00 – 3.54). 0xFF means the gamma information must be storred in an extension block (ex. DI-EXT)
    /// </summary>
    public double DisplayGamma
    {
      get
      {
        var value = _reader.ReadByte(23);
        if (value == 0xFF)
        {
          return double.NaN;
        }
        return (value + 100) / 100d;
      }
    }

    /// <summary>
    ///     Gets the calculated size of the display diagonal in inch
    /// </summary>
    public double DisplaySizeInInch
    {
      get
      {
        var width = PhysicalWidth * 0.393701d;
        var height = PhysicalHeight * 0.393701d;
        return Math.Round(Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2)) * 2, 0) / 2;
      }
    }

    /// <summary>
    ///     Gets a boolean value indicating if the DPMS active-off supported
    /// </summary>
    public bool IsActiveOffSupported => _reader.ReadBit(24, 5);

    /// <summary>
    ///     Gets a boolean value indicating if the Blank-to-black setup (pedestal) is expected
    /// </summary>
    public bool IsBlankToBlackExpected
    {
      get
      {
        if (IsDigital)
          throw new DigitalDisplayException("The device is not analog.");
        return _reader.ReadBit(20, 4);
      }
    }

    /// <summary>
    ///     Gets a boolean value indicating if the composite sync (on HSync) is supported
    /// </summary>
    /// <exception cref="DigitalDisplayException">The device is not analog.</exception>
    public bool IsCompositeSyncSupported
    {
      get
      {
        if (IsDigital)
          throw new DigitalDisplayException("The device is not analog.");
        return _reader.ReadBit(20, 2);
      }
    }

    /// <summary>
    ///     Gets a boolean value indicating if the display supports GTF with default parameter values
    /// </summary>
    public bool IsDefaultGTFSupported => _reader.ReadBit(24, 0);

    /// <summary>
    ///     Gets the supported input type
    /// </summary>
    /// <exception cref="AnalogDisplayException">The device is not digital.</exception>
    public DigitalVideoInputStandard DigitalVideoInterfaceStandardSupported
    {
      get
      {
        if (!IsDigital)
        {
          throw new AnalogDisplayException("The device is not digital.");
        }
        uint value = (uint)_reader.ReadInt(20, 0, 3);
        if (Enum.IsDefined(typeof(DigitalVideoInputStandard), value))
        {
          return (DigitalVideoInputStandard)value;
        }
        return DigitalVideoInputStandard.Reserved;
      }
    }

    /// <summary>
    ///     Gets the supported color bit depth
    /// </summary>
    /// <exception cref="AnalogDisplayException">The device is not digital.</exception>
    public ColorBitDepth ColorBitDepth
    {
      get
      {
        if (!IsDigital)
        {
          throw new AnalogDisplayException("The device is not digital.");
        }
        uint value = (uint)_reader.ReadInt(20, 4, 3);
        if (Enum.IsDefined(typeof(ColorBitDepth), value))
        {
          return (ColorBitDepth)value;
        }
        return ColorBitDepth.Reserved;
      }
    }

    /// <summary>
    ///     Gets a boolean value indicating if the device uses the digital signal for communication
    /// </summary>
    public bool IsDigital => _reader.ReadBit(20, 7);

    /// <summary>
    ///     Gets a boolean value indicating if the preferred timing mode specified in descriptor block 1. For EDID 1.3+ the
    ///     preferred timing mode is always in the first Detailed Timing Descriptor
    /// </summary>
    public bool PTMIncludesPIxelAndRefresh =>
        (_edid.EDIDVersion >= new Version(1, 3)) ||
        _reader.ReadBit(24, 1);

    /// <summary>
    ///     Gets a boolean value indicating if the preferred timing mode includes native pixel format and refresh rate. This
    ///     value is always false for EDID 1.2-
    /// </summary>
    public bool IsPreferredTimingModeIncludesNativeInformation
        => (_edid.EDIDVersion >= new Version(1, 3)) && _reader.ReadBit(24, 1);

    /// <summary>
    ///     Gets a boolean value indicating that the display don't have a fixed size
    /// </summary>
    public bool IsProjector => (_reader.ReadByte(21) == 0) || (_reader.ReadByte(22) == 0);


    /// <summary>
    ///     Gets a boolean value indicating that the separate sync is supported
    /// </summary>
    /// <exception cref="DigitalDisplayException">The device is not analog.</exception>
    public bool IsSeparateSyncSupported
    {
      get
      {
        if (IsDigital)
          throw new DigitalDisplayException("The device is not analog.");
        return _reader.ReadBit(20, 3);
      }
    }

    /// <summary>
    ///     Gets a boolean value indicating that the devices uses the standard sRGB colour space
    /// </summary>
    public bool IsStandardSRGBColorSpace => _reader.ReadBit(24, 2);

    /// <summary>
    ///     Gets a boolean value indicating if the DPMS standby supported
    /// </summary>
    public bool IsStandbySupported => _reader.ReadBit(24, 7);

    /// <summary>
    ///     Gets a boolean value indicating if the DPMS suspend supported
    /// </summary>
    public bool IsSuspendSupported => _reader.ReadBit(24, 6);

    /// <summary>
    ///     Gets a boolean value indicating that sync on green is supported
    /// </summary>
    /// <exception cref="DigitalDisplayException">The device is not analog.</exception>
    public bool IsSyncOnGreenSupported
    {
      get
      {
        if (IsDigital)
          throw new DigitalDisplayException("The device is not analog.");
        return _reader.ReadBit(20, 1);
      }
    }

    /// <summary>
    ///     Gets a boolean value indicating that VSync pulse must be serrated when composite or sync-on-green is used
    /// </summary>
    /// <exception cref="DigitalDisplayException">The device is not analog.</exception>
    public bool IsVSyncSerratedOnComposite
    {
      get
      {
        if (IsDigital)
          throw new DigitalDisplayException("The device is not analog.");
        return _reader.ReadBit(20, 0);
      }
    }

    /// <summary>
    ///     Gets the device physical height in cm
    /// </summary>
    /// <exception cref="ProjectorDisplayException">Display don't have a fixed size.</exception>
    public uint PhysicalHeight
    {
      get
      {
        if (IsProjector)
          throw new ProjectorDisplayException("Display don't have a fixed size.");
        return _reader.ReadByte(22);
      }
    }

    /// <summary>
    ///     Gets the device physical width in cm
    /// </summary>
    /// <exception cref="ProjectorDisplayException">Display don't have a fixed size.</exception>
    public uint PhysicalWidth
    {
      get
      {
        if (IsProjector)
          throw new ProjectorDisplayException("Display don't have a fixed size.");
        return _reader.ReadByte(21);
      }
    }


    /// <summary>
    ///     Gets the analog video white and sync levels, relative to blank
    /// </summary>
    /// <exception cref="DigitalDisplayException">The device is not analog.</exception>
    public AnalogVideoWhiteLevel VideoWhiteLevel
    {
      get
      {
        if (IsDigital)
          throw new DigitalDisplayException("The device is not analog.");
        return (AnalogVideoWhiteLevel)_reader.ReadInt(20, 5, 2);
      }
    }


    /// <inheritdoc />
    public bool Equals(DisplayParameters other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return _reader.ReadBytes(20, 15).SequenceEqual(other._reader.ReadBytes(20, 15));
    }

    /// <inheritdoc />
    public static bool operator ==(DisplayParameters left, DisplayParameters right)
    {
      return Equals(left, right);
    }

    /// <inheritdoc />
    public static bool operator !=(DisplayParameters left, DisplayParameters right)
    {
      return !Equals(left, right);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((DisplayParameters)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return _reader?.ReadBytes(20, 15).GetHashCode() ?? 0;
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return (IsDigital ? "Digital" : "Analog") + " " + (IsProjector ? "Projector" : "Display") + " " +
             (IsDigital ? DigitalDisplayType.ToString() : AnalogDisplayType.ToString());
    }
  }
}