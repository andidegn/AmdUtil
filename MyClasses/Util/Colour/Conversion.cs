using System;
using System.ComponentModel;
using System.Windows.Media;

namespace AMD.Util.Colour
{
  public class ColorHSV : INotifyPropertyChanged
  {
    private bool updatingColors;

    private double hue;
    public double Hue
    {
      get { return hue; }
      set
      {
        if (hue != value)
        {
          hue = value;
          OnPropertyChanged("Hue");
          if (!updatingColors)
          {
            UpdateRgb();
          }
        }
      }
    }
    private double saturation;
    public double Saturation
    {
      get { return saturation; }
      set
      {
        if (saturation != value)
        {
          saturation = value;
          OnPropertyChanged("Saturation");
          if (!updatingColors)
          {
            UpdateRgb();
          }
        }
      }
    }
    private double value;
    public double Value
    {
      get { return value; }
      set
      {
        if (this.value != value)
        {
          this.value = value;
          OnPropertyChanged("Value");
          if (!updatingColors)
          {
            UpdateRgb();
          }
        }
      }
    }

    private byte a;
    public byte A
    {
      get { return a; }
      set
      {
        if (a != value)
        {
          a = value;
          OnPropertyChanged("A");
          if (!updatingColors)
          {
            UpdateHsv();
          }
        }
      }
    }

    private byte r;
    public byte R
    {
      get { return r; }
      private set
      {
        if (r != value)
        {
          r = value;
          OnPropertyChanged("R");
          if (!updatingColors)
          {
            UpdateHsv();
          }
        }
      }
    }
    private byte g;
    public byte G
    {
      get { return g; }
      private set
      {
        if (g != value)
        {
          g = value;
          OnPropertyChanged("G");
          if (!updatingColors)
          {
            UpdateHsv();
          }
        }
      }
    }
    private byte b;
    public byte B
    {
      get { return b; }
      private set
      {
        if (b != value)
        {
          b = value;
          OnPropertyChanged("B");
          if (!updatingColors)
          {
            UpdateHsv();
          }
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(String name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public System.Drawing.Color GetDrawingColor
    {
      get
      {
        Color color = GetMediaColor;
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
      }
    }

    public Color GetMediaColor
    {
      get
      {
        return HSVToColor(A, Hue, Saturation, Value);
      }
    }

    public ColorHSV()
    {
      hue = 0;
      saturation = 0;
      value = 0;
    }

    public ColorHSV(Color color) : this(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)) { }

    public ColorHSV(System.Drawing.Color color)
    {
      byte alpha = A;
      double hue = Hue, saturation = Saturation, value = Value;
      ColorToHSV(color, ref alpha, ref hue, ref saturation, ref value);
      this.A = alpha;
      this.Hue = hue;
      this.Saturation = saturation;
      this.Value = value;
    }

    public ColorHSV(byte a, byte r, byte g, byte b) : this(Color.FromArgb(a, r, g, b)) { }

    public ColorHSV(byte alpha, double hue, double saturation, double value)
    {
      this.A = alpha;
      this.Hue = hue;
      this.Saturation = saturation;
      this.Value = value;
    }

    private void UpdateRgb()
    {
      Color c = GetMediaColor;
      updatingColors = true;
      A = c.A;
      R = c.R;
      G = c.G;
      B = c.B;
      updatingColors = false;
    }



    private void UpdateHsv()
    {
      byte alpha = A;
      double hue = Hue, saturation = Saturation, value = Value;
      ColorToHSV(System.Drawing.Color.FromArgb(A, R, G, B), ref alpha, ref hue, ref saturation, ref value);
      updatingColors = true;
      A = alpha;
      Hue = hue;
      Saturation = saturation;
      Value = value;
      updatingColors = false;
    }

    public static void ColorToHSV(System.Drawing.Color color, ref byte alpha, ref double hue, ref double saturation, ref double value)
    {
      int max = Math.Max(color.R, Math.Max(color.G, color.B));
      int min = Math.Min(color.R, Math.Min(color.G, color.B));
      int delta = max - min;
      //saturation = color.GetSaturation();
      alpha = color.A;
      if (delta == 0)
      {
        saturation = 0;
      }
      else
      {
        hue = color.GetHue();
        saturation = (double)delta / max;
      }
      value = max / 255d;
    }

    public static Color HSVToColor(byte alpha, double hue, double saturation, double value)
    {
      ValidateValues(ref hue, ref saturation, ref value);

      double f = hue / 60 - Math.Floor(hue / 60);

      value = value * 255;
      byte v = Convert.ToByte(value);
      byte p = Convert.ToByte(value * (1 - saturation));
      byte q = Convert.ToByte(value * (1 - f * saturation));
      byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

      if (hue < 60 || hue == 360)
      {
        return Color.FromArgb(alpha, v, t, p);
      }
      else if (hue < 120)
      {
        return Color.FromArgb(alpha, q, v, p);
      }
      else if (hue < 180)
      {
        return Color.FromArgb(alpha, p, v, t);
      }
      else if (hue < 240)
      {
        return Color.FromArgb(alpha, p, q, v);
      }
      else if (hue < 300)
      {
        return Color.FromArgb(alpha, t, p, v);
      }
      else
      {
        return Color.FromArgb(alpha, v, p, q);
      }
    }

    private static void ValidateValues(ref double hue, ref double saturation, ref double value)
    {
      if (hue < 0) hue = 0; if (hue > 360) hue = 360;

      if (saturation < 0) saturation = 0; if (saturation > 1) saturation = 1;

      if (value < 0) value = 0; if (value > 1) value = 1;
    }

    public override String ToString()
    {
      return String.Format("#{0:X2}{1:X2}{2:X2}", R, G, B);
    }
  }
}
