using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AMD.Util.View.WPF.Spinners
{
  /// <summary>
  /// Interaction logic for AppleSpinner.xaml
  /// </summary>
  public partial class DuckSpinner : UserControl
  {


    public Brush Fill
    {
      get { return (Brush)GetValue(FillProperty); }
      set { SetValue(FillProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register("Fill", typeof(Brush), typeof(DuckSpinner), new PropertyMetadata(Brushes.Yellow));



    public DuckSpinner()
    {
      this.DataContext = this;
      InitializeComponent();
    }
  }
}
