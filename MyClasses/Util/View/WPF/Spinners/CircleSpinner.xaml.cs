using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AMD.Util.View.WPF.Spinners
{
  /// <summary>
  /// Interaction logic for Circles.xaml
  /// </summary>
  public partial class CircleSpinner : UserControl
  {


    public Brush Fill
    {
      get { return (Brush)GetValue(FillProperty); }
      set { SetValue(FillProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register("Fill", typeof(Brush), typeof(CircleSpinner), new PropertyMetadata(Brushes.GhostWhite));



    public CircleSpinner()
    {
      this.DataContext = this;
      InitializeComponent();
    }
  }
}
