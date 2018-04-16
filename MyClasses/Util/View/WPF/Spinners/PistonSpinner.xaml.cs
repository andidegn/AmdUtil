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

namespace AMD.Util.View.WPF.Spinners
{
  /// <summary>
  /// Interaction logic for PistonSpinner.xaml
  /// </summary>
  public partial class PistonSpinner : UserControl
  {


    public Brush Fill
    {
      get { return (Brush)GetValue(FillProperty); }
      set { SetValue(FillProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register("Fill", typeof(Brush), typeof(PistonSpinner), new PropertyMetadata(Brushes.GhostWhite));


    public PistonSpinner()
    {
      this.DataContext = this;
      InitializeComponent();
    }
  }
}
