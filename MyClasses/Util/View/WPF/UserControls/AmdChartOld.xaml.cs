using AMD.Util.HID;
using AMD.Util.LiniarAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AMD.Util.View.WPF.UserControls.old
{
  public enum eChartDotShape
  {
    Circle,
    Square,
    Diamond
  }

  internal class TopCanvasValueScaleConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      double retVal = 0;
      if (values[0] is double yAxisCurrentMax && parameter is ValueTuple<ChartValue, Grid> vt)
      {
        double totalHeight = vt.Item2.RowDefinitions[0].ActualHeight;
        retVal = totalHeight - totalHeight * InnerHelper.CalculateFactor(vt.Item1.Y, yAxisCurrentMax is double d ? d : 1d);
      }
      return retVal;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class TopValueScaleConverter : IValueConverter
  {
    internal ChartValue Value { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double dd = 100 - 100 * InnerHelper.CalculateFactor(Value.Y, value is double d ? d : 1d);
      return new GridLength(dd, GridUnitType.Star);
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class BottomValueScaleConverter : IValueConverter
  {
    internal ChartValue Value { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double dd = 100 * InnerHelper.CalculateFactor(Value.Y, value is double d ? d : 1d);
      return new GridLength(dd, GridUnitType.Star);
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class ScrollViewerVisibilityToMarginConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Thickness retVal = new Thickness(0, 0, 0, 0);
      if (value is Visibility v)
      {
        switch (v)
        {
          case Visibility.Visible:
            retVal.Bottom = -10;
            break;
          case Visibility.Hidden:
          case Visibility.Collapsed:
          default:
            break;
        }
      }
      return retVal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class TrendLineYConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      double retVal = double.Epsilon;
      if (4 == values.Length &&
          values[0] is double YAxisCurrentMax &&
          values[1] is Trendline TrendlineCalc &&
          values[2] is double ActualHeight &&
          values[3] is ObservableCollection<ChartValue> ItemsSource &&
          parameter is string yPoint)
      {
        // Y2 = canvasLineData.ActualHeight - canvasLineData.ActualHeight * TrendlineCalc.GetYValue(ItemsSource.Count()) / YAxisCurrentMax,
        retVal = ActualHeight - ActualHeight * TrendlineCalc.GetYValue(yPoint.Equals("Y1") ? 0 : ItemsSource.Count()) / YAxisCurrentMax;
        string s = $"y = {Math.Round(TrendlineCalc.Slope, 4)}x + {Math.Round(TrendlineCalc.Intercept, 2)}";
        if (0 == retVal || double.IsNaN(retVal) || double.IsInfinity(retVal))
        {
          retVal = double.Epsilon;
        }
      }
      return retVal;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class TrendLineYToolTipConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string retVal = "N/A";
      if (value is Trendline TrendlineCalc)
      {
        retVal = $"y = {Math.Round(TrendlineCalc.Slope, 4)}x + {Math.Round(TrendlineCalc.Intercept, 2)}";
      }
      return retVal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class LineXConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double retVal = 0;
      if (parameter is ValueTuple<LineChartValue, Panel> vt)
      {
        Point dotLocation = vt.Item1.Dot.TranslatePoint(new Point(), vt.Item2);
        retVal = dotLocation.X + vt.Item1.Width / (vt.Item1.Dot is Polygon ? 4 : 2);
      }
      return retVal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class LineYConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double retVal = 0;
      if (parameter is ValueTuple<LineChartValue, Panel> vt)
      {
        var prevDotLocation = vt.Item1.Dot.TranslatePoint(new Point(), vt.Item2);
        retVal = prevDotLocation.Y + vt.Item1.Height / 2;
      }
      return retVal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class DashLineWidthConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new Thickness(0, 0, value is double d ? -d : 50, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class TextMarginConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double valueWidth && parameter is TextBlock tb)
      {
        Size charSize = InnerHelper.MeasureString(tb);
        return new Thickness(-((charSize.Width - valueWidth) / 2), 0, 0, 0);
      }
      throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class CanvasXAxisTextHeightConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double retVal = 0;
      if (value is double d && parameter is TextBlock tb)
      {
        Size charSize = InnerHelper.MeasureString(tb);
        return charSize.Height * 0.99;
      }
      return retVal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  internal class EllipseMarginConverter : IValueConverter
  {
    public Panel Parent { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double height)
      {
        return new Thickness(-(height) / 2, -(height / 2), 0, 0);
      }
      return new Thickness(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Interaction logic for Plot.xaml
  /// </summary>
  public partial class AmdChart : UserControl
  {
    #region DependencyProperties
    #region Options
    /// <summary>
    /// Note: No not use Auto as the dynamic height is not handled and will yield an incorrect height of the data
    /// </summary>
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
      get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
      set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
        DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(AmdChart), new PropertyMetadata(ScrollBarVisibility.Disabled));

    public bool ShowTrendline
    {
      get { return (bool)GetValue(ShowTrendlineProperty); }
      set { SetValue(ShowTrendlineProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ShowTrendline.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShowTrendlineProperty =
        DependencyProperty.Register("ShowTrendline", typeof(bool), typeof(AmdChart), new PropertyMetadata(false));
    #endregion // Options

    #region Names
    public string XAxisName
    {
      get { return (string)GetValue(XAxisNameProperty); }
      set { SetValue(XAxisNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for XAxisName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty XAxisNameProperty =
        DependencyProperty.Register("XAxisName", typeof(string), typeof(AmdChart), new PropertyMetadata(default(string)));

    public string YAxisName
    {
      get { return (string)GetValue(YAxisNameProperty); }
      set { SetValue(YAxisNameProperty, value); }
    }

    // Using a DependencyProperty as the backing store for YAxisName.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty YAxisNameProperty =
        DependencyProperty.Register("YAxisName", typeof(string), typeof(AmdChart), new PropertyMetadata(default(string)));

    public string YAxisPrefix
    {
      get { return (string)GetValue(YAxisPrefixProperty); }
      set { SetValue(YAxisPrefixProperty, value); }
    }

    // Using a DependencyProperty as the backing store for YAxisPrefix.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty YAxisPrefixProperty =
        DependencyProperty.Register("YAxisPrefix", typeof(string), typeof(AmdChart), new PropertyMetadata(default(string)));

    public string YAxisPostfix
    {
      get { return (string)GetValue(YAxisPostfixProperty); }
      set { SetValue(YAxisPostfixProperty, value); }
    }

    // Using a DependencyProperty as the backing store for YAxisPostfix.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty YAxisPostfixProperty =
        DependencyProperty.Register("YAxisPostfix", typeof(string), typeof(AmdChart), new PropertyMetadata(default(string)));
    #endregion // Names


    #region Adjustments
    #region Brushes
    public Brush ValueFill
    {
      get { return (Brush)GetValue(ValueFillProperty); }
      set { SetValue(ValueFillProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ValueFill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValueFillProperty =
        DependencyProperty.Register("ValueFill", typeof(Brush), typeof(AmdChart), new PropertyMetadata(Brushes.Red));

    public Brush ValueBorderBrush
    {
      get { return (Brush)GetValue(ValueBorderBrushProperty); }
      set { SetValue(ValueBorderBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ValueBorderBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValueBorderBrushProperty =
        DependencyProperty.Register("ValueBorderBrush", typeof(Brush), typeof(AmdChart), new PropertyMetadata(Brushes.Blue));
    #endregion // Brushes

    #region Text Adjustments
    public FontFamily HeaderFontFamily
    {
      get { return (FontFamily)GetValue(HeaderFontFamilyProperty); }
      set { SetValue(HeaderFontFamilyProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HeaderFontFamily.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeaderFontFamilyProperty =
        DependencyProperty.Register("HeaderFontFamily", typeof(FontFamily), typeof(AmdChart), new PropertyMetadata(new FontFamily("Helvetica")));


    public double HeaderFontSize
    {
      get { return (double)GetValue(HeaderFontSizeProperty); }
      set { SetValue(HeaderFontSizeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HeaderFontSize.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeaderFontSizeProperty =
        DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(AmdChart), new PropertyMetadata(16d));


    public FontWeight HeaderFontWeight
    {
      get { return (FontWeight)GetValue(HeaderFontWeightProperty); }
      set { SetValue(HeaderFontWeightProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HeaderFontWeight.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeaderFontWeightProperty =
        DependencyProperty.Register("HeaderFontWeight", typeof(FontWeight), typeof(AmdChart), new PropertyMetadata(FontWeights.Bold));
    #endregion // Text Adjustments

    #region Adjustment Values
    public double GridOpacity
    {
      get { return (double)GetValue(GridOpacityProperty); }
      set { SetValue(GridOpacityProperty, value); }
    }

    // Using a DependencyProperty as the backing store for GridOpacity.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty GridOpacityProperty =
        DependencyProperty.Register("GridOpacity", typeof(double), typeof(AmdChart), new PropertyMetadata(0.2));

    private double ValueWidth
    {
      get { return (double)GetValue(ValueWidthProperty); }
      set { SetValue(ValueWidthProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ValueWidth.  This enables animation, styling, binding, etc...
    private static readonly DependencyProperty ValueWidthProperty =
        DependencyProperty.Register("ValueWidth", typeof(double), typeof(AmdChart), new PropertyMetadata(0d));

    public Thickness ValueBorderThickness
    {
      get { return (Thickness)GetValue(ValueBorderThicknessProperty); }
      set { SetValue(ValueBorderThicknessProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ValueBorderThickness.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValueBorderThicknessProperty =
        DependencyProperty.Register("ValueBorderThickness", typeof(Thickness), typeof(AmdChart), new PropertyMetadata(new Thickness(0)));

    public CornerRadius ValueCornerRadius
    {
      get { return (CornerRadius)GetValue(ValueCornerRadiusProperty); }
      set { SetValue(ValueCornerRadiusProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ValueCornerRadius.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValueCornerRadiusProperty =
        DependencyProperty.Register("ValueCornerRadius", typeof(CornerRadius), typeof(AmdChart), new PropertyMetadata(new CornerRadius(0)));




    #endregion // Adjustment Values
    #endregion // Adjustments

    #region Options
    public Trendline TrendlineCalc
    {
      get { return (Trendline)GetValue(TrendlineCalcProperty); }
      set { SetValue(TrendlineCalcProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TrendlineCalc.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TrendlineCalcProperty =
        DependencyProperty.Register("TrendlineCalc", typeof(Trendline), typeof(AmdChart), new PropertyMetadata(default(Trendline)));

    public bool ShowHorizontalGridLines
    {
      get { return (bool)GetValue(ShowHorizontalGridLinesProperty); }
      set { SetValue(ShowHorizontalGridLinesProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ShowHorizontalGridLines.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShowHorizontalGridLinesProperty =
        DependencyProperty.Register("ShowHorizontalGridLines", typeof(bool), typeof(AmdChart), new PropertyMetadata(true));


    public bool ShowVerticalGridLines
    {
      get { return (bool)GetValue(ShowVerticalGridLinesProperty); }
      set { SetValue(ShowVerticalGridLinesProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ShowVerticalGridLines.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShowVerticalGridLinesProperty =
        DependencyProperty.Register("ShowVerticalGridLines", typeof(bool), typeof(AmdChart), new PropertyMetadata(true));


    public bool AutoIncrementXIndex
    {
      get { return (bool)GetValue(AutoIncrementXIndexProperty); }
      set { SetValue(AutoIncrementXIndexProperty, value); }
    }

    // Using a DependencyProperty as the backing store for AutoIncrementXIndex.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty AutoIncrementXIndexProperty =
        DependencyProperty.Register("AutoIncrementXIndex", typeof(bool), typeof(AmdChart), new PropertyMetadata(true));

    /// <summary>
    /// The ammount of X Axis names to skip between measurements
    /// </summary>
    public int XAxisNameTextSkip
    {
      get { return (int)GetValue(XAxisNameTextSkipProperty); }
      set { SetValue(XAxisNameTextSkipProperty, value); }
    }

    // Using a DependencyProperty as the backing store for XAxisTextSkip.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty XAxisNameTextSkipProperty =
        DependencyProperty.Register("XAxisNameTextSkip", typeof(int), typeof(AmdChart), new PropertyMetadata(new PropertyChangedCallback(OnXAxisTextSkipChanged)));
    #endregion // Options

    #region Data
    public double? YAxisMax
    {
      get { return (double?)GetValue(YAxisMaxProperty); }
      set { SetValue(YAxisMaxProperty, value); }
    }

    // Using a DependencyProperty as the backing store for YAxisMax.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty YAxisMaxProperty =
        DependencyProperty.Register("YAxisMax", typeof(double?), typeof(AmdChart), new PropertyMetadata(new PropertyChangedCallback(OnYAxisMaxOrIntervalPropertyChanged)));

    public double YAxisInterval
    {
      get { return (double)GetValue(YAxisIntervalProperty); }
      set { SetValue(YAxisIntervalProperty, value); }
    }

    // Using a DependencyProperty as the backing store for YAxisInterval.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty YAxisIntervalProperty =
        DependencyProperty.Register("YAxisInterval", typeof(double), typeof(AmdChart), new PropertyMetadata(new PropertyChangedCallback(OnYAxisMaxOrIntervalPropertyChanged)));

    #region ItemsSource
    public ObservableCollection<ChartValue> ItemsSource
    {
      get { return (ObservableCollection<ChartValue>)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<ChartValue>), typeof(AmdChart), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));
    #endregion // ItemsSource
    #endregion // Data

    #region ReadOnly
    public double YAxisCurrentMax
    {
      get { return (double)GetValue(YAxisCurrentMaxProperty); }
      private set { SetValue(YAxisCurrentMaxProperty, value); }
    }

    // Using a DependencyProperty as the backing store for YAxisCurrentMax.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty YAxisCurrentMaxProperty =
        DependencyProperty.Register("YAxisCurrentMax", typeof(double), typeof(AmdChart), new PropertyMetadata(0d));
    #endregion // ReadOnly
    #endregion // DependencyProperties

    #region DependencyProperty Changed Handlers
    private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is AmdChart p)
      {
        if (p.IsLoaded)
        {
          p.DrawDiagram(true, p.ItemsSource);
        }
        p.ItemsSource.CollectionChanged += p.ItemsSource_CollectionChanged;
      }
    }

    private static void OnYAxisMaxOrIntervalPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is AmdChart p && p.IsLoaded)
      {
        p.DrawYAxisValues(p.ItemsSource, true);
      }
    }

    private static void OnXAxisTextSkipChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is AmdChart p && p.IsLoaded && e.NewValue is int numberOfSkips)
      {
        foreach (ChartValue cv in p.ItemsSource)
        {
          p.SetXAxisValueNameSkips(cv);
        }
      }
    }
    #endregion // DependencyProperty Changed Handlers







    public string TestString
    {
      get { return (string)GetValue(TestStringProperty); }
      set { SetValue(TestStringProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TestString.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TestStringProperty =
        DependencyProperty.Register("TestString", typeof(string), typeof(AmdChart), new PropertyMetadata(""));







    private bool isFirstDraw;
    private LineChartValue previousLCV;
    private DotChartValue previousDCV;
    private int localXIndexCtr;

    public AmdChart()
    {
      previousLCV = null;
      previousDCV = null;
      InitializeComponent();
    }
    Point p1;
    private void ScrollAndCenterOnMouse()
    {
      double extentWidthFactor = svValues.ExtentWidth / sv1;
      Point mouseOnScrollViewerPosition = Mouse.GetPosition(svValues);
      double offset = p1.X * extentWidthFactor;

      if (svValues.HorizontalOffset == svValues.ScrollableWidth && svValues.ExtentWidth < sv1)
      {
        svValues.ScrollToHorizontalOffset(offset + mouseOnScrollViewerPosition.X);
      }
      else
      {
        svValues.ScrollToHorizontalOffset(offset - mouseOnScrollViewerPosition.X);
      }
    }

    private void DrawDiagram(bool redraw, IList values)
    {
      if (redraw)
      {
        Clear();

        ValueWidth = values is null || 0 == values.Count ? 50 : (values[0] as ChartValue)?.MinWidth ?? 50;
      }
      if (0 < values?.Count)
      {
        CalculateTrendline();
        DrawYAxisValues(values, redraw || isFirstDraw);
        DrawValues(values);
        isFirstDraw = false;
      }
    }

    private void UpdatePlotLayout()
    {
      foreach (var child in canvasLineData.Children)
      {
        if (child is Line l)
        {
          l.GetBindingExpression(Line.X1Property).UpdateTarget();
          l.GetBindingExpression(Line.Y1Property).UpdateTarget();
          l.GetBindingExpression(Line.X2Property).UpdateTarget();
          l.GetBindingExpression(Line.Y2Property).UpdateTarget();
        }
      }
    }

    private void DrawYAxisValues(IList values, bool clear)
    {
      if (null != values)
      {
        if (clear)
        {
          YAxisCurrentMax = 0d;
          gridYAxisValues.Children.Clear();
          gridYAxisValues.RowDefinitions.Clear();
        }
        if (YAxisMax is null)
        {
          foreach (ChartValue v in values)
          {
            while (YAxisCurrentMax < v.Y)
            {
              AddYAxisValue(false);
            }
          }
        }
        else
        {
          while (YAxisCurrentMax < YAxisMax)
          {
            AddYAxisValue(false);
          }
        }
      }
    }

    private void Clear()
    {
      previousLCV = null;
      previousDCV = null;
      isFirstDraw = true;
      gridValues.Children.Clear();
      gridValues.ColumnDefinitions.Clear();
      canvasLineData.Children.Clear();
      lastGridValuesChildCount = 0;
      localXIndexCtr = 0;
    }

    #region Value
    private void DrawValues(IList values)
    {
      foreach (ChartValue v in values)
      {
        if (AutoIncrementXIndex)
        {
          v.XIndex = localXIndexCtr++;
        }
        if (gridValues.ColumnDefinitions.Count <= v.XIndex)
        {
          AddXAxisValueName(v);
        }
        switch (v)
        {
          case BarChartValue bcv:
            AddBarValue(bcv);
            break;
          case DotChartValue dcv:
            AddDotValue(dcv, 0 == v.XIndex ? null : previousDCV);
            previousDCV = dcv;
            break;
          case LineChartValue line:
            AddLineValue(line, 0 == v.XIndex ? null : previousLCV);
            previousLCV = line;
            break;
          default:
            break;
        }
      }
    }

    private void AddLineValue(LineChartValue current, LineChartValue previous)
    {
      AddEllipse(current, eChartDotShape.Circle, gridValues, true);

      AddLine(previous, current, gridValues);
    }

    private void AddDotValue(DotChartValue current, DotChartValue previous)
    {
      AddEllipse(current, current.DotShape, gridValues, false);

      if (current.WithLine)
      {
        AddLine(previous, current, gridValues);
      }
    }

    private void AddBarValue(BarChartValue v)
    {
      AddBar(v);
    }

    #region Shapes
    private void AddBar(BarChartValue v)
    {
      Grid gBar = new Grid()
      {
        Name = $"BarGrid{v.XIndex}",
        Background = Brushes.Transparent,
      };

      gBar.MouseDown += (s, e) =>
      {
        if (e.ClickCount == 2)
        {
          v.OnDoubleClick();
        }
      };
      Grid.SetRow(gBar, 0);
      Grid.SetColumn(gBar, v.XIndex);

      gridValues.Children.Add(gBar);
      if (!v.IsSeparator)
      {
        AddColumnAndRowDefinitions(v, gBar, 25);

        Border bValue = new Border()
        {
          Name = $"ValueBar{v.XIndex}",
          ToolTip = v.ToolTip,
          HorizontalAlignment = HorizontalAlignment.Stretch,
          VerticalAlignment = VerticalAlignment.Stretch,
        };

        bValue.SetBinding(Border.BorderBrushProperty, new Binding(nameof(ValueBorderBrush)) { Source = this });
        bValue.SetBinding(Border.BorderThicknessProperty, new Binding(nameof(ValueBorderThickness)) { Source = this });
        bValue.SetBinding(Border.CornerRadiusProperty, new Binding(nameof(ValueCornerRadius)) { Source = this });
        if (v.Fill is null)
        {
          bValue.SetBinding(Panel.BackgroundProperty, new Binding(nameof(ValueFill)) { Source = this });
        }
        else
        {
          bValue.SetBinding(Panel.BackgroundProperty, new Binding(nameof(BarChartValue.Fill)) { Source = v });
        }
        Grid.SetColumn(bValue, 1);
        Grid.SetRow(bValue, 1);
        gBar.Children.Add(bValue);

        if (!string.IsNullOrWhiteSpace(v.BarText))
        {
          TextBlock tbBarText = new TextBlock
          {
            FontWeight = FontWeights.Regular,
            IsHitTestVisible = false,
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 0, 0, 10),
            LayoutTransform = new RotateTransform(270)
            {
              CenterX = 0.5,
              CenterY = 0.5,
            }
          };
          tbBarText.SetBinding(TextBlock.TextProperty, new Binding(nameof(BarChartValue.BarText)) { Source = v });
          Grid.SetColumn(tbBarText, 1);
          Grid.SetRow(tbBarText, 0);
          Grid.SetRowSpan(tbBarText, 2);
          gBar.Children.Add(tbBarText);
        }
      }
    }

    private void AddEllipse(LineChartValue current, eChartDotShape dotShape, Grid parent, bool asLine)
    {
      Grid g = new Grid();
      g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
      g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Pixel) });
      g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
      Canvas cColumn = new Canvas()
      {
        Name = $"EllipseCanvas{current.XIndex}",
        Background = Brushes.Transparent
      };

      cColumn.MouseDown += (s, e) =>
      {
        if (e.ClickCount == 2)
        {
          current.OnDoubleClick();
        }
      };
      Grid.SetColumn(cColumn, 1);
      g.Children.Add(cColumn);

      Grid.SetRow(g, 0);
      Grid.SetColumn(g, current.XIndex);

      switch (dotShape)
      {
        case eChartDotShape.Circle:
          current.Dot = new Ellipse();
          break;
        case eChartDotShape.Square:
          current.Dot = new Rectangle();
          break;
        case eChartDotShape.Diamond:
          current.Dot = new Polygon()
          {
            Stretch = Stretch.Uniform
          };
          (current.Dot as Polygon).Points = new PointCollection
          {
            new Point( 0, -2),
            new Point( 1,  0),
            new Point( 0,  2),
            new Point(-1,  0),
            new Point( 0, -2)
          };

          break;
        default:
          break;
      }

      current.Dot.Name = $"Dot{current.XIndex}";
      current.Dot.Fill = Brushes.Orange;
      current.Dot.IsHitTestVisible = true;
      current.Dot.ToolTip = current.ToolTip;
      current.Dot.HorizontalAlignment = HorizontalAlignment.Center;
      current.Dot.VerticalAlignment = VerticalAlignment.Top;

      Panel.SetZIndex(current.Dot, 102);
      SetEllipseBindings(current, parent, cColumn, asLine);

      cColumn.Children.Add(current.Dot);
      parent.Children.Add(g);
    }

    private void AddColumnAndRowDefinitions(ChartValue cv, Grid gBar, double centerColumnWidth, GridUnitType gut = GridUnitType.Star)
    {
      gBar.ColumnDefinitions.Add(new ColumnDefinition());
      gBar.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(centerColumnWidth, gut) });
      gBar.ColumnDefinitions.Add(new ColumnDefinition());
      RowDefinition rdTopValue = new RowDefinition();
      RowDefinition rdBottomValue = new RowDefinition();
      gBar.RowDefinitions.Add(rdTopValue);
      gBar.RowDefinitions.Add(rdBottomValue);
      rdTopValue.SetBinding(RowDefinition.HeightProperty, new Binding(nameof(YAxisCurrentMax))
      {
        Source = this,
        Converter = new TopValueScaleConverter() { Value = cv }
      });
      rdBottomValue.SetBinding(RowDefinition.HeightProperty, new Binding(nameof(YAxisCurrentMax))
      {
        Source = this,
        Converter = new BottomValueScaleConverter() { Value = cv }
      });
    }

    private void AddLine(LineChartValue previous, LineChartValue current, Panel parent)
    {
      if (null != previous && null != current && null != parent)
      {
        Line l = new Line()
        {
          Name = $"Line{previous?.XIndex}_{current?.XIndex}",
          IsHitTestVisible = true,
          ToolTip = previous?.ToolTip
        };
        Panel.SetZIndex(l, 102);
        SetLineBindings(l, previous, current, parent);
        canvasLineData.Children.Add(l);
      }
    }
    #endregion // Shapes

    #region Bindings
    private void SetEllipseBindings(LineChartValue current, Grid grandParent, Canvas parent, bool asLine)
    {
      current.Dot.SetBinding(Shape.StrokeProperty, new Binding(nameof(LineChartValue.Stroke)) { Source = current });
      MultiBinding canvasTopBinding = new MultiBinding()
      {
        Converter = new TopCanvasValueScaleConverter(),
        ConverterParameter = (current as ChartValue, grandParent)
      };
      canvasTopBinding.Bindings.Add(new Binding(nameof(YAxisCurrentMax)) { Source = this });
      canvasTopBinding.Bindings.Add(new Binding(nameof(ActualHeight)) { Source = this });

      current.Dot.SetBinding(Canvas.TopProperty, canvasTopBinding);

      if (asLine)
      {
        current.Dot.SetBinding(Shape.FillProperty, new Binding(nameof(LineChartValue.Stroke)) { Source = current });
        current.Dot.SetBinding(Shape.MarginProperty, new Binding(nameof(LineChartValue.StrokeThickness)) { Source = current, Converter = new EllipseMarginConverter() { Parent = grandParent } });
        current.Dot.SetBinding(Shape.WidthProperty, new Binding(nameof(LineChartValue.StrokeThickness)) { Source = current });
        current.Dot.SetBinding(Shape.HeightProperty, new Binding(nameof(LineChartValue.StrokeThickness)) { Source = current });
        BindingOperations.SetBinding(current, LineChartValue.WidthProperty, new Binding(nameof(LineChartValue.StrokeThickness)) { Source = current });
        BindingOperations.SetBinding(current, LineChartValue.HeightProperty, new Binding(nameof(LineChartValue.StrokeThickness)) { Source = current });
      }
      else
      {
        if (current.Fill is null)
        {
          current.Dot.SetBinding(Shape.FillProperty, new Binding(nameof(ValueFill)) { Source = this });
        }
        else
        {
          current.Dot.SetBinding(Shape.FillProperty, new Binding(nameof(LineChartValue.Fill)) { Source = current });
        }
        current.Dot.SetBinding(Shape.MarginProperty, new Binding(nameof(LineChartValue.Height)) { Source = current, Converter = new EllipseMarginConverter() { Parent = parent } });
        current.Dot.SetBinding(Shape.WidthProperty, new Binding(nameof(LineChartValue.Width)) { Source = current });
        current.Dot.SetBinding(Shape.HeightProperty, new Binding(nameof(LineChartValue.Height)) { Source = current });
      }
    }

    private void SetLineBindings(Line l, LineChartValue previous, LineChartValue current, Panel parent)
    {
      if (l is null)
      {
        return;
      }
      l.SetBinding(Line.StrokeProperty, new Binding(nameof(LineChartValue.Stroke)) { Source = previous });
      l.SetBinding(Line.X1Property, new Binding(nameof(ActualWidth))
      {
        Source = parent,
        Converter = new LineXConverter(),
        ConverterParameter = (previous, parent)
      });
      l.SetBinding(Line.X2Property, new Binding(nameof(ActualWidth))
      {
        Source = parent,
        Converter = new LineXConverter(),
        ConverterParameter = (current, parent)
      });
      l.SetBinding(Line.Y1Property, new Binding(nameof(ActualHeight))
      {
        Source = parent,
        Converter = new LineYConverter(),
        ConverterParameter = (previous, parent)
      });
      l.SetBinding(Line.Y2Property, new Binding(nameof(ActualHeight))
      {
        Source = parent,
        Converter = new LineYConverter(),
        ConverterParameter = (current, parent)
      });
      l.SetBinding(Line.StrokeThicknessProperty, new Binding(nameof(LineChartValue.StrokeThickness)) { Source = current });
    }

    private double? lastYAxisInterval = null;
    private double? lastYAxisMax = null;
    private int lastGridValuesChildCount = 0;
    private double lastSvHorizontalOffset = 0;
    private double lastSvVerticalOffset = 0;
    private void gridValues_LayoutUpdated(object sender, EventArgs e)
    {
      if (svValues.HorizontalOffset != lastSvHorizontalOffset ||
          svValues.VerticalOffset != lastSvVerticalOffset ||
          gridValues.Children.Count > lastGridValuesChildCount ||
          lastYAxisMax != YAxisMax ||
          lastYAxisInterval != YAxisInterval)
      {
        UpdatePlotLayout();
        lastYAxisInterval = YAxisInterval;
        lastYAxisMax = YAxisMax;
        lastGridValuesChildCount = gridValues.Children.Count;
        lastSvHorizontalOffset = svValues.HorizontalOffset;
        lastSvVerticalOffset = svValues.VerticalOffset;
      }
    }
    #endregion // Bindings
    #endregion // Value

    #region Axis
    private ColumnDefinition lastCdBars;
    private void AddXAxisValueName(ChartValue v)
    {
      int xIndex = v.XIndex;
      Canvas cv = new Canvas()
      {
        Name = $"cv{xIndex}",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = v.MinWidth
      };
      SetXAxisValueNameSkips(v);


      v.TBXAxisValueName = new TextBlock()
      {
        Text = v.XAxisValueName ?? (v is BarChartValue bcv && bcv.IsSeparator ? string.Empty : xIndex.ToString()),
        //FontFamily = new FontFamily("Consolas"),
        Tag = xIndex,
      };
      Panel.SetZIndex(v.TBXAxisValueName, 1000);
      //v.TBXAxisValueName.LayoutTransform = new RotateTransform(-70, 150, -50);
      SetXAxisBindings(v, cv);

      cv.Children.Add(v.TBXAxisValueName);
      Grid.SetRow(cv, 1);
      Grid.SetColumn(cv, xIndex);

      if (gridValues.ColumnDefinitions.Count <= xIndex)
      {
        ColumnDefinition cd = new ColumnDefinition();
        gridValues.ColumnDefinitions.Add(cd);
        if (null != lastCdBars)
        {
          cd.SetBinding(ColumnDefinition.WidthProperty, new Binding(nameof(ColumnDefinition.Width)) { Source = lastCdBars, Mode = BindingMode.TwoWay });
        }
      }

      if (!string.IsNullOrWhiteSpace(v.XAxisValueName))
      {
        Binding bNLStroke = new Binding(nameof(Foreground))
        {
          Source = this
        };
        Line dashLine = new Line()
        {
          X1 = 0,
          X2 = 0,
          Y1 = 0,
          Margin = new Thickness(0),
          Stroke = new SolidColorBrush(Color.FromArgb(0x80, 0x80, 0x80, 0x80)),
          StrokeThickness = 0.4,
          StrokeDashArray = new DoubleCollection(new double[] { 10, 10 }),
          HorizontalAlignment = HorizontalAlignment.Center
        };
        dashLine.SetBinding(Line.OpacityProperty, new Binding(nameof(GridOpacity))
        {
          Source = this
        });
        dashLine.SetBinding(Line.StrokeProperty, bNLStroke);
        Binding y2b = new Binding(nameof(ActualHeight))
        {
          ElementName = nameof(canvasXYAxis)
        };
        dashLine.SetBinding(Line.Y2Property, y2b);
        Binding hgl = new Binding(nameof(ShowVerticalGridLines))
        {
          Source = this,
          Converter = new BooleanToVisibilityConverter()
        };
        dashLine.SetBinding(Line.VisibilityProperty, hgl);
        Grid.SetRow(dashLine, 0);
        Grid.SetColumn(dashLine, xIndex);

        gridValues.Children.Add(dashLine);
      }
      gridValues.Children.Add(cv);
    }

    private void SetXAxisBindings(ChartValue v, Canvas cv)
    {
      v.TBXAxisValueName.SetBinding(TextBlock.TextProperty, new Binding(nameof(ChartValue.XAxisValueName)) { Source = v });
      v.TBXAxisValueName.SetBinding(TextBlock.VisibilityProperty, new Binding(nameof(ChartValue.XAxisValueNameVisibility)) { Source = v });
      v.TBXAxisValueName.SetBinding(TextBlock.MarginProperty, new Binding(nameof(ValueWidth))
      {
        Source = this,
        Converter = new TextMarginConverter(),
        ConverterParameter = v.TBXAxisValueName
      });

      cv.SetBinding(Canvas.WidthProperty, new Binding(nameof(ValueWidth)) { Source = this });
      cv.SetBinding(Canvas.HeightProperty, new Binding(nameof(TextBox.FontSize))
      {
        Source = v.TBXAxisValueName,
        Converter = new CanvasXAxisTextHeightConverter(),
        ConverterParameter = v.TBXAxisValueName
      });
    }

    private void SetXAxisValueNameSkips(ChartValue cv)
    {
      cv.XAxisValueNameVisibility = 0 == cv.XIndex % (XAxisNameTextSkip + 1) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void AddYAxisValue(bool zeroValue)
    {
      Thickness tbMargin = new Thickness(5, -(FontSize * 0.75), 15, 0);
      VerticalAlignment va = VerticalAlignment.Top;
      if (zeroValue)
      {
        tbMargin = new Thickness(tbMargin.Left, tbMargin.Bottom, tbMargin.Right, tbMargin.Top);
        va = VerticalAlignment.Bottom;
      }
      else
      {
        gridYAxisValues.RowDefinitions.Add(new RowDefinition());
      }
      TextBlock tby = new TextBlock()
      {
        Text = $"{YAxisPrefix}{((gridYAxisValues.RowDefinitions.Count) * YAxisInterval):0.00}{YAxisPostfix}",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = va,
        Margin = tbMargin
      };
      Grid.SetRow(tby, 0);
      Line notchLine = new Line()
      {
        X2 = 3,
        Margin = new Thickness(0, 0, 1, 0),
        StrokeThickness = 1,
        HorizontalAlignment = HorizontalAlignment.Right,
      };
      Binding bNLStroke = new Binding(nameof(Foreground))
      {
        Source = this
      };
      notchLine.SetBinding(Line.StrokeProperty, bNLStroke);
      Grid.SetRow(notchLine, 0);
      Line dashLine = new Line()
      {
        X2 = canvasXYAxis.ActualWidth,
        Margin = new Thickness(0, 0, -canvasXYAxis.ActualWidth, 0),
        Stroke = new SolidColorBrush(Color.FromArgb(0x80, 0x80, 0x80, 0x80)),
        StrokeThickness = 0.4,
        StrokeDashArray = new DoubleCollection(new double[] { 10, 10 }),
        HorizontalAlignment = HorizontalAlignment.Right,
      };
      dashLine.SetBinding(Line.OpacityProperty, new Binding(nameof(GridOpacity))
      {
        Source = this
      });
      dashLine.SetBinding(Line.StrokeProperty, bNLStroke);
      dashLine.SetBinding(Line.X2Property, new Binding(nameof(Line.ActualWidth))
      {
        ElementName = nameof(canvasXYAxis)
      });
      dashLine.SetBinding(Line.MarginProperty, new Binding(nameof(Line.ActualWidth))
      {
        ElementName = nameof(canvasXYAxis),
        Converter = new DashLineWidthConverter()
      });
      dashLine.SetBinding(Line.VisibilityProperty, new Binding(nameof(ShowHorizontalGridLines))
      {
        Source = this,
        Converter = new BooleanToVisibilityConverter()
      });
      Grid.SetRow(dashLine, 0);
      foreach (UIElement child in gridYAxisValues.Children)
      {
        Grid.SetRow(child, Grid.GetRow(child) + 1);
      }
      gridYAxisValues.Children.Add(notchLine);
      gridYAxisValues.Children.Add(dashLine);
      gridYAxisValues.Children.Add(tby);
      YAxisCurrentMax = (gridYAxisValues.RowDefinitions.Count) * YAxisInterval;
    }
    #endregion // Axis

    #region Trendline
    private void CalculateTrendline()
    {
      List<double> yValues = ItemsSource.Select(x => x.Y).ToList();
      List<double> xValues = Enumerable.Range(0, ItemsSource.Count).Select(x => (double)x).ToList();
      yValues.Insert(0, 0);
      xValues.Insert(0, 0);
      TrendlineCalc = new Trendline(yValues, xValues);
    }
    #endregion // Trendline

    private void UserControl_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.F5)
      {
        DrawDiagram(true, ItemsSource);
      }
      else if (Key.F3 == e.Key)
      {
        ScrollAndCenterOnMouse();
      }
    }

    private bool zoomDetected;
    private double sv1;
    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (sender is ScrollViewer scrollViewer && null != scrollViewer)
      {
        sv1 = svValues.ExtentWidth;
        var ip = svValues.Content as UIElement;
        Point mouseOnScrollViewerPosition = Mouse.GetPosition(svValues);
        p1 = svValues.TranslatePoint(mouseOnScrollViewerPosition, ip);
        double stepValue = 1.5;
        if (e.Delta < 0)
        {
          if (!Modifier.IsCtrlDown)
          {
            scrollViewer.LineRight();
          }
          else
          {
            if (ScrollBarVisibility.Auto != HorizontalScrollBarVisibility && ScrollBarVisibility.Visible != HorizontalScrollBarVisibility)
            {
              return;
            }

            if (0.5 < ValueWidth && Visibility.Visible == svValues.ComputedHorizontalScrollBarVisibility)
            {
              ValueWidth /= stepValue;
              zoomDetected = true;
            }
          }
        }
        else
        {
          if (!Modifier.IsCtrlDown)
          {
            scrollViewer.LineLeft();
          }
          else
          {
            if (ScrollBarVisibility.Auto != HorizontalScrollBarVisibility && ScrollBarVisibility.Visible != HorizontalScrollBarVisibility)
            {
              return;
            }
            if (ValueWidth < svValues.ActualWidth / 2)
            {
              zoomDetected = true;
              ValueWidth *= stepValue;
            }
          }
        }
        e.Handled = true;
      }
    }


    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      DrawDiagram(true, ItemsSource);
    }

    private void svValues_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      //if (0 < Math.Abs(e.VerticalChange))
      //{
      //  svYAxis.ScrollToVerticalOffset(e.VerticalOffset);
      //}
      if (zoomDetected)
      {
        ScrollAndCenterOnMouse();
        zoomDetected = false;
      }
    }

    private void UserControl_MouseMove(object sender, MouseEventArgs e)
    {
      TestString = (Mouse.DirectlyOver as FrameworkElement)?.Name;
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          if (0 == gridYAxisValues.RowDefinitions.Count)
          {
            DrawDiagram(true, ItemsSource);
          }
          else
          {
            DrawDiagram(false, e.NewItems);
            svValues.ScrollToRightEnd();
          }
          break;
        case NotifyCollectionChangedAction.Remove:
        case NotifyCollectionChangedAction.Replace:
        case NotifyCollectionChangedAction.Move:
        case NotifyCollectionChangedAction.Reset:
          DrawDiagram(true, ItemsSource);
          break;
        default:
          break;
      }
    }
  }

  internal class InnerHelper
  {
    internal static double CalculateFactor(double max, double count)
    {
      double ratioOfHeight = Math.Min(1, max / count);
      if (double.IsNaN(ratioOfHeight))
      {
        ratioOfHeight = 0;
      }

      return ratioOfHeight;
    }

    internal static Size MeasureString(TextBlock tb)
    {
      FormattedText formattedText = new FormattedText(
          tb.Text,
          CultureInfo.CurrentCulture,
          FlowDirection.LeftToRight,
          new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
          tb.FontSize,
          Brushes.Black
          //,
          //VisualTreeHelper.GetDpi(v).PixelsPerDip
          );
      return new Size(formattedText.WidthIncludingTrailingWhitespace * 1, formattedText.Height);
    }
  }

  public abstract class ChartValue : DependencyObject
  {

    public Brush Fill
    {
      get { return (Brush)GetValue(FillProperty); }
      set { SetValue(FillProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register("Fill", typeof(Brush), typeof(ChartValue), new PropertyMetadata(null));

    internal Visibility XAxisValueNameVisibility
    {
      get { return (Visibility)GetValue(XAxisValueNameVisibilityProperty); }
      set { SetValue(XAxisValueNameVisibilityProperty, value); }
    }

    // Using a DependencyProperty as the backing store for XAxisValueNameVisibility.  This enables animation, styling, binding, etc...
    internal static readonly DependencyProperty XAxisValueNameVisibilityProperty =
        DependencyProperty.Register("XAxisValueNameVisibility", typeof(Visibility), typeof(ChartValue), new PropertyMetadata(Visibility.Visible));




    public event EventHandler DoubleClick;
    internal void OnDoubleClick()
    {
      DoubleClick?.Invoke(this, new EventArgs());
    }
    public double Y { get; set; }
    public string XAxisValueName { get; set; }
    public object ToolTip { get; set; }
    public object RawData { get; set; }
    public double MinWidth { get; set; }
    public int XIndex { get; set; }

    internal TextBlock TBXAxisValueName { get; set; }

    public ChartValue(double y)
    {
      Y = y;
      MinWidth = 50;
    }
  }

  public class BarChartValue : ChartValue
  {
    public string BarText { get; set; }
    public bool IsSeparator { get; set; }

    public BarChartValue(double y)
      : base(y)
    {
      XAxisValueName = null;
      IsSeparator = false;
    }
  }

  public class LineChartValue : ChartValue
  {
    public double StrokeThickness
    {
      get { return (double)GetValue(StrokeThicknessProperty); }
      set { SetValue(StrokeThicknessProperty, value); }
    }

    // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty StrokeThicknessProperty =
        DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineChartValue), new PropertyMetadata(1d));

    public Brush Stroke
    {
      get { return (Brush)GetValue(StrokeProperty); }
      set { SetValue(StrokeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineChartValue), new PropertyMetadata(Brushes.Black));

    public double Width
    {
      get { return (double)GetValue(WidthProperty); }
      set { SetValue(WidthProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register("Width", typeof(double), typeof(LineChartValue), new PropertyMetadata(1d));

    public double Height
    {
      get { return (double)GetValue(HeightProperty); }
      set { SetValue(HeightProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register("Height", typeof(double), typeof(LineChartValue), new PropertyMetadata(1d));


    public Shape Dot { get; set; }

    public LineChartValue(double y)
      : base(y)
    {
    }
  }

  public class DotChartValue : LineChartValue
  {
    public bool WithLine { get; set; }
    public eChartDotShape DotShape { get; set; }

    public DotChartValue(double y)
      : base(y)
    {
      DotShape = eChartDotShape.Circle;
      WithLine = true;
    }

    public DotChartValue(LineChartValue from)
      : base(from.Y)
    {
      DotShape = eChartDotShape.Circle;
      WithLine = true;
      Fill = from.Fill;
      XAxisValueName = from.XAxisValueName;
      ToolTip = from.ToolTip;
      RawData = from.RawData;
      MinWidth = from.MinWidth;
      XIndex = from.XIndex;
      Stroke = from.Stroke;
      StrokeThickness = from.StrokeThickness;
      Width = from.Width;
      Height = from.Height;
    }
  }
}





