using AMD.Util.AttributeHelper;
using AMD.Util.Extensions;
using System;
using System.Linq;

namespace AMD.Util.Bios.Tables
{
  /// <summary>
  /// Ref Table 15: https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.3.0.pdf
  /// </summary>
  public enum eSMBiosChassisType : byte
  {
    [Name("Other")]
    Other = 1,

    [Name("Unknown")]
    Unknown,

    [Name("Desktop")]
    Desktop,

    [Name("Low Profile Desktop")]
    LowProfileDesktop,

    [Name("Pizza Box")]
    PizzaBox,

    [Name("Mini Tower")]
    MiniTower,

    [Name("Tower")]
    Tower,

    [Name("Portable")]
    Portable,

    [Name("Laptop")]
    Laptop,

    [Name("Notebook")]
    Notebook,

    [Name("Hand Held")]
    HandHeld,

    [Name("Docking Station")]
    DockingStation,

    [Name("All in One")]
    AllInOne,

    [Name("Sub Notebook")]
    SubNotebook,

    [Name("Space-saving")]
    SpaceSaving,

    [Name("Lunch Box")]
    LunchBox,

    [Name("Main Server Chassis")]
    MainServerChassis,

    [Name("Expansion Chassis")]
    ExpansionChassis,

    [Name("Sub Chassis")]
    SubChassis,

    [Name("Bus Expansion Chassis")]
    BusExpansionChassis,

    [Name("Peripheral Chassis")]
    PeripheralChassis,

    [Name("RAID Chassis")]
    RaidChassis,

    [Name("Rack Mount Chassis")]
    RackMountChassis,

    [Name("Sealed Case PC")]
    SealedCasePC,

    [Name("Multi System Chassis")]
    MultiSystemChassis,

    [Name("Compact PCI")]
    CompactPCI,

    [Name("Advanced TCA")]
    AdvancedTCA,

    [Name("Blade")]
    Blade,

    [Name("Blade Enclosure")]
    BladeEnclosure,

    [Name("Tablet")]
    Tablet,

    [Name("Convertible")]
    Convertible,

    [Name("Detachable")]
    Detachable,

    [Name("IoT Gateway")]
    IoTGateway,

    [Name("Embedded PC")]
    EmbeddedPC,

    [Name("Mini PC")]
    MiniPC,

    [Name("Stic PC")]
    StickPC
  }

  public enum eSMBiosChassisState : byte
  {
    [Name("Other")]
    Other = 1,

    [Name("Unknown")]
    Unknown,
    
    [Name("Safe")]
    Safe,
    
    [Name("Warning")]
    Warning
  }

  public enum eSMBiosChassisSecurityStatus : byte
  {
    [Name("Other")]
    Other = 1,

    [Name("Unknown")]
    Unknown,

    [Name("None")]
    None,

    [Name("External interface locked out")]
    ExternalInterfaceLockedOut,

    [Name("External interface enabled")]
    ExternalInterfaceEnabled
  }

  public class SMBiosChassisTable : ASMBiosTable
  {
    #region INotifyPropertyChanged Properteis
    private string manufacturer;
    [Name("Chassis Manufacturer")]
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

    private bool chassisLock;
    [Name("Chassis Lock")]
    public bool ChassisLock
    {
      get
      {
        return chassisLock;
      }
      set
      {
        chassisLock = value;
        OnPropertyChanged();
      }
    }

    private eSMBiosChassisType type;
    [Name("Chassis Type")]
    public eSMBiosChassisType Type
    {
      get
      {
        return type;
      }
      set
      {
        type = value;
        OnPropertyChanged();
      }
    }

    private string versionString;
    [Name("Chassis Version")]
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
    [Name("Chassis Serial Number")]
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

    private string assetTagNumber;
    [Name("Asset Tag Number")]
    public string AssetTagNumber
    {
      get
      {
        return assetTagNumber;
      }
      set
      {
        assetTagNumber = value;
        OnPropertyChanged();
      }
    }

