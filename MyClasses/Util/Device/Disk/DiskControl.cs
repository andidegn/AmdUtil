using AMD.Util.AttributeHelper;
using AMD.Util.Extensions;
using AMD.Util.Log;
using AMD.Util.ProcessUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

namespace AMD.Util.Device.Disk
{
  public static class DiskControl
  {
    public enum eFileSystem
    {
      [Name("FAT32")]
      FAT32,
      [Name("NTFS")]
      NTFS
    }

    public enum eDiskPartitionType
    {
      [Name("MBR")]
      MBR,
      [Name("GPT")]
      GPT
    }

    public static bool Format(char driveLetter, string label = "", eFileSystem fileSystem = eFileSystem.NTFS, bool quickFormat = true, int? clusterSize = null)
    {
      #region args check

      if (!Char.IsLetter(driveLetter))
      {
        return false;
      }

      #endregion
      bool success = false;
      string drive = driveLetter + ":";
      try
      {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "format.com";
        psi.CreateNoWindow = true;
        psi.WorkingDirectory = Environment.SystemDirectory;
        psi.Arguments = $"/FS:{fileSystem.GetAttribute<NameAttribute>().Name} /Y /V:{label}{(quickFormat ? " /Q" : string.Empty)}{(clusterSize.HasValue ? " /A:" + clusterSize.Value : "")} {drive}";
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        psi.RedirectStandardError = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardInput = true;

        LogWriter.Instance.PrintDebug($"{psi.FileName} {psi.Arguments}");

        Process formatProcess = Process.Start(psi);

        StreamWriter swStandardInput = formatProcess.StandardInput;

        string outputTxt = formatProcess.StandardOutput.ReadToEnd();
        string errTxt = formatProcess.StandardError.ReadToEnd();

        if (!string.IsNullOrWhiteSpace(outputTxt))
        {
          LogWriter.Instance.PrintNotification(outputTxt);
        }
        if (!string.IsNullOrWhiteSpace(errTxt))
        {
          LogWriter.Instance.PrintError(errTxt);
        }

        swStandardInput.WriteLine();

        formatProcess.WaitForExit();
        success = 0 == formatProcess.ExitCode;
      }
      catch (Exception) { }
      return success;
    }

    public static bool Clean(int diskId, bool localLog)
    {
      bool success = false;
      try
      {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "diskpart";
        psi.CreateNoWindow = true;
        psi.WorkingDirectory = Environment.SystemDirectory;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = localLog;
        psi.RedirectStandardError = localLog;
        psi.RedirectStandardOutput = localLog;
        psi.RedirectStandardInput = true;

        LogWriter.Instance.PrintDebug($"{psi.FileName} {psi.Arguments}");

        Process formatProcess = Process.Start(psi);

        StreamWriter swStandardInput = formatProcess.StandardInput;

        swStandardInput.WriteLine($"list disk");
        swStandardInput.WriteLine($"select disk {diskId}");
        swStandardInput.WriteLine($"attributes disk clear readonly");
        //swStandardInput.WriteLine($"online disk");
        swStandardInput.WriteLine($"select disk {diskId}");
        swStandardInput.WriteLine($"clean");
        swStandardInput.WriteLine($"exit");

        if (localLog)
        {
          string outputTxt = formatProcess.StandardOutput.ReadToEnd();
          string errTxt = formatProcess.StandardError.ReadToEnd();

          if (!string.IsNullOrWhiteSpace(outputTxt))
          {
            LogWriter.Instance.PrintNotification(outputTxt);
          }
          if (!string.IsNullOrWhiteSpace(errTxt))
          {
            LogWriter.Instance.PrintError(errTxt);
          }
        }
        formatProcess.WaitForExit();
        success = 0 == formatProcess.ExitCode;
      }
      catch (Exception) { }
      return success;
    }

