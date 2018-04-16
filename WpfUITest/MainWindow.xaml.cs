using AMD.Util.Colour;
using AMD.Util.View.WPF.UserControls;
using System;
using System.Windows;
using System.Windows.Media;

namespace WpfUITest
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      editRibbon.EditRichTextBox = rtb;
    }

    private void ColorPicker_BrushChanged(object sender, BrushChangedEventArgs args)
    {
      return;
      tabColorPicker.Background = args.SelectedBrush;
      tabColorPicker.UpdateLayout();
    }

    private void btnTestTabBackgroundBinding_Click(object sender, RoutedEventArgs e)
    {
      Random r = new Random();
      tvMemory.Background = new SolidColorBrush(Color.FromRgb((byte)r.Next(0xFF), (byte)r.Next(0xFF), (byte)r.Next(0xFF)));
    }

    private void btnColorPickerDialogTest_Click(object sender, RoutedEventArgs e)
    {
      ColorPickerDialog cpd = new ColorPickerDialog();
      cpd.Background = Brushes.Pink;
      cpd.SelectedBrush = tvMemory.Background;
      cpd.SelectedBrushChanged += (s, ev) =>
      {
        SolidColorBrush sb = new SolidColorBrush((ev.SelectedBrush as SolidColorBrush).Color);
        tvMemory.Background = sb;
      };
      cpd.ShowDialog();
      if (cpd.DialogResult == true)
      {
        tvMemory.Background = cpd.SelectedBrush;
      }
    }
  }
}
