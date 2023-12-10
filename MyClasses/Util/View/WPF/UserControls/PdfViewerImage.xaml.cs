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
    #region Bindable Properties

    public string PdfPath
    {
      get { return (string)GetValue(PdfPathProperty); }
      set { SetValue(PdfPathProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PdfPath.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PdfPathProperty =
        DependencyProperty.Register("PdfPath", typeof(string), typeof(PdfViewerImage), new PropertyMetadata(null, propertyChangedCallback: OnPdfPathChanged));

    private static async void OnPdfPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is PdfViewerImage pv && !string.IsNullOrEmpty(pv.PdfPath))
      {
        //making sure it's an absolute path
        var path = System.IO.Path.GetFullPath(pv.PdfPath);

        StorageFile file = await StorageFile.GetFileFromPathAsync(path);
        var pdfDoc = await PdfDocument.LoadFromFileAsync(file);
        await PdfToImages(pv, pdfDoc);

        //StorageFile.GetFileFromPathAsync(path).AsTask()
        //  //load pdf document on background thread
        //  .ContinueWith(t => PdfDocument.LoadFromFileAsync(t.Result).AsTask()).Unwrap()
        //  //display on UI Thread
        //  .ContinueWith(t2 => PdfToImages(pdfDrawer, t2.Result), TaskScheduler.FromCurrentSynchronizationContext());
      }

    }


    public Stream PdfStream
    {
      get { return (Stream)GetValue(PdfStreamProperty); }
      set { SetValue(PdfStreamProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PdfStream.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PdfStreamProperty =
        DependencyProperty.Register("PdfStream", typeof(Stream), typeof(PdfViewerImage), new PropertyMetadata(null, propertyChangedCallback: OnPdfStreamChanged));

    private async static void OnPdfStreamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is PdfViewerImage pv && null != pv.PdfStream)
      {
        var v = await PdfDocument.LoadFromStreamAsync(pv.PdfStream.AsRandomAccessStream());
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
      var items = pdfViewer.PagesContainer.Items;
      items.Clear();

      if (pdfDoc == null) return;

      for (uint i = 0; i < pdfDoc.PageCount; i++)
      {
        using (var page = pdfDoc.GetPage(i))
        {
          var bitmap = await PageToBitmapAsync(page);
          var image = new Image
          {
            Source = bitmap,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 4, 0, 4),
            MaxWidth = 800
          };
          items.Add(image);
        }
      }
    }

    private static async Task<BitmapImage> PageToBitmapAsync(PdfPage page)
    {
      BitmapImage image = new BitmapImage();

      using (var stream = new InMemoryRandomAccessStream())
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
