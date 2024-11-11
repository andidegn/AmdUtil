using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AMD.Util.View.WPF
{
  partial class AMDTouchSliderSnapSideRD : ResourceDictionary
  {
    public AMDTouchSliderSnapSideRD()
    {
      InitializeComponent();
    }

    private double CalcValueBasedOnTouchLocation(Control ctrl, double position, double scale, double thumbRadius)
    {
      double min = thumbRadius;
      double max = ctrl.ActualHeight - thumbRadius;

      double range = max - min;

      return scale - Math.Max(position - min, 0) * scale / range;
    }

    private void Slider_TouchDown(object sender, TouchEventArgs e)
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

    private void Slider_TouchMove(object sender, TouchEventArgs e)
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

    private void Slider_TouchUp(object sender, TouchEventArgs e)
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
  }
}
