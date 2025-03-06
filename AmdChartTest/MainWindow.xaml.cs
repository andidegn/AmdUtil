using AMD.Util.View.WPF.UserControls;
using AmdChartTest.Properties;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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

    public PlotModel oxyPlotModel
    {
      get { return (PlotModel)GetValue(oxyPlotModelProperty); }
      set { SetValue(oxyPlotModelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for oxyPlotModel.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty oxyPlotModelProperty =
        DependencyProperty.Register("oxyPlotModel", typeof(PlotModel), typeof(MainWindow), new PropertyMetadata(null));





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
      string CultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      CultureInfo ci = new CultureInfo(CultureName);
      if (ci.NumberFormat.NumberDecimalSeparator != ".")
      {
        // Forcing use of decimal separator for numerical values
        ci.NumberFormat.NumberDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = ci;
      }
      BarChartStatisticsList = new ObservableCollection<ChartValue>();
      InitializeComponent();
      Title = $"{(4 == IntPtr.Size ? "32" : "64")} bit";
      oxyPlotModel = new PlotModel();
      oxyPlotModel.Series.Add(ls);
      plot.Model = oxyPlotModel;
    }

    private object colLock = new object();
    private Random r = new Random();
    LineSeries ls = new LineSeries()
    {
      Title = "Line Series",
      MarkerType = MarkerType.Circle,
      MarkerSize = 4,
      MarkerStroke = OxyColors.White
    };


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


    private void slStrokeThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (IsLoaded && sender is Slider sl && slDotSize.Value < sl.Value)
      {
        slDotSize.Value = slDotSize.Minimum;
      }
    }

    private int ctrSingleBar, ctrBar, ctrDot, ctrLine, ctrOxy = 0;
    Timer tBar, tDot, tLine, oxy;
    System.Threading.Thread thBar;
    bool threadStop = false;
    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.D1:
          {
            string xName = GenerateRandomString(5, 50);
            double yValue = 33 + r.NextDouble() * 33d;
            eChartDotShape shape = (eChartDotShape)(ctrSingleBar % 3);
            DotChartValue dtv = new DotChartValue(yValue)
            {
              XIndex = ctrSingleBar,
              XAxisValueName = ctrSingleBar.ToString(),
              ToolTip = $"{yValue} - {ctrDot} : {DateTime.Now}",
              //XAxisValueName = BarChartStatisticsList.Count % 10 == 0 ? $"{(ctr).ToString()} : {DateTime.Now}" : string.Empty,
              Width = 25,
              Height = 25,
              MinWidth = 50,
              DotShape = shape,
              Stroke = Brushes.Blue
            };
            BindingOperations.SetBinding(dtv, LineChartValue.WidthProperty, new Binding(nameof(Settings.Default.DotSize)) { Source = Settings.Default });
            BindingOperations.SetBinding(dtv, LineChartValue.HeightProperty, new Binding(nameof(Settings.Default.DotSize)) { Source = Settings.Default });
            BindingOperations.SetBinding(dtv, LineChartValue.StrokeThicknessProperty, new Binding(nameof(Settings.Default.StrokeThickness)) { Source = Settings.Default });
            BindingOperations.SetBinding(dtv, ChartValue.FillProperty, new Binding(nameof(Settings.Default.ChartBarBrush)) { Source = Settings.Default });
            //BindingOperations.SetBinding(dtv, LineChartValue.StrokeProperty, new Binding(nameof(Settings.Default.ChartStroke)) { Source = Settings.Default });
            BarChartStatisticsList.Add(dtv);
            ctrSingleBar++;
          }
          break;
        case Key.D2:
          if (tBar is null)
          {
            tBar = new Timer()
            {
              Interval = 0.1,
              AutoReset = true
            };
            tBar.Elapsed += (s1, e1) =>
            {
              Dispatcher.Invoke(() =>
              {
                double yValue = r.NextDouble() * 100d;
                if (ctrBar < 100)
                {
                  //BarChartValue bcv = new BarChartValue(r.NextDouble() * 100d)
                  BarChartValue bcv = new BarChartValue(yValue)
                  {
                    XIndex = ctrBar,
                    XAxisValueName = $"{ctrBar} : {DateTime.Now}",
                    ToolTip = $"{yValue} - {ctrBar} : {DateTime.Now}",
                    BarText = yValue.ToString(),
                    //XAxisValueName = BarChartStatisticsList.Count % 10 == 0 ? $"{(ctr).ToString()} : {DateTime.Now}" : string.Empty,
                    MinWidth = 50
                  };
                  bcv.Fill = new LinearGradientBrush()
                  {
                    GradientStops =
                  {
                    new GradientStop(Colors.Red, 0.0),
                    new GradientStop(Colors.Red, 0.0),
                    new GradientStop(Colors.Orange, 0.8),
                    new GradientStop(Colors.DarkRed, 1.0)
                  }
                  };
                  if (bcv.Y > 20)
                  {
                    bcv.Fill = new LinearGradientBrush()
                    {
                      GradientStops =
                  {
                    new GradientStop(Colors.Green, 0.0),
                    new GradientStop(Colors.Green, 0.0),
                    new GradientStop(Colors.Orange, 0.8),
                    new GradientStop(Colors.DarkGreen, 1.0)
                  }
                    };
                  }

                  //BindingOperations.SetBinding(bcv, BarChartValue.FillProperty, new Binding(nameof(Settings.Default.ChartBarBrush)) { Source = Settings.Default });

                  lock (colLock)
                  {
                    BarChartStatisticsList.Add(bcv);
                  }
                }
                else
                {
                  tBar.Stop();
                  tBar = null;
                  lock (colLock)
                  {
                    BarChartValue bcv = BarChartStatisticsList[ctrBar % 100] as BarChartValue;
                    bcv.Y = yValue;
                    bcv.BarText = yValue.ToString();
                  }
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
              Interval = 1,
              AutoReset = true
            };
            tDot.Elapsed += (s1, e1) =>
            {
              Dispatcher.Invoke(() =>
              {
                if (ctrDot == 1000)
                {
                  tDot.Stop();
                  tDot = null;
                }
                double yValue = 33 + r.NextDouble() * 33d;
                yValue = r.NextDouble() * 100;
                if (ctrDot == 250)
                {
                  yValue = 0;
                }
                //yValue = ctrDot % 100;
                //DotChartValue dtv = new DotChartValue(r.NextDouble() * 100d)
                eChartDotShape shape = (eChartDotShape)(ctrDot % 3);
                shape = eChartDotShape.Circle;
                DotChartValue dtv = new DotChartValue(yValue)
                {
                  XIndex = ctrDot,
                  DotShape = shape,
                  XAxisValueName = ctrDot.ToString(),
                  WithLine = true,
                  ToolTip = $"{yValue} - {ctrDot} : {DateTime.Now}",
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
                var t = chart.ItemsSource;
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
              Interval = 0.01,
              AutoReset = true
            };
            tLine.Elapsed += (s1, e1) =>
            {
              Dispatcher.Invoke(() =>
              {
                double yValue = 66 + r.NextDouble() * 33d;
                //LineChartValue dtv = new LineChartValue(r.NextDouble() * 100d)
                LineChartValue dtv = new LineChartValue(yValue)
                {
                  XIndex = ctrLine,
                  XAxisValueName = ctrLine.ToString(),
                  ToolTip = $"{yValue} - {ctrLine} : {DateTime.Now}",
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
        case Key.D5:
          var newChart = new List<ChartValue>();
          for (int i = 0; i < 5000; i++)
          {
            double yValue = r.NextDouble() * 100d;
            //LineChartValue dtv = new LineChartValue(r.NextDouble() * 100d)
            LineChartValue dtv = new LineChartValue(yValue)
            {
              XIndex = i,
              XAxisValueName = i.ToString(),
              ToolTip = $"{yValue} - {i} : {DateTime.Now}",
              //XAxisValueName = BarChartStatisticsList.Count % 10 == 0 ? $"{(ctr).ToString()} : {DateTime.Now}" : string.Empty,
              MinWidth = 50,
            };
            BindingOperations.SetBinding(dtv, LineChartValue.StrokeThicknessProperty, new Binding(nameof(Settings.Default.StrokeThickness)) { Source = Settings.Default });
            BindingOperations.SetBinding(dtv, DotChartValue.FillProperty, new Binding(nameof(Settings.Default.ChartBarBrush)) { Source = Settings.Default });
            BindingOperations.SetBinding(dtv, DotChartValue.StrokeProperty, new Binding(nameof(Settings.Default.ChartStroke)) { Source = Settings.Default });
            newChart.Add(dtv);
          }
          BarChartStatisticsList = new ObservableCollection<ChartValue>(newChart);

          break;

        case Key.D6:
          if (oxy is null)
          {
            oxy = new Timer()
            {
              Interval = 1,
              AutoReset = true
            };
            oxy.Elapsed += (s1, e1) =>
            {
              if (ctrOxy == 1000)
              {
                oxy.Stop();
                oxy = null;
              }
              double yValue = 33 + r.NextDouble() * 33d;

              ls.Points.Add(new DataPoint(ctrOxy, yValue));

              ctrOxy += 1;
              Dispatcher.Invoke(() => { plot.InvalidatePlot(); });
            };
            oxy.Start();
          }
          else
          {
            oxy.Stop();
            oxy = null;
          }
          break;

          case Key.D7:
          if (thBar is null)
          {
            thBar = new System.Threading.Thread(() =>
            {
              threadStop = false;
              while (!threadStop)
              {
                Dispatcher.Invoke(() =>
                {
                  double yValue = r.NextDouble() * 100d;
                  yValue = r.NextDouble() + ctrBar % 100;
                  if (ctrBar < 100)
                  {
                    //BarChartValue bcv = new BarChartValue(r.NextDouble() * 100d)
                    BarChartValue bcv = new BarChartValue(yValue)
                    {
                      XIndex = ctrBar,
                      XAxisValueName = $"{ctrBar} : {DateTime.Now}",
                      ToolTip = $"{yValue} - {ctrBar} : {DateTime.Now}",
                      BarText = yValue.ToString(),
                      //XAxisValueName = BarChartStatisticsList.Count % 10 == 0 ? $"{(ctr).ToString()} : {DateTime.Now}" : string.Empty,
                      MinWidth = 50
                    };
                    bcv.Fill = new LinearGradientBrush()
                    {
                      GradientStops =
                    {
                    new GradientStop(Colors.Red, 0.0),
                    new GradientStop(Colors.Red, 0.0),
                    new GradientStop(Colors.Orange, 0.8),
                    new GradientStop(Colors.DarkRed, 1.0)
                    }
                    };
                    if (bcv.Y > 20)
                    {
                      bcv.Fill = new LinearGradientBrush()
                      {
                        GradientStops =
                    {
                    new GradientStop(Colors.Green, 0.0),
                    new GradientStop(Colors.Green, 0.0),
                    new GradientStop(Colors.Orange, 0.8),
                    new GradientStop(Colors.DarkGreen, 1.0)
                    }
                      };
                    }

                    //BindingOperations.SetBinding(bcv, BarChartValue.FillProperty, new Binding(nameof(Settings.Default.ChartBarBrush)) { Source = Settings.Default });

                    lock (colLock)
                    {
                      BarChartStatisticsList.Add(bcv);
                    }
                  }
                  else
                  {
                    lock (colLock)
                    {
                      BarChartValue bcv = BarChartStatisticsList[ctrBar % 100] as BarChartValue;
                      bcv.Y = yValue;
                      bcv.BarText = yValue.ToString();
                    }
                  }
                  ctrBar += 1;
                });
              }
              thBar = null;
            });
            thBar.Start();
          }
          else
          {
            threadStop = true;
          }
          break;

        case Key.D8:
          if (thBar is null)
          {
            thBar = new System.Threading.Thread(() =>
            {
            //double yValue = r.Next(0, 30000);
            threadStop = false;
              int valueCount = 30;
              while (!threadStop)
              {
                double yValue = r.NextDouble() * 100;
                yValue = r.NextDouble() + ctrDot % valueCount;
                Dispatcher.Invoke(() =>
                {
                  if (ctrDot < valueCount)
                  {
                    eChartDotShape shape = (eChartDotShape)(ctrDot % 3);
                    shape = eChartDotShape.Circle;
                    DotChartValue dtv = new DotChartValue(yValue)
                    {
                      XIndex = ctrDot,
                      DotShape = shape,
                      XAxisValueName = ctrDot.ToString(),
                      WithLine = true,
                      ToolTip = $"{yValue} - {ctrDot} : {DateTime.Now}",
                      //XAxisValueName = BarChartStatisticsList.Count % 10 == 0 ? $"{(ctr).ToString()} : {DateTime.Now}" : string.Empty,
                      Width = 25,
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
                    var t = chart.ItemsSource;
                  }
                  else
                  {
                    lock (colLock)
                    {
                      DotChartValue bcv = BarChartStatisticsList[ctrDot % valueCount] as DotChartValue;
                      bcv.Y = yValue;
                    }
                    System.Threading.Thread.Sleep(100);
                  }
                  ctrDot += 1;
                });
              }
              thBar = null;
            });
            thBar.Start();
          }
          else
          {
            threadStop = true;
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
