using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for PdfViewerImage.xaml
  /// </summary>
  public partial class PdfViewerImage : UserControl
  {
    #region Properties

    public string PdfPath
    {
      get { return (string)GetValue(PdfPathProperty); }
      set { SetValue(PdfPathProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PdfPath.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PdfPathProperty =
        DependencyProperty.Register("PdfPath", typeof(string), typeof(PdfViewerImage), new PropertyMetadata(null, OnPdfPathChanged));

    private static async void OnPdfPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is PdfViewerImage pv && !string.IsNullOrEmpty(pv.PdfPath))
      {
        string path = Path.GetFullPath(pv.PdfPath);

        StorageFile file = await StorageFile.GetFileFromPathAsync(path);
        PdfDocument pdfDoc = await PdfDocument.LoadFromFileAsync(file);
        await PdfToImages(pv, pdfDoc);
      }
    }

    public Stream PdfStream
    {
      get { return (Stream)GetValue(PdfStreamProperty); }
      set { SetValue(PdfStreamProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PdfStream.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PdfStreamProperty =
        DependencyProperty.Register("PdfStream", typeof(Stream), typeof(PdfViewerImage), new PropertyMetadata(null, OnPdfStreamChanged));

    private async static void OnPdfStreamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is PdfViewerImage pv && null != pv.PdfStream)
      {
        PdfDocument v = await PdfDocument.LoadFromStreamAsync(pv.PdfStream.AsRandomAccessStream());
        await PdfToImages(pv, v);
      }
    }
    #endregion


    public PdfViewerImage()
    {
      InitializeComponent();
    }

    private async static Task PdfToImages(PdfViewerImage pdfViewer, PdfDocument pdfDoc)
    {
      ItemCollection items = pdfViewer.PagesContainer.Items;
      items.Clear();

      if (pdfDoc is null)
      {
        return;
      }

      for (uint i = 0; i < pdfDoc.PageCount; i++)
      {
        using (var page = pdfDoc.GetPage(i))
        {
          BitmapImage bitmap = await PageToBitmapAsync(page);
          Image image = new Image
          {
            Source = bitmap,
            HorizontalAlignment = HorizontalAlignment.Center
          };
          items.Add(image);
        }
      }
    }

    private static async Task<BitmapImage> PageToBitmapAsync(PdfPage page)
    {
      BitmapImage image = new BitmapImage();

      using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
      {
        await page.RenderToStreamAsync(stream);

        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream.AsStream();
        image.EndInit();
      }

      return image;
    }
  }
}