    public static bool Initialize(int diskId, char desiredDriveLetter, eDiskPartitionType partitionType, string label = "", eFileSystem fileSystem = eFileSystem.NTFS, bool quickFormat = true, int clusterSize = 64, bool localLog = true)
    {
      bool success = false;
      try
      {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "diskpart";
        psi.CreateNoWindow = true;
        psi.WorkingDirectory = Environment.SystemDirectory;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = localLog;
        psi.RedirectStandardError = localLog;
        psi.RedirectStandardOutput = localLog;
        psi.RedirectStandardInput = true;

        LogWriter.Instance.PrintDebug($"{psi.FileName} {psi.Arguments}");

        Process formatProcess = Process.Start(psi);

        StreamWriter swStandardInput = formatProcess.StandardInput;

        swStandardInput.WriteLine("list disk");
        swStandardInput.WriteLine($"select disk {diskId}");
        swStandardInput.WriteLine("attributes disk clear readonly");
        //swStandardInput.WriteLine("online disk");
        swStandardInput.WriteLine($"select disk {diskId}");
        swStandardInput.WriteLine("clean");
        swStandardInput.WriteLine($"convert {partitionType.GetAttribute<NameAttribute>().Name}");
        swStandardInput.WriteLine("create partition primary");
        LogWriter.Instance.PrintDebug($"format {(quickFormat ? "quick" : string.Empty)} fs={fileSystem.GetAttribute<NameAttribute>().Name} label=\"{label}\" unit={clusterSize}k");
        swStandardInput.WriteLine($"format {(quickFormat ? "quick" : string.Empty)} fs={fileSystem.GetAttribute<NameAttribute>().Name} label=\"{label}\" unit={clusterSize}k");
        if (char.IsLetter(desiredDriveLetter) && !DriveExists(desiredDriveLetter))
        {
          swStandardInput.WriteLine($"assign letter=\"{desiredDriveLetter}\"");
        }
        swStandardInput.WriteLine("exit");

        if (localLog)
        {
          string outputTxt = formatProcess.StandardOutput.ReadToEnd();
          string errTxt = formatProcess.StandardError.ReadToEnd();

          if (!string.IsNullOrWhiteSpace(outputTxt))
          {
            LogWriter.Instance.PrintNotification(outputTxt);
          }
          if (!string.IsNullOrWhiteSpace(errTxt))
          {
            LogWriter.Instance.PrintError(errTxt);
          }
        }
        formatProcess.WaitForExit();
        success = 0 == formatProcess.ExitCode;
      }
      catch (Exception) { }
      return success;
    }


