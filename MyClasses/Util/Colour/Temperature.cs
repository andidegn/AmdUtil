using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Colour
{
  public static class Temperature
  {
    public static RGB GetRgbFromKelvin(int kelvin)
    {
      if (kelvin < 1000 || kelvin > 40000)
      {
        return null;
      }

      kelvin = kelvin / 100;

      RGB rgb = new RGB();
      // Red
      if (kelvin <= 66)
      {
        rgb.R = 0xFF;
      }
      else
      {
        rgb.R = (int)(329.698727446 * Math.Pow(kelvin - 60, -0.1332047592));
      }

      // Green
      if (kelvin <= 66)
      {
        rgb.G = (int)(99.4708025861 * Math.Log(kelvin) - 161.1195681661);
      }
      else
      {
        rgb.G = (int)(288.1221695283 * Math.Pow(kelvin - 60, -0.0755148492));
      }

      // Blue
      if (kelvin >= 66)
      {
        rgb.B = 0xFF;
      }
      else
      {
        if (kelvin <= 19)
        {
          rgb.B = 0;
        }
        else
        {
          rgb.B = (int)(138.5177312231 * Math.Log(kelvin - 10) - 305.0447927307);
        }
      }

      rgb.R = Math.Min(rgb.R, 0xFF);
      rgb.R = Math.Max(rgb.R, 0x00);
      rgb.G = Math.Min(rgb.G, 0xFF);
      rgb.G = Math.Max(rgb.G, 0x00);
      rgb.B = Math.Min(rgb.B, 0xFF);
      rgb.B = Math.Max(rgb.B, 0x00);

      return rgb;
    }

    public static (double kelvin, double duv) GetColorTempAndDuvFromLvxy(double x, double y)
    {
      return (GetColorTempFromLvxy(x, y), GetDuvFromLvxy(x, y));
    }

    private static double GetColorTempFromLvxy(double x, double y)
    {
      double n = (x - 0.3320) / (0.1858 - y);
      return 437 * Math.Pow(n, 3) + 3601 * Math.Pow(n, 2) + 6861 * n + 5517;
    }

    private static double GetDuvFromLvxy(double x, double y)
    {
      double u = (4 * x) / (-2 * x + 12 * y + 3);
      double v = (6 * y) / (-2 * x + 12 * y + 3);
      double k6 = -0.00616793;
      double k5 = 0.0893944;
      double k4 = -0.5179722;
      double k3 = 1.5317403;
      double k2 = -2.4243787;
      double k1 = 1.925865;
      double k0 = -0.471106;
      double Lfp = Math.Sqrt(Math.Pow((u - 0.292), 2) + Math.Pow((v - 0.24), 2));
      double a = Math.Acos((u - 0.292) / Lfp);
      double Lbb = k6 * Math.Pow(a, 6) + k5 * Math.Pow(a, 5) + k4 * Math.Pow(a, 4) + k3 * Math.Pow(a, 3) + k2 * Math.Pow(a, 2) + k1 * a + k0;
      return Lfp - Lbb;
    }
  }
}
