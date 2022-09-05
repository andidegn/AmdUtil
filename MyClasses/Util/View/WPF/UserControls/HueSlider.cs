using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AMD.Util.View.WPF.UserControls
{
  public class ColorSlider : Slider
  {
    public enum SliderColorStyles
    {
      HueGradient,

    }
    #region DependencyProperties


    public SliderColorStyles SliderColorStyle
    {
      get { return (SliderColorStyles)GetValue(SliderColorStyleProperty); }
      set { SetValue(SliderColorStyleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SliderColorStyle.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SliderColorStyleProperty =
        DependencyProperty.Register("SliderColorStyle", typeof(SliderColorStyles), typeof(ColorSlider), new PropertyMetadata(SliderColorStyles.HueGradient));


    #endregion // DependencyProperties

    static ColorSlider()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
    }

    public ColorSlider()
    {
      SetupHandlers();
    }

    private void SetupHandlers()
    {
      TouchDown += MTS_TouchDown;
      TouchMove += MTS_TouchMove;
      TouchUp += MTS_TouchUp;
    }

    private void MTS_TouchDown(object sender, TouchEventArgs e)
    {
      if (sender is Slider)
      {
        Slider sl = sender as Slider;
        if (sl.TouchesCaptured.Count() == 0)
        {
          sl.CaptureTouch(e.TouchDevice);
          e.Handled = false;
        }
      }
    }

    private void MTS_TouchMove(object sender, TouchEventArgs e)
    {
      if (sender is Slider)
      {
        Slider sl = sender as Slider;
        double position = e.GetTouchPoint(sl).Position.Y;

        if (position > 0 && position < sl.ActualHeight)
        {
          Track track = sl.Template.FindName("PART_Track", sl) as Track;
          Thumb thumb = track.Thumb;
          sl.Value = CalcValueBasedOnTouchLocation(sl, position, sl.Maximum, thumb.Height / 2);
        }

        e.Handled = true;
      }
    }

    private void MTS_TouchUp(object sender, TouchEventArgs e)
    {
      if (sender is Slider)
      {
        Slider sl = sender as Slider;
        if (e.TouchDevice.Captured == sl)
        {
          sl.ReleaseTouchCapture(e.TouchDevice);
          e.Handled = true;
        }
      }
    }

    private double CalcValueBasedOnTouchLocation(Control ctrl, double position, double scale, double thumbRadius)
    {
      double min = thumbRadius;
      double max = ctrl.ActualHeight - thumbRadius;

      double range = max - min;

      return scale - Math.Max(position - min, 0) * scale / range;
    }
  }
}
