using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AMD.Util.Validation
{
  public class StringValidater
  {
    public static readonly String REGEX_HEX = @"\A\b[0-9a-fA-F-]+\b\Z";
    public static readonly String REGEX_DEC = @"\A\b[0-9]+\b\Z";
    public static readonly String REGEX_ALPHANUMERIC = @"^[a-zA-Z0-9]+$";
    public static readonly String REGEX_LETTERS = @"^[a-zA-Z]+$";
    public static readonly String REGEX_NUMBERS = @"^[0-9]+$";
    public static readonly String REGEX_FLOAT = @"^\d*\.?\d*$";

    public static readonly String REGEX_PROPERTY_NAME = @"^[a-zA-Z_]+[a-zA-Z0-9_]*$";

    public static bool ValidateNumber(String text)
    {
      return Regex.IsMatch(text, REGEX_NUMBERS);
    }

    public static bool ValidateHex(String text)
    {
      return Regex.IsMatch(text, REGEX_HEX);
    }

    public static bool ValidateAlphanumeric(String text)
    {
      return Regex.IsMatch(text, REGEX_ALPHANUMERIC);
    }

    public static bool ValidatePropertyName(String text)
    {
      return Regex.IsMatch(text, REGEX_PROPERTY_NAME);
    }
  }
}
