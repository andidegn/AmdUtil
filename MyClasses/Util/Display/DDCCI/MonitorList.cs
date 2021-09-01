using AMD.Util.Display.DDCCI.Util;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace AMD.Util.Display
{
  public class MonitorList
  {
    private ObservableCollection<Monitor> ml;

    public ObservableCollection<Monitor> List
    {
      get { return ml; }
      set { ml = value; }
    }

    private static MonitorList instance;

    public MonitorList()
    {
      ml = new ObservableCollection<Monitor>();
      var @delegate = new NativeMethods.MonitorEnumDelegate(MonitorEnum);
      NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, @delegate, IntPtr.Zero);
    }

    public static MonitorList Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new MonitorList();
        }
        return instance;
      }
    }

    private bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref Rectangle lprcMonitor, IntPtr dwData)
    {
      Add(hMonitor);
      return true;
    }

    public bool Add(IntPtr hMonitor)
    {
      bool retVal = false;
      uint monitorCount = 0;

      if (NativeMethods.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref monitorCount))
      {
        var monitorArray = new NativeStructures.PHYSICAL_MONITOR[monitorCount];
        NativeMethods.GetPhysicalMonitorsFromHMONITOR(hMonitor, monitorCount, monitorArray);

        NativeStructures.MonitorInfoEx mInfo = new NativeStructures.MonitorInfoEx();
        for (int i = 0; i < monitorArray.Length; i++)
        //foreach (var physicalMonitor in monitorArray)
        {
          var physicalMonitor = monitorArray[i];
          NativeMethods.GetMonitorInfo(hMonitor, ref mInfo);
          Monitor newMonitor = new Monitor(physicalMonitor);

          ml.Add(newMonitor);
        }
        retVal = true;
      }
      return retVal;
    }
  }
}
