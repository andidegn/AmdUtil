using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AMD.Util.View.WPF.UserControls
{
  public class WatermarkTextBox : TextBox
  {
    #region DependencyProperties
    public string Watermark
    {
      get { return (string)GetValue(WatermarkProperty); }
      set { SetValue(WatermarkProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty WatermarkProperty =
        DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox), new PropertyMetadata(default(string)));


    private static readonly DependencyPropertyKey RemoveWatermarkPropertyKey = 
      DependencyProperty.RegisterReadOnly("RemoveWatermark", typeof(bool), typeof(WatermarkTextBox), new FrameworkPropertyMetadata((bool)false));

    public static readonly DependencyProperty RemoveWatermarkProperty = RemoveWatermarkPropertyKey.DependencyProperty;
    
    #endregion // DependencyProperties
    static WatermarkTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
      TextProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
    }

    public bool RemoveWatermark
    {
      get { return (bool)GetValue(RemoveWatermarkProperty); }
    }

    static void TextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      WatermarkTextBox watermarkTextBox = (WatermarkTextBox)sender;

      bool textExists = watermarkTextBox.Text.Length > 0;
      if (textExists != watermarkTextBox.RemoveWatermark)
      {
        watermarkTextBox.SetValue(RemoveWatermarkPropertyKey, textExists);
      }
    }
  }
}
