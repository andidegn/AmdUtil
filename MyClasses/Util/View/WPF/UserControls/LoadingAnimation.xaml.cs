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
  /// Interaction logic for LoadingAnimation.xaml
  /// </summary>
  public partial class LoadingAnimation : UserControl
  {


    public String Text
    {
      get { return (String)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(String), typeof(LoadingAnimation), new PropertyMetadata("Loading..."));



    public Brush Foreground
    {
      get { return (Brush)GetValue(ForegroundProperty); }
      set { SetValue(ForegroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Brush), typeof(LoadingAnimation), new PropertyMetadata(Brushes.Black));




    public LoadingAnimation()
    {
      this.DataContext = this;
      InitializeComponent();
    }
  }
}
