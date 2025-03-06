using AMD.Util.View.WPF.UserControls;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace PdfViewerTest
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent(); 
    }

    public static string GetFileNameFromOpenFileDialog(string initialDir, string filter = null)
    {
      OpenFileDialog ofd = new OpenFileDialog()
      {
        Multiselect = false,
      };
      if (Directory.Exists(initialDir))
      {
        ofd.InitialDirectory = initialDir;
      }
      ofd.Filter = filter ?? "Compatible |*.rtd;*.bin;*.hex;*.clm;*.rgb|EDID files |*.rtd|Firmware |*.bin;*.hex|ColorMap |*.clm;*.rgb|All Files |*.*";
      ofd.FilterIndex = 1;
      if (ofd.ShowDialog() == true)
      {
        return ofd.FileName;
      }
      else
      {
        return null;
      }
    }

    private void btnLoadFile_Click(object sender, RoutedEventArgs e)
    {
      string fileName = GetFileNameFromOpenFileDialog(@"c:\users\ade\downloads\", "PDF File |*.pdf;");
      if (!string.IsNullOrWhiteSpace(fileName))
      {
        var tti = new TearableTabItem()
        {
          Header = Path.GetFileName(fileName),
          Closeable = true,
          Content = new PdfViewerImage()
          {
            PdfPath = fileName
          }
        };
        ttc.Items.Add(tti);
        tti.Focus();
      }
    }
  }
}
