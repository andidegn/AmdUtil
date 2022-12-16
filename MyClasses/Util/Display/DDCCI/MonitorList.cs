using AMD.Util.Display.DDCCI.Util;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AMD.Util.Display
{
  public class MonitorList
  {
    #region Progress
    public IProgress<ProgressChangedEventArgs> Progress { get; set; }
    private void Report(string status, int percent)
    {
      Progress?.Report(new ProgressChangedEventArgs(percent, status));
    }
    #endregion // Progress

    public static bool SkipCapabilityCheck { get; set; }
    public static bool RunAsync { get; set; } = true;
    public ObservableCollection<Monitor> List { get; set; }
    private List<Monitor> tempList;
    private LogWriter log;

    private static MonitorList instance;

    public static MonitorList Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new MonitorList(RunAsync);
        }
        return instance;
      }
    }

    private MonitorList(bool async = true)
    {
      log = LogWriter.Instance;
      List = new ObservableCollection<Monitor>();
      ScanSystem(async);
    }

    public async void ScanSystem(bool async = true)
    {
      Report("Scan system start...", 0);
      log.WriteToLog(LogMsgType.Notification, "System scan start...");
      Dispatcher d = null;
      try
      {
        d = Application.Current.Dispatcher;
      }
      catch (Exception e)
      {
        log.WriteToLog(e);
      }
        
      if (null != d)
      {
        d.Invoke(() => List.Clear());
      }
      else
      {
        log.WriteToLog(LogMsgType.Notification, "Clearing list...");
        List.Clear();
      }
      tempList = new List<Monitor>();
      log.WriteToLog(LogMsgType.Notification, "awaiting EnumDisplayMonitors...");
      NativeMethods.MonitorEnumDelegate del = new NativeMethods.MonitorEnumDelegate(MonitorEnum);
      if (async)
      {
        log.WriteToLog(LogMsgType.Debug, "Running async");
        await Task.Factory.StartNew(() => NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, del, IntPtr.Zero));
      }
      else
      {
        log.WriteToLog(LogMsgType.Debug, "Running NOT async");
        NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, del, IntPtr.Zero);
      }

      //string errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
      //log.WriteToLog(LogMsgType.Error, $"Last error: {errorMessage}");

      log.WriteToLog(LogMsgType.Notification, $"done, iterating monitors (count: {tempList.Count})");
      foreach (Monitor monitor in tempList)
      {
        if (null != d)
        {
          d.Invoke(() => List.Add(monitor));
        }
        else
        {
          List.Add(monitor);
        }
      }
      tempList.Clear();
      tempList = null;
    }

    private bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref Rectangle lprcMonitor, IntPtr dwData)
    {
      Add(new HandleRef(this, hMonitor));
      return true;
    }

    public bool Add(HandleRef hMonitor)
    {
      bool retVal = false;
      uint monitorCount = 0;

      if (NativeMethods.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref monitorCount))
      {
        var monitorArray = new NativeStructures.PHYSICAL_MONITOR[monitorCount];
        NativeMethods.GetPhysicalMonitorsFromHMONITOR(hMonitor, monitorCount, monitorArray);

        NativeStructures.MonitorInfoEx mInfo = new NativeStructures.MonitorInfoEx();
        for (int i = 0; i < monitorArray.Length; i++)
        {
          NativeStructures.PHYSICAL_MONITOR physicalMonitor = monitorArray[i];
          NativeMethods.GetMonitorInfo(hMonitor, mInfo);
          Monitor newMonitor = new Monitor(physicalMonitor, hMonitor, mInfo, SkipCapabilityCheck, Progress);

          tempList.Add(newMonitor);
        }
        retVal = true;
      }
      return retVal;
    }
  }
}
