using AMD.Util.Display.Edid.Exceptions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AMD.Util.Display.Edid.Descriptors
{
  /// <summary>
  ///     Represents an EDID monitor range limit descriptor block
  /// </summary>
  public class MonitorRangeLimitsDescriptor : EDIDDescriptor
  {
    private static readonly byte[] FixedHeader = { 0x00, 0x00, 0x00, 0xFD, 0x00 };

    internal MonitorRangeLimitsDescriptor(EDID edid, BitAwareReader reader, int offset) : base(edid, reader, offset)
    {
      IsValid = Reader.ReadBytes(Offset, 5).SequenceEqual(FixedHeader);
      if (IsValid)
      {
        HeaderName = "Monitor Range Limits";
      }
    }

    /// <summary>
    ///     Gets the GTF C value (0–127.5)
    /// </summary>
    /// <exception cref="ExtendedTimingNotAvailable">Secondary GTF is not supported.</exception>
    public double GTFC
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        if (!IsSecondaryGTFSupported)
        {
          EDID.ParsingErrors.Add($"{HeaderName}.{nameof(GTFC)}: Secondary GTF is not supported.");
          //throw new ExtendedTimingNotAvailable("Secondary GTF is not supported.");
        }
        return Reader.ReadByte(Offset + 13) / 2d;
      }
    }

    /// <summary>
    ///     Gets the GTF J value (0–127.5)
    /// </summary>
    /// <exception cref="ExtendedTimingNotAvailable">Secondary GTF is not supported.</exception>
    public double GTFJ
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        if (!IsSecondaryGTFSupported)
        {
          EDID.ParsingErrors.Add($"{HeaderName}.{nameof(GTFJ)}: Secondary GTF is not supported.");
          //throw new ExtendedTimingNotAvailable("Secondary GTF is not supported.");
        }
        return Reader.ReadByte(Offset + 17) / 2d;
      }
    }

    /// <summary>
    ///     Gets the GTF K value (0–255)
    /// </summary>
    /// <exception cref="ExtendedTimingNotAvailable">Secondary GTF is not supported.</exception>
    public byte GTFK
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        if (!IsSecondaryGTFSupported)
        {
          EDID.ParsingErrors.Add($"{HeaderName}.{nameof(GTFK)}: Secondary GTF is not supported.");
          //throw new ExtendedTimingNotAvailable("Secondary GTF is not supported.");
        }
        return Reader.ReadByte(Offset + 16);
      }
    }

    /// <summary>
    ///     Gets the GTF M value (0–65535)
    /// </summary>
    /// <exception cref="ExtendedTimingNotAvailable">Secondary GTF is not supported.</exception>
    public ushort GTFM
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        if (!IsSecondaryGTFSupported)
        {
          EDID.ParsingErrors.Add($"{HeaderName}.{nameof(GTFM)}: Secondary GTF is not supported.");
          //throw new ExtendedTimingNotAvailable("Secondary GTF is not supported.");
        }
        return (ushort)Reader.ReadInt(Offset + 14, 0, 2 * 8);
      }
    }

    /// <summary>
    ///     Gets a boolean value indicating that the secondary GTF is supported
    /// </summary>
    public bool IsSecondaryGTFSupported
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        return Reader.ReadByte(Offset + 10) == 0x02;
      }
    }

    /// <summary>
    ///     Gets the maximum horizontal field rate
    /// </summary>
    public uint MaximumHorizontalFieldRate
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        return Reader.ReadByte(Offset + 8) * 1000u;
      }
    }

    /// <summary>
    ///     Gets the maximum pixel clock rate in hz (10–2550 MHz)
    /// </summary>
    public ulong MaximumPixelClockRate
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        return Reader.ReadByte(Offset + 9) * 10000000ul;
      }
    }

    /// <summary>
    ///     Gets the maximum vertical field rate
    /// </summary>
    public uint MaximumVerticalFieldRate
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        return Reader.ReadByte(Offset + 6);
      }
    }

    /// <summary>
    ///     Gets the minimum horizontal field rate
    /// </summary>
    public uint MinimumHorizontalFieldRate
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        return Reader.ReadByte(Offset + 7) * 1000u;
      }
    }

    /// <summary>
    ///     Gets the minimum vertical field rate
    /// </summary>
    public uint MinimumVerticalFieldRate
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        return Reader.ReadByte(Offset + 5);
      }
    }

    /// <summary>
    ///     Gets the start frequency for the secondary curve in hz (0–510 kHz)
    /// </summary>
    /// <exception cref="ExtendedTimingNotAvailable">Secondary GTF is not supported.</exception>
    public uint SecondaryCurveStartFrequency
    {
      get
      {
        if (!IsValid)
        {
          throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
        }
        if (!IsSecondaryGTFSupported)
        {
          EDID.ParsingErrors.Add($"{HeaderName}.{nameof(SecondaryCurveStartFrequency)}: Secondary GTF is not supported.");
          //throw new ExtendedTimingNotAvailable("Secondary GTF is not supported.");
        }
        return (uint)Reader.ReadByte(Offset + 12) * 2000;
      }
    }

    public string PToString
    {
      get
      {
        return ToString();
      }
    }

    /// <inheritdoc />
    public override string ToString()
    {
      if (!IsValid)
      {
        throw new InvalidDescriptorException("The provided data does not belong to this descriptor.");
      }
      StringBuilder sb = new StringBuilder();

      sb.AppendLine($"{new string(' ', EDID.secondLevelIndent)}{"Horizontal Scan Range".PadRight(EDID.secondLevelDescriptionWidth)} : {MinimumHorizontalFieldRate / 1000.0}KHz - {MaximumHorizontalFieldRate / 1000.0}KHz");
      sb.AppendLine($"{new string(' ', EDID.secondLevelIndent)}{"Vertical Scan Range".PadRight(EDID.secondLevelDescriptionWidth)} : {MinimumVerticalFieldRate}Hz - {MaximumVerticalFieldRate}Hz");
      sb.AppendLine($"{new string(' ', EDID.secondLevelIndent)}{"Max Pixel Clock".PadRight(EDID.secondLevelDescriptionWidth)} : {MaximumPixelClockRate / 1000000.0}MHz");
      sb.AppendLine($"{new string(' ', EDID.secondLevelIndent)}{"Secondary GTF".PadRight(EDID.secondLevelDescriptionWidth)} : {(IsSecondaryGTFSupported ? "Supported" : "Not Supported")}");
      if (IsSecondaryGTFSupported)
      {
        sb.AppendLine("<ERROR> Secondary GTF values are not supported yet <ERROR>");
      }
      return sb.ToString();
    }
  }
}