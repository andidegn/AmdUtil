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
  /// Interaction logic for StatusBar.xaml
  /// </summary>
  public partial class StatusBar : UserControl
  {
    public String Status
    {
      get
      {
        return sbStatus.Text;
      }
      set
      {
        sbStatus.Text = value;
      }
    }
    public String Info
    {
      get
      {
        return sbInfo.Text;
      }
      set
      {
        sbInfo.Text = value;
      }
    }
    public double Progress
    {
      get
      {
        return sbProgress.Value;
      }
      set
      {
        sbProgress.Value = value;
      }
    }

    public Brush ProgressBrush
    {
      get
      {
        return sbProgress.Foreground;
      }
      set
      {
        sbProgress.Foreground = value;
      }
    }



    public Brush MainBackground
    {
      get
      {
        return (Brush)GetValue(MainBackgroundProperty);
      }
      set
      {
        SetValue(MainBackgroundProperty, value);
      }
    }

    // Using a DependencyProperty as the backing store for MainBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MainBackgroundProperty =
        DependencyProperty.Register("MainBackground", typeof(Brush), typeof(StatusBar), new PropertyMetadata(Brushes.WhiteSmoke));

    public Visibility ProgressBarVisible
    {
      get
      {
        return sbProgress.Visibility;
      }
      set
      {
        if (Visibility.Visible == value)
        {
          Grid.SetColumnSpan(sbInfo, 1);
        }
        else
        {
          Grid.SetColumnSpan(sbInfo, 2);
        }
        sbProgress.Visibility = value;
      }
    }

    public Visibility ResizeThumbVisible
    {
      get
      {
        return resizeThumb.Visibility;
      }
      set
      {
        resizeThumb.Visibility = value;
      }
    }

    public StatusBar()
    {
      InitializeComponent();
      this.DataContext = this;
    }
  }
}
