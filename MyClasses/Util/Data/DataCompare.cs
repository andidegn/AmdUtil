using AMD.Util.Extensions;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AMD.Util.Data
{
  public static class DataCompare
  {

    private static Size CharSize;
    private static Rectangle rectBlankLine;

    public static (FlowDocument fdLeft, FlowDocument fdRight) CompareBytes
      (
        IEnumerable<byte> leftData, 
        IEnumerable<byte> rightData, 
        bool formatAsMemoryView, 
        SolidColorBrush brushDefaultForeground,
        SolidColorBrush brushDiffForeground,
        SolidColorBrush brushDiffBackground,
        int charsToSearchForOffset,
        double fontSize,
        LogWriter log = null)
    {
      string strLeft = string.Empty, strRight = string.Empty;
      if (leftData != null)
      {
        if (formatAsMemoryView)
        {
          strLeft = StringFormatHelper.GetFormattedMemoryString(0, leftData.ToArray().GetNullableUIntArray());
        }
        else
        {
          strLeft = leftData.ToArray().GetString();
        }
      }
      if (rightData != null)
      {
        if (formatAsMemoryView)
        {
          strRight = StringFormatHelper.GetFormattedMemoryString(0, rightData.ToArray().GetNullableUIntArray());
        }
        else
        {
          strRight = rightData.ToArray().GetString();
        }
      }
      return CompareStrings(strLeft, strRight, brushDefaultForeground, brushDiffForeground, brushDiffBackground, charsToSearchForOffset, fontSize, log);
    }

    /// <summary>
    /// Compares two strings and returns left and right documents, which can be loaded into RichTextBox
    /// </summary>
    /// <param name="leftText"></param>
    /// <param name="rightText"></param>
    /// <param name="brushDefaultForeground"></param>
    /// <param name="brushDiffForeground"></param>
    /// <param name="brushDiffBackground"></param>
    /// <param name="charsToSearchForOffset">The number of characters to search into a line in order to determine if they are comparable</param>
    /// <param name="fontSize">The size of the font used in the destination control. Ex RichTextBox.FontSize</param>
    /// <param name="log"></param>
    /// <returns></returns>
    public static (FlowDocument fdLeft, FlowDocument fdRight) CompareStrings
      (
        string leftText,
        string rightText,
        SolidColorBrush brushDefaultForeground,
        SolidColorBrush brushDiffForeground,
        SolidColorBrush brushDiffBackground,
        int charsToSearchForOffset,
        double fontSize,
        LogWriter log = null
      )
    {
      rectBlankLine = Application.Current.TryFindResource("RectBlankLine") as Rectangle;

      FlowDocument flowDocumentLeft = new FlowDocument();
      FlowDocument flowDocumentRight = new FlowDocument();

      leftText = leftText.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n');
      rightText = rightText.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n');

      if (string.IsNullOrWhiteSpace(leftText) || string.IsNullOrWhiteSpace(rightText))
      {
        brushDiffForeground = brushDefaultForeground;
        brushDiffBackground = Brushes.Transparent;
      }

      string[] linesLeft = leftText.Split(new string[] { "\n" }, StringSplitOptions.None);
      string[] linesRight = rightText.Split(new string[] { "\n" }, StringSplitOptions.None);

      bool offsetTried = false, skipAddToDocument = false;
      int lOffsetIndex = -1, rOffsetIndex = -1;

      string longestLineLeft = linesLeft.Where(x => x.Length == linesLeft.Max(y => y.Length)).FirstOrDefault();
      string longestLineRight = linesRight.Where(x => x.Length == linesRight.Max(y => y.Length)).FirstOrDefault();

      int leftLineLength = longestLineLeft.Length;
      int rightLineLength = longestLineRight.Length;
      int longestLineLength = Math.Max(leftLineLength, rightLineLength);

      string longestLine = leftLineLength > rightLineLength ? longestLineLeft : longestLineRight;

      int maxLines = Math.Max(linesLeft.Length, linesRight.Length);

      Paragraph paragraphLeft = new Paragraph() { Margin = new Thickness(0), LineHeight = 10 };
      Paragraph paragraphRight = new Paragraph() { Margin = new Thickness(0), LineHeight = 10 };

      CharSize = MeasureString("0", paragraphLeft, fontSize);

      for (int l = 0, r = 0; l < maxLines || r < maxLines; l++, r++)
      {
        bool lastEqual = true;
        bool changed = false;
        bool lineNotEqual = false;
        offsetTried = false;
        skipAddToDocument = false;

        int maxLineLength = Math.Max(linesLeft[Math.Min(l, linesLeft.Length - 1)].Length, linesRight[Math.Min(r, linesRight.Length - 1)].Length);
        int minLineLength = Math.Min(linesLeft[Math.Min(l, linesLeft.Length - 1)].Length, linesRight[Math.Min(r, linesRight.Length - 1)].Length);

        paragraphLeft = new Paragraph();
        paragraphRight = new Paragraph();

        if (0 < lOffsetIndex)
        {
          try
          {
            do
            {
              maxLines++;
              AddCompensationLine(linesLeft[l], flowDocumentLeft, flowDocumentRight, brushDiffForeground, brushDiffBackground, longestLineLength);
            } while (++l < lOffsetIndex);
            lOffsetIndex = -1;
          }
          catch (Exception ex)
          {
            log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}");
          }
        }
        else if (0 < rOffsetIndex)
        {
          try
          {
            do
            {
              maxLines++;
              AddCompensationLine(linesRight[r], flowDocumentRight, flowDocumentLeft, brushDiffForeground, brushDiffBackground, longestLineLength);
            } while (++r < rOffsetIndex);
            rOffsetIndex = -1;
          }
          catch (Exception ex)
          {
            log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}");
          }
        }

        if (linesRight.Length - 1 < r)
        {
          try
          {
            if (linesLeft.Length - 1 < l)
            {
              break;
            }
            maxLines++;
            AddCompensationLine(linesLeft[l], flowDocumentLeft, flowDocumentRight, brushDiffForeground, brushDiffBackground, longestLineLength);
            skipAddToDocument = true;
          }
          catch (Exception ex)
          {
            log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}");
          }
        }
        else if (linesLeft.Length - 1 < l)
        {
          try
          {
            if (linesRight.Length - 1 < r)
            {
              break;
            }
            maxLines++;
            AddCompensationLine(linesRight[r], flowDocumentRight, flowDocumentLeft, brushDiffForeground, brushDiffBackground, longestLineLength);
            skipAddToDocument = true;
          }
          catch (Exception ex)
          {
            log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}");
          }
        }
        else
        {
          for (int j = 0, lastIndex = 0; j < maxLineLength; j++)
          {
            char charLeft = linesLeft[l].Length > j ? linesLeft[l][j] : '\0';
            char charRight = linesRight[r].Length > j ? linesRight[r][j] : '\0';
            bool equal = charLeft == charRight;
            if (!equal)
            {
              lineNotEqual = true;
            }

            changed = equal != lastEqual;

            try
            {
              if (!offsetTried && 4 > j && !equal)
              {
                offsetTried = true;

                if (0 < linesRight[r].Length && 0 < (lOffsetIndex = Array.IndexOf(linesLeft, linesLeft.SubArray(l, linesLeft.Length - l - 1).Where(x => x.Contains(linesRight[r].Substring(0, Math.Min(charsToSearchForOffset, linesRight[r].Length)))).FirstOrDefault())))
                {
                  r--;
                  l--;
                  skipAddToDocument = true;
                  if (lOffsetIndex > linesLeft.Length)
                  {

                  }
                  break;
                }
                else if (0 < linesLeft[l].Length && 0 < (rOffsetIndex = Array.IndexOf(linesRight, linesRight.SubArray(r, linesRight.Length - r - 1).Where(x => x.Contains(linesLeft[l].Substring(0, Math.Min(charsToSearchForOffset, linesLeft[l].Length)))).FirstOrDefault())))
                {
                  l--;
                  r--;
                  skipAddToDocument = true;
                  if (rOffsetIndex > linesRight.Length)
                  {

                  }
                  break;
                }
              }
            }
            catch (Exception ex)
            {
              log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}, charLeft:{charLeft}, charRight:{charRight}, equal:{equal}, lastEqual:{lastEqual}, changed:{changed}");
            }

            try
            {
              if (changed || j == maxLineLength)
              {
                paragraphLeft.Inlines.Add(new Run(linesLeft[l].Substring(lastIndex, Math.Min(j - lastIndex, linesLeft[l].Length - lastIndex))) { Foreground = (j == maxLineLength && !changed ? equal : lastEqual) ? brushDefaultForeground : brushDiffForeground });
                paragraphRight.Inlines.Add(new Run(linesRight[r].Substring(lastIndex, Math.Min(j - lastIndex, linesRight[r].Length - lastIndex))) { Foreground = (j == maxLineLength && !changed ? equal : lastEqual) ? brushDefaultForeground : brushDiffForeground });

                lastEqual = equal;
                lastIndex = j;
              }
              if (j + 1 == maxLineLength)
              {
                j++;
                if (lastIndex <= linesLeft[l].Length)
                {
                  try
                  {
                    paragraphLeft.Inlines.Add(new Run(linesLeft[l].Substring(lastIndex, Math.Min(j - lastIndex, linesLeft[l].Length - lastIndex))) { Foreground = equal ? brushDefaultForeground : brushDiffForeground });
                  }
                  catch (Exception)
                  {
                    log?.PrintError($"l: {l}, str: \"{linesLeft[l]}\", SubString({lastIndex}, {Math.Min(j - lastIndex, linesLeft[l].Length - lastIndex)}) (j - lastIndex: {j - lastIndex}, linesRight[r].Length: {linesLeft[l].Length - lastIndex})");
                  }
                }

                if (lastIndex <= linesRight[r].Length)
                {
                  try
                  {
                    paragraphRight.Inlines.Add(new Run(linesRight[r].Substring(lastIndex, Math.Min(j - lastIndex, linesRight[r].Length - lastIndex))) { Foreground = equal ? brushDefaultForeground : brushDiffForeground });
                  }
                  catch (Exception)
                  {
                    log?.PrintError($"r: {r}, str: \"{linesRight[r]}\",, SubString({lastIndex}, {Math.Min(j - lastIndex, linesRight[r].Length - lastIndex)}) (j - lastIndex: {j - lastIndex}, linesRight[r].Length: {linesRight[r].Length - lastIndex})");
                  }
                }
              }
            }
            catch (Exception ex)
            {
              log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}, charLeft:{charLeft}, charRight:{charRight}, equal:{equal}, lastEqual:{lastEqual}, changed:{changed}");
            }
          }
        }
        if (!skipAddToDocument)
        {
          if (lineNotEqual)
          {
            paragraphLeft.Background = paragraphRight.Background = brushDiffBackground;
          }
          flowDocumentLeft.Blocks.Add(paragraphLeft);
          flowDocumentRight.Blocks.Add(paragraphRight);
        }
        if (linesLeft.Length - 1 < l && linesRight.Length - 1 < r)
        {
          break;
        }
      }

      CharSize = MeasureString(longestLine, paragraphLeft, fontSize);
      flowDocumentLeft.PageWidth = flowDocumentRight.PageWidth = CharSize.Width * (600 > longestLine.Length ? 1.205 : 1.3);// longestLine * CharSize.Width;
      return (flowDocumentLeft, flowDocumentRight);
    }

    private static void AddCompensationLine(string line, FlowDocument fdWithText, FlowDocument fdBlank, SolidColorBrush brushFailForeground, SolidColorBrush backgroundBrush, int longestLine)
    {
      Paragraph paraWithText = new Paragraph() { Background = backgroundBrush };
      Paragraph paraBlankLine = new Paragraph();
      paraWithText.Inlines.Add(new Run(line) { Foreground = brushFailForeground });

      double lineHeight = paraWithText.Inlines.Last().FontSize * paraWithText.Inlines.Last().FontFamily.LineSpacing * 0.88;
      Rectangle r = rectBlankLine;
      r.Height = CharSize.Height;

      paraBlankLine.Inlines.Add(new Rectangle() { Fill = rectBlankLine.Fill, Height = lineHeight, Width = 1000, IsEnabled = false, Focusable = false });

      fdWithText.Blocks.Add(paraWithText);
      fdBlank.Blocks.Add(paraBlankLine);
    }

    private static Size MeasureString(string candidate, Paragraph paragraph, double fontSize)
    {
      FormattedText formattedText = new FormattedText(
          candidate,
          CultureInfo.CurrentCulture,
          FlowDirection.LeftToRight,
          new Typeface(paragraph.FontFamily, paragraph.FontStyle, paragraph.FontWeight, paragraph.FontStretch),
          fontSize,
          Brushes.Black
          //,
          //VisualTreeHelper.GetDpi(v).PixelsPerDip
          );
      return new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
    }
  }
}
