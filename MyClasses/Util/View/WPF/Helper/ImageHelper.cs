using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AMD.Util.View.WPF.Helper
{
	public static class ImageHelper
	{
		public static BitmapSource GetFromJpegStream(byte[] rawData)
		{
			using (MemoryStream ms = new MemoryStream(rawData))
			{
				JpegBitmapDecoder decoder = null;
				BitmapSource bitmapSource = null;
				decoder = new JpegBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
				bitmapSource = decoder.Frames[0];
				bitmapSource.Freeze();
				return bitmapSource;
			}
		}
	}
}
