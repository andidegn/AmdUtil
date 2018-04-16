using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for ResizeThumb.xaml
  /// </summary>
  public partial class ResizeThumb : UserControl
  {
    private Window parentWindow;
    private bool moving;
    public ResizeThumb()
    {
      InitializeComponent();
    }

    private void thumbFloatingMenuResize_MouseEnter(object sender, MouseEventArgs e)
    {
      Mouse.OverrideCursor = Cursors.SizeNWSE;
    }

    private void thumbFloatingMenuResize_MouseLeave(object sender, MouseEventArgs e)
    {
      Mouse.OverrideCursor = null;
    }

    private void thumbFloatingMenuResize_DragDelta(object sender, DragDeltaEventArgs e)
    {
      e.Handled = Resize(e.HorizontalChange, e.VerticalChange);
    }

    private void thumbFloatingMenuResize_MouseMove(object sender, MouseEventArgs e)
    {
      if (Mouse.LeftButton == MouseButtonState.Pressed)
      {
        Point mousePosition = Mouse.GetPosition(parentWindow);
        if (mousePosition.X > 0)
        {
          parentWindow.Width = mousePosition.X;
        }
        if (mousePosition.Y > 0)
        {
          parentWindow.Height = mousePosition.Y;
        }
      }
    }

    private bool Resize(double horizontalChange, double verticalChange)
    {
      if (!moving)
      {
        moving = true;
        Point mousePosition = Mouse.GetPosition(parentWindow);
        if (mousePosition.X > 0)
        {
          parentWindow.Width = mousePosition.X;
        }
        if (mousePosition.Y > 0)
        {
          parentWindow.Height = mousePosition.Y;
        }


        //double yAdjust = parentWindow.Width + horizontalChange;
        //double xAdjust = parentWindow.Height + verticalChange;
        //if (yAdjust >= 0 && xAdjust >= 0)
        //{
        //  //if (yAdjust >= 0 && yAdjust >= viewBoxMinWidth && yAdjust <= viewBoxMaxWidth)
        //  //{
        //  parentWindow.Width = yAdjust;
        //  parentWindow.Height = xAdjust;
        //}
        moving = false;
        return true;
      }
      return false;
    }

    private void ResizeThumb_StateChanged(object sender, System.EventArgs e)
    {
      switch (parentWindow.WindowState)
      {
        case WindowState.Normal:
        case WindowState.Minimized:
          this.Visibility = Visibility.Visible;
          break;
        case WindowState.Maximized:
          this.Visibility = Visibility.Collapsed;
          break;
        default:
          break;
      }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        if (parentWindow == null)
        {
          parentWindow = Window.GetWindow(this);
          parentWindow.StateChanged += ResizeThumb_StateChanged;
        }
      }
      catch { }
    }
  }
}
