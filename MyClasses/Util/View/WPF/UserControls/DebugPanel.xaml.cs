using AMD.Util.Log;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for DebugPanel.xaml
  /// </summary>
  public partial class DebugPanel : Window, IDisposable
  {
    private bool busy;

    #region Public properties
    /// <summary>
    /// Defines the animation time of the slide in/out
    /// </summary>
    public TimeSpan AnimationTime { get; set; }
    /// <summary>
    /// Sets the animation goto width
    /// </summary>
    public double DesiredWidth { get; set; }
    /// <summary>
    /// The color of the line number and the messages
    /// </summary>
    public SolidColorBrush BrushMessage
    {
      get
      {
        return DebugPane.BrushMessage;
      }
      set
      {
        DebugPane.BrushMessage = value;
      }
    }
    /// <summary>
    /// The color of the background
    /// </summary>
    public SolidColorBrush BrushBackground
    {
      get
      {
        return DebugPane.BrushBackground;
      }
      set
      {
        DebugPane.BrushBackground = value;
      }
    }

    public Window Owner
    {
      get
      {
        return base.Owner;
      }
      set
      {
        base.Owner = value;
      }
    }

    public FontFamily LogFontFamily
    {
      set
      {
        DebugPane.FontFamily = value;
      }
    }

    public int LogFontSize
    {
      set
      {
        DebugPane.FontSize = value;
      }
    }
    #endregion // Public properties


    /// <summary>
    /// This class attaches a debug panel to the right side of the owner application which enables the option to have a simultaneous display of the LogWriter log
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="log"></param>
    /// <param name="width"></param>
    public DebugPanel(Window owner, LogWriter log, double width = Double.NaN)
    {
      if (owner != null)
      {
        this.Owner = owner;
      }

      InitializeComponent();

      InitialiseLog(log);

      this.DataContext = this;

      this.AnimationTime = new TimeSpan(0, 0, 0, 0, 250);

      this.DesiredWidth = width == Double.NaN ? 500 : width;
		}

		#region Initialisers
    public void InitialiseLog(LogWriter log)
    {
      if (null != log)
      {
        DebugPane.Initialise(log);
      }
    }

		private void InitialisePosition()
		{
			AdjustPosition();
			AdjustHeight(Owner.Height);

			Owner.LocationChanged += (s, e) =>
			{
				AdjustPosition();
			};

			Owner.SizeChanged += (s, e) =>
			{
				AdjustHeight(e.NewSize.Height);
				AdjustPosition();
			};
		}
		#endregion // Initialisers

		#region Panel Adjustments
		private void AdjustPosition()
		{
			this.Left = Owner.Left + Owner.Width;
			this.Top = Owner.Top + Owner.Height * 10 / 100;
		}

		private void AdjustHeight(double parentHeight)
		{
			this.Height = this.MaxHeight = this.MinHeight = parentHeight * 80 / 100;
		}

    /// <summary>
    /// Hides the panel by sliding it to the left
    /// </summary>
    public new void Hide()
    {
      if (busy)
      {
        return;
      }
      //lvLog.ItemsSource = null;
      DesiredWidth = this.ActualWidth;
      AnimateWindowHide();
      Owner.Focus();
    }

    /// <summary>
    /// Displays the panel by sliding it to the right
    /// </summary>
    public new void Show()
    {
      if (busy)
      {
        return;
      }
      InitialisePosition();
      //SetLvLogItemsSource();
      DebugPane.UpdateFilter();
      base.Show();
      DebugPane.ScrollToBottom();
      AnimateWindowShow();
    }

    public new void Toggle()
    {
      if (this.Visibility == Visibility.Visible)
      {
        Hide();
      }
      else
      {
        Show();
      }
    }

		/// <summary>
		/// Disposes the log eventhandler
		/// </summary>
		public void Dispose()
		{
      DebugPane.Dispose();
		}
		#endregion // Panel Adjustments

		#region Animation
		private void AnimateWindowShow()
		{
			busy = true;
			DoubleAnimation animationShow = new DoubleAnimation(DesiredWidth, AnimationTime);
			animationShow.AccelerationRatio = 0.1;
			animationShow.DecelerationRatio = 0.9;

			animationShow.Completed += (s, e) =>
			{
				//try
				//{
				//	this.ResizeMode = ResizeMode.CanResizeWithGrip;
				//}
				//catch (Exception)
				//{

				//	throw;
				//}
				this.BeginAnimation(Window.WidthProperty, null);
				busy = false;
			};

			this.BeginAnimation(Window.WidthProperty, animationShow);
		}

		private void AnimateWindowHide()
		{
			busy = true;
			//this.ResizeMode = ResizeMode.NoResize;
			DoubleAnimation animationHide = new DoubleAnimation(0, AnimationTime);
			animationHide.AccelerationRatio = 0.1;
			animationHide.DecelerationRatio = 0.9;

			animationHide.Completed += (s, e) =>
			{
				this.Visibility = Visibility.Collapsed;
				this.BeginAnimation(Window.WidthProperty, null);
				busy = false;
			};

			this.BeginAnimation(Window.WidthProperty, animationHide);
		}
    #endregion // Animation

    #region EventHandlers
    private void Thumb_MouseEnter(object sender, MouseEventArgs e)
    {
      System.Windows.Input.Mouse.OverrideCursor = Cursors.SizeWE;
    }

    private void Thumb_MouseLeave(object sender, MouseEventArgs e)
    {
      System.Windows.Input.Mouse.OverrideCursor = null;
    }

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      double yAdjust = this.Width + e.HorizontalChange;
      if (yAdjust >= 0)// && yAdjust >= viewBoxMinWidth && yAdjust <= viewBoxMaxWidth)
      {
        this.Width = DesiredWidth = yAdjust;
      }
    }
    #endregion // EventHandlers
  }
}
