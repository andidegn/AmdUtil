using AMD.Util.AttributeHelper;
using System;

namespace AMD.Util.Bios.Tables
{
  /// <summary>
  /// Ref Table 15: https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.3.0.pdf
  /// </summary>
  public enum eSMBiosBoardType : byte
  {
    [Name("Unknown")]
    Unknown = 1,

    [Name("Other")]
    Other,

    [Name("Server Blade")]
    ServerBlade,

    [Name("Connectivity Switch")]
    ConnectivitySwitch,

    [Name("System Management Module")]
    SystemManagementModule,

    [Name("Processor Module")]
    ProcessorModule,

    [Name("IO Module")]
    IOModule,

    [Name("Memory Module")]
    MemoryModule,

    [Name("Daughter Board")]
    Daughterboard,

    [Name("Motherboard")]
    Motherboard,

    [Name("Processor or Memory Module")]
    ProcessorOrMemoryModule,

    [Name("Processor or IO Module")]
    ProcessorOrIOModule,

    [Name("Interconnect Board")]
    InterconnectBoard,
  }

  public class SMBiosBaseboardInfoTable : ASMBiosTable
  {
    #region INotifyPropertyChanged Properteis
    private string manufacturer;
    [Name("Board Manufacturer")]
    public string Manufacturer
    {
      get
      {
        return manufacturer;
      }
      set
      {
        manufacturer = value;
        OnPropertyChanged();
      }
    }

    private string product;
    [Name("Board Name")]
    public string Product
    {
      get
      {
        return product;
      }
      set
      {
        product = value;
        OnPropertyChanged();
      }
    }

    private string versionString;
    [Name("Board Version")]
    public string VersionString
    {
      get
      {
        return versionString;
      }
      set
      {
        versionString = value;
        OnPropertyChanged();
      }
    }

    private string serialNumber;
    [Name("Board Serial Number")]
    public string SerialNumber
    {
      get
      {
        return serialNumber;
      }
      set
      {
        serialNumber = value;
        OnPropertyChanged();
      }
    }

    private string assetTag;
    [Name("Board Asset Tag")]
    public string AssetTag
    {
      get
      {
        return assetTag;
      }
      set
      {
        assetTag = value;
        OnPropertyChanged();
      }
    }

    private byte featureFlag;
    [Name("Board Feature Flag")]
    public byte FeatureFlag
    {
      get
      {
        return featureFlag;
      }
      set
      {
        featureFlag = value;
        OnPropertyChanged();
      }
    }

    private string locationInChassis;
    [Name("Location in Chassis")]
    public string LocationInChassis
    {
      get
      {
        return locationInChassis;
      }
      set
      {
        locationInChassis = value;
        OnPropertyChanged();
      }
    }

    private eSMBiosBoardType boardType;
    [Name("Board Type")]
    public eSMBiosBoardType BoardType
    {
      get
      {
        return boardType;
      }
      set
      {
        boardType = value;
        OnPropertyChanged();
      }
    }
    #endregion // INotifyPropertyChanged Properties

    internal override void Read(byte[] formatted, byte[] unformatted)
    {
      string[] strings = DecodeStrings(unformatted);
      if (formatted.Length > 3)
      {
        Manufacturer      = ReadString(0, formatted, strings);
        Product           = ReadString(1, formatted, strings);
        VersionString     = ReadString(2, formatted, strings);
        SerialNumber      = ReadString(3, formatted, strings);
        AssetTag          = ReadString(4, formatted, strings);
        FeatureFlag       = formatted[5];
        LocationInChassis = ReadString(6, formatted, strings);
        BoardType         = (eSMBiosBoardType)formatted[9];
      }
    }
  }
}
