using AMD.Util.Keyboard;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
		/// The color of the line number and the message text
		/// <note>Not implemented</note>
		/// </summary>
		public SolidColorBrush BrushMessage { get; set; }
		/// <summary>
		/// The color of the result identifier
		/// <note>Not implemented</note>
		/// </summary>
		public SolidColorBrush BrushResult { get; set; }
		/// <summary>
		/// The color of the error and exception identifier
		/// <note>Not implemented</note>
		/// </summary>
		public SolidColorBrush BrushError { get; set; }
		/// <summary>
		/// The color of the timestamp
		/// <note>Not implemented</note>
		/// </summary>
		public SolidColorBrush BrushTimestamp { get; set; }
		/// <summary>
		/// The color of the debug and notification identifier
		/// <note>Not implemented</note>
		/// </summary>
		public SolidColorBrush BrushDebug { get; set; }
		/// <summary>
		/// the color of the warning identifier
		/// <note>Not implemented</note>
		/// </summary>
		public SolidColorBrush BrushWarning { get; set; }
		/// <summary>
		/// The color of the background
		/// </summary>
		public SolidColorBrush BrushBackground { get; set; }
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
		public DebugPanel(Window owner, LogWriter log, double width = Double.NaN)
		{
			this.Owner = owner;
			this.log = log;
			this.DesiredWidth = width == Double.NaN ? 500 : width;
			this.AnimationTime = new TimeSpan(0, 0, 0, 0, 150);

			InitialiseColors();
			InitializeComponent();
			InitialiseLogQueue();
			InitialisePosition();
			InitialiseLog();

			this.DataContext = this;
		}

		#region Initialisers
		private void InitialiseColors()
		{
			BrushMessage = Brushes.White;
			BrushResult = (SolidColorBrush)Application.Current.TryFindResource("ConsoleGreen");
			BrushError = (SolidColorBrush)Application.Current.TryFindResource("ConsoleRed");
			BrushTimestamp = (SolidColorBrush)Application.Current.TryFindResource("ConsoleOrange");
			BrushDebug = (SolidColorBrush)Application.Current.TryFindResource("ConsoleBlue");
			BrushWarning = (SolidColorBrush)Application.Current.TryFindResource("WarningYellow");
			BrushBackground = (SolidColorBrush)Application.Current.TryFindResource("ConsoleBackgroundDark");
		}

		private void InitialiseLogQueue()
		{
			if (logQueue == null)
			{
				logQueue = new ObservableCollection<LogEntry>();
			}
			lvLog.ItemsSource = logQueue;
			CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvLog.ItemsSource);
			view.Filter = o =>
			{
				LogEntry l = o as LogEntry;
				return filter.Contains(l.MessageType);
			};
			view.CurrentChanged += LogQueue_Changed;
			logQueue.CollectionChanged += LogQueue_Changed;
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
			if (busy)
			{
				return;
			}
			DesiredWidth = this.ActualWidth;
			AnimateWindowHide();
			Owner.Focus();
		}

		/// <summary>
		/// Displays the panel by sliding it to the right
		/// </summary>
		public new void Show()
		{
			base.Show();
			//ScrollToBottom();
			AnimateWindowShow();
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
			DoubleAnimation animationShow = new DoubleAnimation(DesiredWidth, AnimationTime);
			animationShow.AccelerationRatio = 0.1;
			animationShow.DecelerationRatio = 0.9;

			this.BeginAnimation(Window.WidthProperty, animationShow);
		}

		private void AnimateWindowHide()
		{
			DoubleAnimation animationHide = new DoubleAnimation(0, AnimationTime);
			animationHide.AccelerationRatio = 0.1;
			animationHide.DecelerationRatio = 0.9;

			animationHide.Completed += (s, e) =>
			{
				this.Visibility = Visibility.Collapsed;
				busy = false;
			};

			this.BeginAnimation(Window.WidthProperty, animationHide);
		}
		#endregion // Animation

		#region Filter
		private void UpdateFilter(bool addFilter, LogMsgType type)
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
			InitialiseLogQueue();
		}
		#endregion // Filter

		#region EventHandlers
		private delegate void dgLogQueueUpdate(LogEntry logEntry);
		private void log_Update(object sender, LogEventArgs e)
		{
			LogEntry logEntry = e.Log;
			Dispatcher.Invoke(new dgLogQueueUpdate(logQueue.Add), logEntry);
			//Dispatcher.Invoke(() => logQueue.Add(logEntry));
		}

		private void LogQueue_Changed(object sender, EventArgs e)
		{
			ScrollToBottom();
		}

		private void tbDebug_Click(object sender, RoutedEventArgs e)
		{
			UpdateFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Debug);
		}

		private void tbNotify_Click(object sender, RoutedEventArgs e)
		{
			UpdateFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Notification);
		}

		private void tbWarning_Click(object sender, RoutedEventArgs e)
		{
			UpdateFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Warning);
		}

		private void tbError_Click(object sender, RoutedEventArgs e)
		{
			UpdateFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Error);
		}

		private void tbException_Click(object sender, RoutedEventArgs e)
		{
			UpdateFilter((sender as ToggleButton).IsChecked == true, LogMsgType.Exception);
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
		#endregion // EventHandlers
	}
}
