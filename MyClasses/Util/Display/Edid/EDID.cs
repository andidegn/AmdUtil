using AMD.Util.Data;
using AMD.Util.Display.Edid.Descriptors;
using AMD.Util.Display.Edid.Enums;
using AMD.Util.Display.Edid.Exceptions;
using AMD.Util.Extensions;
using AMD.Util.GraphicsCard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AMD.Util.Display.Edid
{
  /// <summary>
  ///     Represents a Extended Display Identification Data instance
  /// </summary>
  public class EDID : IEquatable<EDID>, INotifyPropertyChanged
  {
    private static readonly byte[] FixedHeader = { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 };
    private readonly BitAwareReader _reader;

    #region Interface OnPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion // Interface OnPropertyChanged

    private ObservableCollection<string> parsingErrors;
    public ObservableCollection<string> ParsingErrors
    {
      get
      {
        return parsingErrors;
      }
      private set
      {
        parsingErrors = value;
        OnPropertyChanged();
      }
    }

    private string monitorNameDefaultIfEmpty = "No Name Found";

    public string FirstMonitorNameFromDescriptor
    {
      get
      {
        return MonitorNameFromDescriptor.DefaultIfEmpty(monitorNameDefaultIfEmpty).FirstOrDefault();
      }
    }

    /// <summary>
    ///     Creates a new EDID instance with the provided EDID binary data
    /// </summary>
    /// <param name="data">An array of bytes holding the EDID binary data</param>
    /// <exception cref="InvalidEDIDException">Invalid EDID binary data.</exception>
    public EDID(byte[] data)
    {
      ParsingErrors = new ObservableCollection<string>();
      if (data.Length < 128)
      {
        throw new InvalidEDIDException("EDID data must be at least 128 bytes.");
      }
      _reader = new BitAwareReader(data);
      if (!_reader.ReadBytes(0, 8).SequenceEqual(FixedHeader))
      {
        throw new InvalidEDIDException("EDID header missing.");
      }
      if (_reader.Data.Take(128).Aggregate(0, (i, b) => (i + b) % 256) > 0)
      {
        throw new InvalidEDIDException("EDID checksum failed.");
      }
      if (EDIDVersion.Major != 1)
      {
        ParsingErrors.Add("Invalid EDID major version.");
        //throw new InvalidEDIDException("Invalid EDID major version.");
      }
      if (EDIDVersion.Minor == 0)
      {
        ParsingErrors.Add("Invalid EDID minor version.");
        //throw new InvalidEDIDException("Invalid EDID minor version.");
      }
      if (string.IsNullOrWhiteSpace(MonitorNameFromDescriptor.FirstOrDefault()))
      {
        ParsingErrors.Add("Invalid EDID, No Monitor Name Descriptor found");
        //throw new InvalidEDIDException("Invalid EDID, No Monitor Name Descriptor found");
      }
      DisplayParameters = new DisplayParameters(this, _reader);
      OnPropertyChanged(null);
    }

    public byte[] RawData
    {
      get
      {
        return _reader.Data;
      }
    }

    public IEnumerable<string> MonitorNameFromDescriptor
    {
      get
      {
        foreach (var item in Descriptors)
        {
          if ((item as StringDescriptor)?.Type == StringDescriptorType.MonitorName)
          {
            yield return (item as StringDescriptor).Value;
          }
        }
      }
    }

    /// <summary>
    ///     Gets the enumerable list of descriptor blocks
    /// </summary>
    public IEnumerable<EDIDDescriptor> Descriptors
    {
      get
      {
        for (var i = 54; i < 126; i += 18)
        {
          var descriptor = EDIDDescriptor.FromData(this, _reader, i);
          if (descriptor != null)
          {
            yield return descriptor;
          }
        }
      }
    }

    /// <summary>
    ///     Gets an instance of DisplayParameters type representing the basic display parameters
    /// </summary>
    public DisplayParameters DisplayParameters { get; }

    /// <summary>
    ///     Gets the EDID specification version number
    /// </summary>
    public Version EDIDVersion => new Version(_reader.ReadByte(18), _reader.ReadByte(19));


    /// <summary>
    ///     Gets the enumerable list of extensions blocks
    /// </summary>
    public IEnumerable<EDIDExtension> Extensions
    {
      get
      {
        for (var i = 1; i <= NumberOfExtensions; i++)
        {
          var extension = EDIDExtension.FromData(this, _reader, i * 128);
          if (extension != null)
            yield return extension;
        }
      }
    }

    /// <summary>
    ///     Gets the date of manufacturing of the device with accuracy of +-6 days
    /// </summary>
    public DateTime ManufactureDate
    {
      get
      {
        var jan1 = new DateTime((int)ManufactureYear, 1, 1);
        var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

        var firstThursday = jan1.AddDays(daysOffset);
        var cal = CultureInfo.CurrentCulture.Calendar;
        var firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        var weekNum = ManufactureWeek;
        if (firstWeek <= 1)
          weekNum -= 1;
        var result = firstThursday.AddDays(weekNum * 7);
        return result.AddDays(-3);
      }
    }

    /// <summary>
    ///     Gets the manufacturer identification assigned by Microsoft to the device vendors in string
    /// </summary>
    public string ManufacturerCode
    {
      get
      {
        var edidCode = ManufacturerId;
        edidCode = ((edidCode & 0xff00) >> 8) | ((edidCode & 0x00ff) << 8);
        var byte1 = (byte)'A' + ((edidCode >> 0) & 0x1f) - 1;
        var byte2 = (byte)'A' + ((edidCode >> 5) & 0x1f) - 1;
        var byte3 = (byte)'A' + ((edidCode >> 10) & 0x1f) - 1;
        return $"{Convert.ToChar(byte3)}{Convert.ToChar(byte2)}{Convert.ToChar(byte1)}";
      }
    }

    /// <summary>
    ///     Gets the manufacturer identification assigned by Microsoft to the device vendors as a numberic value
    /// </summary>
    public uint ManufacturerId => (uint)_reader.ReadInt(8, 0, 2 * 8);

    /// <summary>
    ///     Gets the week of the device device production date as a number between 1 and 54
    /// </summary>
    /// <exception cref="ManufactureDateMissingException">Manufacture date is not available.</exception>
    public uint ManufactureWeek
    {
      get
      {
        var week = (uint)_reader.ReadByte(16);
        if (week == 0)
          throw new ManufactureDateMissingException("Manufacture date is not available.");
        return week;
      }
    }

    /// <summary>
    ///     Gets the year of the device device production date as a number between 1990 and 2245
    /// </summary>
    /// <exception cref="ManufactureDateMissingException">Manufacture date is not available.</exception>
    public uint ManufactureYear
    {
      get
      {
        if (ManufactureWeek > 0)
          return ProductYear;
        throw new ManufactureDateMissingException("Manufacture date is not available.");
      }
    }

    /// <summary>
    ///     Gets the expected number of extension blocks
    /// </summary>
    public uint NumberOfExtensions => _reader.ReadByte(126);

    /// <summary>
    ///     Gets the product identification code assigned by Microsoft to this series of devices
    /// </summary>
    public uint ProductCode => (uint)_reader.ReadInt(10, 0, 2 * 8);

    /// <summary>
    ///     Gets the year of the device device production or the model year of this product as a number between 1990 and 2245
    /// </summary>
    public uint ProductYear => (uint)_reader.ReadByte(17) + 1990;

    /// <summary>
    ///     Gets the numberic serial number of the device
    /// </summary>
    public uint SerialNumber => (uint)_reader.ReadInt(12, 0, 4 * 8);

    /// <summary>
    ///     Gets the enumerable list of valid timing combinations
    /// </summary>
    public IEnumerable<ITiming> Timings
    {
      get
      {
        var commonTiming = (CommonTimingIdentification)_reader.ReadInt(35, 0, 3 * 8);
        foreach (Enum value in Enum.GetValues(typeof(CommonTimingIdentification)))
          if (commonTiming.HasFlag(value))
            yield return new CommonTiming((CommonTimingIdentification)value);
        for (var i = 38; i < 54; i += 2)
        {
          var isValid = _reader.ReadInt(i, 0, 2 * 8) != 0x0101;
          if (isValid)
          {
            var width = _reader.ReadInt(i, 0, 8);
            var freq = _reader.ReadInt(i + 1, 0, 6);
            var ratio = (int)_reader.ReadInt(i + 1, 6, 2);
            if ((EDIDVersion < new Version(1, 3)) && (ratio == 0))
              ratio = -1;
            yield return new StandardTiming(((uint)width + 31) * 8, (PixelRatio)ratio, (uint)freq + 60);
          }
        }
      }
    }

    /// <inheritdoc />
    public bool Equals(EDID other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return _reader.ReadBytes(0, 128).SequenceEqual(other._reader.ReadBytes(0, 128));
    }

    /// <inheritdoc />
    public static bool operator ==(EDID left, EDID right)
    {
      return Equals(left, right);
    }

    /// <inheritdoc />
    public static bool operator !=(EDID left, EDID right)
    {
      return !Equals(left, right);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((EDID)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return _reader?.ReadBytes(0, 128).GetHashCode() ?? 0;
    }

    internal static int leftPad = 12;
    internal static int descriptionWidth = 38;
    internal static int firstLevelIndent = 20;
    internal static int firstLevelDescriptionWidth = 28;
    internal static int secondLevelIndent = 24;
    internal static int secondLevelDescriptionWidth = 24;
    
    /// <inheritdoc />
    public override string ToString()
    {
      bool? useByteFormat = true;
      StringBuilder sb = new StringBuilder();
      string rawEdidFormattedString = string.Empty;
      string warningHeader = ">------------------ WARNING -------------------<";


      if (0 < ParsingErrors.Count)
      {
        sb.AppendLine(warningHeader);
        sb.AppendLine($"The following {(1 < ParsingErrors.Count ? "errors were" : "error was")} found during parsing of the EDID file");
        foreach (string err in ParsingErrors)
        {
          sb.AppendLine(err);
        }
        sb.AppendLine(warningHeader);
        sb.AppendLine();
        sb.AppendLine();
      }

      if (useByteFormat == null || true == useByteFormat)
      {
        rawEdidFormattedString += StringFormatHelper.GetFormattedMemoryStringByteSeparated(0, RawData.GetNullableUIntArray(), 8);
      }
      if (useByteFormat == null || false == useByteFormat)
      {
        if (0 < rawEdidFormattedString.Length) 
        {
          rawEdidFormattedString += Environment.NewLine;
        }
        rawEdidFormattedString += StringFormatHelper.GetFormattedMemoryString(0, RawData.GetNullableUIntArray());
      }
      foreach (string line in rawEdidFormattedString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
      {
        sb.AppendLine($"{new string(' ', leftPad)}{line}");
      }
      sb.AppendLine();

      sb.AppendLine($"{"(  8h-9h  )".PadRight(leftPad)}{"ID Manufacture Name".PadRight(descriptionWidth)} : {ManufacturerCode}");
      sb.AppendLine($"{"( 0Ah-0Bh )".PadRight(leftPad)}{"ID Product Code".PadRight(descriptionWidth)} : {ProductCode:X2}");
      sb.AppendLine($"{"( 0Ch-0Fh )".PadRight(leftPad)}{"ID Serial Number".PadRight(descriptionWidth)} : {SerialNumber:X8}");
      sb.AppendLine($"{"(   10h   )".PadRight(leftPad)}{"Week of Manufacturer".PadRight(descriptionWidth)} : {ManufactureWeek}");
      sb.AppendLine($"{"(   11h   )".PadRight(leftPad)}{"Year of Manufacturer".PadRight(descriptionWidth)} : {ManufactureYear}");
      sb.AppendLine();
      sb.AppendLine($"{"( 12h-13h )".PadRight(leftPad)}{"EDID Version".PadRight(descriptionWidth)} : {EDIDVersion}");
      sb.AppendLine();
      if (DisplayParameters.IsDigital)
      {
        sb.AppendLine($"{"(   14h   )".PadRight(leftPad)}{"Video Input Definition".PadRight(descriptionWidth)} : Digital");
        sb.AppendLine($"{new string(' ', leftPad)}{"Color Depth".PadRight(descriptionWidth)} : {DisplayParameters.ColorBitDepth.GetAttribute<DescriptionAttribute>().Description}");
        sb.AppendLine($"{new string(' ', leftPad)}{"Video Interface".PadRight(descriptionWidth)} : {DisplayParameters.DigitalVideoInterfaceStandardSupported.GetAttribute<DescriptionAttribute>().Description}");
      }
      else
      {
        sb.AppendLine($"{"(   14h   )".PadRight(leftPad)}{"Video Input Definition".PadRight(descriptionWidth)} : Analog");
        sb.AppendLine($"{new string(' ', leftPad)}{"Serration of Vsync".PadRight(descriptionWidth)} : {(DisplayParameters.IsVSyncSerratedOnComposite ? "Yes" : "No")}");
        sb.AppendLine($"{new string(' ', leftPad)}{"Sync on Green Supported".PadRight(descriptionWidth)} : {(DisplayParameters.IsSyncOnGreenSupported ? "Yes" : "No")}");
        sb.AppendLine($"{new string(' ', leftPad)}{"Composite Sync Supported".PadRight(descriptionWidth)} : {(DisplayParameters.IsCompositeSyncSupported ? "Yes" : "No")}");
        sb.AppendLine($"{new string(' ', leftPad)}{"Separate Syncs Supported".PadRight(descriptionWidth)} : {(DisplayParameters.IsSeparateSyncSupported ? "Yes" : "No")}");
        sb.AppendLine($"{new string(' ', leftPad)}{"Is Black-To-Black Expected".PadRight(descriptionWidth)} : {(DisplayParameters.IsBlankToBlackExpected ? "Yes" : "No")}");
        sb.AppendLine($"{new string(' ', leftPad)}{"Signal Level Standard".PadRight(descriptionWidth)} : {DisplayParameters.VideoWhiteLevel.GetAttribute<DescriptionAttribute>().Description}");
      }
      sb.AppendLine();


      sb.AppendLine($"{"( 15h-16h )".PadRight(leftPad)}{"Is Projector".PadRight(descriptionWidth)} : {DisplayParameters.IsProjector}");
      if (!DisplayParameters.IsProjector)
      {
        sb.AppendLine($"{new string(' ', leftPad)}{"Display Size".PadRight(descriptionWidth)} : {DisplayParameters.DisplaySizeInInch} \"");
        sb.AppendLine($"{new string(' ', leftPad)}{"Width in cm".PadRight(descriptionWidth)} : {DisplayParameters.PhysicalWidth} cm");
        sb.AppendLine($"{new string(' ', leftPad)}{"Height in cm".PadRight(descriptionWidth)} : {DisplayParameters.PhysicalHeight} cm");
      }
      sb.AppendLine();

      double gamma = DisplayParameters.DisplayGamma;
      if (double.IsNaN(gamma))
      {
        sb.AppendLine($"{"(   17h   )".PadRight(leftPad)}{">-- WARNING --< Display Gamma is not defined here and must be defined in an extension block!".PadRight(descriptionWidth)}");
      }
      else
      {
        sb.AppendLine($"{"(   17h   )".PadRight(leftPad)}{"Display Gamma".PadRight(descriptionWidth)} : {DisplayParameters.DisplayGamma}");
      }
      sb.AppendLine();

      sb.AppendLine($"{"(   18h   )".PadRight(leftPad)}{"Default GTF Supported".PadRight(descriptionWidth)} : {(DisplayParameters.IsDefaultGTFSupported ? "Yes" : "No")}");
      sb.AppendLine($"{new string(' ', leftPad)}{"PTM has pixel format and refresh rate".PadRight(descriptionWidth)} : {(DisplayParameters.PTMIncludesPIxelAndRefresh ? "Yes" : "No")}");
      sb.AppendLine($"{new string(' ', leftPad)}{"Is Standard Default Color Space sRGB".PadRight(descriptionWidth)} : {(DisplayParameters.IsStandardSRGBColorSpace? "Yes" : "No")}");
      if (DisplayParameters.IsDigital)
      {
        sb.AppendLine($"{new string(' ',leftPad)}{"Digital Display Type".PadRight(descriptionWidth)} : {DisplayParameters.DigitalDisplayType.GetAttribute<DescriptionAttribute>().Description}");

      }
      else
      {
        sb.AppendLine($"{new string(' ', leftPad)}{"Analog Display Type".PadRight(descriptionWidth)} : {DisplayParameters.AnalogDisplayType}");
      }
      sb.AppendLine($"{new string(' ', leftPad)}Supported Power Modes");
      sb.AppendLine();
      sb.AppendLine($"{new string(' ', firstLevelIndent)}{"Active Off/Very Low Power".PadRight(firstLevelDescriptionWidth)} : {(DisplayParameters.IsActiveOffSupported ? "Yes" : "No")}");
      sb.AppendLine($"{new string(' ', firstLevelIndent)}{"Suspend".PadRight(firstLevelDescriptionWidth)} : {(DisplayParameters.IsSuspendSupported ? "Yes" : "No")}");
      sb.AppendLine($"{new string(' ', firstLevelIndent)}{"Standby".PadRight(firstLevelDescriptionWidth)} : {(DisplayParameters.IsStandbySupported ? "Yes" : "No")}");
      sb.AppendLine();

      sb.AppendLine($"{"( 19h-22h )".PadRight(leftPad)}Color Characteristics");
      sb.AppendLine();
      foreach (string chro in DisplayParameters.ChromaticityCoordinates.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
      {
        sb.AppendLine($"{new string(' ', firstLevelIndent)}{ chro}");
      }

      StringBuilder sbEt = new StringBuilder();
      StringBuilder sbSt = new StringBuilder();
      foreach (ITiming timing in Timings.OrderBy(x => x.Width))
      {
        switch (timing)
        {
          case CommonTiming ct:
            sbEt.AppendLine($"{new string(' ', firstLevelIndent)}{timing}");
            break;

          case StandardTiming st:
            sbSt.AppendLine($"{new string(' ', firstLevelIndent)}{timing}");
            break;

          default:
            break;
        }
      }
      sb.AppendLine($"{"( 23h-25h )".PadRight(leftPad)}Established Timings I/II and MF");
      sb.AppendLine();
      sb.Append(sbEt.ToString());
      sb.AppendLine();

      sb.AppendLine($"{"( 26h-35h )".PadRight(leftPad)}Standard Timings");
      sb.AppendLine();
      sb.Append(sbSt.ToString());
      sb.AppendLine();

      int ctr = 1;
      int idxStart = 54, idxEnd = 0;
      int desCnt = Descriptors.Count();
      foreach (EDIDDescriptor descriptor in Descriptors)
      {
        idxEnd = idxStart + 17;
        sb.AppendLine($"{$"({idxStart,3:2X}h-{idxEnd,-3:2X})".PadRight(leftPad)}Descriptor #{ctr++}: {descriptor.HeaderName}");
        sb.AppendLine();
        sb.AppendLine(descriptor.ToString());
        idxStart += 18;
      }

      return sb.ToString();
    }
  }
}