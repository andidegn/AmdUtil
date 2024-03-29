﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace AMD.Util.Extensions
{
  public static class ExtensionFlowDocument
  {
    public static string GetRawText(this FlowDocument doc)
    {
      return new TextRange(doc.ContentStart, doc.ContentEnd).Text;
    }

    public static FlowDocument Clone(this FlowDocument doc)
    {
      string str = XamlWriter.Save(doc);
      StringReader sr = new StringReader(str);
      XmlReader xml = XmlReader.Create(sr);
      return XamlReader.Load(xml) as FlowDocument;
    }

    public static FlowDocument Clone2(this FlowDocument from)
    {
      FlowDocument to = new FlowDocument();
      TextRange range = new TextRange(from.ContentStart, from.ContentEnd);
      MemoryStream stream = new MemoryStream();
      XamlWriter.Save(range, stream);
      range.Save(stream, DataFormats.XamlPackage);
      TextRange range2 = new TextRange(to.ContentEnd, to.ContentEnd);
      range2.Load(stream, DataFormats.XamlPackage);
      return to;
    }

    public static FormattedText GetFormattedText(this FlowDocument doc)
    {
      if (doc is null)
      {
        throw new ArgumentNullException("doc");
      }

      FormattedText output = new FormattedText(
        GetText(doc),
        CultureInfo.CurrentCulture,
        doc.FlowDirection,
        new Typeface(doc.FontFamily, doc.FontStyle, doc.FontWeight, doc.FontStretch),
        doc.FontSize,
        doc.Foreground);

      int offset = 0;

      foreach (TextElement el in GetRunsAndParagraphs(doc))
      {
        if (el is Run run && run != null)
        {
          int count = run.Text.Length;

          output.SetFontFamily(run.FontFamily, offset, count);
          output.SetFontStyle(run.FontStyle, offset, count);
          output.SetFontWeight(run.FontWeight, offset, count);
          output.SetFontSize(run.FontSize, offset, count);
          output.SetForegroundBrush(run.Foreground, offset, count);
          output.SetFontStretch(run.FontStretch, offset, count);
          output.SetTextDecorations(run.TextDecorations, offset, count);

          offset += count;
        }
        else
        {
          offset += Environment.NewLine.Length;
        }
      }

      return output;
    }

    private static IEnumerable<TextElement> GetRunsAndParagraphs(FlowDocument doc)
    {
      for (TextPointer position = doc.ContentStart;
        position != null && position.CompareTo(doc.ContentEnd) <= 0;
        position = position.GetNextContextPosition(LogicalDirection.Forward))
      {
        if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
        {
          Run run = position.Parent as Run;

          if (run != null)
          {
            yield return run;
          }
          else
          {
            Paragraph para = position.Parent as Paragraph;

            if (para != null)
            {
              yield return para;
            }
          }
        }
      }
    }

    private static string GetText(FlowDocument doc)
    {
      StringBuilder sb = new StringBuilder();

      foreach (TextElement el in GetRunsAndParagraphs(doc))
      {
        Run run = el as Run;
        sb.Append(run == null ? Environment.NewLine : run.Text);
      }
      return sb.ToString();
    }
  }
}
