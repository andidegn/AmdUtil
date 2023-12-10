using AMD.Util.AttributeHelper;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AMD.Util.Bios.Tables
{
  public enum eSMBiosTableType
  {
    [Name("BIOS Information")]
    BiosInformation                         = 0,

    [Name("System Information")]
    SystemInformation                       = 1,

    [Name("Baseboard Information")]
    BaseboardInformation                    = 2,

    [Name("Chassis Information")]
    ChassisInformation                      = 3,
    /* These are not implemented yet
    ProcessorInformation                    = 4,
    MemoryControllerInformation_OBSOLETE    = 5,
    MemoryModuleInformation_OBSOLETE        = 6,
    CacheInformation                        = 7,
    PortConnectorInformation                = 8,
    SystemSlots                             = 9,
    OnBoardDevicesInformation_OBSOLETE      = 10,
    OEMStrings                              = 11,
    SystemConfigurationOptions              = 12,
    BIOSLanguageInformation                 = 13,
    GroupAssociations                       = 14,
    SystemEventLog                          = 15,
    PhysicalMemoryArray                     = 16,
    MemoryDevice                            = 17,
    _32BitMemoryErrorInformation            = 18,
    MemoryArrayMappedAddress                = 19,
    MemoryDeviceMappedAddress               = 20,
    BuiltinPointingDevice                   = 21,
    PortableBattery                         = 22,
    SystemReset                             = 23,
    HardwareSecurity                        = 24,
    SystemPowerControls                     = 25,
    VoltageProbe                            = 26,
    CoolingDevice                           = 27,
    TemperatureProbe                        = 28,
    ElectricalCurrentProbe                  = 29,
    OutofBandRemoteAccess                   = 30,
    BISEntryPoint                           = 31,
    SystemBootInformation                   = 32,
    _64BitMemoryErrorInformation            = 33,
    ManagementDevice                        = 34,
    ManagementDeviceComponent               = 35,
    ManagementDeviceThresholdData           = 36,
    MemoryChannel                           = 37,
    IPMIDeviceInformation                   = 38,
    SystemPowerSupply                       = 39,
    AdditionalInformation                   = 40,
    OnboardDevicesExtendedInformation       = 41,
    ManagementControllerHostInterface       = 42,
    TPMDevice                               = 43,
    ProcessorAdditionalInformation          = 44,
    Inactive                                = 126,
    EndofTable                              = 127
    */
  }

  public abstract class ASMBiosTable : INotifyPropertyChanged
  {
    #region Interface OnPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion // Interface OnPropertyChanged

    internal SectionHeader SectionHeader { get; set; }

    internal Version SmBiosVersion { get; set; }

    protected string[] DecodeStrings(byte[] unformatted)
    {
      return Encoding.ASCII.GetString(unformatted).Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
    }
    internal string ReadString(int index, byte[] formatted, string[] strings)
    {
      int stringIndex = formatted[index] - 1;
      string retVal = string.Empty;
      if ((stringIndex >= 0) && (stringIndex < strings.Length))
      {
        retVal = strings[stringIndex];
      }
      return retVal;
    }

    internal abstract void Read(byte[] formatted, byte[] unformatted);
  }

  internal class SectionHeader
  {
    internal const int Size = 4;

    internal eSMBiosTableType StructureType;
    internal byte FormattedLength;
    internal ushort Handle;
  }
}
