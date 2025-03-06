using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AMD.Util.View.WPF.UserControls.Chart
{
  internal class LineChartValueLocal
  {
    internal LineChartValue LCV { get; set; }
    internal Panel Parent { get; set; }
    internal Point Location { get; set; }

    public LineChartValueLocal(LineChartValue lcv, Panel parent)
    {
      LCV = lcv;
      Parent = parent;
    }
  }

  internal class LineCanvas : Canvas
  {

    public double ValueWidth
    {
      get { return (double)GetValue(ValueWidthProperty); }
      set { SetValue(ValueWidthProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ValueWidth.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValueWidthProperty =
        DependencyProperty.Register("ValueWidth", typeof(double), typeof(LineCanvas), new PropertyMetadata(0d));

    public double? YAxisMax
    {
      get { return (double?)GetValue(YAxisMaxProperty); }
      set { SetValue(YAxisMaxProperty, value); }
    }

    // Using a DependencyProperty as the backing store for YAxisMax.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty YAxisMaxProperty =
        DependencyProperty.Register("YAxisMax", typeof(double?), typeof(LineCanvas), new PropertyMetadata(null));

    public List<LineChartValueLocal> Lines { get; set; } = new List<LineChartValueLocal>();

    protected override void OnRender(DrawingContext dc)
    {
      base.OnRender(dc);
      if (Lines is null || YAxisMax is null)
      {
        return;
      }

      Point prevPoint = new Point(0, 0);
      bool first = true;
      foreach (LineChartValueLocal line in Lines)
      {
        prevPoint = DrawLine(dc, first, line, prevPoint);
        first = false;
      }
    }

    private Point DrawLine(DrawingContext dc, bool first, LineChartValueLocal line, Point prevDotLocation)
    {
      LineChartValue prev = line.LCV.Previous as LineChartValue;
      double strokeThickness = line.LCV.StrokeThickness;
      Point currDotLocation = new Point(ValueWidth * (line.LCV.XIndex + 1) - ValueWidth / 2, ActualHeight - (line.LCV.Y * ActualHeight / YAxisMax.Value));// line.LCV.Dot.TranslatePoint(new Point(), line.Parent);

      if (!first)
      {
        Pen pen = new Pen(line.LCV.Stroke, line.LCV.StrokeThickness);
        dc.DrawLine(pen, currDotLocation, prevDotLocation);
        Pen penLine = new Pen(line.LCV.Stroke, 0);
        dc.DrawEllipse(line.LCV.Fill, pen, currDotLocation, line.LCV.Width, line.LCV.Height);
        //dc.DrawRectangle(line.LCV.Fill, pen, new Rect(currDotLocation.X, currDotLocation.Y, line.LCV.Width, line.LCV.Height));
      }
      return currDotLocation;
    }
  }
}
