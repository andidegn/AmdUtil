using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace AMD.Util.View.WPF.UserControls
{
  //public enum DisplayMode
  //{
  //  Slider,
  //  Window
  //}
  /// <summary>
  /// Interaction logic for DebugPanel.xaml
  /// </summary>
  public partial class DebugPanel1 : Window, IDisposable
	{
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
		public SolidColorBrush BrushMessage { get; set; }
		/// <summary>
		/// The color of the background
		/// </summary>
		public SolidColorBrush BrushBackground { get; set; }
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
				lvLog.FontFamily = value;
			}
		}

		public int LogFontSize
		{
			set
			{
				lvLog.FontSize = value;
			}
		}

    /// <summary>
    /// Determines which way the DebugPanel is displayed. Either as a 
    /// slider from "behind" the main window or as a seperate window
    /// </summary>
    //public DisplayMode DisplayMode { get; set; }
		#endregion // Public properties

		private bool busy;
		private LogWriter log;
		private ObservableCollection<LogEntry> logQueue;
		private List<LogMsgType> filter;


		/// <summary>
		/// This class attaches a debug panel to the right side of the owner application which enables the option to have a simultaneous display of the LogWriter log
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="log"></param>
		/// <param name="width"></param>
		public DebugPanel1(Window owner, LogWriter log, double width = Double.NaN)
		{
			if (owner != null)
			{
				this.Owner = owner;
			}
			this.log = log;
			this.DesiredWidth = width == Double.NaN ? 500 : width;
			this.AnimationTime = new TimeSpan(0, 0, 0, 0, 250);

			InitialiseColors();
			InitializeComponent();
			InitialiseLogQueue();
			//InitialisePosition();
			InitialiseLog();

			this.DataContext = this;
		}

		#region Initialisers
		private void InitialiseColors()
		{
			BrushMessage = Brushes.White;
			BrushBackground = (SolidColorBrush)Application.Current.TryFindResource("ConsoleBackgroundDark");
		}

		private void InitialiseLogQueue()
		{
			if (logQueue == null)
			{
				logQueue = new ObservableCollection<LogEntry>();
			}
		}

		private void SetLvLogItemsSource()
		{
			if (lvLog.ItemsSource == null)
			{
				lvLog.ItemsSource = logQueue;
			}
		}

		private void ScrollToBottom()
		{
			int numOfEntries = lvLog.Items.Count;
			if (numOfEntries > 0)
			{
				lvLog.ScrollIntoView(lvLog.Items[numOfEntries - 1]);
			}
		}

		private void InitialiseLog()
		{
			filter = new List<LogMsgType>(Enum.GetValues(typeof(LogMsgType)).Cast<LogMsgType>());
			log.OnLogEntry += log_Update;
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
      //switch (DisplayMode)
      //{
      //  case DisplayMode.Slider:
          if (busy)
          {
            return;
          }
          lvLog.ItemsSource = null;
          DesiredWidth = this.ActualWidth;
          AnimateWindowHide();
          Owner.Focus();
      //    break;
      //  case DisplayMode.Window:
      //    this.Visibility = Visibility.Hidden;
      //    break;
      //  default:
      //    break;
      //}
		}

		/// <summary>
		/// Displays the panel by sliding it to the right
		/// </summary>
		public new void Show()
		{
      //switch (DisplayMode)
      //{
      //  case DisplayMode.Slider:
          if (busy)
          {
            return;
          }
          InitialisePosition();
          SetLvLogItemsSource();
          UpdateFilter();
          base.Show();
          //ScrollToBottom();
          AnimateWindowShow();
      //    break;
      //  case DisplayMode.Window:
      //    this.Visibility = Visibility.Visible;
      //    break;
      //  default:
      //    break;
      //}
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
			log.OnLogEntry -= log_Update;
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

		#region Filter
		private void SetFilter(bool addFilter, LogMsgType type)
		{
			if (addFilter == true)
			{
				if (!filter.Contains(type))
				{
					filter.Add(type);
				}
			}
			else
			{
				filter.Remove(type);
			}
			UpdateFilter();
		}

		private void UpdateFilter()
		{
			CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvLog.ItemsSource);
			view.Filter = o =>
			{
				LogEntry l = o as LogEntry;
				return filter.Contains(l.MessageType);
			};
			view.CurrentChanged += LogQueue_Changed;
			logQueue.CollectionChanged += LogQueue_Changed;
		}
		#endregion // Filter

		#region EventHandlers
		private delegate void dgLogQueueUpdate(LogEntry logEntry);
		private void log_Update(object sender, LogEventArgs e)
		{
			LogEntry logEntry = e.Log;
			Dispatcher.BeginInvoke(DispatcherPriority.Background, new dgLogQueueUpdate(logQueue.Add), logEntry);
		}

		private void LogQueue_Changed(object sender, EventArgs e)
		{
			ScrollToBottom();
		}

		private void tbDebug_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Debug);
		}

		private void tbNotify_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Notification);
		}

		private void tbSerial_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Serial);
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Rx);
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Tx);
		}

		private void tbTest_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Test);
		}

		private void tbMeasurement_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Measurement);
		}

		private void tbResult_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Result);
		}

		private void tbWarning_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Warning);
		}

		private void tbError_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Error);
		}

		private void tbException_Click(object sender, RoutedEventArgs e)
		{
			SetFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Exception);
		}

		private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			foreach (LogEntry item in (sender as ListView).SelectedItems)
			{
				sb.AppendLine(item.ToString());
			}
			Clipboard.SetText(sb.ToString());
		}

		private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Hide();
		}

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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//AnimateWindowShow();
			//AnimateWindowHide();
		}
    #endregion // EventHandlers

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      //if (DisplayMode == DisplayMode.Window)
      //{
      //  DragMove();
      //}
    }
  }
}
