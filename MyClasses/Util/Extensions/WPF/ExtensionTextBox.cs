using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AMD.Util.Extensions.WPF
{
  public static class ExtensionTextBox
  {
    public static bool ValidateNumber(this TextBox tb, long minValue = long.MinValue, long maxValue = long.MaxValue, String input = "")
    {
      bool retVal;
      long val = 0;
      String completeInput = $"{tb.Text}{input}";
      retVal = completeInput.IsNumber();
      retVal &= long.TryParse(completeInput, NumberStyles.Number, CultureInfo.InvariantCulture, out val);
      retVal &= 0 < tb.SelectedText.Length || (minValue <= val && val <=maxValue);

      return retVal;
    }
    public static bool ValidateHexNumber(this TextBox tb, long minValue = long.MinValue, long maxValue = long.MaxValue, String input = "")
    {
      bool retVal;
      long val = 0;
      String completeInput = $"{tb.Text}{input}";
      retVal = completeInput.IsHexNumber();
      retVal &= long.TryParse(completeInput, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out val);
      retVal &= 0 < tb.SelectedText.Length || (minValue <= val && val <= maxValue);

      return retVal;
    }
  }
}
