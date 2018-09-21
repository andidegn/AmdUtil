using AMD.Util.HID;
using AMD.Util.View.WPF.UserControls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

    private void gridWrapper_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (canvasTarget.IsMouseOver)
      {
        Point mousePos = e.GetPosition(canvasTarget);
        BulletHole bh = new BulletHole();
        bh.Width = bh.Height = 10;
        Canvas.SetLeft(bh, mousePos.X - bh.Width / 2);
        Canvas.SetTop(bh, mousePos.Y - bh.Height / 2);
        canvasTarget.Children.Add(bh);
      }
    }

    private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (Modifier.IsAltDown && Modifier.IsCtrlDown && Modifier.IsShiftDown)
      {
        if (canvasTarget.Visibility == Visibility.Visible)
        {
          canvasTarget.Visibility = Visibility.Collapsed;
        }
        else
        {
          canvasTarget.Visibility = Visibility.Visible;
        }
      }
    }
  }
}