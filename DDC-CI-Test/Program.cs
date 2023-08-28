using AMD.Util.Display;
using AMD.Util.Display.DDCCI.MCCSCodeStandard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDC_CI_Test
{
  internal class Program
  {
    static void Main(string[] args)
    {
      Monitor isicMonitor = null;

      MonitorList.RunAsync = false;
      MonitorList.SkipCapabilityCheck = true;
      Stopwatch sw = Stopwatch.StartNew();
      System.Collections.ObjectModel.ObservableCollection<Monitor> list = MonitorList.Instance.List;
      Console.WriteLine($"MonitorList Initialisation time: {sw.Elapsed}");
      foreach (Monitor mon in list)
      {
        Stopwatch swInner = Stopwatch.StartNew();
        mon.CheckLowLevelCapabilities();
        Console.WriteLine($"CapabilityString Query time for{mon.Name}: {swInner.Elapsed}");
        if (true == mon.VCPCodes?.Contains(eVCPCode.ManufacturerSpecific0xE0) &&
            true == mon.VCPCodes?.Contains(eVCPCode.ManufacturerSpecific0xEA))
        {
          isicMonitor = mon;
        }
      }

      Console.WriteLine($"Search for ISIC monitor time: {sw.Elapsed}");
      Console.WriteLine($"Mon found: {isicMonitor?.Name}");
      Console.ReadKey();
    }
  }
}
