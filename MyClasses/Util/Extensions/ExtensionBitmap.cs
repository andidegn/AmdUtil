using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace AMD.Util.Extensions
{
  public static class ExtensionBitmap
  {


    public static BitmapImage ToBitmapImage(this Bitmap bitmap)
    {
      using (MemoryStream memory = new MemoryStream())
      {
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        memory.Position = 0;
        BitmapImage bitmapimage = new BitmapImage();
        bitmapimage.BeginInit();
        bitmapimage.StreamSource = memory;
        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapimage.EndInit();

        return bitmapimage;
      }
    }
  }
}
