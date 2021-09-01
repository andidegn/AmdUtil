using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
  /// Interaction logic for NumericUpDown.xaml
  /// </summary>
  public partial class NumericUpDown1 : UserControl
  {
    public NumericUpDown1()
    {
      InitializeComponent();
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {

    }
  }
}
