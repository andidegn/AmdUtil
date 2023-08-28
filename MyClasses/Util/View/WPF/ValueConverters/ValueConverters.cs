using AMD.Util.Data;
using AMD.Util.Extensions;
using AMD.Util.Log;
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static AMD.Util.Data.DataCompare;

namespace AMD.Util.View.WPF.ValueConverters
{
	/// <summary>
	/// Converts LogMsgType to predefined SolidColorBrush
	/// </summary>
	public class LogMsgTypeToColorConverter : IValueConverter
	{
		/// <summary>
		/// Convert LogMsgType to SolidColorBrush
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is LogMsgType))
			{
				return null;
			}
			LogMsgType msgType = (LogMsgType)value;
			SolidColorBrush typeBrush = null;
			switch (msgType)
			{
				case LogMsgType.Debug:
				case LogMsgType.Notification:
				case LogMsgType.Test:
					typeBrush = (SolidColorBrush)Application.Current.TryFindResource("ConsoleLightBlue");
					break;
				case LogMsgType.Serial:
					typeBrush = (SolidColorBrush)Application.Current.TryFindResource("ConsoleBlue");
					break;
				case LogMsgType.Tx:
				case LogMsgType.Error:
					typeBrush = (SolidColorBrush)Application.Current.TryFindResource("ConsoleRed");
					break;
				case LogMsgType.Exception:
					typeBrush = (SolidColorBrush)Application.Current.TryFindResource("ConsolePurple");
					break;
				case LogMsgType.Warning:
					typeBrush = (SolidColorBrush)Application.Current.TryFindResource("WarningYellow");
					break;
				case LogMsgType.Rx:
				case LogMsgType.Result:
					typeBrush = (SolidColorBrush)Application.Current.TryFindResource("ConsoleGreen");
					break;
				case LogMsgType.Measurement:
					typeBrush = Brushes.Lime;
					break;
				default:
					typeBrush = Brushes.White;
					break;
			}
			return typeBrush;
		}

		/// <summary>
		/// Convert SolidColorBrush to LogMsgType (Not implemented)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

  public class ColorToSolidColorBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Color)
      {
        return new SolidColorBrush((Color)value);
      }
      else
      {
        return default(Color);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is SolidColorBrush)
      {
        return (value as SolidColorBrush).Color;
      }
      else
      {
        return default(SolidColorBrush);
      }
    }
  }

  public class ListViewItemIndexToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is ListViewItem))
			{
				return null;
			}
			ListViewItem lvi = value as ListViewItem;
			ListView lv = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
			return lv.ItemContainerGenerator.IndexFromContainer(lvi).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
  }

  public class IntToHexStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return System.Convert.ToUInt32(value).ToString("X");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is String)
      {
        String valueString = value as String;
        if (!String.IsNullOrEmpty(valueString))
        {
          return UInt32.Parse(value as String, NumberStyles.HexNumber);
        }
        else
        {
          return 0;
        }
      }
      throw new NotImplementedException();
    }
  }

  public class BoolToVisibility : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch (Type.GetTypeCode(value.GetType()))
      {
        case TypeCode.Boolean:
          return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        case TypeCode.SByte:
          return (sbyte)value > 0 ? Visibility.Visible : Visibility.Collapsed;
        case TypeCode.Byte:
          return (byte)value > 0 ? Visibility.Visible : Visibility.Collapsed;
        case TypeCode.Decimal:
          return (Decimal)value > 0 ? Visibility.Visible : Visibility.Collapsed;
        case TypeCode.Char:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        default:
          LogWriter.Instance.WriteToLog(LogMsgType.Error, "Type: {0}", value.GetType().ToString());
          throw new NotSupportedException();
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool retVal = false;

      if (value is Visibility vis)
      {
        retVal = Visibility.Visible == vis;
      }

      return retVal;
    }
  }

  public class InvertedBoolToVisibility : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch (Type.GetTypeCode(value.GetType()))
      {
        case TypeCode.Boolean:
          return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        case TypeCode.SByte:
          return (sbyte)value == 0 ? Visibility.Visible : Visibility.Collapsed;
        case TypeCode.Byte:
          return (byte)value == 0 ? Visibility.Visible : Visibility.Collapsed;
        case TypeCode.Decimal:
          return (Decimal)value == 0 ? Visibility.Visible : Visibility.Collapsed;
        case TypeCode.Char:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        default:
          AMD.Util.Log.LogWriter.Instance.WriteToLog(AMD.Util.Log.LogMsgType.Error, "Type: {0}", value.GetType().ToString());
          throw new NotSupportedException();
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  public class VisibilityToBool : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is Visibility ? (Visibility)value == Visibility.Visible ? true : false : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class InvertedVisibilityToBool : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is Visibility ? (Visibility)value == Visibility.Visible ? false : true : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class IsStringHexNumberToBool : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (value as String)?.IsHexNumber();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class IsStringNumberToBool : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (value as String)?.IsNumber();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class InvertBool : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is bool ? !(bool)value : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is bool ? !(bool)value : false;
    }
  }

  public class ValueZeroToVisibility : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value.IsNumber() ? 0 == (double)value ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is bool ? !(bool)value : false;
    }
  }

  public class LeftRightTextToFormattedString : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string retVal = "Error";

      if (value is OverviewLine ol)
      {
        retVal = $"{ol.LeftLine} - {ol.RightLine}";
      }

      return retVal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class LineCountToLineNumberStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      StringBuilder sb = new StringBuilder();

      if (value is int lineCount)
      {
        int charCnt = lineCount.ToString().Length;
        for (int i = 1; i <= lineCount; i++)
        {
          sb.AppendLine(i.ToString().PadLeft(charCnt));
        }
      }

      return sb.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
