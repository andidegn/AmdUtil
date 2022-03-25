using AMD.Util.Log;
using AMD.Util.Permissions;
using System;
using System.Windows;
using System.Windows.Input;

namespace AMD.Util.View.WPF.UserControls
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class LoginDialog : Window
	{
		public bool UserCancel { get; set; }
		public string Domain { get; set; }
    public bool Fixed { get; set; }
    public string FixedUserName { get; set; }
    public string FixedPassword { get; set; }
    private LogWriter log;

		public LoginDialog(LogWriter log, double top, double left, string domain = "andidegn.com")
		{
			UserCancel = true;
			Domain = domain;
			this.log = log;
			InitializeComponent();
			tbUser.Focus();
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

		private void tbPass_KeyUp(object sender, KeyEventArgs e)
		{
      bool passed = false;
			if (e.Key == Key.Enter)
			{
        if (Fixed)
        {
          if (tbUser.Text.Trim().Equals(FixedUserName) && tbPass.Password.Equals(FixedPassword))
          {
            passed = true;
          }
          else
          {
            passed = false;
          }
        }
        else
        {
          try
          {
            lblNotification.Text = string.Empty;
            Impersonator impersonater = new Impersonator(tbUser.Text, Domain, tbPass.Password);
            passed = true;
          }
          catch (Exception ex)
          {
            passed = false;
            log.WriteToLog(ex, "Error while trying to log in: {0}, {1}", tbUser.Text, Domain);
          }
        }

        if (passed)
        {
          UserCancel = false;
          DialogResult = true;
        }
        else
        {
          lblNotification.Text = "Error logging in";
        }
			}
		}

		private void tbUser_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				tbPass.Focus();
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
	}
}
