using AMD.Util.Extensions;
using AMD.Util.Log;
using AMD.Util.Serial;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace AMD.Util.Serial
{
  public class Comm
  {

    private static LogWriter log = LogWriter.Instance;

    public static bool SerialSetup(SerialPort sp, String portName, int baud)
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

    public static bool SerialSetup(SerialPort sp, String portName, BaudRate baud)
    {
      return SerialSetup(sp, portName, baud.Value);
    }

    public static bool Write(SerialPort port, byte[] b_buf)
    {
      if (port == null)
      {
        log.WriteToLog(LogMsgType.Error, "Port null");
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
          log.WriteToLog(LogMsgType.Error, "Error opening port. Exception: {0}\n{1}", ex.Message, ex.StackTrace);
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
              log.WriteToLog(LogMsgType.Error, "Error waiting for data. Bytes to read: {0}", port.BytesToRead);
              return false;
            }
          }
          reply = new byte[port.BytesToRead];
          port.Read(reply, 0, port.BytesToRead);
        }
        catch (Exception ex)
        {
          log.WriteToLog(LogMsgType.Error, "Exception: {0}\n{1}", ex.Message, ex.StackTrace);
          return false;
        }



      }
      catch (TimeoutException toex)
      {
        log.WriteToLog(LogMsgType.Error, "TimeoutException: {0}\n{1}", toex.Message, toex.StackTrace);
        return false;
      }
      return true;
    }

    //public static bool Write(SerialPort port, byte[] b_buf, out byte[] reply, int replyLenExpected, int timeout = 500)
    //{
    //	reply = null;
    //	if (port == null)
    //	{
    //		log.WriteToLog(LogMsgType.Error, "Port null");
    //	}
    //	if (!port.IsOpen)
    //	{
    //		try
    //		{
    //			port.Open();
    //		}
    //		catch (Exception ex)
    //		{
    //			log.WriteToLog(LogMsgType.Error, "Error opening port. Exception: {0}\n{1}", ex.Message, ex.StackTrace);
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
    //					log.WriteToLog(LogMsgType.Error, "Error waiting for data. Bytes to read: {0}", port.BytesToRead);
    //					return false;
    //				}
    //			}
    //			reply = new byte[port.BytesToRead];
    //			port.Read(reply, 0, port.BytesToRead);
    //		}
    //		catch (Exception ex)
    //		{
    //			log.WriteToLog(LogMsgType.Error, "Exception: {0}\n{1}", ex.Message, ex.StackTrace);
    //			return false;
    //		}



    //	}
    //	catch (TimeoutException toex)
    //	{
    //		log.WriteToLog(LogMsgType.Error, "TimeoutException: {0}\n{1}", toex.Message, toex.StackTrace);
    //		return false;
    //	}
    //	return true;
    //}

    public static String[] GetAvalComPort()
    {
      return SerialPort.GetPortNames();
    }
    public static String[] GetAvalComPortSorted()
    {
      String[] ports = SerialPort.GetPortNames();
      return SerialPort.GetPortNames().OrderBy(x => int.Parse(3 < x.Length && x.ToUpper().StartsWith("COM") && x.Substring(3).IsNumber() ? x.Substring(3) : "0")).ToArray<String>();
    }

    private static void RefreshPort(String identifier)
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
}