    public static IEnumerable<ExtendedDriveInfo> GetDrives()
    {
      var drives = DriveInfo.GetDrives();
      var externalDrives = new List<ExtendedDriveInfo>();
      PDiskInformation pd = new PDiskInformation();

      var scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
      var query = new ObjectQuery("select * from Win32_LogicalDisk");
      var allPhysicalDisks = new ManagementObjectSearcher("select *, DeviceID from Win32_DiskDrive").Get();

      foreach (ManagementObject physicalDisk in allPhysicalDisks)
      {
        ExtendedDriveInfo edi = new ExtendedDriveInfo()
        {
          DiskDrive = new DiskDrive(physicalDisk),
          DriveInfo = null,
          LogicalDisk = null
        };
        edi.PhysicalDiskStruct = pd.PDiskInfo[edi.DiskDrive.Index.ToString()];
        var allPartitionsOnPhysicalDisk = new ManagementObjectSearcher($"associators of {{Win32_DiskDrive.DeviceID='{physicalDisk["DeviceID"]}'}} where AssocClass = Win32_DiskDriveToDiskPartition").Get();
        foreach (ManagementObject partition in allPartitionsOnPhysicalDisk)
        {
          if (partition == null)
          {
            continue;
          }

          var allLogicalDisksOnPartition = new ManagementObjectSearcher($"associators of {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} where AssocClass = Win32_LogicalDiskToPartition").Get();
          foreach (ManagementObject logicalDisk in allLogicalDisksOnPartition)
          {
            edi.LogicalDisk = new LogicalDisk(logicalDisk);
            if (logicalDisk == null)
            {
              continue;
            }

            edi.DriveInfo = drives.Where(x => x.Name.StartsWith(logicalDisk["Name"] as string, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var mediaType = (physicalDisk["MediaType"] as string).ToLowerInvariant();
            if (mediaType.Contains("external") || mediaType.Contains("removable"))
            {
              edi.IsExternal = true;
            }
          }
        }
        externalDrives.Add(edi);
      }
      return externalDrives;
    }

    public static int GetDiskNumber(string letter)
    {
      var ret = "0";
      var scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
      var query = new ObjectQuery("Associators of {Win32_LogicalDisk.DeviceID='" + letter + ":'} WHERE ResultRole=Antecedent");
      var searcher = new ManagementObjectSearcher(scope, query);
      var queryCollection = searcher.Get();
      foreach (ManagementObject m in queryCollection)
      {
        ret = m["Name"].ToString().Replace("Disk #", "")[0].ToString();
      }
      return ret.IsNumber() ? int.Parse(ret) : -1;
    }

    public static bool DriveExists(char driveLetter)
    {
      if (!char.IsLetter(driveLetter))
      {
        return false;
      }
      string rootPath;
      rootPath = $"{driveLetter}:\\";
      return Directory.Exists(Path.GetPathRoot(rootPath));
    }

    #region for testing only
    public static DriveType GetDriveType(char driveLetter)
    {
      DriveType retVal = DriveType.Unknown;
      var scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
      var query = new ObjectQuery("select * from Win32_LogicalDisk");
      var searcher = new ManagementObjectSearcher(scope, query);
      var queryCollection = searcher.Get();
      foreach (ManagementObject m in queryCollection)
      {
        if (m["DeviceID"].ToString().Equals(driveLetter.ToString(), StringComparison.OrdinalIgnoreCase))
        {
          var v = m.GetPropertyValue(nameof(DriveType));
          retVal = (DriveType)Enum.Parse(typeof(DriveType), m[nameof(DriveType)].ToString());
          break;
        }
      }
      return retVal;
    }

    public static int GetWin32_DiskDrive()
    {
      LogWriter log = LogWriter.Instance;
      var ret = "0";
      var scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
      var query = new ObjectQuery("select * from Win32_DiskDrive");
      var searcher = new ManagementObjectSearcher(scope, query);
      var queryCollection = searcher.Get();
      foreach (ManagementObject m in queryCollection)
      {

        try { log.PrintDebug($"Name: {m["Name"]}"); } catch { }
        try { log.PrintDebug($"  Model: {m["Model"]}"); } catch { }
        try { log.PrintDebug($"  Availability: {m["Availability"]}"); } catch { }
        try { log.PrintDebug($"  BytesPerSector: {m["BytesPerSector"]}"); } catch { }
        try { log.PrintDebug($"  Capabilities[]: {m["Capabilities[]"]}"); } catch { }
        try { log.PrintDebug($"  CapabilityDescriptions: {m["CapabilityDescriptions"]}"); } catch { }
        try { log.PrintDebug($"  Caption: {m["Caption"]}"); } catch { }
        try { log.PrintDebug($"  CompressionMethod: {m["CompressionMethod"]}"); } catch { }
        try { log.PrintDebug($"  ConfigManagerErrorCode: {m["ConfigManagerErrorCode"]}"); } catch { }
        try { log.PrintDebug($"  ConfigManagerUserConfig: {m["ConfigManagerUserConfig"]}"); } catch { }
        try { log.PrintDebug($"  CreationClassName: {m["CreationClassName"]}"); } catch { }
        try { log.PrintDebug($"  DefaultBlockSize: {m["DefaultBlockSize"]}"); } catch { }
        try { log.PrintDebug($"  Description: {m["Description"]}"); } catch { }
        try { log.PrintDebug($"  DeviceID: {m["DeviceID"]}"); } catch { }
        try { log.PrintDebug($"  ErrorCleared: {m["ErrorCleared"]}"); } catch { }
        try { log.PrintDebug($"  ErrorDescription: {m["ErrorDescription"]}"); } catch { }
        try { log.PrintDebug($"  ErrorMethodology: {m["ErrorMethodology"]}"); } catch { }
        try { log.PrintDebug($"  FirmwareRevision: {m["FirmwareRevision"]}"); } catch { }
        try { log.PrintDebug($"  Index: {m["Index"]}"); } catch { }
        try { log.PrintDebug($"  InstallDate: {m["InstallDate"]}"); } catch { }
        try { log.PrintDebug($"  InterfaceType: {m["InterfaceType"]}"); } catch { }
        try { log.PrintDebug($"  LastErrorCode: {m["LastErrorCode"]}"); } catch { }
        try { log.PrintDebug($"  Manufacturer: {m["Manufacturer"]}"); } catch { }
        try { log.PrintDebug($"  MaxBlockSize: {m["MaxBlockSize"]}"); } catch { }
        try { log.PrintDebug($"  MaxMediaSize: {m["MaxMediaSize"]}"); } catch { }
        try { log.PrintDebug($"  MediaLoaded: {m["MediaLoaded"]}"); } catch { }
        try { log.PrintDebug($"  MediaType: {m["MediaType"]}"); } catch { }
        try { log.PrintDebug($"  MinBlockSize: {m["MinBlockSize"]}"); } catch { }
        try { log.PrintDebug($"  NeedsCleaning: {m["NeedsCleaning"]}"); } catch { }
        try { log.PrintDebug($"  NumberOfMediaSupported: {m["NumberOfMediaSupported"]}"); } catch { }
        try { log.PrintDebug($"  Partitions: {m["Partitions"]}"); } catch { }
        try { log.PrintDebug($"  PNPDeviceID: {m["PNPDeviceID"]}"); } catch { }
        try { log.PrintDebug($"  PowerManagementCapabilities: {m["PowerManagementCapabilities"]}"); } catch { }
        try { log.PrintDebug($"  PowerManagementSupported: {m["PowerManagementSupported"]}"); } catch { }
        try { log.PrintDebug($"  SCSIBus: {m["SCSIBus"]}"); } catch { }
        try { log.PrintDebug($"  SCSILogicalUnit: {m["SCSILogicalUnit"]}"); } catch { }
        try { log.PrintDebug($"  SCSIPort: {m["SCSIPort"]}"); } catch { }
        try { log.PrintDebug($"  SCSITargetId: {m["SCSITargetId"]}"); } catch { }
        try { log.PrintDebug($"  SectorsPerTrack: {m["SectorsPerTrack"]}"); } catch { }
        try { log.PrintDebug($"  SerialNumber: {m["SerialNumber"]}"); } catch { }
        try { log.PrintDebug($"  Signature: {m["Signature"]}"); } catch { }
        try { log.PrintDebug($"  Size: {m["Size"]}"); } catch { }
        try { log.PrintDebug($"  Status: {m["Status"]}"); } catch { }
        try { log.PrintDebug($"  StatusInfo: {m["StatusInfo"]}"); } catch { }
        try { log.PrintDebug($"  SystemCreationClassName: {m["SystemCreationClassName"]}"); } catch { }
        try { log.PrintDebug($"  SystemName: {m["SystemName"]}"); } catch { }
        try { log.PrintDebug($"  TotalCylinders: {m["TotalCylinders"]}"); } catch { }
        try { log.PrintDebug($"  TotalHeads: {m["TotalHeads"]}"); } catch { }
        try { log.PrintDebug($"  TotalSectors: {m["TotalSectors"]}"); } catch { }
        try { log.PrintDebug($"  TotalTracks: {m["TotalTracks"]}"); } catch { }
        try { log.PrintDebug($"  TracksPerCylinder: {m["TracksPerCylinder"]}"); } catch { }

        log.PrintDebug("    -----");
      }
      return ret.IsNumber() ? int.Parse(ret) : -1;
    }

    public static int GetWin32_LogicalDisk()
    {
      LogWriter log = LogWriter.Instance;
      var ret = "0";
      var scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
      var query = new ObjectQuery("select * from Win32_LogicalDisk");
      var searcher = new ManagementObjectSearcher(scope, query);
      var queryCollection = searcher.Get();
      foreach (ManagementObject m in queryCollection)
      {
        try { log.PrintDebug($"Name: {m["Name"]}"); } catch { }
        try { log.PrintDebug($"  Access: {m["Access"]}"); } catch { }
        try { log.PrintDebug($"  Availability: {m["Availability"]}"); } catch { }
        try { log.PrintDebug($"  BlockSize: {m["BlockSize"]}"); } catch { }
        try { log.PrintDebug($"  Caption: {m["Caption"]}"); } catch { }
        try { log.PrintDebug($"  Compressed: {m["Compressed"]}"); } catch { }
        try { log.PrintDebug($"  ConfigManagerErrorCode: {m["ConfigManagerErrorCode"]}"); } catch { }
        try { log.PrintDebug($"  ConfigManagerUserConfig: {m["ConfigManagerUserConfig"]}"); } catch { }
        try { log.PrintDebug($"  CreationClassName: {m["CreationClassName"]}"); } catch { }
        try { log.PrintDebug($"  Description: {m["Description"]}"); } catch { }
        try { log.PrintDebug($"  DeviceID: {m["DeviceID"]}"); } catch { }
        try { log.PrintDebug($"  DriveType: {m["DriveType"]}"); } catch { }
        try { log.PrintDebug($"  ErrorCleared: {m["ErrorCleared"]}"); } catch { }
        try { log.PrintDebug($"  ErrorDescription: {m["ErrorDescription"]}"); } catch { }
        try { log.PrintDebug($"  ErrorMethodology: {m["ErrorMethodology"]}"); } catch { }
        try { log.PrintDebug($"  FileSystem: {m["FileSystem"]}"); } catch { }
        try { log.PrintDebug($"  FreeSpace: {m["FreeSpace"]}"); } catch { }
        try { log.PrintDebug($"  InstallDate: {m["InstallDate"]}"); } catch { }
        try { log.PrintDebug($"  LastErrorCode: {m["LastErrorCode"]}"); } catch { }
        try { log.PrintDebug($"  MaximumComponentLength: {m["MaximumComponentLength"]}"); } catch { }
        try { log.PrintDebug($"  MediaType: {m["MediaType"]}"); } catch { }
        try { log.PrintDebug($"  NumberOfBlocks: {m["NumberOfBlocks"]}"); } catch { }
        try { log.PrintDebug($"  PNPDeviceID: {m["PNPDeviceID"]}"); } catch { }
        try { log.PrintDebug($"  PowerManagementCapabilities: {m["PowerManagementCapabilities"]}"); } catch { }
        try { log.PrintDebug($"  PowerManagementSupported: {m["PowerManagementSupported"]}"); } catch { }
        try { log.PrintDebug($"  ProviderName: {m["ProviderName"]}"); } catch { }
        try { log.PrintDebug($"  Purpose: {m["Purpose"]}"); } catch { }
        try { log.PrintDebug($"  QuotasDisabled: {m["QuotasDisabled"]}"); } catch { }
        try { log.PrintDebug($"  QuotasIncomplete: {m["QuotasIncomplete"]}"); } catch { }
        try { log.PrintDebug($"  QuotasRebuilding: {m["QuotasRebuilding"]}"); } catch { }
        try { log.PrintDebug($"  Size: {m["Size"]}"); } catch { }
        try { log.PrintDebug($"  Status: {m["Status"]}"); } catch { }
        try { log.PrintDebug($"  StatusInfo: {m["StatusInfo"]}"); } catch { }
        try { log.PrintDebug($"  SupportsDiskQuotas: {m["SupportsDiskQuotas"]}"); } catch { }
        try { log.PrintDebug($"  SupportsFileBasedCompression: {m["SupportsFileBasedCompression"]}"); } catch { }
        try { log.PrintDebug($"  SystemCreationClassName: {m["SystemCreationClassName"]}"); } catch { }
        try { log.PrintDebug($"  SystemName: {m["SystemName"]}"); } catch { }
        try { log.PrintDebug($"  VolumeDirty: {m["VolumeDirty"]}"); } catch { }
        try { log.PrintDebug($"  VolumeName: {m["VolumeName"]}"); } catch { }
        try { log.PrintDebug($"  VolumeSerialNumber: {m["VolumeSerialNumber"]}"); } catch { }

        log.PrintDebug("    -----");
      }
      return ret.IsNumber() ? int.Parse(ret) : -1;
    }

    public static int GetWin32_Volume()
    {
      LogWriter log = LogWriter.Instance;
      var ret = "0";
      var scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
      var query = new ObjectQuery("select * from Win32_Volume");
      ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
      ManagementObjectCollection queryCollection = searcher.Get();
      foreach (ManagementObject m in queryCollection)
      {
        try { log.PrintDebug($"Name: {m["Name"]}"); } catch { }
        try { log.PrintDebug($"  Access: {m["Access"]}"); } catch { }
        try { log.PrintDebug($"  Availability: {m["Availability"]}"); } catch { }
        try { log.PrintDebug($"  BlockSize: {m["BlockSize"]}"); } catch { }
        try { log.PrintDebug($"  Caption: {m["Caption"]}"); } catch { }
        try { log.PrintDebug($"  Compressed: {m["Compressed"]}"); } catch { }
        try { log.PrintDebug($"  ConfigManagerErrorCode: {m["ConfigManagerErrorCode"]}"); } catch { }
        try { log.PrintDebug($"  ConfigManagerUserConfig: {m["ConfigManagerUserConfig"]}"); } catch { }
        try { log.PrintDebug($"  CreationClassName: {m["CreationClassName"]}"); } catch { }
        try { log.PrintDebug($"  Description: {m["Description"]}"); } catch { }
        try { log.PrintDebug($"  DeviceID: {m["DeviceID"]}"); } catch { }
        try { log.PrintDebug($"  DriveType: {m["DriveType"]}"); } catch { }
        try { log.PrintDebug($"  ErrorCleared: {m["ErrorCleared"]}"); } catch { }
        try { log.PrintDebug($"  ErrorDescription: {m["ErrorDescription"]}"); } catch { }
        try { log.PrintDebug($"  ErrorMethodology: {m["ErrorMethodology"]}"); } catch { }
        try { log.PrintDebug($"  FileSystem: {m["FileSystem"]}"); } catch { }
        try { log.PrintDebug($"  FreeSpace: {m["FreeSpace"]}"); } catch { }
        try { log.PrintDebug($"  InstallDate: {m["InstallDate"]}"); } catch { }
        try { log.PrintDebug($"  LastErrorCode: {m["LastErrorCode"]}"); } catch { }
        try { log.PrintDebug($"  MaximumComponentLength: {m["MaximumComponentLength"]}"); } catch { }
        try { log.PrintDebug($"  MediaType: {m["MediaType"]}"); } catch { }
        try { log.PrintDebug($"  NumberOfBlocks: {m["NumberOfBlocks"]}"); } catch { }
        try { log.PrintDebug($"  PNPDeviceID: {m["PNPDeviceID"]}"); } catch { }
        try { log.PrintDebug($"  PowerManagementCapabilities: {m["PowerManagementCapabilities"]}"); } catch { }
        try { log.PrintDebug($"  PowerManagementSupported: {m["PowerManagementSupported"]}"); } catch { }
        try { log.PrintDebug($"  ProviderName: {m["ProviderName"]}"); } catch { }
        try { log.PrintDebug($"  Purpose: {m["Purpose"]}"); } catch { }
        try { log.PrintDebug($"  QuotasDisabled: {m["QuotasDisabled"]}"); } catch { }
        try { log.PrintDebug($"  QuotasIncomplete: {m["QuotasIncomplete"]}"); } catch { }
        try { log.PrintDebug($"  QuotasRebuilding: {m["QuotasRebuilding"]}"); } catch { }
        try { log.PrintDebug($"  Size: {m["Size"]}"); } catch { }
        try { log.PrintDebug($"  Status: {m["Status"]}"); } catch { }
        try { log.PrintDebug($"  StatusInfo: {m["StatusInfo"]}"); } catch { }
        try { log.PrintDebug($"  SupportsDiskQuotas: {m["SupportsDiskQuotas"]}"); } catch { }
        try { log.PrintDebug($"  SupportsFileBasedCompression: {m["SupportsFileBasedCompression"]}"); } catch { }
        try { log.PrintDebug($"  SystemCreationClassName: {m["SystemCreationClassName"]}"); } catch { }
        try { log.PrintDebug($"  SystemName: {m["SystemName"]}"); } catch { }
        try { log.PrintDebug($"  VolumeDirty: {m["VolumeDirty"]}"); } catch { }
        try { log.PrintDebug($"  VolumeName: {m["VolumeName"]}"); } catch { }
        try { log.PrintDebug($"  VolumeSerialNumber: {m["VolumeSerialNumber"]}"); } catch { }

        log.PrintDebug("    -----");
      }
      return ret.IsNumber() ? int.Parse(ret) : -1;
    }
    #endregion // for testing only
  }

  public class PDiskInformation
  {
    private static bool DEBUG = false;
    private Dictionary<string, PhysicalDiskStruct> _pdiCache = new Dictionary<string, PhysicalDiskStruct>();
    public Dictionary<string, PhysicalDiskStruct> PDiskInfo
    {
      get
      {
        if (_pdiCache.Count == 0)
          _pdiCache = GetDiskPI();
        return _pdiCache;
      }
    }

    public void ClearCache()
    {
      if (_pdiCache.Count > 0)
        _pdiCache.Clear();
    }

    private static Dictionary<string, PhysicalDiskStruct> GetDiskPI()
    {
      var DI = new Dictionary<string, PhysicalDiskStruct>();
      var scope = new ManagementScope(@"\\localhost\ROOT\Microsoft\Windows\Storage");
      var query = new ObjectQuery("SELECT * FROM MSFT_PhysicalDisk");
      var searcher = new ManagementObjectSearcher(scope, query);
      var dObj = searcher.Get();
      var wobj = new ManagementObjectSearcher("select * from MSFT_PhysicalDisk");
      foreach (ManagementObject diskobj in dObj)
      {
        var dis = new PhysicalDiskStruct();
        try
        {
          ushort[] dsuValues = (ushort[])diskobj["SupportedUsages"];
          dis.SupportedUsages = new DiskUsage[dsuValues.Length];
          dis.SupportedUsages = dsuValues.Select(x => (DiskUsage)x).Cast<DiskUsage>().ToArray();
        }
        catch (Exception ex)
        {
          dis.SupportedUsages = null;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "SupportedUsages");
        }
        try
        {
          dis.CannotPoolReason = (ushort[])diskobj["CannotPoolReason"];
        }
        catch (Exception ex)
        {
          dis.CannotPoolReason = null;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "CannotPoolReason");
        }
        try
        {
          dis.OperationalStatus = (ushort[])diskobj["OperationalStatus"];
        }
        catch (Exception ex)
        {
          dis.OperationalStatus = null;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "OperationalStatus");
        }
        try
        {
          dis.OperationalDetails = (string[])diskobj["OperationalDetails"];
        }
        catch (Exception ex)
        {
          dis.OperationalDetails = null;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "OperationalDetails");
        }
        try
        {
          dis.UniqueIdFormat = (DiskUniqueIdFormat)(ushort)diskobj["UniqueIdFormat"];
        }
        catch (Exception ex)
        {
          dis.UniqueIdFormat = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "UniqueIdFormat");
        }
        try
        {
          dis.DeviceId = diskobj["DeviceId"].ToString();
        }
        catch (Exception ex)
        {
          dis.DeviceId = "NA";
          if (DEBUG) LogWriter.Instance.PrintException(ex, "DeviceId");
        }
        try
        {
          dis.FriendlyName = (string)diskobj["FriendlyName"];
        }
        catch (Exception ex)
        {
          dis.FriendlyName = "?";
          if (DEBUG) LogWriter.Instance.PrintException(ex, "FriendlyName");
        }
        try
        {
          dis.HealthStatus = (DiskHealthStatus)(ushort)diskobj["HealthStatus"];
        }
        catch (Exception ex)
        {
          dis.HealthStatus = DiskHealthStatus.Unknown;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "HealthStatus");
        }
        try
        {
          dis.PhysicalLocation = (string)diskobj["PhysicalLocation"];
        }
        catch (Exception ex)
        {
          dis.PhysicalLocation = "?";
          if (DEBUG) LogWriter.Instance.PrintException(ex, "PhysicalLocation");
        }
        try
        {
          dis.VirtualDiskFootprint = (ulong)diskobj["VirtualDiskFootprint"];
        }
        catch (Exception ex)
        {
          dis.VirtualDiskFootprint = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "VirtualDiskFootprint");
        }
        try
        {
          dis.Usage = (DiskUsage)(ushort)diskobj["Usage"];
        }
        catch (Exception ex)
        {
          dis.Usage = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "Usage");
        }
        try
        {
          dis.Description = (string)diskobj["Description"];
        }
        catch (Exception ex)
        {
          dis.Description = "?";
          if (DEBUG) LogWriter.Instance.PrintException(ex, "Description");
        }
        try
        {
          dis.PartNumber = (string)diskobj["PartNumber"];
        }
        catch (Exception ex)
        {
          dis.PartNumber = "?";
          if (DEBUG) LogWriter.Instance.PrintException(ex, "PartNumber");
        }
        try
        {
          dis.FirmwareVersion = (string)diskobj["FirmwareVersion"];
        }
        catch (Exception ex)
        {
          dis.FirmwareVersion = "?";
          if (DEBUG) LogWriter.Instance.PrintException(ex, "FirmwareVersion");
        }
        try
        {
          dis.SoftwareVersion = (string)diskobj["SoftwareVersion"];
        }
        catch (Exception ex)
        {
          dis.SoftwareVersion = "?";
          if (DEBUG) LogWriter.Instance.PrintException(ex, "SoftwareVersion");
        }
        try
        {
          dis.Size = (ulong)diskobj["Size"];
        }
        catch (Exception ex)
        {
          dis.Size = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "Size");
        }
        try
        {
          dis.AllocatedSize = (ulong)diskobj["AllocatedSize"];
        }
        catch (Exception ex)
        {
          dis.AllocatedSize = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "AllocatedSize");
        }
        try
        {
          dis.BusType = (DiskBusType)(ushort)diskobj["BusType"];
        }
        catch (Exception ex)
        {
          dis.BusType = DiskBusType.Unknown;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "BusType");
        }
        try
        {
          dis.IsWriteCacheEnabled = (bool)diskobj["IsWriteCacheEnabled"];
        }
        catch (Exception ex)
        {
          dis.IsWriteCacheEnabled = false;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "IsWriteCacheEnabled");
        }
        try
        {
          dis.IsPowerProtected = (bool)diskobj["IsPowerProtected"];
        }
        catch (Exception ex)
        {
          dis.IsPowerProtected = false;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "IsPowerProtected");
        }
        try
        {
          dis.PhysicalSectorSize = (ulong)diskobj["PhysicalSectorSize"];
        }
        catch (Exception ex)
        {
          dis.PhysicalSectorSize = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "PhysicalSectorSize");
        }
        try
        {
          dis.LogicalSectorSize = (ulong)diskobj["LogicalSectorSize"];
        }
        catch (Exception ex)
        {
          dis.LogicalSectorSize = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "LogicalSectorSize");
        }
        try
        {
          dis.SpindleSpeed = (uint)diskobj["SpindleSpeed"];
        }
        catch (Exception ex)
        {
          dis.SpindleSpeed = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "SpindleSpeed");
        }
        try
        {
          dis.IsIndicationEnabled = (bool)diskobj["IsIndicationEnabled"];
        }
        catch (Exception ex)
        {
          dis.IsIndicationEnabled = false;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "IsIndicationEnabled");
        }
        try
        {
          dis.EnclosureNumber = (ushort)diskobj["EnclosureNumber"];
        }
        catch (Exception ex)
        {
          dis.EnclosureNumber = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "EnclosureNumber");
        }
        try
        {
          dis.SlotNumber = (ushort)diskobj["SlotNumber"];
        }
        catch (Exception ex)
        {
          dis.SlotNumber = 0;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "SlotNumber");
        }
        try
        {
          dis.CanPool = (bool)diskobj["CanPool"];
        }
        catch (Exception ex)
        {
          dis.CanPool = false;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "CanPool");
        }
        try
        {
          dis.OtherCannotPoolReasonDescription = (string)diskobj["OtherCannotPoolReasonDescription"];
        }
        catch (Exception ex)
        {
          dis.OtherCannotPoolReasonDescription = "?";
          if (DEBUG) LogWriter.Instance.PrintException(ex, "OtherCannotPoolReasonDescription");
        }
        try
        {
          dis.IsPartial = (bool)diskobj["IsPartial"];
        }
        catch (Exception ex)
        {
          dis.IsPartial = false;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "IsPartial");
        }
        try
        {
          dis.MediaType = (DiskMediaType)(ushort)diskobj["MediaType"];
        }
        catch (Exception ex)
        {
          dis.MediaType = DiskMediaType.Unspecified;
          if (DEBUG) LogWriter.Instance.PrintException(ex, "MediaType");
        }
        DI.Add(dis.DeviceId, dis);
      }
      return DI;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      foreach (var drive in PDiskInfo)
      {
        sb.AppendLine($"drive #{drive.Key}");
        sb.AppendLine($"  {drive.Value}");
      }
      return sb.ToString();
    }
  }

  public enum DiskBusType
  {
    Unknown = 0,  // The bus type is unknown.
    SCSI = 1,  // SCSI
    ATAPI = 2,  // ATAPI
    ATA = 3,  // ATA
    _1394 = 4,  // IEEE 1394
    SSA = 5,  // SSA
    FibreChannel = 6,  // Fibre Channel
    USB = 7,  // USB
    RAID = 8,  // RAID
    iSCSI = 9,  // iSCSI
    SAS = 10, // Serial Attached SCSI (SAS)
    SATA = 11, // Serial ATA (SATA)
    SD = 12, // Secure Digital (SD)
    MMC = 13, // Multimedia Card (MMC)
    MAX = 14, // This value is reserved for system use.
    FileBackedVirtual = 15, // File-Backed Virtual
    StorageSpaces = 16, // Storage Spaces
    NVMe = 17, //
    MicrosoftReserved = 18, // This value is reserved for system use.
  }

  public enum DiskHealthStatus
  {
    Healthy = 0,
    Warning = 1,
    Unhealthy = 2,
    Unknown = 5
  }

  public enum DiskMediaType
  {
    Unspecified = 0,
    HDD = 3,
    SSD = 4,
    SCM = 5
  }

  public enum DiskUsage
  {
    Unknown = 0,
    AutoSelect = 1,
    ManualSelect = 2,
    HotSpare = 3,
    Retired = 4,
    Journal = 5
  }

  public enum DiskUniqueIdFormat
  {
    VendorSpecific = 0,
    VenderId = 1,
    EUI64 = 2,
    FCPHName = 3,
    SCSINameString = 8
  }

  public class PhysicalDiskStruct
  {
    public ulong AllocatedSize { get; set; }
    public DiskBusType BusType { get; set; }
    public ushort[] CannotPoolReason { get; set; }
    public bool CanPool { get; set; }
    public string Description { get; set; }
    public string DeviceId { get; set; }
    public ushort EnclosureNumber { get; set; }
    public string FirmwareVersion { get; set; }
    public string FriendlyName { get; set; }
    public DiskHealthStatus HealthStatus { get; set; }
    public bool IsIndicationEnabled { get; set; }
    public bool IsPartial { get; set; }
    public bool IsPowerProtected { get; set; }
    public bool IsWriteCacheEnabled { get; set; }
    public ulong LogicalSectorSize { get; set; }
    public DiskMediaType MediaType { get; set; }
    public string[] OperationalDetails { get; set; }
    public ushort[] OperationalStatus { get; set; }
    public string OtherCannotPoolReasonDescription { get; set; }
    public string PartNumber { get; set; }
    public string PhysicalLocation { get; set; }
    public ulong PhysicalSectorSize { get; set; }
    public ulong Size { get; set; }
    public ushort SlotNumber { get; set; }
    public string SoftwareVersion { get; set; }
    public uint SpindleSpeed { get; set; }
    public DiskUsage[] SupportedUsages { get; set; }
    public DiskUniqueIdFormat UniqueIdFormat { get; set; }
    public DiskUsage Usage { get; set; }
    public ulong VirtualDiskFootprint { get; set; }


    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
      {
        string name = descriptor.Name;
        object value = descriptor.GetValue(this);
        sb.AppendLine($"  {name}: \"{value}\"");
      }
      return sb.ToString();
    }
  }

  public class ExtendedDriveInfo
  {
    public bool IsExternal { get; set; }
    public DriveInfo DriveInfo { get; set; }
    public LogicalDisk LogicalDisk { get; set; }
    public DiskDrive DiskDrive { get; set; }
    public PhysicalDiskStruct PhysicalDiskStruct { get; set; }
  }

  public enum eDiskMediaType
  {
    [Name("Unknown")]
    Unknown,

    [Name("Fixed hard disk media")]
    FixedHD,

    [Name("External hard disk media")]
    ExternalHD
  }

  public abstract class DiskBase
  {
    public string Name { get; set; }
    public string Caption { get; set; }
    public string Description { get; set; }
    public string DeviceID { get; set; }
  }


  public class LogicalDisk : DiskBase
  {
    public DriveType DriveType { get; set; }
    public string DriveTypeStr { get; set; }
    public string FileSystem { get; set; }

    public LogicalDisk() { }

    public LogicalDisk(ManagementObject m)
      : this()
    {
      Parse(m);
    }

    private void Parse(ManagementObject m)
    {
      try { Name = m["Name"].ToString(); } catch { }
      try { Caption = m["Caption"].ToString(); } catch { }
      try { Description = m["Description"].ToString(); } catch { }
      try { DeviceID = m["DeviceID"].ToString(); } catch { }
      try { DriveTypeStr = m["DriveType"].ToString(); } catch { }
      try { DriveType = (DriveType)Enum.Parse(typeof(DriveType), m["DriveType"].ToString()); } catch { }
      try { FileSystem = m["FileSystem"].ToString(); } catch { }
    }
  }

  public class DiskDrive : DiskBase
  {
    public string Model { get; set; }
    public int Index { get; set; }
    public string InterfaceType { get; set; }
    public eDiskMediaType MediaType { get; set; }
    public string PhysicalLocation { get; set; }

    public DiskDrive() { }

    public DiskDrive(ManagementObject m)
      : this()
    {
      Parse(m);
    }

    private void Parse(ManagementObject m)
    {
      try { Name = m["Name"].ToString(); } catch { }
      try { Model = m["Model"].ToString(); } catch { }
      try { Caption = m["Caption"].ToString(); } catch { }
      try { Description = m["Description"].ToString(); } catch { }
      try { DeviceID = m["DeviceID"].ToString(); } catch { }
      try { Index = int.Parse(m["Index"].ToString()); } catch { }
      try { InterfaceType = m["InterfaceType"].ToString(); } catch { }
      try { PhysicalLocation = m["PhysicalLocation"].ToString(); } catch { }
      try { MediaType = ((eDiskMediaType[])Enum.GetValues(typeof(eDiskMediaType))).Where(x => x.GetAttribute<NameAttribute>().Name.Equals(m["MediaType"].ToString())).DefaultIfEmpty(eDiskMediaType.Unknown).FirstOrDefault(); } catch { }
    }
  }
}
