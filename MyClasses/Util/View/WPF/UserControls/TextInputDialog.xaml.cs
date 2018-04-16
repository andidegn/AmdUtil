using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMD.Util.View.WPF.UserControls
{
	/// <summary>
	/// Interaction logic for TextInputDialog.xaml
	/// </summary>
	public partial class TextInputDialog : Window
	{
		public enum ButtonAssignments
		{
			NotSet,
			YesNo,
			OkCancel
		}

		private ButtonAssignments buttonAssignments { get; set; }
		private LogWriter log;

		public bool UserCancel { get; set; }

		public String PropTitle
		{
			get
			{
				return lblTitle.Text;
			}
			set
			{
				lblTitle.Text = value;
			}
		}

		public int PropMaxCharacters
		{
			get
			{
				return tbText.MaxLength;
			}
			set
			{
				lblCharMax.Text = String.Format("/{0}", value);
				tbText.MaxLength = value;
			}
		}

		public ButtonAssignments PropButtonAssignments
		{
			set
			{
				buttonAssignments = value;
				switch (buttonAssignments)
				{
					case ButtonAssignments.YesNo:
						btnLeft.Content = "Yes";
						btnRight.Content = "No";
						break;
					case ButtonAssignments.OkCancel:
					default:
						btnLeft.Content = "OK";
						btnRight.Content = "Cancel";
						break;
				}
			}
		}

		public String PropText
		{
			get
			{
				return tbText.Text;
			}
			set
			{
				tbText.Text = value;
			}
		}

		public TextInputDialog(LogWriter log, double top, double left)
		{
			UserCancel = true;
			this.log = log;
			InitializeComponent();
			tbText.Focus();
			this.Top = top - this.Height / 2;
			this.Left = left - this.Width / 2;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1)
			{
				DragMove();
			}
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				UserCancel = true;
				DialogResult = false;
			}
		}

		private void tbText_TextChanged(object sender, TextChangedEventArgs e)
		{
			lblCharUsed.Text = (sender as TextBox).Text.Length.ToString();
		}

		private void btnLeft_Click(object sender, RoutedEventArgs e)
		{
			switch (buttonAssignments)
			{
				case ButtonAssignments.YesNo:
				case ButtonAssignments.NotSet:
				case ButtonAssignments.OkCancel:
				default:
					DialogResult = true;
					break;
			}
		}

		private void btnRight_Click(object sender, RoutedEventArgs e)
		{
			switch (buttonAssignments)
			{
				case ButtonAssignments.YesNo:
				case ButtonAssignments.NotSet:
				case ButtonAssignments.OkCancel:
				default:
					DialogResult = false;
					break;
			}
		}
	}
}
