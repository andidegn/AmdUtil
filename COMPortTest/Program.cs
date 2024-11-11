using AMD.Util.Device.Hardware;
using AMD.Util.Log;
using AMD.Util.Serial;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMPortTest
{
  internal class Program
  {
    static void Main(string[] args)
    {
      try
      {
        HardwareHelper hh = new HardwareHelper();
        var all = hh.GetAll();
        var touchDevice = hh.GetAll().Where(x => x.name.Contains("touch screen")).FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(touchDevice.name))
        {
          hh.SetDeviceState(touchDevice, DeviceStatus.Enabled == touchDevice.status ? false : true);
        }
      }
      catch (Exception ex)
      {
      }
    }
    static void Main1(string[] args)
    {
      byte oldPort = 44;
      byte newPort = 45;
      LogWriter lr = LogWriter.Instance;
      lr.PrintNotification(">>>>>>>>>>>>>>>>>>>> starting test <<<<<<<<<<<<<<<<<<<<<");
      //lr.PrintNotification($"Test result: {(Comm.MoveComPort(0x10C4, 0xEA60, oldPort, newPort) ? "Passed" : "Failed")}");

      foreach (string arg in Comm.GetAvalComPortSorted())
      {
        ComPortParameter cpp = Comm.GetComPortParameter(byte.Parse(arg.Substring(3)), true).FirstOrDefault();

        lr.PrintNotification($"\nPortname: \n\t{string.Join($"{Environment.NewLine}\t", Comm.GetComPortParameters(cpp.VID, cpp.PID, 0, 0, cpp.Postfix, true))}");

        if (cpp.PortNumber == oldPort )
        {
          Comm.MoveComPort(cpp, newPort);
        }
      }

      //lr.PrintNotification($"Portname: {string.Join(Environment.NewLine, Comm.GetPortName(0x0403, 0x6001, false))}");
      //lr.PrintNotification($"Portname: {string.Join(Environment.NewLine, Comm.GetPortName(0x10C4, 0xEA60, false))}");
      //lr.PrintNotification($"VID PID: {Comm.GetPortVidPid(4, false)}");

      SerialPort sp = new SerialPort();
      try
      {
        bool success = Comm.SerialSetup(sp, $"COM{newPort}", 19200);
        lr.PrintNotification($"Com port opening {(sp.IsOpen ? "succeeded" : "failed")}");
      }
      catch (Exception)
      {

        throw;
      }
      finally
      {
        sp.Close();
        sp.Dispose();
      }



      lr.PrintNotification("<<<<<<<<<<<<<<<<<<<<< ending test >>>>>>>>>>>>>>>>>>>>");
    }
  }
}
