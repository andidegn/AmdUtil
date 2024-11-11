using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AMD.Util.View.WPF.UserControls
{

  /// <summary>
  /// Interaction logic for StatusBar.xaml
  /// </summary>
  public partial class StatusBar : UserControl
  {
    public string Status
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
    public string Info
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

    public bool IsIndeterminate
    {
      get
      {
        return sbProgress.IsIndeterminate;
      }
      set
      {
        sbProgress.IsIndeterminate = value;
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
