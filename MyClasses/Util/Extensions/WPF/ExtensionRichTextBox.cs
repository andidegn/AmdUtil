using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace AMD.Util.Extensions.WPF
{
	public static class ExtensionRichTextBox
	{
		public static void AppendText(this RichTextBox box, String text, System.Windows.Media.Brush color = null, FontStyle fontStyle = FontStyle.Regular)
		{
			TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
			tr.Text = text;
			tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
		}

		public static void AppendText(this RichTextBox box, char c, System.Windows.Media.Brush color = null, FontStyle fontStyle = FontStyle.Regular)
		{
			TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
			tr.Text = c.ToString();
			tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
		}

		public static void AddHyperlinkText(this RichTextBox box, String linkURL, String linkName = null, String TextBeforeLink = null, String TextAfterLink = null)
		{
			Paragraph para = new Paragraph();
			para.Margin = new System.Windows.Thickness(0); // remove indent between paragraphs

			Hyperlink link = new Hyperlink();
			link.IsEnabled = true;

			link.Inlines.Add(linkName ?? linkURL);
			link.NavigateUri = new Uri(linkURL);
			link.RequestNavigate += (sender, args) => Process.Start(args.Uri.ToString());
			link.IsEnabled = true;

			//para.Inlines.Add(new Run("[" + DateTime.Now.ToLongTimeString() + "]: "));
			//para.Inlines.Add(TextBeforeLink);
			para.Inlines.Add(link);
			//para.Inlines.Add(new Run(TextAfterLink));

			box.Document.Blocks.Add(para);
    }

    /// <summary>
    /// Gets all the text in plain text
    /// </summary>
    /// <param name="rtb"></param>
    /// <returns></returns>
    public static String GetText(this RichTextBox rtb)
    {
     return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
    }

    /// <summary>
    /// Gets the formatted text as RTF
    /// </summary>
    /// <param name="rtb"></param>
    /// <returns></returns>
    public static String GetRtf(this RichTextBox rtb)
    {
      TextRange tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
      MemoryStream ms = new MemoryStream();
      tr.Save(ms, System.Windows.DataFormats.Rtf);
      ms.Seek(0, SeekOrigin.Begin);
      return new StreamReader(ms).ReadToEnd();
    }

    /// <summary>
    /// Loads an RTF formatted string
    /// </summary>
    /// <param name="rtb"></param>
    /// <param name="rtf"></param>
    public static void LoadRtf(this RichTextBox rtb, String rtf)
    {
      if (rtf != null && rtf.Length > 0)
      {
        TextRange tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        StreamWriter sw = new StreamWriter(ms);
        sw.Write(rtf);
        sw.Flush();
        ms.Seek(0, SeekOrigin.Begin);
        tr.Load(ms, System.Windows.DataFormats.Rtf);
      }
      else
      {
        rtb.Document.Blocks.Clear();
      }
    }

    /// <summary>
    /// Gets the current line of text in the RichTextBox
    /// </summary>
    /// <param name="rtb"></param>
    /// <returns></returns>
    public static String GetCurrentLine(this RichTextBox rtb)
    {
      TextPointer caretPos = rtb.CaretPosition;
      TextPointer start = caretPos.GetLineStartPosition(0);
      TextPointer end = (caretPos.GetLineStartPosition(1) != null ? caretPos.GetLineStartPosition(1) : caretPos.DocumentEnd);

      TextRange tr = new TextRange(start, end);
      return tr.Text;
    }

    /// <summary>
    /// Sets the caret position to the position of the mouse
    /// </summary>
    /// <param name="rtb"></param>
    public static void SetCaretOnMousePosition(this RichTextBox rtb)
    {
      rtb.CaretPosition = rtb.GetPositionFromPoint(Mouse.GetPosition(rtb), true);
    }
  }
}
