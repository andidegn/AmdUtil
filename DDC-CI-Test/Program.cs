using AMD.Util.Display;
using AMD.Util.Display.DDCCI.MCCSCodeStandard;
using AMD.Util.Display.DDCCI.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AMD.Util.Extensions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace DDC_CI_Test
{
  public class PadBenchmarkTest
  {
    private int tests = 1000;
    private int numOfPads = 1000;
    [Benchmark]
    public void MyLeftPadTest()
    {
      string str = "test";
      for (int i = 0; i < tests; i++)
      {
        str = str.MyPadLeft(numOfPads, '|');
      }
    }

    [Benchmark]
    public void FastLeftPadTest()
    {
      string str = "test";
      for (int i = 0; i < tests; i++)
      {
        str = str.FastPadLeft(numOfPads, '|');
      }
    }

    [Benchmark]
    public void StringLeftPadTest()
    {
      string str = "test";
      for (int i = 0; i < tests; i++)
      {
        str = str.PadLeft(numOfPads, '|');
      }
    }
  }

  internal class Program
  {
    static void Main(string[] args)
    {
      //StringBuilder sb = new StringBuilder("public enum eVCPCode : byte");
      //sb.AppendLine("{");
      //foreach (eVCPCode item in Enum.GetValues(typeof(eVCPCode)))
      //{
      //  sb.AppendLine($"  [Name(\"{VCPCodeStandard.Instance.GetName(item)}\")]");
      //  sb.AppendLine($"  {item},");
      //  sb.AppendLine();
      //}
      //sb.AppendLine("}");
      //Clipboard.SetText(sb.ToString());

      //return;

      var summary = BenchmarkRunner.Run<PadBenchmarkTest>();
      Console.ReadKey();
      return;
#if false
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
#endif
    }
  }
}
