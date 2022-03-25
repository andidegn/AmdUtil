using AMD.Util.Colour;
using AMD.Util.Extensions;
using AMD.Util.Extensions.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AMD.Util.View.WPF.UserControls
{
  public class ValuePickerFillConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double)
      {
        double position = (double)value;
        return position > 0.55 ? Brushes.Black : Brushes.White;
      }
      return Brushes.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Interaction logic for ColorPicker.xaml
  /// </summary>
  public partial class ColorPicker : UserControl
  {
    #region Events
    /// <summary>
    /// Eventhandler SelectedBrushChanged event
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void SelectedBrushChangedHandler(object sender, BrushChangedEventArgs args);
    public event SelectedBrushChangedHandler SelectedBrushChanged;

    private void UpdateSelectedBrushChanged()
    {
      SelectedBrushChanged?.Invoke(this, new BrushChangedEventArgs(Resources["OriginalBrush"] as SolidColorBrush, SelectedBrush));
    }
    #endregion // Events

    #region Private Properties
    private bool updatingColors;
    #endregion // Private Properties

    #region Public Properties
    public ColorHSV ColorHsv
    {
      get { return (ColorHSV)GetValue(ColorHsvProperty); }
      set { SetValue(ColorHsvProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ColorHsv.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ColorHsvProperty =
        DependencyProperty.Register("ColorHsv", typeof(ColorHSV), typeof(ColorPicker), new PropertyMetadata(new ColorHSV(0xFF, 0, 0, 1)));

    public SolidColorBrush SelectedBrush
    {
      get { return (SolidColorBrush)GetValue(SelectedBrushProperty); }
      set { SetValue(SelectedBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedBrushProperty =
        DependencyProperty.Register("SelectedBrush", typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(default(Brush), SelectedBrushPropertyCallback));

    private static void SelectedBrushPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ColorPicker cp = d as ColorPicker;
      cp?.SelectedBrushCallbackUpdate(e.NewValue as SolidColorBrush);
    }

    private void SelectedBrushCallbackUpdate(SolidColorBrush b)
    {
      if (!updatingColors)
      {
        String s = this.Name;
        ColorHSV c = new ColorHSV(b.Color);
        if (ColorHsv == null)
        {
          ColorHsv = new ColorHSV();
        }
        ColorHsv.Hue = c.Hue;
        ColorHsv.Saturation = c.Saturation;
        ColorHsv.Value = c.Value;
        UpdateColor(true);
      }
    }

    public SolidColorBrush OriginalBrush
    {
      get { return (SolidColorBrush)GetValue(OriginalBrushProperty); }
      set { SetValue(OriginalBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OriginalBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OriginalBrushProperty =
        DependencyProperty.Register("OriginalBrush", typeof(SolidColorBrush), typeof(ColorPicker), new PropertyMetadata(default(Brush)));

    public Color CurrentBaseColor
    {
      get { return (Color)GetValue(CurrentBaseColorProperty); }
      set { SetValue(CurrentBaseColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentBaseColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentBaseColorProperty =
        DependencyProperty.Register("CurrentBaseColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Red));
    #endregion // Public Properties

    public ColorPicker()
    {
      this.DataContext = this;

      InitializeComponent();

      sliderAlpha.AddHandler(Slider.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonDown), true);
      sliderAlpha.AddHandler(Slider.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonUp), true);

      sliderRed.AddHandler(Slider.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonDown), true);
      sliderRed.AddHandler(Slider.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonUp), true);

      sliderGreen.AddHandler(Slider.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonDown), true);
      sliderGreen.AddHandler(Slider.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonUp), true);

      sliderBlue.AddHandler(Slider.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonDown), true);
      sliderBlue.AddHandler(Slider.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonUp), true);



      sliderHue.AddHandler(Slider.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonDown), true);
      sliderHue.AddHandler(Slider.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonUp), true);

      sliderSaturation.AddHandler(Slider.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonDown), true);
      sliderSaturation.AddHandler(Slider.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonUp), true);

      sliderValue.AddHandler(Slider.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonDown), true);
      sliderValue.AddHandler(Slider.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(slider_PreviewMouseLeftButtonUp), true);
    }

    #region Private Functions
    private void SetColorHueFromPicker()
    {
      Point mousePosition = Mouse.GetPosition(rectColorHue);
      double hue = 360 * mousePosition.X / rectColorHue.ActualWidth;
      if (hue < 0) hue = 0; if (hue > 360) hue = 360;
      ColorHsv.Hue = hue;
      UpdateColor();
    }

    private void SetColorSaturationValueFromPicker()
    {
      Point position = Mouse.GetPosition(rectColorSaturation);

      double saturation = 1 * position.X / rectColorSaturation.ActualWidth;
      double value = 1 - 1 * position.Y / rectColorSaturation.ActualHeight;

      if (saturation < 0) saturation = 0; if (saturation > 1) saturation = 1;
      if (value < 0) value = 0; if (value > 1) value = 1;

      ColorHsv.Saturation = saturation;
      ColorHsv.Value = value;

      UpdateColor();
    }

    private void UpdateColor(bool skipSelectedBrush = false)
    {
      if (IsLoaded)
      {
        String s = this.Name;
        updatingColors = true;
        CurrentBaseColor = new ColorHSV(0xFF, ColorHsv.Hue, 1, 1).GetMediaColor;

        if (!skipSelectedBrush)
        {
          SelectedBrush = new SolidColorBrush(ColorHsv.GetMediaColor);
          UpdateSelectedBrushChanged();
        }

        UpdateSelectorPositions();
        UpdateHexTextBox();

        //tbCurrentMousePosition.Text = String.Format("Base R: {0} G: {1}, B: {2}\nNew R: {3} G: {4}, B: {5}\nH: {6:0.000}, S: {7:0.000}, L: {8:0.000}", CurrentBaseColor.R, CurrentBaseColor.G, CurrentBaseColor.B, SelectedBrush.Color.R, SelectedBrush.Color.G, SelectedBrush.Color.B, ColorHsv.Hue, ColorHsv.Saturation, ColorHsv.Value);
        updatingColors = false;
      }
    }

    private void TrySetColorFromRGBTextBoxes()
    {
      byte alpha, red, green, blue;
      if (byte.TryParse(tbAlpha?.Text, out red) && byte.TryParse(tbRed?.Text, out red) && byte.TryParse(tbGreen?.Text, out green) && byte.TryParse(tbBlue?.Text, out blue))
      {
        ColorHsv = new ColorHSV(Color.FromArgb
        (
          byte.Parse(tbAlpha.Text),
          byte.Parse(tbRed.Text),
          byte.Parse(tbGreen.Text),
          byte.Parse(tbBlue.Text)
        ));
        UpdateColor();
      }
    }

    private void TrySetColorFromHSVTextBoxes()
    {
      double hue, saturation, value;
      if (double.TryParse(tbHue?.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out hue) && 
        double.TryParse(tbSaturation?.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out saturation) && 
        double.TryParse(tbValue?.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
      {
        ColorHsv = new ColorHSV()
        {
          Hue = hue,
          Saturation = saturation,
          Value = value
        };
        UpdateColor();
      }
    }

    private void SetSliderValue(double delta, Slider slider)
    {
      double value = slider.Value + delta;
      slider.Value = Math.Max(Math.Min(slider.Maximum, value), slider.Minimum);
    }

    #region Selectors
    private void UpdateSelectorPositions()
    {
      SetHueSelectorPosition(ColorHsv.Hue);
      SetColorSelectorPosition(ColorHsv.Saturation, ColorHsv.Value);

      // Hack to eliminate drawing artifacts when moving selector 
      rectColorSaturation.Visibility = Visibility.Collapsed;
      rectColorSaturation.Visibility = Visibility.Visible;
    }

    private void SetColorSelectorPosition(double saturation, double value)
    {
      double width = saturation * rectColorSaturation.ActualWidth - ellipseColorSelector.ActualWidth / 2;
      Canvas.SetLeft(ellipseColorSelector, width);
      double height = (1 - value) * rectColorSaturation.ActualHeight - ellipseColorSelector.ActualHeight / 2;
      Canvas.SetTop(ellipseColorSelector, height);
    }

    private void SetHueSelectorPosition(double hue)
    {
      double d = hue * rectColorHue.ActualWidth / 359.99999;
      Canvas.SetLeft(rectColorHueSelector, (d) - rectColorHueSelector.ActualWidth / 2);
    }
    #endregion // Selectors

    #region TextBoxes
    private void UpdateHexTextBox()
    {
      int caretIndex = tbHexValue.CaretIndex;
      if (!tbHexValue.Text.Contains("#"))
      {
        caretIndex++;
      }
      tbHexValue.Text = ColorHsv.ToString();
      tbHexValue.CaretIndex = Math.Min(caretIndex, tbHexValue.Text.Length);
    }
    #endregion // Textboxes
    #endregion // Private Functions

    #region EventHandlers
    private void rectColorHue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is Rectangle)
      {
        Mouse.Capture(sender as Rectangle);
        SetColorHueFromPicker();
      }
    }
    private void rectColorHue_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Mouse.Capture(null);
    }

    private void rectColorHue_MouseMove(object sender, MouseEventArgs e)
    {
      if ((sender as Rectangle) == Mouse.Captured)
      {
        SetColorHueFromPicker();
      }
    }

    private void rectColorSaturation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is Rectangle)
      {
        Mouse.Capture(sender as Rectangle);
        SetColorSaturationValueFromPicker();
      }
    }

    private void rectColorSaturation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Mouse.Capture(null);
    }

    private void rectColorSaturation_MouseMove(object sender, MouseEventArgs e)
    {
      if ((sender as Rectangle) == Mouse.Captured)
      {
        SetColorSaturationValueFromPicker();
      }
    }

    private void rectOriginalColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ColorHsv = new ColorHSV(OriginalBrush.Color);
      UpdateColor();
    }

    private void sliderRgb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if ((sender as Slider).IsKeyboardFocused)
      {
        UpdateColor();
      }
    }

    private void sliderHsv_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if ((sender as Slider).IsKeyboardFocused)
      {
        UpdateColor();
      }
    }

    private void slider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is Control)
      {
        Mouse.Capture(sender as Control);
      }
    }

    private void slider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Mouse.Capture(null);
    }

    private void slider_MouseMove(object sender, MouseEventArgs e)
    {
      if (sender is Slider && (sender as Slider) == Mouse.Captured)
      {
        Slider sl = sender as Slider;
        sl.Focus();
        Point mousePosition = e.GetPosition(sl);
        sl.Value = Math.Min(Math.Max(1.0d / sl.ActualWidth * mousePosition.X * sl.Maximum, sl.Minimum), sl.Maximum);
      }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (SelectedBrush == null)
      {
        ColorHsv = new ColorHSV(0xFF, 0d, 0d, 1d);
      }
      else
      {
        ColorHsv = new ColorHSV(SelectedBrush.Color);
      }
      OriginalBrush = SelectedBrush;
      CurrentBaseColor = new ColorHSV(1, ColorHsv.Hue, 1, 1).GetMediaColor;
      UpdateColor();
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateColor();
    }

    private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      e.Handled = false;
      if (sliderAlpha.IsKeyboardFocused)
      {
        SetSliderValue(e.Delta / 120, sliderAlpha);
        e.Handled = true;
      }
      else if (sliderRed.IsKeyboardFocused)
      {
        SetSliderValue(e.Delta / 120, sliderRed);
        e.Handled = true;
      }
      else if (sliderGreen.IsKeyboardFocused)
      {
        SetSliderValue(e.Delta / 120, sliderGreen);
        e.Handled = true;
      }
      else if (sliderBlue.IsKeyboardFocused)
      {
        SetSliderValue(e.Delta / 120, sliderBlue);
        e.Handled = true;
      }
      else if (sliderHue.IsKeyboardFocused)
      {
        SetSliderValue(e.Delta / 120, sliderHue);
        e.Handled = true;
      }
      else if (sliderSaturation.IsKeyboardFocused)
      {
        SetSliderValue(e.Delta / 12000d, sliderSaturation);
        e.Handled = true;
      }
      else if (sliderValue.IsKeyboardFocused)
      {
        SetSliderValue(e.Delta / 12000d, sliderValue);
      }
    }
    #endregion // EventHandlers

    private void testColorButton_Click(object sender, RoutedEventArgs e)
    {
      //CurrentColor = Color.FromRgb(0, 0xFF, 0);
      SelectedBrush = new SolidColorBrush(Colors.YellowGreen);
    }

    private void testColorButton1_Click(object sender, RoutedEventArgs e)
    {
      //CurrentBaseColor = Colors.Yellow;
      OriginalBrush = SelectedBrush;
    }

    private void tbHexValue_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!updatingColors)
      {
        string text = tbHexValue.Text.Trim(' ', '#');
        if ((6 == text.Length || 8 == text.Length) && text.IsHexNumber())
        {
          string[] values = text.Split(2);
          List<byte> byteValues = new List<byte>();

          if (4 > values.Length)
          {
            byteValues.Add(0xFF);
          }

          foreach (string sValue in values)
          {
            byteValues.Add(byte.Parse(sValue, NumberStyles.HexNumber));
          }

          ColorHsv = new ColorHSV(Color.FromArgb
          (
            byteValues[0],
            byteValues[1],
            byteValues[2],
            byteValues[3]
          ));
          UpdateColor();
        }
      }
    }

    private void tbHexValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      string text = string.Format("{0}{1}", tbHexValue.Text, e.Text);
      if ((e.Text.IsHexNumber() && tbHexValue.SelectedText.Length > 0) ||
        (text.Length <= 6 && text.IsHexNumber()) ||
        (text.Length <= 8 && text.IsHexNumber()) ||
        (text.Length <= 7 && text[0] == '#' && text.Substring(1).IsHexNumber()) ||
        (text.Length <= 9 && text[0] == '#' && text.Substring(1).IsHexNumber()))
      {
        e.Handled = false;
      }
      else
      {
        e.Handled = true;
      }
    }

    private void tbRGB_KeyUp(object sender, KeyEventArgs e)
    {
      if (Key.Enter == e.Key)
      {
        TrySetColorFromRGBTextBoxes();
      }
    }

    private void tbHSV_KeyUp(object sender, KeyEventArgs e)
    {
      if (Key.Enter == e.Key)
      {
        TrySetColorFromHSVTextBoxes();
      }
    }
  }
}
