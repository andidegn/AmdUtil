using AMD.Util.Colour;
using AMD.Util.Data;
using AMD.Util.Log;
using AMD.Util.Versioning;
using AMD.Util.View.WPF.UserControls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
    public MainWindow()
    {
      InitializeComponent();
      editRibbon.EditRichTextBox = rtb;
    }

    private void ColorPicker_BrushChanged(object sender, BrushChangedEventArgs args)
    {
      StackTrace st = new StackTrace();
      log.WriteToLog(st.GetFrame(1).GetMethod().Name, LogMsgType.Notification, "This is a test: \"{0}\"", "test2");
      return;
      tabColorPicker.Background = args.SelectedBrush;
      tabColorPicker.UpdateLayout();
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
      DebugPanel dp = new DebugPanel(this, log, 500);
      StackTrace st = new StackTrace();
      log = LogWriter.Instance;
      log.WriteToLog(LogMsgType.Notification, st.GetFrame(1).GetMethod().Name, "This is a test: {0}", "Test1");
      dp.Show();

      //ShowAboutModal();
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
  }
}
