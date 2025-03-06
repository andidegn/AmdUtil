using AMD.Util.Display;
using AMD.Util.HID;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Windows.ApplicationModel.DataTransfer.DragDrop;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for TitleBar.xaml
  /// </summary>
  public partial class TitleBar : UserControl
  {
    public enum ButtonTypes
    {
      AMD,
      Apple
    }
    private enum WindowsKeyDirection
    {
      Up,
      Down,
      Left,
      Right
    }

    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(TitleBar), new PropertyMetadata("Title"));

    public bool CanSnapToEdge
    {
      get { return (bool)GetValue(CanSnapToEdgeProperty); }
      set { SetValue(CanSnapToEdgeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CanSnapToEdge.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CanSnapToEdgeProperty =
        DependencyProperty.Register("CanSnapToEdge", typeof(bool), typeof(TitleBar), new PropertyMetadata(false));

    public double SnapThreshold
    {
      get { return (double)GetValue(SnapThresholdProperty); }
      set { SetValue(SnapThresholdProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SnapThreshold.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SnapThresholdProperty =
        DependencyProperty.Register("SnapThreshold", typeof(double), typeof(TitleBar), new PropertyMetadata(20d));

    public Brush Background
    {
      get { return (Brush)GetValue(BackgroundProperty); }
      set { SetValue(BackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BackgroundProperty =
        DependencyProperty.Register("Background", typeof(Brush), typeof(TitleBar), new PropertyMetadata(null));


    //public System.Drawing.Rectangle? orgBounds
    //{
    //  get { return (System.Drawing.Rectangle?)GetValue(orgBoundsProperty); }
    //  set { SetValue(orgBoundsProperty, value); }
    //}

    //// Using a DependencyProperty as the backing store for orgBounds.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty orgBoundsProperty =
    //    DependencyProperty.Register("orgBounds", typeof(System.Drawing.Rectangle?), typeof(TitleBar), new PropertyMetadata(null));


    public ImageSource Icon
    {
      get { return imgIcon.Source; }
      set { imgIcon.Source = value; }
    }

    private ButtonTypes buttonType;
    public ButtonTypes ButtonType
    {
      get
      {
        return buttonType;
      }
      set
      {
        buttonType = value;
        switch (value)
        {
          case ButtonTypes.AMD:
            gridAmdBtnExit.Visibility = Visibility.Visible;
            gridAppleBtnExit.Visibility = Visibility.Collapsed;
            break;
          case ButtonTypes.Apple:
            gridAmdBtnExit.Visibility = Visibility.Collapsed;
            gridAppleBtnExit.Visibility = Visibility.Visible;
            break;
        }
      }
    }

    public Button ExitButtonApple
    {
      get
      {
        return btnAppleExit;
      }
    }

    private WindowStyle windowStyle;
    public WindowStyle WindowStyle
    {
      get
      {
        return windowStyle;
      }
      set
      {
        windowStyle = value;

        switch (value)
        {
          case WindowStyle.None:
            gridAmdBtnExit.Visibility = gridAppleBtnExit.Visibility = Visibility.Collapsed;
            break;
          case WindowStyle.SingleBorderWindow:
          case WindowStyle.ThreeDBorderWindow:
            btnAppleExit.Visibility = btnAppleMinimize.Visibility = btnAppleMaximize.Visibility = Visibility.Visible;
            break;
          case WindowStyle.ToolWindow:
            btnAppleMinimize.Visibility = btnAppleMaximize.Visibility = Visibility.Collapsed;
            switch (ButtonType)
            {
              case ButtonTypes.AMD:
                gridAmdBtnExit.Visibility = Visibility.Visible;
                break;
              case ButtonTypes.Apple:
                gridAppleBtnExit.Visibility = Visibility.Visible;
                break;
            }
            break;
        }

        ButtonType = ButtonType;
      }
    }

    public new bool IsMouseDirectlyOver
    {
      get
      {
        return Mouse.DirectlyOver == gridTitleBar || Mouse.DirectlyOver == lblTitle;
      }
    }

    /// <summary>
    /// Depricated, use Background
    /// </summary>
    public Brush MainBackgroundBrush
    {
      get
      {
        return titleBar.Background;
      }
      set
      {
        titleBar.Background = value;
      }
    }

    #region Private Variables
    private Window parentWindow;
    private double mousePositionYStart;
    private ResizeMode defaultResizeMode;
    private bool moving;
    private System.Windows.Forms.Screen currentScreen;
    private System.Drawing.Rectangle? orgBounds;
    private Point dragStartPoint;
    #endregion // Private Variables

    public TitleBar()
    {
      InitializeComponent();
      parentWindow = null;
    }

    private void SnapToEdge(double newLeft, double newTop, double snapThreshold)
    {
      if (currentScreen is null)
      {
        return;
      }

      double screenZeroLeft = currentScreen.WorkingArea.X;
      double screenZeroTop = currentScreen.WorkingArea.Y;
      double screenWidth = currentScreen.WorkingArea.Width;
      double screenHeight = currentScreen.WorkingArea.Height;

      if (Math.Abs(newLeft - screenZeroLeft) < snapThreshold)
      {
        parentWindow.Left = screenZeroLeft;
      }
      else if (Math.Abs((newLeft - screenZeroLeft) + parentWindow.Width - screenWidth) < snapThreshold)
      {
        parentWindow.Left = screenWidth - parentWindow.Width + screenZeroLeft;
      }
      else
      {
        parentWindow.Left = newLeft;
      }

      if (Math.Abs(newTop - screenZeroTop) < snapThreshold)
      {
        parentWindow.Top = screenZeroTop;
      }
      else if (Math.Abs((newTop - screenZeroTop) + parentWindow.Height - screenHeight) < snapThreshold)
      {
        parentWindow.Top = screenHeight - parentWindow.Height + screenZeroTop;
      }
      else
      {
        parentWindow.Top = newTop;
      }
    }

    #region WindowStates
    private void SetWindowStateMaximized()
    {
      if (parentWindow?.ResizeMode > ResizeMode.NoResize)
      {
        parentWindow.WindowState = WindowState.Maximized;
      }
    }

    private void SetWindowStateNormal()
    {
      if (null != orgBounds)
      {
        parentWindow.Left = orgBounds.Value.Left;
        parentWindow.Top = orgBounds.Value.Top;
      }
      parentWindow.WindowState = WindowState.Normal;
      parentWindow.Focus();
      parentWindow.BringIntoView();
    }

    private void SetWindowStateMinimized()
    {
      if (parentWindow?.ResizeMode > ResizeMode.NoResize)
      {
        parentWindow.WindowState = WindowState.Minimized;
      }
    }
    #endregion // WindowStates

    /// <summary>
    /// Eventhandler for progress information
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void ExitHandler(object sender, RoutedEventArgs args);
    public event ExitHandler Exit;

    private void ExecuteExit(RoutedEventArgs e)
    {
      Exit?.Invoke(this, e);
    }

    /// <summary>
    /// Eventhandler for progress information
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void MinimizeHandler(object sender, RoutedEventArgs args);
    public event MinimizeHandler Minimize;

    private void ExecuteMinimize(RoutedEventArgs e)
    {
      Minimize?.Invoke(this, e);
    }

    /// <summary>
    /// Eventhandler for progress information
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void MaximizeHandler(object sender, RoutedEventArgs args);
    public event MaximizeHandler Maximize;

    private void ExecuteMaximize(RoutedEventArgs e)
    {
      Maximize?.Invoke(this, e);
    }

    /// <summary>
    /// Eventhandler for progress information
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void TitleBarMouseLeftDownHandler(object sender, RoutedEventArgs args);
    public event TitleBarMouseLeftDownHandler TitleBarMouseLeftDown;

    private void ExecuteTitleBarLeftDown(RoutedEventArgs e)
    {
      TitleBarMouseLeftDown?.Invoke(this, e);
    }

    /// <summary>
    /// Eventhandler for progress information
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void TitleBarMouseLeftUpHandler(object sender, RoutedEventArgs args);
    public event TitleBarMouseLeftUpHandler TitleBarMouseLeftUp;

    private void TitleBarLeftUp(RoutedEventArgs e)
    {
      TitleBarMouseLeftUp?.Invoke(this, e);
    }

    private void Exit_EventHandler(object sender, RoutedEventArgs e)
    {
      if (Exit != null)
      {
        ExecuteExit(e);
      }
      else
      {
        Application.Current.Shutdown();
      }
    }

    private void Minimize_EventHandler(object sender, RoutedEventArgs e)
    {
      if (Minimize != null)
      {
        ExecuteMinimize(e);
      }
      else
      {
        Window.GetWindow(this).WindowState = WindowState.Minimized;
      }
    }

    private void Maximize_EventHandler(object sender, RoutedEventArgs e)
    {
      if (Maximize != null)
      {
        ExecuteMaximize(e);
      }
      else
      {
        if (parentWindow.WindowState == WindowState.Normal)
        {
          SetWindowStateMaximized();
        }
        else
        {
          SetWindowStateNormal();
        }
      }
    }

    private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (TitleBarMouseLeftDown != null)
      {
        ExecuteTitleBarLeftDown(e);
      }
      else
      {
        mousePositionYStart = parentWindow.WindowState == WindowState.Maximized ? MouseUtil.GetMousePosition().Y : double.NaN;
        if (e.ClickCount == 1)
        {
          if (Mouse.LeftButton == MouseButtonState.Pressed)
          {
            try
            {
              if (WindowState.Maximized != parentWindow.WindowState)
              {
                orgBounds = new System.Drawing.Rectangle((int)parentWindow.Left, (int)parentWindow.Top, (int)parentWindow.Width, (int)parentWindow.Height);
              }
              dragStartPoint = e.GetPosition(null);
              gridTitleBar.CaptureMouse();
              moving = true;
            }
            catch (Exception ex)
            {
              LogWriter.Instance.WriteToLog(ex, "Error dragging windows");
            }
          }
        }
        else if (IsMouseDirectlyOver)
        {
          Maximize_EventHandler(sender, e);
        }
      }
    }

    private void gridTitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (TitleBarMouseLeftUp != null)
      {
        TitleBarLeftUp(e);
      }
      else
      {
        mousePositionYStart = double.NaN;
      }
      if (moving && parentWindow.WindowState == WindowState.Normal && null != currentScreen)
      {
        Point mousePosition = MouseUtil.GetMousePosition();
        if (mousePosition.Y <= currentScreen.Bounds.Y + this.ActualHeight / 8)
        {
          System.Windows.Forms.Screen containedScreen = ScreenUtil.GetContainedScreen(parentWindow.Left + parentWindow.ActualWidth / 2, parentWindow.Top);
          if (!currentScreen.Equals(containedScreen))
          {
            parentWindow.Left = currentScreen.Bounds.Left;
            parentWindow.Top = currentScreen.Bounds.Top;
          }
          parentWindow.WindowState = WindowState.Maximized;
        }
        moving = false;
      }
      gridTitleBar.ReleaseMouseCapture();
    }

    private void UserControl_MouseMove(object sender, MouseEventArgs e)
    {
      if (Mouse.LeftButton == MouseButtonState.Pressed && moving)
      {
        Point mousePosition = MouseUtil.GetMousePosition();
        double newLeft = mousePosition.X - dragStartPoint.X;
        double newTop = mousePosition.Y - dragStartPoint.Y;

        if (mousePositionYStart != double.NaN && parentWindow.WindowState == WindowState.Maximized)
        {
          double delta = mousePositionYStart - mousePosition.Y;
          if (delta < -5)
          {
            Maximize_EventHandler(sender, e);
            parentWindow.Left = mousePosition.X - this.ActualWidth / 2;
            parentWindow.Top = mousePosition.Y - titleBar.ActualHeight / 2;
            dragStartPoint = new Point(titleBar.ActualWidth / 2, titleBar.ActualHeight / 2);
            gridTitleBar.CaptureMouse();
          }
        }
        else if (CanSnapToEdge && null != currentScreen)
        {
          SnapToEdge(newLeft, newTop, SnapThreshold);
        }
        else
        {
          parentWindow.Left = newLeft;
          parentWindow.Top = newTop;
        }
      }
    }

    private void ParentWindow_LocationChanged(object sender, EventArgs e)
    {
      Point mousePos = new Point();

      if (Mouse.LeftButton == MouseButtonState.Pressed && IsMouseDirectlyOver)
      {
        mousePos = MouseUtil.GetMousePosition();
      }

      if (null != parentWindow)
      {
        if (!mousePos.Equals(new Point(0, 0))) // Skipping point 0,0 as this for some reason is the mouse position when mouse is released at the top of screen when dragging window
        {
          System.Windows.Forms.Screen screen;
          if (moving)
          {
            screen = ScreenUtil.GetContainedScreen(mousePos.X, parentWindow.Top);
          }
          else
          {
            screen = ScreenUtil.GetContainedScreen(parentWindow.Left + parentWindow.ActualWidth / 2, parentWindow.Top);
          }

          if (false == screen?.Equals(currentScreen))
          {
            // This is formatted this way due to characters in screens ToString which breaks formatting
            //LogWriter.Instance.PrintDebug("screen", "Changing screen. screen: {0} - newScreen: {1}", screen, currentScreen);
            currentScreen = screen;
            parentWindow.MaxHeight = currentScreen.WorkingArea.Height;
          }
        }
      }
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        if (parentWindow is null)
        {
          parentWindow = Window.GetWindow(this);
        }
        if (Background is null)
        {
          Background = parentWindow.TryFindResource("TitleBarBackground") as LinearGradientBrush;
        }
        parentWindow.LocationChanged += ParentWindow_LocationChanged;
        parentWindow.StateChanged += ParentWindow_StateChanged;

        if (!ScreenUtil.IsWithinScreenArea(parentWindow.Left, parentWindow.Top))
        {
          parentWindow.Left = 100;
          parentWindow.Top = 100;
        }

        currentScreen = ScreenUtil.GetContainedScreen(parentWindow.Left + parentWindow.ActualWidth / 2, parentWindow.Top);
        if (null != currentScreen)
        {
          parentWindow.MaxHeight = currentScreen.WorkingArea.Height;
        }
      }
      catch { }
    }

    private ResizeMode originalResizeMode;
    private bool stateChangeInProgress = false;
    private void ParentWindow_StateChanged(object sender, EventArgs e)
    {
      if (!stateChangeInProgress)
      {
        stateChangeInProgress = true;
        switch (parentWindow?.WindowState)
        {
          case WindowState.Normal:
            parentWindow.ResizeMode = originalResizeMode;
            break;

          case WindowState.Maximized:
            parentWindow.WindowState = WindowState.Minimized;
            originalResizeMode = parentWindow.ResizeMode;
            parentWindow.ResizeMode = ResizeMode.NoResize;
            parentWindow.WindowState = WindowState.Maximized;
            break;

          case WindowState.Minimized:
            break;

          default:
            break;
        }
        stateChangeInProgress = false;
      }
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
      if (parentWindow != null)
      {
        parentWindow.LocationChanged -= ParentWindow_LocationChanged;
      }
    }








    private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      switch (msg)
      {
        case 0x0024:
          WmGetMinMaxInfo(hwnd, lParam);
          handled = true;
          break;
      }
      return (IntPtr)0;
    }

    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
      MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
      int MONITOR_DEFAULTTONEAREST = 0x00000002;
      IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
      if (monitor != IntPtr.Zero)
      {
        MONITORINFO monitorInfo = new MONITORINFO();
        GetMonitorInfo(monitor, monitorInfo);
        RECT rcWorkArea = monitorInfo.rcWork;
        RECT rcMonitorArea = monitorInfo.rcMonitor;
        mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
        mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
        mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
        mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
      }
      Marshal.StructureToPtr(mmi, lParam, true);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
      /// <summary>x coordinate of point.</summary>
      public int x;
      /// <summary>y coordinate of point.</summary>
      public int y;
      /// <summary>Construct a point of coordinates (x,y).</summary>
      public POINT(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
      public POINT ptReserved;
      public POINT ptMaxSize;
      public POINT ptMaxPosition;
      public POINT ptMinTrackSize;
      public POINT ptMaxTrackSize;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
      public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
      public RECT rcMonitor = new RECT();
      public RECT rcWork = new RECT();
      public int dwFlags = 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct RECT
    {
      public int left;
      public int top;
      public int right;
      public int bottom;
      public static readonly RECT Empty = new RECT();
      public int Width { get { return Math.Abs(right - left); } }
      public int Height { get { return bottom - top; } }
      public RECT(int left, int top, int right, int bottom)
      {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
      }
      public RECT(RECT rcSrc)
      {
        left = rcSrc.left;
        top = rcSrc.top;
        right = rcSrc.right;
        bottom = rcSrc.bottom;
      }
      public bool IsEmpty { get { return left >= right || top >= bottom; } }
      public override string ToString()
      {
        if (this == Empty) { return "RECT {Empty}"; }
        return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
      }
      public override bool Equals(object obj)
      {
        if (!(obj is Rect)) { return false; }
        return (this == (RECT)obj);
      }
      /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
      public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
      /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
      public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
      /// <summary> Determine if 2 RECT are different(deep compare)</summary>
      public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
    }

    [DllImport("user32")]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

    [DllImport("User32")]
    internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
  }
}
