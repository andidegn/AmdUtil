using AMD.Util.HID;
using AMD.Util.View.WPF.UserControls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AMD.Util.View.WPF.Spinners
{
  public enum DuckSpinnerCostume
  {
    None,
    Christmas
  }

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

    public DuckSpinnerCostume Custome
    {
      get { return (DuckSpinnerCostume)GetValue(CustomeProperty); }
      set { SetValue(CustomeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Custome.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CustomeProperty =
        DependencyProperty.Register("Custome", typeof(DuckSpinnerCostume), typeof(DuckSpinner), new PropertyMetadata(DuckSpinnerCostume.None));

    private Storyboard aniChristmas;

    public DuckSpinner()
    {
      InitializeComponent();

      aniChristmas = Resources["ChristmasHat"] as Storyboard;

      SelectCustomeBasedOnTime();
    }

    private void SelectCustomeBasedOnTime()
    {
      switch (DateTime.Now)
      {
        case var t when 12 == t.Month:
          Christmas(true);
          break;

        case var t when 11 == t.Month:
          Beard(t.Day / 8 + 1);
          break;

        default:
          break;
      }
    }

    private void Christmas(bool show)
    {
      if (show)
      {
        gridChristmasHat.Visibility = Visibility.Visible;
        BeginStoryboard(aniChristmas);
      }
      else
      {
        gridChristmasHat.Visibility = Visibility.Collapsed;
        aniChristmas.Stop();
      }
    }

    private void Beard(int beardNumber)
    {
      gridBeard.Visibility = Visibility.Visible;
      gridBeard1.Visibility = gridBeard2.Visibility = gridBeard3.Visibility = gridBeard4.Visibility = Visibility.Collapsed;
      switch (beardNumber)
      {
        case 1:
          gridBeard1.Visibility = Visibility.Visible;
          break;

        case 2:
          gridBeard2.Visibility = Visibility.Visible;
          break;

        case 3:
          gridBeard3.Visibility = Visibility.Visible;
          break;

        case 4:
          gridBeard4.Visibility = Visibility.Visible;
          break;

        case 0:
        default:
          gridBeard.Visibility = Visibility.Collapsed;
          break;
      }
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