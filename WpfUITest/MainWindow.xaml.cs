using AMD.Util.Colour;
using AMD.Util.Data;
using AMD.Util.Display;
using AMD.Util.Display.Edid;
using AMD.Util.HID;
using AMD.Util.Log;
using AMD.Util.Versioning;
using AMD.Util.View.WPF.Helper;
using AMD.Util.View.WPF.UserControls;
using AMD.Util.View.WPF.UserControls.TearableTabs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WpfUITest.Properties;

namespace WpfUITest
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private LogWriter log;
    private DebugPanel dp;

    public MainWindow()
    {
      InitializeComponent();
      editRibbon.EditRichTextBox = rtb;
      //InitialiseDDCMonitorTests();
      InitialiseCompareWindow();
    }

    private bool filesSwapped;
    private void InitialiseCompareWindow()
    {

      compareView.Compare(File.ReadAllText(@"c:\isic\comparetestleft.txt"), File.ReadAllText(@"c:\isic\comparetestright.txt"), Brushes.Black, Brushes.Red, new SolidColorBrush(Color.FromArgb(0x20, 0xFF, 0, 0)), Brushes.Orange, 15);
      CompareWindow cw = new CompareWindow(File.ReadAllText(@"c:\isic\comparetestleft.txt"), File.ReadAllText(@"c:\isic\comparetestright.txt"), 15);
      cw.MissingLine = Brushes.Orange;
      Window w = new Window()
      {
        Content = new CompareView() { Background = Brushes.GhostWhite }
      };

      cw.KeyUp += (s, e) =>
      {
        if (Key.F5 == e.Key)
        {
          string filePath1 = @"c:\isic\comparetestleft.txt";
          string filePath2 = @"c:\isic\comparetestright.txt";
          if (Modifier.IsCtrlDown)
          {
            cw.Compare(File.ReadAllText(filePath2), File.ReadAllText(filePath1), 15, 5);
          }
          else
          {
            cw.Compare(File.ReadAllText(filePath1), File.ReadAllText(filePath2), 15, 5);
            //cw.Compare(File.ReadAllText(filePath1), File.ReadAllText(filePath2), 915, 35);
          }
        }
      };

      w.KeyUp += (s, e) =>
      {
        if (Key.F5 == e.Key)
          (w.Content as CompareView).Compare(File.ReadAllText(@"c:\isic\comparetestleft.txt"), File.ReadAllText(@"c:\isic\comparetestright.txt"), Brushes.Black, Brushes.Red, new SolidColorBrush(Color.FromArgb(0x20, 0xFF, 0, 0)), Brushes.Orange, 7);
      };
      //w.Show();
      cw.Show();
    }

    private void InitialiseDDCMonitorTests()
    {
      MonitorList.SkipCapabilityCheck = true;
      MonitorList.RunAsync = false;
      foreach (AMD.Util.Display.Monitor mon in MonitorList.Instance.List)
      {
        mon.CheckLowLevelCapabilities();
        if (mon.SupportsLowLevelDDC)
        {
          tbCapabilityString.Text = $"{mon.CapabilityString}\n\n{mon.CapabilityStringFormatted}";
          //break;
        }
      }
    }

    string GetRandomLoremIpsum()
    {
      return LoremIpsum(5, 10, 1, 2, 1);
    }

    string LoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs)
    {

      var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

      var rand = new Random();
      int numSentences = rand.Next(maxSentences - minSentences)
          + minSentences + 1;
      int numWords = rand.Next(maxWords - minWords) + minWords + 1;

      StringBuilder result = new StringBuilder();

      for (int p = 0; p < numParagraphs; p++)
      {
        result.Append("<p>");
        for (int s = 0; s < numSentences; s++)
        {
          for (int w = 0; w < numWords; w++)
          {
            if (w > 0) { result.Append(" "); }
            result.Append(words[rand.Next(words.Length)]);
          }
          result.Append(". ");
        }
        result.Append("</p>");
      }

      return result.ToString();
    }

    private void StartLogBurst()
    {
      BackgroundWorker bw = new BackgroundWorker();
      bw.DoWork += (s, e) =>
      {

        int i = 0;
        int r;
        Random rand = new Random();


        Enumerable.Range(0, 10000).ToList().ForEach(x =>
        {
          r = rand.Next(0, 10);
          log.WriteToLog((LogMsgType)r, "LogSniffer test #{0}, {1}", i++, GetRandomLoremIpsum());
          if (i % 1000 == 0)
          {
            Console.WriteLine("progress: {0}", i);
          }
          Thread.Sleep(new TimeSpan(0, 0, 0, 0, 0));
        });
        //Console.WriteLine("Press key to start");
        //Console.ReadKey();


        while (true)
        {
          r = rand.Next(0, 10);
          Console.WriteLine("Printing log type: {0}, value: {1}", Enum.GetName(typeof(LogMsgType), r), i);
          log.WriteToLog((LogMsgType)r, "LogSniffer test #{0}, {1}", i++, GetRandomLoremIpsum());
          Thread.Sleep(0);
        }
      };
      bw.RunWorkerAsync();
    }

    private void CreateNewTabWindow()
    {
      TearableTabItemWindow ttiw = new TearableTabItemWindow(tabControlCount++);
      tabWindows.Add(ttiw);
      for (int i = 0; i < 10; i++)
      {
        ttiw.DropTab(new TearableTabItem() { Name = $"TabName{i}", Header = $"Tab#{i}" });
      }
      ttiw.Show();
    }

    private void CreateNewRandomTab()
    {
      Random r = new Random();
      String name = String.Format("{0} test tab", tabCount++);
      for (int i = 0; i < 1; i++)
      {
        TearableTabItem tti = new TearableTabItem()
        {
          Header = name,
          Content = new TextBlock()
          {
            Text = name
          },
          Background = GetRandomBrush(10, 240)
        };
        int tabControlIndex = r.Next(4 + tabWindows.Count);
        switch (tabControlIndex)
        {
          case 0:
            ttcTestFirst.Items.Add(tti);
            break;
          case 1:
            ttcTestSecond.Items.Add(tti);
            break;
          case 2:
            ttcTestThird.Items.Add(tti);
            break;
          case 3:
            ttcTestFourth.Items.Add(tti);
            break;
          default:
            //tabWindows[tabControlIndex - 4].Items.Add(tti);
            break;
        }
      }
    }

    private SolidColorBrush GetRandomBrush(byte min, byte max)
    {
      Random r = new Random();
      return new SolidColorBrush(Color.FromRgb((byte)r.Next(min, max), (byte)r.Next(min, max), (byte)r.Next(min, max)));
    }

    private void ColorPicker_BrushChanged(object sender, BrushChangedEventArgs args)
    {
      StackTrace st = new StackTrace();
      log.WriteToLog(st.GetFrame(1).GetMethod().Name, LogMsgType.Notification, "This is a test: \"{0}\"", "test2");
      return;
      //tabColorPicker.Background = args.SelectedBrush;
      //tabColorPicker.UpdateLayout();
    }

    private void btnTestTabBackgroundBinding_Click(object sender, RoutedEventArgs e)
    {
      Random r = new Random();
      tvMemory.Background = new SolidColorBrush(Color.FromRgb((byte)r.Next(0xFF), (byte)r.Next(0xFF), (byte)r.Next(0xFF)));
    }

    private void btnColorPickerDialogTest_Click(object sender, RoutedEventArgs e)
    {
      ColorPickerDialog cpd = new ColorPickerDialog();
      cpd.Background = Brushes.Pink;
      cpd.SelectedBrush = tvMemory.Background;
      cpd.SelectedBrushChanged += (s, ev) =>
      {
        SolidColorBrush sb = new SolidColorBrush((ev.SelectedBrush as SolidColorBrush).Color);
        tvMemory.Background = sb;
      };
      cpd.ShowDialog();
      if (cpd.DialogResult == true)
      {
        tvMemory.Background = cpd.SelectedBrush;
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      dp = new DebugPanel(this, log, 500);
      StackTrace st = new StackTrace();
      log = LogWriter.Instance;
      log.WriteToLog(LogMsgType.Notification, st.GetFrame(1).GetMethod().Name, "This is a test: {0}", "Test1");
      dp.KeyUp += Window_KeyUp;
      //CreateNewTabWindow();
      //ShowAboutModal();
    }

    private void PrintEdidToRtb()
    {
      byte[] edidRaw = File.ReadAllBytes(@"C:\Users\ade\Sync\ISIC\Projects\Edids\09297-013C edid_156 DP1.bin");

      EDID edid = new EDID(edidRaw);

      rtb.Document.Blocks.Clear();
      rtb.AppendText(edid.ToString());
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (Modifier.IsCtrlDown)
      {
        switch (e.Key)
        {
          case Key.D:
            dp.Toggle();
            break;

          case Key.E:
            PrintEdidToRtb();
            break;

          case Key.N:
            if (Modifier.IsShiftDown)
            {
              CreateNewTabWindow();
            }
            else
            {
              CreateNewRandomTab();
            }
            break;
          default:
            break;
        }
      }
    }

#region About Modal
    /// <summary>
    /// Sets up and displays the About modal
    /// </summary>
    private static void ShowAboutModal()
    {
      AboutWindow about = new AboutWindow();
      about.Topmost = true;
      about.ResizeMode = ResizeMode.CanResizeWithGrip;
      about.PropIconPath = new Uri(@"\Resources\Images\Intel_Icon_TarmacTool_64x64.png", UriKind.Relative);
      about.PropLogoPath = new Uri(@"\Resources\Images\logo_intel.png", UriKind.Relative);
      about.PropLogo.HorizontalAlignment = HorizontalAlignment.Left;
      about.PropApplicationName = "Tarmac Parser";
      about.PropCompanyName = "INTEL";
      about.PropDeveloper = "adegn";
      about.PropYear = 2017;
      about.PropVersion = RunningVersion.GetRunningVersion;
      about.PropDescription.FontFamily = new FontFamily("Consolas");
      about.Height = Settings.Default.HelpHeight;
      about.Width = Settings.Default.HelpWidth;

      FlowDocument fd = new FlowDocument();
      Paragraph para = new Paragraph();

      para.Inlines.Add(new Run(Properties.Resources.AboutHowToGetStartedText));
      para.Inlines.Add(new Run("\n"));
      para.Inlines.Add(new Bold(new Run(Properties.Resources.AboutNoteText) { Foreground = Brushes.Red }));
      fd.Blocks.Add(para);
      about.PropDescription.Document = fd;

      Button btnShowChangelog = new Button();
      btnShowChangelog.Content = "Show Changelog";
      btnShowChangelog.HorizontalAlignment = HorizontalAlignment.Right;
      btnShowChangelog.Width = 120;
      btnShowChangelog.Margin = new Thickness(0, 0, 10, 5);
      btnShowChangelog.Click += (s, e) =>
      {
        Paragraph paraChangelog = new Paragraph();
        para.Inlines.Add(new Run("\n"));
        para.Inlines.Add(new Run("\n"));
        paraChangelog.Inlines.Add(new Bold(new Run("Changelog")));
        para.Inlines.Add(new Run("\n"));
        para.Inlines.Add(new Run("\n"));
        paraChangelog.Inlines.Add(new Run(StreamHelper.GetEmbeddedResource("changelog")));
        about.PropDescription.Document.Blocks.Add(paraChangelog);
      };
      about.PropGridBottomCustom.Children.Add(btnShowChangelog);

      about.Show();
    }
#endregion // About Modal

    private void btnTestException_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        throw new Exception("This is a test Exception");
      }
      catch (Exception ex)
      {
        log.WriteToLog(ex);
        log.WriteToLog(ex, "Testing exception");
      }
    }

    private void btnLogBurst_Click(object sender, RoutedEventArgs e)
    {
      StartLogBurst();
    }

    private int tabCount, tabControlCount;
    private List<TearableTabItemWindow> tabWindows = new List<TearableTabItemWindow>();

    private void btnNewWindowTabTest_Click(object sender, RoutedEventArgs e)
    {
      CreateNewTabWindow();
    }

    private void btnNewTabTest_Click(object sender, RoutedEventArgs e)
    {
      CreateNewRandomTab();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      //Settings.Default.TabPositioning = tabSplitControl.GetSerializedStreamOfAllContent();
    }

    private void BtnCheckCapabilities_Click(object sender, RoutedEventArgs e)
    {
      InitialiseDDCMonitorTests();
    }

    private void btnClearAllTabs_Click(object sender, RoutedEventArgs e)
    {
      foreach (TearableTabControl tabControl in VisualHelper.FindVisualChildren<TearableTabControl>(this))
      {
        if (tabControl.Name.StartsWith("ttcTest"))
        {
          tabControl.Items.Clear();
        }
      }
      foreach (TearableTabItemWindow tabWindow in tabWindows)
      {
        foreach (TearableTabControl tabControl in VisualHelper.FindVisualChildren<TearableTabControl>(tabWindow))
        {
          if (tabControl.Name.StartsWith("ttcTest"))
          {
            tabControl.Items.Clear();
          }
        }
      }
    }
  }
}
