using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for RichTextBoxRibbon.xaml
  /// </summary>
  public partial class RichTextBoxRibbon : UserControl
  {


    public RichTextBox EditRichTextBox
    {
      get { return (RichTextBox)GetValue(EditRichTextBoxProperty); }
      set
      {
        value.SelectionChanged += EditRichTextBox_SelectionChanged;
        SetValue(EditRichTextBoxProperty, value);
      }
    }

    // Using a DependencyProperty as the backing store for EditRichTextBox.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EditRichTextBoxProperty =
        DependencyProperty.Register("EditRichTextBox", typeof(RichTextBox), typeof(RichTextBoxRibbon), new PropertyMetadata(null));


    public RichTextBoxRibbon()
    {
      InitializeComponent();
      cbbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
      cbbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
    }
    public void EditRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
      RichTextBox rtb = sender as RichTextBox;
      if (rtb != null)
      {
        object temp = rtb.Selection.GetPropertyValue(Inline.FontWeightProperty);
        btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
        temp = rtb.Selection.GetPropertyValue(Inline.FontStyleProperty);
        btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
        temp = rtb.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
        btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

        temp = rtb.Selection.GetPropertyValue(Inline.FontFamilyProperty);
        cbbFontFamily.SelectedItem = temp;
        temp = rtb.Selection.GetPropertyValue(Inline.FontSizeProperty);
        cbbFontSize.Text = temp.ToString();
      }
    }

    private void cbbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cbbFontFamily.SelectedItem != null)
      {
        if (EditRichTextBox.Selection.Text.Length > 0)
        {
          EditRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cbbFontFamily.SelectedItem);
        }
        else
        {
          TextRange tr = new TextRange(EditRichTextBox.Selection.Start, EditRichTextBox.Selection.End);
          tr.ApplyPropertyValue(Inline.FontFamilyProperty, cbbFontFamily.SelectedItem);
          //EditRichTextBox.FontFamily = cbbFontFamily.SelectedItem as FontFamily;
        }
        EditRichTextBox.Focus();
      }
    }

    private void cbbFontSize_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (EditRichTextBox.Selection.Text.Length > 0)
      {
        EditRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cbbFontSize.Text);
      }
      else
      {
        TextRange tr = new TextRange(EditRichTextBox.Selection.Start, EditRichTextBox.Selection.End);
        tr.ApplyPropertyValue(Inline.FontSizeProperty, cbbFontSize.Text);
        //EditRichTextBox.FontFamily = cbbFontFamily.SelectedItem as FontFamily;
      }
      EditRichTextBox.Focus();
    }
  }
}
