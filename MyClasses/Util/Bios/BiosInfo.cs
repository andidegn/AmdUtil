
using AMD.Util.AttributeHelper;
using AMD.Util.Bios.Tables;
using AMD.Util.Collections.Dictionary;
using AMD.Util.Data;
using AMD.Util.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace AMD.Util.Bios
{
  /// <summary>
  /// Provides data read from the system BIOS.
  /// </summary>
  public class BiosInfo // : INotifyPropertyChanged
  {
    //#region Interface OnPropertyChanged
    //public event PropertyChangedEventHandler PropertyChanged;
    //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //{
    //  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}
    //#endregion // Interface OnPropertyChanged

    #region Public Properties
    public SerializableDictionary<eSMBiosTableType, ASMBiosTable> SmBiosTables { get; private set; }
    public AcpiTable AcpiTable { get; private set; }
    #endregion // Public Properties

    #region NativeMethods
    [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    private static extern uint GetSystemFirmwareTable(uint firmwareTableProviderSignature, uint firmwareTableId, IntPtr firmwareTableBuffer, uint bufferSize);

    [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    private static extern uint EnumSystemFirmwareTables(uint FirmwareTableProviderSignature, IntPtr pFirmwareTableEnumBuffer, uint BufferSize);
    #endregion // NativeMethods

    #region Constants
    private const uint AcpiTableProvider    = 'A' << 24 | 'C' << 16 | 'P' << 8 | 'I';
    //private const uint FirmwareTableProvider = (byte)'F' << 24 | (byte)'I' << 16 | (byte)'R' << 8 | (byte)'M';
    private const uint SmBiosTableProvider  = 'R' << 24 | 'S' << 16 | 'M' << 8 | 'B';
    private const uint MCFGTableProvider    = 'M' | 'C' << 8 | 'F' << 16 | 'G' << 24;
    private const uint FACPTableProvider    = 'F' | 'A' << 8 | 'C' << 16 | 'P' << 24;
    private const uint DSDTTableProvider    = 'D' | 'S' << 8 | 'D' << 16 | 'T' << 24;
    private const uint DefaultTable = 0;
    #endregion // Constants

    private BiosHeader biosHeader;

    private static BiosInfo instance;
    /// <summary>
    /// Reads some of the SMBIOS tables
    /// SMBIOS specifications: https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.3.0.pdf
    /// </summary>
    /// <returns></returns>
    public static BiosInfo Instance
    {
      get
      {
        if (null == instance)
        {
          instance = new BiosInfo();
        }

        return instance;
      }
    }

    private BiosInfo()
    {
      Refresh();
    }

    public void Refresh()
    {
      byte[] smBytes = GetTable(SmBiosTableProvider, DefaultTable);
      //byte[] acpiBytes = EnumTable(AcpiTableProvider);

      // It should be possible to enumerate through acpiBytes 4 bytes at a time and use those 4 bytes in GetTable(AcpiTableProvider, <the-4-bytes-as-uint-in-reverse-order-maybe>)

      //StringBuilder sb = new StringBuilder();
      //sb.AppendFormat("MCFG:{0}{1}", Environment.NewLine, StringFormatHelper.GetFormattedMemoryString(0, GetTable(AcpiTableProvider, MCFGTableProvider).GetNullableUIntArray()));
      //sb.AppendFormat("FCAP:{0}{1}", Environment.NewLine, StringFormatHelper.GetFormattedMemoryString(0, GetTable(AcpiTableProvider, FACPTableProvider).GetNullableUIntArray()));
      //sb.AppendFormat("MCFG:{0}{1}", Environment.NewLine, StringFormatHelper.GetFormattedMemoryStringByteSeparated(0, GetTable(AcpiTableProvider, MCFGTableProvider).GetNullableUIntArray(), 32));
      //sb.AppendFormat("FCAP:{0}{1}", Environment.NewLine, StringFormatHelper.GetFormattedMemoryStringByteSeparated(0, GetTable(AcpiTableProvider, FACPTableProvider).GetNullableUIntArray(), 32));

      //foreach (string line in sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
      //{
      //  LogWriter.Instance.PrintDebug("none", "{0}", line);
      //}

      using (BinaryReader reader = new BinaryReader(new MemoryStream(smBytes)))
      {
        // SMBIOS specifications: https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.3.0.pdf
        biosHeader = ReadBiosHeader(reader);
        if (biosHeader.SmBiosVersion.Major >= 2)
        {
          ReadSmBiosBytes(reader, smBytes);
        }
      }
      //using (BinaryReader reader = new BinaryReader(new MemoryStream(acpiBytes)))
      //{
      //  AcpiTable = new AcpiTable();
      //  AcpiTable.Read(reader, acpiBytes);
      //}
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      PropertyInfo[] properties = null;
      int maxNameLength = -1;

      foreach (KeyValuePair<eSMBiosTableType, ASMBiosTable> kvp in SmBiosTables)
      {
        eSMBiosTableType type = kvp.Key;
        ASMBiosTable table = kvp.Value;

        sb.AppendLine(type.GetAttribute<NameAttribute>().Name);

        properties = table.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        maxNameLength = properties.Select(x => x.GetCustomAttribute<NameAttribute>().Name.Length).Max() + 2;
        foreach (PropertyInfo property in properties)
        {
          if (property.Name.Equals(nameof(SectionHeader)))
          {
            continue;
          }
          sb.AppendLine($"{property.GetCustomAttribute<NameAttribute>().Name.PadRight(maxNameLength)}{property.GetValue(table, null)}");
        }
      }


      properties = AcpiTable.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      maxNameLength = properties.Select(x => x.Name.Length).Max() + 2;
      foreach (PropertyInfo property in properties)
      {
        object value = property.GetValue(AcpiTable, null);
        string valueStr = value.ToString(); ;
        if (property.PropertyType.IsArray && value is byte[] bArr)
        {
          valueStr = $"{Environment.NewLine}{StringFormatHelper.GetFormattedMemoryStringByteSeparated(0, bArr.GetNullableUIntArray(), 16)}";
        }
        sb.AppendLine($"{property.Name.PadRight(maxNameLength)}{valueStr}");
      }
      return sb.ToString();
    }

    #region Private Methods
    private BiosHeader ReadBiosHeader(BinaryReader reader)
    {
      BiosHeader header = new BiosHeader();
      header.Used20CallingMethod = reader.ReadByte();
      byte vMajor = reader.ReadByte();
      byte vMinor = reader.ReadByte();
      header.SmBiosVersion = new Version(vMajor, vMinor);
      header.DmiRevision = reader.ReadByte();
      header.Length = reader.ReadUInt32();
      return header;
    }

    private byte[] GetTable(uint provider, uint table)
    {
      uint size = GetSystemFirmwareTable(provider, table, IntPtr.Zero, 0);
      if (0 == size)
      {
        throw new Win32Exception();
      }

      var buffer = new byte[size];
      IntPtr nativeBuffer = Marshal.AllocHGlobal((int)size);
      try
      {
        if (0 == GetSystemFirmwareTable(provider, table, nativeBuffer, size))
        {
          throw new Win32Exception();
        }

        Marshal.Copy(nativeBuffer, buffer, 0, (int)size);
      }
      finally
      {
        Marshal.FreeHGlobal(nativeBuffer);
      }

      return buffer;
    }

    private byte[] EnumTable(uint provider)
    {
      uint size = EnumSystemFirmwareTables(provider, IntPtr.Zero, 0);
      if (0 == size)
      {
        throw new Win32Exception();
      }

      var buffer = new byte[size];
      IntPtr nativeBuffer = Marshal.AllocHGlobal((int)size);
      try
      {
        if (0 == EnumSystemFirmwareTables(provider, nativeBuffer, size))
        {
          throw new Win32Exception();
        }

        Marshal.Copy(nativeBuffer, buffer, 0, (int)size);
      }
      finally
      {
        Marshal.FreeHGlobal(nativeBuffer);
      }

      return buffer;
    }

    private SectionHeader ReadSectionHeader(BinaryReader reader)
    {
      var header = new SectionHeader();
      header.StructureType = (eSMBiosTableType)reader.ReadByte();
      header.FormattedLength = reader.ReadByte();
      header.Handle = reader.ReadUInt16();
      return header;
    }

    private void ReadSmBiosBytes(BinaryReader reader, byte[] bytes)
    {
      if (SmBiosTables is null)
      {
        SmBiosTables = new SerializableDictionary<eSMBiosTableType, ASMBiosTable>();
      }

      SmBiosTables[eSMBiosTableType.BiosInformation] = new SMBiosInfoTable();
      SmBiosTables[eSMBiosTableType.SystemInformation] = new SMBiosSystemInfoTable();
      SmBiosTables[eSMBiosTableType.BaseboardInformation] = new SMBiosBaseboardInfoTable();
      SmBiosTables[eSMBiosTableType.ChassisInformation] = new SMBiosChassisTable();

      while (reader.BaseStream.Position < biosHeader.Length)
      {
        SectionHeader sectionHeader = ReadSectionHeader(reader);
        byte[] formatted = reader.ReadBytes(sectionHeader.FormattedLength - SectionHeader.Size);
        int end = (int)reader.BaseStream.Position + 2;
        for (; end < bytes.Length; end++)
        {
          if ((bytes[end - 2] == 0) && (bytes[end - 1] == 0))
          {
            break;
          }
        }

        byte[] unformatted = reader.ReadBytes((int)(end - reader.BaseStream.Position));

        if (Enum.IsDefined(typeof(eSMBiosTableType), sectionHeader.StructureType))
        {
          var table = SmBiosTables[(eSMBiosTableType)sectionHeader.StructureType];
          table.SmBiosVersion = biosHeader.SmBiosVersion;
          table.SectionHeader = sectionHeader;
          table.Read(formatted, unformatted);
        }
      }
    }


    #endregion Private Methods
  }

  internal class BiosHeader
  {
    #region Fields

    internal byte Used20CallingMethod;
    internal Version SmBiosVersion;
    internal byte DmiRevision;
    internal uint Length;

    #endregion Fields
  }

  public class AcpiTable
  {
    public const int AcpiTableHeaderLength = 36;

    public byte[] RawData { get; set; }

    public string Signature { get; set; }
    public uint Length { get; set; }
    public byte Revision { get; set; }
    public byte Checksum { get; set; }
    public bool ChecksumIsValid { get; set; }
    public string OemId { get; set; }
    public string OemTableId { get; set; }
    public uint OemRevision { get; set; }
    public string CreatorId { get; set; }
    public uint CreatorRevision { get; set; }
    public byte[] Payload { get; set; }

    internal void Read(BinaryReader reader, byte[] bytes)
    {
      if (bytes.Length < AcpiTable.AcpiTableHeaderLength)
      {
        throw new ArgumentException("Invalid ACPI data");
      }

      RawData = bytes;
      Signature = Encoding.ASCII.GetString(reader.ReadBytes(4));
      Length = reader.ReadUInt32();
      Revision = reader.ReadByte();
      Checksum = reader.ReadByte();
      OemId = Encoding.ASCII.GetString(reader.ReadBytes(6));
      OemTableId = Encoding.ASCII.GetString(reader.ReadBytes(8));
      OemRevision = reader.ReadUInt32();
      CreatorId = Encoding.ASCII.GetString(reader.ReadBytes(4));
      CreatorRevision = reader.ReadUInt32();

      int payloadLength = bytes.Length - AcpiTable.AcpiTableHeaderLength;
      Payload = new byte[payloadLength];
      Array.Copy(bytes, AcpiTable.AcpiTableHeaderLength, Payload, 0, payloadLength);

      ChecksumIsValid = AcpiTable.ValidateChecksum(bytes);
    }

    public byte GetPayloadByte(int index)
    {
      return this.Payload[index];
    }

    public ushort GetPayloadUInt16(int index)
    {
      return BitConverter.ToUInt16(this.Payload, index);
    }

    public UInt32 GetPayloadUInt32(Int32 index)
    {
      return BitConverter.ToUInt32(this.Payload, index);
    }

    public UInt64 GetPayloadUInt64(Int32 index)
    {
      return BitConverter.ToUInt64(this.Payload, index);
    }

    public String GetPayloadString(Int32 index, Int32 length)
    {
      return Encoding.ASCII.GetString(this.Payload, index, length);
    }

    internal static bool ValidateChecksum(byte[] data)
    {
      byte sum = 0;

      for (var i = 0; i < data.Length; i++)
      {
        sum += data[i];
      }

      return 0 == sum;
    }
  }
}
