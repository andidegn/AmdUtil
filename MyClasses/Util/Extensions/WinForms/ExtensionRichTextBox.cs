using System;
using System.Drawing;
using System.IO;
using System.Windows.Documents;
using System.Windows.Forms;

namespace AMD.Util.Extensions.WinForms
{
	public static class ExtensionRichTextBox
	{
		public static void AppendText(this System.Windows.Forms.RichTextBox box, String text, Color? color = null, FontStyle fontStyle = FontStyle.Regular)
		{
			try
			{
				box.SelectionStart = box.TextLength;
				box.SelectionLength = 0;

				box.SelectionColor = color ?? Color.Black;
				box.SelectionFont = new Font(box.Font, fontStyle);
				box.AppendText(text);
				box.SelectionColor = box.ForeColor;
				box.SelectionFont = new Font(box.Font, FontStyle.Regular);
			}
			catch { }
		}

		public static void AppendText(this System.Windows.Forms.RichTextBox box, char c, Color? color = null, FontStyle fontStyle = FontStyle.Regular)
		{
			try
			{
				box.SelectionStart = box.TextLength;
				box.SelectionLength = 0;

				box.SelectionColor = color ?? Color.Black;
				box.SelectionFont = new Font(box.Font, fontStyle);
				box.AppendText(c + "");
				box.SelectionColor = box.ForeColor;
				box.SelectionFont = new Font(box.Font, FontStyle.Regular);
			}
			catch { }
		}
	}
}
