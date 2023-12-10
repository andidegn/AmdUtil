using AMD.Util.AttributeHelper;
using AMD.Util.Extensions;
using System;

namespace AMD.Util.Bios.Tables
{
  public class SMBiosSystemInfoTable : ASMBiosTable
  {
    #region INotifyPropertyChanged Properteis
    private string manufacturer;
    [Name("Manufacturer")]
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

    private string productName;
    [Name("Product Name")]
    public string ProductName
    {
      get
      {
        return productName;
      }
      set
      {
        productName = value;
        OnPropertyChanged();
      }
    }

    private string versionString;
    [Name("Product Version")]
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
    [Name("Serial Number")]
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

    private Guid guid;
    [Name("GUID")]
    public Guid Guid
    {
      get
      {
        return guid;
      }
      set
      {
        guid = value;
        OnPropertyChanged();
      }
    }

    private string skuNumber;
    [Name("SKU Number")]
    public string SkuNumber
    {
      get
      {
        return skuNumber;
      }
      set
      {
        skuNumber = value;
        OnPropertyChanged();
      }
    }

    private string productFamily;
    [Name("Product Family")]
    public string ProductFamily
    {
      get
      {
        return productFamily;
      }
      set
      {
        productFamily = value;
        OnPropertyChanged();
      }
    }
    #endregion // INotifyPropertyChanged Properties

    internal override void Read(byte[] formatted, byte[] unformatted)
    {
      string[] strings = DecodeStrings(unformatted);
      if (formatted.Length > 3)
      {
        Manufacturer = ReadString(0, formatted, strings);
        ProductName = ReadString(1, formatted, strings);
        VersionString = ReadString(2, formatted, strings);
        SerialNumber = ReadString(3, formatted, strings);

        if (SmBiosVersion >= new Version(2, 1) && (formatted.Length > 19))
        {
          Guid = new Guid(formatted.SubArray(4, 16));
        }

        if (SmBiosVersion >= new Version(2, 4) && (formatted.Length > 22))
        {
          SkuNumber = ReadString(21, formatted, strings);
          ProductFamily = ReadString(22, formatted, strings);
        }
      }
    }
  }
}
