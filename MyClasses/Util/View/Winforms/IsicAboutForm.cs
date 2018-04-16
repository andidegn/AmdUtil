using AMD.Util.Extensions.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Isic.Util.Winforms
{
  public partial class IsicAboutForm : Form {
		public IsicAboutForm() {
			InitializeComponent();
		}

		public String Title {
			get { return this.Text; }
			set { this.Text = value; }
		}

		public String Version {
			get { return lbl_version.Text; }
			set { lbl_version.Text = value; }
		}

		public String ShortDescription {
			get { return lbl_text2.Text; }
			set { lbl_text2.Text = value; }
		}

		public String Description {
			get { return rtb_description.Text; }
			set { rtb_description.Text = value; }
		}

		public RichTextBox DescriptionRTF {
			get { return rtb_description; }
			set { rtb_description = value; }
		}

		public void AppendDescription(String text) {
			rtb_description.AppendText(text);
		}

		public void AppendDescription(String text, Color color) {
			rtb_description.AppendText(text, color);
		}

		private void btn_close_Click(object sender, EventArgs e) {
			this.Dispose();
		}
	}
}
