using AMD.Util.CH341;
using AMD.Util.Extensions;
using AMD.Util.HID;
using AMD.Util.Log;
using AMD.Util.View.WPF.UserControls;
using CH341_Test.Properties;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static AMD.Util.CH341.CH341NativeFunctions;

namespace CH341_Test
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
	{
		private LogWriter log;
		private DebugPanel debugPanel;

		public MainWindow()
		{
			InitialiseLog();
			InitializeComponent();
			cbbDeviceId.ItemsSource = Enumerable.Range(0, 100);
		}

		private void InitialiseLog()
		{
			log = LogWriter.Instance;
			log.LogDir = Settings.Default.logPath;
			log.logFile = Settings.Default.logFile;
			log.logExtension = Settings.Default.logExtension;
			log.Type = LogType.DateCoded;
		}

		private void InitialiseDebugPanel()
		{
			debugPanel = new DebugPanel(this, this.log, Settings.Default.DebugWindowWidth);
			//debugPanel.AnimationTime = new TimeSpan(0, 0, 2);
			debugPanel.KeyUp += (s, ev) =>
			{
				switch (ev.Key)
				{
					case Key.D:
						if (Modifier.IsCtrlDown)
						{
							debugPanel.Hide();
						}
						break;
					default:
						break;
				}
			};

			debugPanel.Closing += (s, ev) =>
			{
				Settings.Default.DebugWindowWidth = debugPanel.DesiredWidth;
			};
		}

		private int DeviceId
		{
			get
			{
				return (int)cbbDeviceId.SelectedItem;
			}
		}

		private void ClearLeft()
		{
			tbLeftTop.Text = tbLeftBottom.Text = String.Empty;
		}

		private void ClearRight()
		{
			tbRightTop.Text = tbRightBottom.Text = String.Empty;
		}

		private void PrintLeft(String format, params object[] args)
		{
			String text = String.Format(format, args);
			tbLeftTop.Text += text;
			tbLeftBottom.Text += String.Format("{0}\n", text.GetBytes().GetHexString());
		}

		private void PrintRight(String format, params object[] args)
		{
			String text = String.Format(format, args);
			tbRightTop.Text += text;
			tbRightBottom.Text += String.Format("{0}\n", text.GetBytes().GetHexString());
		}

		private void btnOpenDevice_Click(object sender, RoutedEventArgs e)
		{
			int deviceId = DeviceId;

			ClearLeft();
			PrintLeft("Device ID: {0} - status: {1}\n", deviceId, CH341NativeFunctions.CH341OpenDevice(deviceId));

			byte[] bArr = new byte[100];
			StringBuilder sb = new StringBuilder("", 256);
			int ioLength = 100;
			CH341NativeFunctions.CH341GetInput(deviceId, bArr);
			PrintLeft("Status: {0}\n", bArr.GetString());

			CH341NativeFunctions.CH341GetConfigDescr(deviceId, sb, ref ioLength);
			PrintLeft("Config description: {0}\n", bArr.GetString());

			CH341NativeFunctions.CH341GetDeviceDescr(deviceId, sb, ref ioLength);
			PrintLeft("Device description: {0}\n", bArr.GetString());
			object obj = CH341NativeFunctions.CH341GetDeviceName(deviceId);
			//PrintLeft("Device name: {0}\n", sb.ToString());

			PrintLeft("Driver version: {0}\n", CH341NativeFunctions.CH341GetDrvVersion());
		}

		private void btnWrite_Click(object sender, RoutedEventArgs e)
		{
			int deviceId = DeviceId;
			CH341NativeFunctions.CH341OpenDevice(deviceId);
			String outStr = String.Format("{0}\0", tbInput.Text);
			PrintRight("Write status: {0}: {1}\n", CH341NativeFunctions.CH341WriteEEPROM(deviceId, EEPROM_TYPE.ID_24C128, 0, outStr.Length, outStr.GetBytes()) ? "Success!" : "Fail", outStr);
			CH341NativeFunctions.CH341CloseDevice(deviceId);
		}

		private void btnRead_Click(object sender, RoutedEventArgs e)
		{
			int deviceId = DeviceId;
			byte[] bArr = new byte[50];
			String text = String.Empty;
			try
			{
				CH341NativeFunctions.CH341OpenDevice(deviceId);
				if (CH341NativeFunctions.CH341ReadEEPROM(deviceId, EEPROM_TYPE.ID_24C128, 0, 33, bArr))
				{
					String inStr = String.Empty;
					foreach (char b in bArr)
					{
						if (b == '\0')
						{
							break;
						}
						inStr += b;
					}
					text = String.Format("Read from EEPROM ID: {0} successful\nValue: {1}\n", deviceId, inStr);
				}
				else
				{
					text = String.Format("Error reading from EEPROM ID: {0}\n", deviceId);
				}
				PrintRight(text);
			}
			catch (Exception ex)
			{
				log.WriteToLog(ex);
			}
			CH341NativeFunctions.CH341CloseDevice(deviceId);
		}

		private void ctClearRight_Click(object sender, RoutedEventArgs e)
		{
			ClearRight();
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (Modifier.IsCtrlDown)
			{
				switch (e.Key)
				{
					case Key.D:
						if (debugPanel.Visibility == Visibility.Visible)
						{
							debugPanel.Hide();
						}
						else
						{
							debugPanel.Show();
						}
						break;

					default:
						break;
				}
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InitialiseDebugPanel();
		}
	}
}
