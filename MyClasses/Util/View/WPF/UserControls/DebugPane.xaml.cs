using AMD.Util.Log;
using AMD.Util.View.WPF.Helper;
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
using System.Windows.Threading;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for DebugPanel.xaml
  /// </summary>
  public partial class DebugPane : UserControl, IDisposable
	{
		#region Public properties
		/// <summary>
		/// The color of the line number and the messages
		/// </summary>
		public SolidColorBrush BrushMessage { get; set; }
		/// <summary>
		/// The color of the background
		/// </summary>
		public SolidColorBrush BrushBackground { get; set; }

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
		#endregion // Public properties
    
		private LogWriter log;
		private ObservableCollection<LogEntry> logQueue;
		private List<LogMsgType> filter;

    public DebugPane()
    {
      InitialiseColors();
      InitializeComponent();
      InitialiseLogQueue();

      this.DataContext = this;

      SetLvLogItemsSource();

      UpdateFilter();
    }

		public void Initialise(LogWriter log)
		{
			this.log = log;
      InitialiseLog();
    }

		#region Initialisers
		private void InitialiseColors()
		{
			BrushMessage = Brushes.White;
			BrushBackground = (SolidColorBrush)TryFindResource("ConsoleBackgroundDark");
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

		public void ScrollToBottom()
		{
      ScrollViewer svLvLog = VisualHelper.GetChildDependencyObjectFromVisualTree(lvLog, typeof(ScrollViewer)) as ScrollViewer;
      if (null != svLvLog)
      {
        svLvLog.ScrollToEnd();
      }
   //   int numOfEntries = lvLog.Items.Count;
			//if (numOfEntries > 0)
			//{
			//	lvLog.ScrollIntoView(lvLog.Items[numOfEntries - 1]);
			//}
		}

		private void InitialiseLog()
		{
      if (log == null)
      {
        log = LogWriter.Instance;
      }
			filter = new List<LogMsgType>(Enum.GetValues(typeof(LogMsgType)).Cast<LogMsgType>());
			log.OnLogEntry += log_Update;
		}
		#endregion // Initialisers

		/// <summary>
		/// Disposes the log eventhandler
		/// </summary>
		public void Dispose()
		{
			log.OnLogEntry -= log_Update;
		}

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

		public void UpdateFilter()
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

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      ScrollToBottom();
    }

    private void log_Update(object sender, LogEventArgs e)
		{
      Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => { logQueue.Add(e.Log); }));
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
			Clipboard.SetDataObject(sb.ToString());
		}
    #endregion // EventHandlers

    private void miShowStackTrace_Click(object sender, RoutedEventArgs e)
    {
      LogEntry le = lvLog.SelectedItem as LogEntry;
      MessageBox.Show(le.StackTrace.ToString());
      // ToDo: Make a visualization of the stacktrace from the log
    }
  }
}
