using AMD.Util.Tasks;
using AMD.Util.View.WPF.InvokeControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfUITest
{
  public delegate void LocalProgressHandler(object sender, LocalTaskArgs args);
  public class LocalTaskArgs : EventArgs
  {
    public double Progress { get; set; }
    public int ProcessIdx { get; set; }
    public int Weight { get; set; }

    public LocalTaskArgs(double progress, int processIdx, int weight) 
    { 
      this.Progress = progress;
      this.ProcessIdx = processIdx; 
      this.Weight = weight;
    }
  }

  public class WorkTask
  {
    public bool UserCancel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event LocalProgressHandler ProgressChanged;

    private void OnProgressChanged(double progress) 
    {
      ProgressChanged?.Invoke(this, new LocalTaskArgs(progress, ProcessIdx, Weight));
    }

    public int Seconds { get; set; }
    public int ProcessIdx { get; set; }
    public int Weight { get; set; }

    private const int stepLengthInMs = 25;

    public WorkTask(int seconds, int pIdx, int weight)
    {
      Seconds = seconds;
      ProcessIdx = pIdx;
      Weight = weight;
    }

    public TaskResult Execute()
    {
      int totalMs = Seconds * 1000;
      int ms = 0;
      while (ms < totalMs)
      {
        Task.Delay(stepLengthInMs).Wait();
        ms += stepLengthInMs;
        OnProgressChanged(100 * ms / totalMs);
      }
      return TaskResult.Success;
    }
  }































  /// <summary>
  /// Interaction logic for ProgressBarsTest.xaml
  /// </summary>
  public partial class ProgressBarsTest : Window
  {


    public ObservableCollection<string> strings
    {
      get { return (ObservableCollection<string>)GetValue(stringsProperty); }
      set { SetValue(stringsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for strings.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty stringsProperty =
        DependencyProperty.Register("strings", typeof(ObservableCollection<string>), typeof(ProgressBarsTest), new PropertyMetadata(default(string)));


    Queue<WorkTask> tasks = new Queue<WorkTask>();


    private InvokeControl ic;

    private int totalProcesses;
    private int totalWeight;

    public ProgressBarsTest()
    {
      InitializeComponent();
      ic = new InvokeControl(this);
      strings = new ObservableCollection<string>();

      tasks = SetupWork();
    }

    private void UpdateProgress(double progress, int progressWeight, bool resetProcessProgress = false)
    {
      if (ic.InvokeRequired)
      {
        Dispatcher.Invoke(() => UpdateProgress(progress, progressWeight));
        return;
      }

      if (resetProcessProgress)
      {
        progressTotal.Value = 0;
      }
      if (progress > progressSingle.Value)
      {
        progressTotal.Value += progressWeight * (progress - progressSingle.Value) / 100;
      }

      progressSingle.Value = progress;
    }

    private Queue<WorkTask> SetupWork()
    {
      Queue<WorkTask> tasks = new Queue<WorkTask>();
      Random r = new Random();
      int totalWorkTasks = 10;

      strings.Add($"Total Tasks: {totalWorkTasks}");

      for (int i = 0; i < totalWorkTasks; i++)
      {
        int weight = 2 + r.Next(4);
        totalWeight += weight;

        WorkTask wt = new WorkTask(weight, i, weight);
        tasks.Enqueue(wt);

        strings.Add($"Task added: {weight}");
      }

      strings.Add($"Total weight: {totalWeight}");
      progressTotal.Maximum = totalWeight;
      return tasks;
    }

    private async void StartWork(Queue<WorkTask> tasks)
    {
      await Task.Factory.StartNew(() =>
      {
        UpdateProgress(0, 0, true);
        do
        {
          WorkTask t = tasks.Dequeue();
          t.ProgressChanged += T_ProgressChanged;

          t.Execute();

        } while (0 < tasks.Count());
      });
    }

    private void T_ProgressChanged(object sender, LocalTaskArgs args)
    {
      UpdateProgress(args.Progress, args.Weight);
    }

    private async void btnStartWork_Click(object sender, RoutedEventArgs e)
    {
      StartWork(tasks);
    }

    private void btnSetupWork_Click(object sender, RoutedEventArgs e)
    {
      tasks = SetupWork();
    }
  }
}
