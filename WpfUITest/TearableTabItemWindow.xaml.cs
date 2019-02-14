using AMD.Util.View.WPF.UserControls;
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
using System.Windows.Shapes;

namespace WpfUITest
{
  /// <summary>
  /// Interaction logic for TearableTabItemWindow.xaml
  /// </summary>
  public partial class TearableTabItemWindow : Window
  {


    public ItemCollection Items
    {
      get
      {
        return this.tearableTabControl.Items;
      }
    }

    public TearableTabItemWindow(int count)
    {
      InitializeComponent();
      this.tearableTabControl.Name = String.Format("ttcTest_{0}", count);
    }
  }
}
