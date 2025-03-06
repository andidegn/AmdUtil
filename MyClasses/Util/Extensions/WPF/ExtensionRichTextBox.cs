using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace AMD.Util.Extensions.WPF
{
  public static class ExtensionRichTextBox
	{
    private static TextRange PutTextAtEnd(RichTextBox box, string text)
    {
      return new TextRange(box.Document.ContentEnd, box.Document.ContentEnd) { Text = text };
    }

    private static TextRange PutTextAtEnd(RichTextBox box, char c)
    {
      return new TextRange(box.Document.ContentEnd, box.Document.ContentEnd) { Text = c.ToString() };
    }

    public static void AppendText(this RichTextBox box, string text, params (DependencyProperty property, object value)[] textElementPropertyAndValue)
    {
      TextRange tr = PutTextAtEnd(box, text);
      foreach ((DependencyProperty prop, object value) propValuePair in textElementPropertyAndValue)
      {
        tr.ApplyPropertyValue(propValuePair.prop, propValuePair.value);
      }
    }

    public static void AppendText(this RichTextBox box, char c, params (DependencyProperty property, object value)[] textElementPropertyAndValue)
    {
      TextRange tr = PutTextAtEnd(box, c);
      foreach ((DependencyProperty prop, object value) propValuePair in textElementPropertyAndValue)
      {
        tr.ApplyPropertyValue(propValuePair.prop, propValuePair.value);
      }
    }

    public static void AppendText(this RichTextBox box, string text, System.Windows.Media.Brush color = null)
		{
      TextRange tr = PutTextAtEnd(box, text);
			tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
		}

		public static void AppendText(this RichTextBox box, char c, System.Windows.Media.Brush color = null)
    {
      TextRange tr = PutTextAtEnd(box, c);
      tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
    }

    public static void AppendText(this RichTextBox box, string text, FontWeight fontWeight, System.Windows.Media.Brush color = null)
    {
      TextRange tr = PutTextAtEnd(box, text);
      tr.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
      tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
    }

    public static void AppendText(this RichTextBox box, char c, FontWeight fontWeight, System.Windows.Media.Brush color = null)
    {
      TextRange tr = PutTextAtEnd(box, c);
      tr.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
      tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
    }

    public static void AppendText(this RichTextBox box, string text, double fontSize, System.Windows.Media.Brush color = null)
    {
      TextRange tr = PutTextAtEnd(box, text);
      tr.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
      tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
    }

    public static void AppendText(this RichTextBox box, char c, double fontSize, System.Windows.Media.Brush color = null)
    {
      TextRange tr = PutTextAtEnd(box, c);
      tr.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
      tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
    }

    public static void AppendText(this RichTextBox box, string text, FontWeight fontWeight, double fontSize, System.Windows.Media.Brush color = null)
    {
      TextRange tr = PutTextAtEnd(box, text);
      tr.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
      tr.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
      tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
    }

    public static void AppendText(this RichTextBox box, char c, FontWeight fontWeight, double fontSize, System.Windows.Media.Brush color = null)
    {
      TextRange tr = PutTextAtEnd(box, c);
      tr.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
      tr.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
      tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
    }

    public static void AddHyperlinkText(this RichTextBox box, string linkURL, string linkName = null, string TextBeforeLink = null, string TextAfterLink = null)
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
    public static string GetText(this RichTextBox rtb)
    {
     return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
    }

    /// <summary>
    /// Gets the formatted text as RTF
    /// </summary>
    /// <param name="rtb"></param>
    /// <returns></returns>
    public static string GetRtf(this RichTextBox rtb)
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
    public static void LoadRtf(this RichTextBox rtb, string rtf)
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
    public static string GetCurrentLine(this RichTextBox rtb)
    {
      TextPointer caretPos = rtb.CaretPosition;
      TextPointer start = caretPos.GetLineStartPosition(0);
      TextPointer end = caretPos.GetLineStartPosition(1) ?? caretPos.DocumentEnd;

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
