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
  }
}
