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
        return statusBar.Background;
      }
      set
      {
        statusBar.Background = value;
      }
    }

    public bool ProgressBarVisible
    {
      get
      {
        return sbProgress.Visibility == Visibility.Visible;
      }
      set
      {
        if (value)
        {
          sbProgress.Visibility = Visibility.Visible;
          Grid.SetColumnSpan(sbiInfo, 1);
        }
        else
        {
          sbProgress.Visibility = Visibility.Hidden;
          Grid.SetColumnSpan(sbiInfo, 2);
        }
      }
    }

    public StatusBar()
    {
      InitializeComponent();
    }
  }
}
