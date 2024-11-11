using AMD.Util.View.WPF.UserControls;
using AmdChartTest.Properties;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AmdChartTest
{
  internal class BoolToScrollBarVisibility : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is bool b && b ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is ScrollBarVisibility sbv && ScrollBarVisibility.Visible == sbv;
    }
  }

  internal class FixedYAxisScaleConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      double? retVal = null;
      if (2 == values.Length && values[0] is bool enabled && enabled && values[1] is decimal value)
      {
        retVal = (double)value;
      }
      return retVal;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public ObservableCollection<ChartValue> BarChartStatisticsList
    {
      get { return (ObservableCollection<ChartValue>)GetValue(BarChartStatisticsListProperty); }
      set { SetValue(BarChartStatisticsListProperty, value); }
    }

    // Using a DependencyProperty as the backing store for BarChartStatisticsList.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BarChartStatisticsListProperty =
        DependencyProperty.Register("BarChartStatisticsList", typeof(ObservableCollection<ChartValue>), typeof(MainWindow), new PropertyMetadata(default(ObservableCollection<BarChartValue>)));

    public string XAxisName
    {
      get { return (string)GetValue(XAxisNameProperty); }
      set { SetValue(XAxisNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for XAxisName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty XAxisNameProperty =
        DependencyProperty.Register("XAxisName", typeof(string), typeof(MainWindow), new PropertyMetadata("Date"));

    public double LineWidth
    {
      get { return (double)GetValue(LineWidthProperty); }
      set { SetValue(LineWidthProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LineWidth.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LineWidthProperty =
        DependencyProperty.Register("LineWidth", typeof(double), typeof(MainWindow), new PropertyMetadata(11d));



    public MainWindow()
    {
      BarChartStatisticsList = new ObservableCollection<ChartValue>();
      InitializeComponent();
    }

    private object colLock = new object();
    private Random r = new Random();
    public string GenerateRandomString(int from, int to)
    {
      // Define the character set (alphanumeric)
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

      // Generate a random length between 5 and 50
      int length = r.Next(from, to + 1); // 51 because the upper limit is exclusive

      // Create a StringBuilder for efficiency
      StringBuilder result = new StringBuilder(length);

      // Generate the random string
      for (int i = 0; i < length; i++)
      {
        // Select a random character from the character set
        result.Append(chars[r.Next(chars.Length)]);
      }

      return result.ToString();
    }

    private int ctrSingleBar, ctrBar, ctrDot, ctrLine = 0;
    Timer tBar, tDot, tLine;
    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.D1:
          string xName = GenerateRandomString(5, 50);
          BarChartStatisticsList.Add(new BarChartValue(r.NextDouble() * 100d)
          {
            XIndex = ctrSingleBar++,
            XAxisValueName = xName,
          });
          break;
        case Key.D2:
          if (tBar is null)
          {
            tBar = new Timer()
            {
              Interval = 10,
              AutoReset = true
            };
            tBar.Elapsed += (s1, e1) =>
            {
              Dispatcher.Invoke(() =>
              {
                //BarChartValue bcv = new BarChartValue(r.NextDouble() * 100d)
                BarChartValue bcv = new BarChartValue(r.NextDouble() * 33d)
                {
                  XIndex = ctrBar,
                  XAxisValueName = $"{ctrBar} : {DateTime.Now}",
                  ToolTip = $"{ctrBar} : {DateTime.Now}",
                  //XAxisValueName = BarChartStatisticsList.Count % 10 == 0 ? $"{(ctr).ToString()} : {DateTime.Now}" : string.Empty,
                  MinWidth = 50
                };
                BindingOperations.SetBinding(bcv, BarChartValue.FillProperty, new Binding(nameof(Settings.Default.ChartBarBrush)) { Source = Settings.Default });

                lock (colLock)
                {
                  BarChartStatisticsList.Add(bcv);
                }
                ctrBar += 1;
              });
            };
            tBar.Start();
          }
          else
          {
            tBar.Stop();
            tBar = null;
          }
          break;
        case Key.D3:
          if (tDot is null)
          {
            tDot = new Timer()
            {
              Interval = 10,
              AutoReset = true
            };
            tDot.Elapsed += (s1, e1) =>
            {
              Dispatcher.Invoke(() =>
              {
                //DotChartValue dtv = new DotChartValue(r.NextDouble() * 100d)
                DotChartValue dtv = new DotChartValue(33 + r.NextDouble() * 33d)
                {
                  XIndex = ctrDot,
                  XAxisValueName = ctrDot.ToString(),
                  ToolTip = $"{ctrDot} : {DateTime.Now}",
                  //XAxisValueName = BarChartStatisticsList.Count % 10 == 0 ? $"{(ctr).ToString()} : {DateTime.Now}" : string.Empty,
                  Width  = 25,
                  Height = 25,
                  MinWidth = 50
                };
                BindingOperations.SetBinding(dtv, LineChartValue.WidthProperty, new Binding(nameof(Settings.Default.DotSize)) { Source = Settings.Default });
                BindingOperations.SetBinding(dtv, LineChartValue.HeightProperty, new Binding(nameof(Settings.Default.DotSize)) { Source = Settings.Default });
                BindingOperations.SetBinding(dtv, LineChartValue.StrokeThicknessProperty, new Binding(nameof(Settings.Default.StrokeThickness)) { Source = Settings.Default });
                BindingOperations.SetBinding(dtv, ChartValue.FillProperty, new Binding(nameof(Settings.Default.ChartBarBrush)) { Source = Settings.Default });
                BindingOperations.SetBinding(dtv, LineChartValue.StrokeProperty, new Binding(nameof(Settings.Default.ChartStroke)) { Source = Settings.Default });
                lock (colLock)
                {
                  BarChartStatisticsList.Add(dtv);
                }
                ctrDot += 1;
              });
            };
            tDot.Start();
          }
          else
          {
            tDot.Stop();
            tDot = null;
          }
          break;
        case Key.D4:
          if (tLine is null)
          {
            tLine = new Timer()
            {
              Interval = 10,
              AutoReset = true
            };
            tLine.Elapsed += (s1, e1) =>
            {
              Dispatcher.Invoke(() =>
              {
                //LineChartValue dtv = new LineChartValue(r.NextDouble() * 100d)
                LineChartValue dtv = new LineChartValue(66 + r.NextDouble() * 33d)
                {
                  XIndex = ctrLine,
                  XAxisValueName = ctrLine.ToString(),
                  ToolTip = $"{ctrLine} : {DateTime.Now}",
                  //XAxisValueName = BarChartStatisticsList.Count % 10 == 0 ? $"{(ctr).ToString()} : {DateTime.Now}" : string.Empty,
                  MinWidth = 50,
                };
                BindingOperations.SetBinding(dtv, LineChartValue.StrokeThicknessProperty, new Binding(nameof(Settings.Default.StrokeThickness)) { Source = Settings.Default });
                BindingOperations.SetBinding(dtv, DotChartValue.FillProperty, new Binding(nameof(Settings.Default.ChartBarBrush)) { Source = Settings.Default });
                BindingOperations.SetBinding(dtv, DotChartValue.StrokeProperty, new Binding(nameof(Settings.Default.ChartStroke)) { Source = Settings.Default });
                lock (colLock)
                {
                  BarChartStatisticsList.Add(dtv);
                }
                ctrLine += 1;
              });
            };
            tLine.Start();
          }
          else
          {
            tLine.Stop();
            tLine = null;
          }
          break;
        case Key.Delete:
          ctrSingleBar = ctrBar = ctrDot = ctrLine = 0;
          BarChartStatisticsList = new ObservableCollection<ChartValue>();
          break;
        default:
          break;
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Settings.Default.Save();
    }
  }
}
