using AMD.Util.Log;
using AMD.Util.View.WPF.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
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
    public SolidColorBrush BrushMessage
    {
      get { return (SolidColorBrush)GetValue(BrushMessageProperty); }
      set { SetValue(BrushMessageProperty, value); }
    }

    // Using a DependencyProperty as the backing store for BrushMessage.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BrushMessageProperty =
        DependencyProperty.Register("BrushMessage", typeof(SolidColorBrush), typeof(DebugPane), new PropertyMetadata(default(SolidColorBrush)));

    /// <summary>
    /// The color of the background
    /// </summary>
    public SolidColorBrush BrushBackground
    {
      get { return (SolidColorBrush)GetValue(BrushBackgroundProperty); }
      set { SetValue(BrushBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for BrushBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BrushBackgroundProperty =
				DependencyProperty.Register("BrushBackground", typeof(SolidColorBrush), typeof(DebugPane), new PropertyMetadata(default(SolidColorBrush)));

		public LogFilterCollection Filter
		{
			get { return (LogFilterCollection)GetValue(FilterProperty); }
			private set { SetValue(FilterProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FilterProperty =
				DependencyProperty.Register("Filter", typeof(LogFilterCollection), typeof(DebugPane), new PropertyMetadata(default(LogFilterCollection)));





		public bool Enable
		{
			get { return (bool)GetValue(EnableProperty); }
			set { SetValue(EnableProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Enable.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty EnableProperty =
				DependencyProperty.Register("Enable", typeof(bool), typeof(DebugPane), new PropertyMetadata(true, OnEnableValueChanged));

    private static void OnEnableValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
		{
			if (element is DebugPane dp)
			{
				if (dp.Enable)
				{
					foreach (var log in dp.logs)
          {
            log.OnLogEntry += dp.log_Update;
          }
        }
				else
        {
          foreach (var log in dp.logs)
          {
            log.OnLogEntry -= dp.log_Update;
          }
        }
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

    private Window owner;
    public Window Owner
    {
      get
      {
        if (owner is null)
        {
          owner = Window.GetWindow(this);
        }
        return owner;
      }
    }
    #endregion // Public properties

    private ObservableCollection<LogWriter> logs;
		private ObservableCollection<LogEntry> logQueue;

    public DebugPane()
    {
      InitialiseColors();
      InitializeComponent();
      InitialiseLogQueue();

      this.DataContext = this;

      SetLvLogItemsSource();

      UpdateFilter();
    }

		public void Initialise(LogWriter log, LogFilterCollection customFilter = null)
		{
			if (log is null && 0 == logs.Count)
			{
				log = LogWriter.Instance;
      }

			if (null != log)
      {
        logs.Add(log);
        log.OnLogEntry += log_Update;
      }

			if (null != customFilter)
			{
				Filter = customFilter;
				foreach (var item in Filter)
				{
					bool isSelected = item.Value;
					switch (item.Key)
					{
						case LogMsgType.Debug:
							tbDebug.IsChecked = isSelected;
							break;
						case LogMsgType.Error:
              tbError.IsChecked = isSelected;
              break;
						case LogMsgType.Exception:
              tbException.IsChecked = isSelected;
              break;
						case LogMsgType.Warning:
              tbWarning.IsChecked = isSelected;
              break;
						case LogMsgType.Notification:
              tbNotify.IsChecked = isSelected;
              break;
						case LogMsgType.Serial:
						case LogMsgType.Rx:
						case LogMsgType.Tx:
              tbSerial.IsChecked = isSelected;
              break;
						case LogMsgType.Result:
              tbResult.IsChecked = isSelected;
              break;
						case LogMsgType.Test:
              tbTest.IsChecked = isSelected;
              break;
						case LogMsgType.Measurement:
              tbMeasurement.IsChecked = isSelected;
              break;
						case LogMsgType.Assert:
              tbAssert.IsChecked = isSelected;
              break;
						default:
							break;
					}
				}
				UpdateFilter();
			}
    }

		#region Initialisers
		private void InitialiseColors()
		{
			BrushMessage = Brushes.White;
			BrushBackground = (SolidColorBrush)TryFindResource("ConsoleBackgroundDark");
		}

		private void InitialiseLogQueue()
		{
			if (logs is null)
			{
				logs = new ObservableCollection<LogWriter>();
			}
			if (logQueue is null)
			{
				logQueue = new ObservableCollection<LogEntry>();
      }
			if (Filter is null)
      {
				Filter = new LogFilterCollection();
				foreach (var item in Enum.GetValues(typeof(LogMsgType)).Cast<LogMsgType>())
				{
					Filter[item] = true;
				}
      }
    }

		private void SetLvLogItemsSource()
		{
			if (lvLog.ItemsSource == null)
			{
				lvLog.ItemsSource = logQueue;
			}
    }
    #endregion // Initialisers

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

		public void Clear()
		{
			if (!Dispatcher.Thread.Equals(Thread.CurrentThread))
			{
				Dispatcher.Invoke(() => Clear());
				return;
			}
			logQueue.Clear();
		}

    /// <summary>
    /// Disposes the log eventhandler
    /// </summary>
    public void Dispose()
    {
      foreach (LogWriter log in logs)
      {
        log.OnLogEntry -= log_Update;
      }
    }

		#region Filter
		private void SetFilter(bool addFilter, LogMsgType type)
		{
			Filter[type] = addFilter;
			UpdateFilter();
    }

    private void SetFilterFromToggleButton(object sender, LogMsgType type)
    {
      if (sender is ToggleButton tb)
      {
        SetFilter(true == tb.IsChecked, type);
      }
    }

    public void UpdateFilter()
    {
      CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvLog.ItemsSource);
			view.Filter = o =>
			{
				LogEntry l = o as LogEntry;
				return Filter[l.MessageType];
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
      SetFilterFromToggleButton(sender, LogMsgType.Debug);
		}

		private void tbNotify_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Notification);
    }

		private void tbSerial_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Serial);
      SetFilterFromToggleButton(sender, LogMsgType.Rx);
      SetFilterFromToggleButton(sender, LogMsgType.Tx);
		}

		private void tbTest_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Test);
    }

    private void tbAssert_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Assert);
    }

    private void tbMeasurement_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Measurement);
    }

		private void tbResult_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Result);
    }

		private void tbWarning_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Warning);
    }

		private void tbError_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Error);
    }

		private void tbException_Click(object sender, RoutedEventArgs e)
    {
      SetFilterFromToggleButton(sender, LogMsgType.Exception);
    }

		private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			foreach (LogEntry item in (sender as ListView).SelectedItems)
			{
				sb.AppendLine(item.ToString());
      }
      try
      {
        Clipboard.SetDataObject(sb.ToString(), true);
      }
      catch { }
    }
    #endregion // EventHandlers

    private void miShowStackTrace_Click(object sender, RoutedEventArgs e)
    {
      LogEntry le = lvLog.SelectedItem as LogEntry;
      AMDMessageBox.SizeToContentOnNextShow = true;
      AMDMessageBox.Show(Owner, le.StackTrace.ToString(), "Stack Trace");
      // ToDo: Make a visualization of the stacktrace from the log
    }

    private void miShowCaller_Click(object sender, RoutedEventArgs e)
    {
      LogEntry le = lvLog.SelectedItem as LogEntry;
      AMDMessageBox.Show(Owner, le.Caller, "Caller");
    }
  }
}
