using AMD.Util.AttributeHelper;
using System;

namespace AMD.Util.Bios.Tables
{
  public class SMBiosInfoTable : ASMBiosTable
  {
    #region INotifyPropertyChanged Properteis
    private string biosVendor;
    [Name("BIOS Vendor")]
    public string BiosVendor
    {
      get
      {
        return biosVendor;
      }
      set
      {
        biosVendor = value;
        OnPropertyChanged();
      }
    }

    private string biosVersionString;
    [Name("BIOS Version")]
    public string BiosVersionString
    {
      get
      {
        return biosVersionString;
      }
      set
      {
        biosVersionString = value;
        OnPropertyChanged();
      }
    }

    private string biosReleaseDate;
    [Name("BIOS Release Date")]
    public string BiosReleaseDate
    {
      get
      {
        return biosReleaseDate;
      }
      set
      {
        biosReleaseDate = value;
        OnPropertyChanged();
      }
    }

    private Version biosVersion;
    [Name("BIOS Version")]
    public Version BiosVersion
    {
      get
      {
        return biosVersion;
      }
      set
      {
        biosVersion = value;
        OnPropertyChanged();
      }
    }
    #endregion // INotifyPropertyChanged Properteis
    internal override void Read(byte[] formatted, byte[] unformatted)
    {
      string[] strings = DecodeStrings(unformatted);
      if (formatted.Length > 4)
      {
        BiosVendor = ReadString(0, formatted, strings);
        BiosVersionString = ReadString(1, formatted, strings);
        BiosReleaseDate = ReadString(4, formatted, strings);

        if (SmBiosVersion >= new Version(2, 4) && (formatted.Length > 17))
        {
          BiosVersion = new Version(formatted[16], formatted[17]);
        }
      }
    }
  }
}