    private eSMBiosChassisState lastBootUpState;
    [Name("Last Boot-up State")]
    public eSMBiosChassisState LastBootUpState
    {
      get
      {
        return lastBootUpState;
      }
      set
      {
        lastBootUpState = value;
        OnPropertyChanged();
      }
    }

    private eSMBiosChassisState powerSupplyState;
    [Name("Power Supply State")]
    public eSMBiosChassisState PowerSupplyState
    {
      get
      {
        return powerSupplyState;
      }
      set
      {
        powerSupplyState = value;
        OnPropertyChanged();
      }
    }

    private eSMBiosChassisState thermalState;
    [Name("Thermal State")]
    public eSMBiosChassisState ThermalState
    {
      get
      {
        return thermalState;
      }
      set
      {
        thermalState = value;
        OnPropertyChanged();
      }
    }

    private eSMBiosChassisSecurityStatus securityStatus;
    [Name("Security Status")]
    public eSMBiosChassisSecurityStatus SecurityStatus
    {
      get
      {
      return securityStatus;
      }
      set
      {
        securityStatus = value;
        OnPropertyChanged();
      }
    }

    private uint oemDefined;
    [Name("OEM-defined")]
    public uint OemDefined
    {
      get
      {
        return oemDefined;
      }
      set
      {
        oemDefined = value;
        OnPropertyChanged();
      }
    }

    private byte height;
    [Name ("Height")]
    public byte Height
    {
      get
      {
        return height;
      }
      set
      {
        height = value;
        OnPropertyChanged();
      }
    }

    private byte numberOfPowerCords;
    [Name("Number of Power Cords")]
    public byte NumberOfPowerCords
    {
      get
      {
        return numberOfPowerCords;
      }
      set
      {
        numberOfPowerCords = value;
        OnPropertyChanged();
      }
    }

    private byte containedElementCount;
    [Name("Contained Element Count (n)")]
    public byte ContainedElementCount
    {
      get
      {
        return containedElementCount;
      }
      set
      {
        containedElementCount = value;
        OnPropertyChanged();
      }
    }

    private byte containedElementRecordLength;
    [Name("Contained Element Record Length (m)")]
    public byte ContainedElementRecordLength
    {
      get
      {
        return containedElementRecordLength;
      }
      set
      {
        containedElementRecordLength = value;
        OnPropertyChanged();
      }
    }

    private byte containedElements;
    [Name("Contained Elements")]
    public byte ContainedElements
    {
      get
      {
        return containedElements;
      }
      set
      {
        containedElements = value;
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
    #endregion // INotifyPropertyChanged Properteis
    internal override void Read(byte[] formatted, byte[] unformatted)
    {
      string[] strings = DecodeStrings(unformatted);
      if (formatted.Length > 3)
      {

        Manufacturer = ReadString(0, formatted, strings);

        ChassisLock = 0 < (formatted[1] & 0x80);
        Type = (eSMBiosChassisType)(formatted[1] & 0x7F);

        VersionString                 = ReadString(2, formatted, strings);
        SerialNumber                  = ReadString(3, formatted, strings);
        AssetTagNumber                = ReadString(4, formatted, strings);
        LastBootUpState               = (eSMBiosChassisState)(formatted[5]);
        PowerSupplyState              = (eSMBiosChassisState)(formatted[6]);
        ThermalState                  = (eSMBiosChassisState)(formatted[7]);
        SecurityStatus                = (eSMBiosChassisSecurityStatus)(formatted[8]);
        OemDefined                    = formatted.SubArray(9, 4).GetUIntArray()[0];
        Height                        = formatted[13];
        NumberOfPowerCords            = formatted[14];
        ContainedElementCount         = formatted[15];
        ContainedElementRecordLength  = formatted[16];
        ContainedElements             = formatted[17];
        SkuNumber                     = strings.Last();
      }
    }
  }
}
