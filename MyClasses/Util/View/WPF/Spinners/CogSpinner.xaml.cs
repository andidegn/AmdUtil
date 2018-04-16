using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AMD.Util.View.WPF.Spinners
{
  /// <summary>
  /// Interaction logic for CogSpinner.xaml
  /// </summary>
  public partial class CogSpinner : UserControl
  {


    public Brush Fill
    {
      get { return (Brush)GetValue(FillProperty); }
      set { SetValue(FillProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register("Fill", typeof(Brush), typeof(CogSpinner), new PropertyMetadata(Brushes.GhostWhite));



    public CogSpinner()
    {
      this.DataContext = this;
      InitializeComponent();
    }
  }
}
