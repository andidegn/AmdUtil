using AMD.Util.AttributeHelper;
using AMD.Util.Extensions;
using AMD.Util.Log;
using AMD.Util.ProcessUtil;
using AMD.Util.Serial;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace AMD.Util.Serial
{
  public class Comm
  {

    private static LogWriter log = LogWriter.Instance;

    public static bool SerialSetup(SerialPort sp, string portName, int baud)
    {
      try
      {
        if (portName == null || portName.Length < 4) return false;

        if (sp.IsOpen)
        {
          sp.Close();
        }

        sp.PortName = portName;
        sp.BaudRate = baud;
        sp.Parity = Parity.None;
        sp.StopBits = StopBits.One;
        sp.Handshake = Handshake.None;
        sp.DataBits = 8;
        sp.WriteTimeout = 1000;
        sp.ReadTimeout = 1000;
        sp.ReadBufferSize = 65536;
        sp.WriteBufferSize = 65536;

        sp.Open();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static bool SerialSetup(SerialPort sp, string portName, BaudRate baud)
    {
      return SerialSetup(sp, portName, baud.Value);
    }

    public static bool Write(SerialPort port, byte[] b_buf)
    {
      if (port == null)
      {
        log.PrintError("Port null");
        return false;
      }
      if (!port.IsOpen)
      {
        try
        {
          port.Open();
        }
        catch (Exception ex)
        {
          log.PrintError("Error opening port. Exception: {0}\n{1}", ex.Message, ex.StackTrace);
          throw;
        }
      }
      try
      {
        port.Write(b_buf, 0, b_buf.Length);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static bool Write(SerialPort port, byte[] b_buf, out byte[] reply, int replyLenExpected, int timeout = 500)
    {
      reply = null;

      if (!Write(port, b_buf))
      {
        return false;
      }

      try
      {
        Stopwatch sw = new Stopwatch();
        try
        {
          sw.Start();
          while (port.BytesToRead < replyLenExpected)
          {
            if (sw.Elapsed.TotalMilliseconds > timeout)
            {
              log.PrintError("Error waiting for data. Bytes to read: {0}", port.BytesToRead);
              return false;
            }
          }
          reply = new byte[port.BytesToRead];
          port.Read(reply, 0, port.BytesToRead);
        }
        catch (Exception ex)
        {
          log.PrintError("Exception: {0}\n{1}", ex.Message, ex.StackTrace);
          return false;
        }



      }
      catch (TimeoutException toex)
      {
        log.PrintError("TimeoutException: {0}\n{1}", toex.Message, toex.StackTrace);
        return false;
      }
      return true;
    }

    //public static bool Write(SerialPort port, byte[] b_buf, out byte[] reply, int replyLenExpected, int timeout = 500)
    //{
    //	reply = null;
    //	if (port == null)
    //	{
    //		log.PrintError("Port null");
    //	}
    //	if (!port.IsOpen)
    //	{
    //		try
    //		{
    //			port.Open();
    //		}
    //		catch (Exception ex)
    //		{
    //			log.PrintError("Error opening port. Exception: {0}\n{1}", ex.Message, ex.StackTrace);
    //			throw;
    //		}
    //	}
    //	try
    //	{
    //		port.Write(b_buf, 0, b_buf.Length);

    //		Stopwatch sw = new Stopwatch();
    //		try
    //		{
    //			sw.Start();
    //			while (port.BytesToRead < replyLenExpected)
    //			{
    //				if (sw.Elapsed.TotalMilliseconds > timeout)
    //				{
    //					log.PrintError("Error waiting for data. Bytes to read: {0}", port.BytesToRead);
    //					return false;
    //				}
    //			}
    //			reply = new byte[port.BytesToRead];
    //			port.Read(reply, 0, port.BytesToRead);
    //		}
    //		catch (Exception ex)
    //		{
    //			log.PrintError("Exception: {0}\n{1}", ex.Message, ex.StackTrace);
    //			return false;
    //		}



    //	}
    //	catch (TimeoutException toex)
    //	{
    //		log.PrintError("TimeoutException: {0}\n{1}", toex.Message, toex.StackTrace);
    //		return false;
    //	}
    //	return true;
    //}

    public static string[] GetAvalComPort()
    {
      return SerialPort.GetPortNames();
    }

    public static string[] GetAvalComPortSorted()
    {
      string[] ports = SerialPort.GetPortNames();
      return SerialPort.GetPortNames().OrderBy(x => int.Parse(3 < x.Length && x.ToUpper().StartsWith("COM") && x.Substring(3).IsNumber() ? x.Substring(3) : "0")).ToArray<string>();
    }

    public static List<ComPortParameter> GetComPortParameters(ushort VID, ushort PID, ushort VEN, ushort DEV, string postFix = null, bool activeOnly = true)
    {
      List<ComPortParameter> ports = new List<ComPortParameter>();

      RegistryKey regLM = Registry.LocalMachine;
      RegistryKey regSysCCSEnum = regLM.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

      foreach (string subKeyName in regSysCCSEnum.GetSubKeyNames())
      {
        RegistryKey regName = regSysCCSEnum.OpenSubKey(subKeyName);
        foreach (string subKeyNameInner in regName.GetSubKeyNames())
        {
          List<string> patternParts = new List<string>();

          if (0 < VID)
          {
            patternParts.Add(Regex.Escape($"VID_{VID:X4}+PID_{PID:X4}{postFix}"));
            patternParts.Add(Regex.Escape($"VID_{VID:X4}&PID_{PID:X4}{postFix}"));
          }
          if (0 < VEN)
          {
            patternParts.Add(Regex.Escape($"VEN_{VEN:X4}&DEV_{DEV:X4}{postFix}"));
          }

          string pattern = $"^({string.Join("|", patternParts)})";
          if (Regex.Match(subKeyNameInner, pattern, RegexOptions.IgnoreCase).Success)
          {
            RegistryKey rk4 = regName.OpenSubKey(subKeyNameInner);
            foreach (string s2 in rk4.GetSubKeyNames())
            {
              RegistryKey rk5 = rk4.OpenSubKey(s2);
              RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
              string portName = (string)rk6.GetValue("PortName");
              if (!string.IsNullOrEmpty(portName) && portName.StartsWith("COM") && byte.TryParse(portName.Substring(3), out byte portNumber) && (!activeOnly || GetAvalComPort().Contains(portName)))
              {
                string friendlyName = (string)rk5.GetValue("FriendlyName");
                string location = (string)rk5.GetValue("LocationInformation");
                ports.Add(new ComPortParameter()
                {
                  PortNumber = portNumber,
                  FriendlyName = friendlyName,
                  Location = location,
                  VID = VID,
                  PID = PID,
                  VEN = VEN,
                  DEV = DEV,
                  Postfix = postFix,
                  SubKeyName = s2,
                  CompleteKey = rk6.Name
                });
              }
            }
          }
        }
      }
      return ports.OrderBy(x => x.PortNumber).ToList();
    }

    public static List<ComPortParameter> GetComPortParameters(IEnumerable<int> portNumbers, bool activeOnly = true)
    {
      List<ComPortParameter> retVal = new List<ComPortParameter>();
      foreach (var pn in portNumbers)
      {
        try
        {
          foreach (ComPortParameter cpp in GetComPortParameter(pn, activeOnly))
          {
            if (null != cpp)
            {
              retVal.Add(cpp);
            }
          }
        }
        catch (Exception ex)
        {

        }
      }
      return retVal;
    }

    public static IEnumerable<ComPortParameter> GetComPortParameter(int portNumber, bool activeOnly = true)
    {
      string targetPortName = $"COM{portNumber}";
      RegistryKey regLM = Registry.LocalMachine;
      RegistryKey regSysCCSEnum = regLM.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
      List<ComPortParameter> retValList = new List<ComPortParameter>();

      //try
      //{
        foreach (string subKeyName in regSysCCSEnum.GetSubKeyNames())
        {
          if (subKeyName.Equals("USB"))
          {

          }
          RegistryKey regName = regSysCCSEnum.OpenSubKey(subKeyName);
          foreach (string subKeyNameInner in regName.GetSubKeyNames())
          {
            RegistryKey rk4 = regName.OpenSubKey(subKeyNameInner);
            foreach (string s2 in rk4.GetSubKeyNames())
            {
              RegistryKey rk5 = rk4.OpenSubKey(s2);
                RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                if (null != rk6)
            {
              string portName = (string)rk6.GetValue("PortName");
              if (!string.IsNullOrEmpty(portName) &&
                  portName.Equals(targetPortName, StringComparison.OrdinalIgnoreCase) &&
                  (!activeOnly || GetAvalComPort().Contains(portName)))
              {
                string friendlyName = (string)rk5.GetValue("FriendlyName");
                string location = (string)rk5.GetValue("LocationInformation");

                if (false)
                {
                  log.PrintDebug($"subKeyName: \"{subKeyName}\" - subKeyNameInner: \"{subKeyNameInner}\" for {portName}");
                  log.PrintDebug($"s2: \"{s2}\"");
                  log.PrintDebug($"rk4: \"{rk4}\"");
                  log.PrintDebug($"rk5: \"{rk5}\"");
                  log.PrintDebug($"rk6: \"{rk6}\"");
                }


                // String format ex: VID_0403+PID_6001+A658SC9XA
                // String format ex: VID_0416&PID_511C&MI_00
                // String format ex: VID_10C4&PID_EA60

                ComPortParameter retVal = new ComPortParameter()
                {
                  PortNumber = portNumber,
                  FriendlyName = friendlyName,
                  Location = location,
                  SubKeyName = s2,
                  CompleteKey = rk6.Name
                };

                string postFix = string.Empty;
                if (subKeyNameInner.Contains("VID_"))
                {

                  string vidStr = subKeyNameInner.Substring(subKeyNameInner.IndexOf("VID_", 0) + 4, 4);
                  string pidStr = subKeyNameInner.Substring(subKeyNameInner.IndexOf("PID_", 0) + 4, 4);
                  postFix = subKeyNameInner.Substring(subKeyNameInner.IndexOf(pidStr, 0) + pidStr.Length);
                  retVal.VID = ushort.Parse(vidStr, NumberStyles.HexNumber);
                  retVal.PID = ushort.Parse(pidStr, NumberStyles.HexNumber);
                }
                else if (subKeyNameInner.Contains("VEN_"))
                {
                  string venStr = subKeyNameInner.Substring(subKeyNameInner.IndexOf("VEN_", 0) + 4, 4);
                  string devStr = subKeyNameInner.Substring(subKeyNameInner.IndexOf("DEV_", 0) + 4, 4);
                  postFix = subKeyNameInner.Substring(subKeyNameInner.IndexOf(devStr, 0) + devStr.Length);
                  retVal.VEN = ushort.Parse(venStr, NumberStyles.HexNumber);
                  retVal.DEV = ushort.Parse(devStr, NumberStyles.HexNumber);
                }

                retVal.Postfix = postFix;
                yield return retVal;
              }
            }
          }
          }
        }
      //}
      //catch (Exception ex)
      //{
      //  LogWriter.Instance.PrintException(ex, "Error getting ComPOrtParameter");
      //}
      //return null;
    }

    public static IEnumerable<ComPortParameter> GetComPortParameterList(bool activeOnly = true)
    {
      RegistryKey regLM = Registry.LocalMachine;
      RegistryKey regSysCCSEnum = regLM.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
      List<ComPortParameter> retValList = new List<ComPortParameter>();

      //try
      //{
      foreach (string subKeyName in regSysCCSEnum.GetSubKeyNames())
      {
        if (subKeyName.Equals("USB"))
        {

        }
        RegistryKey regName = regSysCCSEnum.OpenSubKey(subKeyName);
        foreach (string subKeyNameInner in regName.GetSubKeyNames())
        {
          RegistryKey rk4 = regName.OpenSubKey(subKeyNameInner);
          foreach (string s2 in rk4.GetSubKeyNames())
          {
            RegistryKey rk5 = rk4.OpenSubKey(s2);
            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
            if (null != rk6)
            {
              string portName = (string)rk6.GetValue("PortName");
              if (!string.IsNullOrEmpty(portName) &&
                  Regex.Match(portName, @"^(com|COM)\d+$").Success &&
                  (!activeOnly || GetAvalComPort().Contains(portName)))
              {
                string friendlyName = (string)rk5.GetValue("FriendlyName");
                string location = (string)rk5.GetValue("LocationInformation");

                if (false)
                {
                  log.PrintDebug($"subKeyName: \"{subKeyName}\" - subKeyNameInner: \"{subKeyNameInner}\" for {portName}");
                  log.PrintDebug($"s2: \"{s2}\"");
                  log.PrintDebug($"rk4: \"{rk4}\"");
                  log.PrintDebug($"rk5: \"{rk5}\"");
                  log.PrintDebug($"rk6: \"{rk6}\"");
                }


                // String format ex: VID_0403+PID_6001+A658SC9XA
                // String format ex: VID_0416&PID_511C&MI_00
                // String format ex: VID_10C4&PID_EA60

                ComPortParameter retVal = new ComPortParameter()
                {
                  PortNumber = int.Parse(portName.Substring(3)),
                  FriendlyName = friendlyName,
                  Location = location,
                  SubKeyName = s2,
                  CompleteKey = rk6.Name
                };

                string postFix = string.Empty;
                if (subKeyNameInner.Contains("VID_"))
                {

                  string vidStr = subKeyNameInner.Substring(subKeyNameInner.IndexOf("VID_", 0) + 4, 4);
                  string pidStr = subKeyNameInner.Substring(subKeyNameInner.IndexOf("PID_", 0) + 4, 4);
                  postFix = subKeyNameInner.Substring(subKeyNameInner.IndexOf(pidStr, 0) + pidStr.Length);
                  retVal.VID = ushort.Parse(vidStr, NumberStyles.HexNumber);
                  retVal.PID = ushort.Parse(pidStr, NumberStyles.HexNumber);
                }
                else if (subKeyNameInner.Contains("VEN_"))
                {
                  string venStr = subKeyNameInner.Substring(subKeyNameInner.IndexOf("VEN_", 0) + 4, 4);
                  string devStr = subKeyNameInner.Substring(subKeyNameInner.IndexOf("DEV_", 0) + 4, 4);
                  postFix = subKeyNameInner.Substring(subKeyNameInner.IndexOf(devStr, 0) + devStr.Length);
                  retVal.VEN = ushort.Parse(venStr, NumberStyles.HexNumber);
                  retVal.DEV = ushort.Parse(devStr, NumberStyles.HexNumber);
                }

                retVal.Postfix = postFix;
                yield return retVal;
              }
            }
          }
        }
      }
      //}
      //catch (Exception ex)
      //{
      //  LogWriter.Instance.PrintException(ex, "Error getting ComPOrtParameter");
      //}
      //return null;
    }

    public static bool MoveComPort(ComPortParameter currentPort, int newPortNumber)
    {
      return MoveComPort(currentPort.VID, currentPort.PID, currentPort.VEN, currentPort.DEV, currentPort.Postfix, currentPort.SubKeyName, currentPort.PortNumber, newPortNumber);
    }

    public static bool MoveComPort(ushort VID, ushort PID, ushort VEN, ushort DEV, string postFix, string subKeyName, int currentPortNumber, int newPortNumber)
    {
      if (!MoveComPortVidPidVenDev(VID, PID, VEN, DEV, postFix, subKeyName, currentPortNumber, newPortNumber))
      {
        LogWriter.Instance.PrintError($"MoveComPortVidPid failed");
        return false;
      }

      if (!MoveComPortArbiter(currentPortNumber, newPortNumber))
      {
        LogWriter.Instance.PrintError($"MoveComPortArbiter failed");
        return false;
      }

      if (!MoveComPortSerialComm(currentPortNumber, newPortNumber))
      {
        LogWriter.Instance.PrintError($"MoveComPortSerialComm failed");
        return false;
      }

      //if (!MoveComPortProcessChangePort(currentPortNumber, newPortNumber))
      //{
      //  LogWriter.Instance.PrintError($"MoveComPortProcessChangePort failed");
      //  return false;
      //}
      return true;
    }

    public static List<int> GetReservedComPortsFromComArbiter()
    {
      List<int> retVal = new List<int>();
      string dataName = "ComDB";
      RegistryKey regHWDMSC = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\COM Name Arbiter", true);

      byte[] regComDB = regHWDMSC.GetValue(dataName) as byte[];

      for (int i = 0; i < regComDB.Length; i++)
      {
        for (int j = 0; j < 8; j++)
        {
          if (0 < (regComDB[i] & (1 << j)))
          {
            retVal.Add((8 * i) + j + 1);
          }
        }
      }
      return retVal;
    }

    private static bool MoveComPortArbiter(int currentPortNumber, int newPortNumber)
    {
      string dataName = "ComDB";
      RegistryKey regHWDMSC = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\COM Name Arbiter", true);

      byte[] regComDB = regHWDMSC.GetValue(dataName)as byte[];

      int curByteIndex = (currentPortNumber - 1) / 8;
      int newByteIndex = (newPortNumber - 1) / 8;

      int curBitIndex = (currentPortNumber - 1) % 8;
      int newBitIndex = (newPortNumber - 1) % 8;

      regComDB[curByteIndex] &= (byte)~(1 << ((byte)curBitIndex));
      regComDB[newByteIndex] |= (byte)(1 << ((byte)newBitIndex));

      regHWDMSC.SetValue(dataName, regComDB);

      return true;
    }

    private static bool MoveComPortSerialComm(int currentPortNumber, int newPortNumber)
    {
      RegistryKey regHWDMSC = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM", true);

      foreach (string entryName in regHWDMSC.GetValueNames())
      {
        string regValue = regHWDMSC.GetValue(entryName) as string;

        if (regValue.Equals($"COM{currentPortNumber}"))
        {
          regHWDMSC.SetValue(entryName, $"COM{newPortNumber}");
          return true;
        }
      }
      return false;
    }

    private static bool MoveComPortVidPidVenDev(ushort VID, ushort PID, ushort VEN, ushort DEV, string postFix, string subKeyName, int currentPortNumber, int newPortNumber)
    {
      List<string> patternParts = new List<string>();
      if (0 < VID)
      {
        patternParts.Add(Regex.Escape($"VID_{VID:X4}+PID_{PID:X4}{postFix}"));
        patternParts.Add(Regex.Escape($"VID_{VID:X4}&PID_{PID:X4}{postFix}"));
      }
      if (0 < VEN)
      {
        patternParts.Add(Regex.Escape($"VEN_{VEN:X4}&DEV_{DEV:X4}{postFix}"));
      }

      string pattern = $"^({string.Join("|", patternParts)})";

      RegistryKey regLM = Registry.LocalMachine;
      RegistryKey regSysCCSEnum = regLM.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum");

      foreach (string s1 in regSysCCSEnum.GetSubKeyNames())
      {
        RegistryKey regName = regSysCCSEnum.OpenSubKey(s1);
        foreach (string subKeyNameInner in regName.GetSubKeyNames())
        {
          if (Regex.Match(subKeyNameInner, pattern, RegexOptions.IgnoreCase).Success)
          {
            RegistryKey rk4 = regName.OpenSubKey(subKeyNameInner);
            foreach (string s2 in rk4.GetSubKeyNames())
            {
              if (subKeyName is null || s2.Equals(subKeyName, StringComparison.OrdinalIgnoreCase))
              {
                RegistryKey rk5 = rk4.OpenSubKey(s2, true);
                string location = rk5.GetValue("LocationInformation") as string;
                RegistryKey rk6 = rk5.OpenSubKey("Device Parameters", true);
                string portName = rk6.GetValue("PortName") as string;
                if (portName.Contains(currentPortNumber.ToString()))
                {
                  string friendlyName = (rk5.GetValue("FriendlyName") as string).Replace($"COM{currentPortNumber}", $"COM{newPortNumber}");

                  rk5.SetValue("FriendlyName", friendlyName);
                  rk6.SetValue("PortName", $"COM{newPortNumber}");

                  return true;
                }
              }
            }
          }
        }
      }
      return false;
    }

    private static bool MoveComPortProcessChangePort(int currentPortNumber, int newPortNumber)
    {
      bool success = false;
      try
      {
        ProcessHelper.DisableWow64FSRedirection();
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "change.exe";
        psi.CreateNoWindow = true;
        psi.WorkingDirectory = Environment.SystemDirectory;
        psi.Arguments = $"port COM{newPortNumber}=COM{currentPortNumber}";
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


        formatProcess.WaitForExit();
        success = 1 == formatProcess.ExitCode;


        psi.Arguments = $@"port /d COM{currentPortNumber}"; 
        formatProcess = Process.Start(psi);

        outputTxt += formatProcess.StandardOutput.ReadToEnd();
        errTxt += formatProcess.StandardError.ReadToEnd();


        psi.Arguments = @"port /query";
        formatProcess = Process.Start(psi);

        outputTxt += formatProcess.StandardOutput.ReadToEnd();
        errTxt += formatProcess.StandardError.ReadToEnd();

        if (!string.IsNullOrWhiteSpace(outputTxt))
        {
          LogWriter.Instance.PrintNotification(outputTxt);
        }
        if (!string.IsNullOrWhiteSpace(errTxt))
        {
          LogWriter.Instance.PrintError(errTxt);
        }
        formatProcess.WaitForExit();
        success = 1 == formatProcess.ExitCode;






        //swStandardInput.WriteLine($@"change port /d COM{currentPortNumber}");

        //swStandardInput.WriteLine(@"change port /query");
      }
      catch (Exception ex)
      {
        log.PrintException(ex, "Error calling change.exe");
      }
      finally
      {
        ProcessHelper.RevertWow64FSRedirection();
      }
      return success;
    }

    private static void RefreshPort(string identifier)
    {
      string[] portNames = SerialPort.GetPortNames();
      using (var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
      {
        //using (var searcher = new System.Management.ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSSerial_PortName")) {
        var prts = searcher.Get().Cast<System.Management.ManagementBaseObject>().ToList();
        foreach (var item in prts)
        {
          Console.WriteLine(item);
        }
        //var tList = (from n in portNames
        //             join p in prts on n equals p["DeviceID"].ToString()
        //             select p["Caption"] + ":" + n).ToList();
        //foreach (string s in tList) {
        //    if (s.Substring(0, identifier.Length).Equals(identifier)) {
        //        cbbCONPort.Items.Add(s.Substring(s.LastIndexOf(':') + 1));
        //    }
        //}
        //if (cbbCONPort.Items.Count > 0)
        //    cbbCONPort.SelectedIndex = 0;
      }
    }
  }

  public class ComPortParameter
  {
    public string FriendlyName { get; set; }
    public int PortNumber { get; set; }
    public ushort VID { get; set; }
    public ushort PID { get; set; }
    public ushort VEN { get; set; }
    public ushort DEV { get; set; }
    public string Postfix { get; set; }
    public string SubKeyName { get; set; }
    public string Location { get; set; }
    public string CompleteKey { get; set; }

    [XmlIgnore]
    public string PortName
    {
      get
      {
        return $"COM{PortNumber}";
      }
    }

    public ComPortParameter() 
    {
      VID = PID = VEN = DEV = 0;
    }

    public override string ToString()
    {
      return $"{FriendlyName} ({PortNumber}) - VID: {VID:X4} PID: {PID:X4}{Postfix}\\{SubKeyName} - Location: {Location} - Key: \"{CompleteKey}\"";
    }
  }
}