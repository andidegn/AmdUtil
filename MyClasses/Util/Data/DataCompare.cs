using AMD.Util.Extensions;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AMD.Util.Data
{
  public class DataCompare : INotifyPropertyChanged
  {
    #region Interface OnPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ObservableCollection<OverviewLine> lines;
    public ObservableCollection<OverviewLine> Lines
    {
      get
      {
        return lines;
      }
      private set
      {
        lines = value;
        OnPropertyChanged();
      }
    }

    private FlowDocument documentLeft;
    public FlowDocument DocumentLeft
    {
      get
      {
        return documentLeft;
      }
      private set
      {
        documentLeft = value;
        OnPropertyChanged();
      }
    }

    private FlowDocument documentRight;
    public FlowDocument DocumentRight
    {
      get
      {
        return documentRight;
      }
      private set
      {
        documentRight = value;
        OnPropertyChanged();
      }
    }

    private int lineCount;
    public int LineCount
    {
      get
      {
        return lineCount;
      }
      private set
      {
        lineCount = value;
        OnPropertyChanged();
      }
    }

    private int overviewWidth;
    public int OverviewWidth
    {
      get
      {
        return overviewWidth;
      }
      set
      {
        overviewWidth = value;
        OnPropertyChanged();
      }
    }

    private int numberOfUnequalLines;
    public int NumberOfUnequalLines
    {
      get
      {
        return numberOfUnequalLines;
      }
      private set
      {
        numberOfUnequalLines = value;
        OnPropertyChanged();
      }
    }
    #endregion // Interface OnPropertyChanged

    private Size CharSize;
    private double lineHeight;

    public DataCompare()
    {
      OverviewWidth = 20;
      lineHeight = 13;
      Lines = new ObservableCollection<OverviewLine>();
      Clear();
    }

    public void Clear()
    {
      Lines.Clear();
      LineCount = 0;
      DocumentLeft?.Blocks?.Clear();
      DocumentRight?.Blocks?.Clear();
    }


    /// <summary>
    /// Compares two strings and returns left and right documents, which can be loaded into RichTextBox
    /// </summary>
    /// <param name="leftData"></param>
    /// <param name="rightData"></param>
    /// <param name="formatAsMemoryView"></param>
    /// <param name="brushDefaultForeground"></param>
    /// <param name="brushDiffForeground"></param>
    /// <param name="brushDiffBackground"></param>
    /// <param name="brushBlankLine"></param>
    /// <param name="charsToSearchForOffset">The number of characters to search into a row in order to determine if they are comparable</param>
    /// <param name="lineIndexBeforeLineMatch">The number of rows to search for a matching row before detecting it as a difference</param>
    /// <param name="fontSize">The size of the font used in the destination control. Ex RichTextBox.FontSize</param>
    /// <param name="log"></param>
    /// <returns></returns>
    public (FlowDocument fdLeft, FlowDocument fdRight) CompareBytes
      (
        IEnumerable<byte> leftData,
        IEnumerable<byte> rightData,
        bool formatAsMemoryView,
        Brush brushDefaultForeground,
        Brush brushDiffForeground,
        Brush brushDiffBackground,
        Brush brushBlankLine,
        int charsToSearchForOffset,
        int lineIndexBeforeLineMatch,
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
      return CompareStrings(strLeft, strRight, brushDefaultForeground, brushDiffForeground, brushDiffBackground, brushBlankLine, charsToSearchForOffset, lineIndexBeforeLineMatch, fontSize, log);
    }

    /// <summary>
    /// Compares two strings and returns left and right documents, which can be loaded into RichTextBox
    /// </summary>
    /// <param name="leftText"></param>
    /// <param name="rightText"></param>
    /// <param name="brushDefaultForeground"></param>
    /// <param name="brushDiffForeground"></param>
    /// <param name="brushDiffBackground"></param>
    /// <param name="brushBlankLine"></param>
    /// <param name="charsToSearchForOffset">The number of characters to search into a line in order to determine if they are comparable</param>
    /// <param name="lineIndexBeforeLineMatch">The number of lines to search for a matching line before detecting it as a difference</param>
    /// <param name="fontSize">The size of the font used in the destination control. Ex RichTextBox.FontSize</param>
    /// <param name="log"></param>
    /// <returns></returns>
    public (FlowDocument fdLeft, FlowDocument fdRight) CompareStrings
      (
        string leftText,
        string rightText,
        Brush brushDefaultForeground,
        Brush brushDiffForeground,
        Brush brushDiffBackground,
        Brush brushBlankLine,
        int charsToSearchForOffset,
        int lineIndexBeforeLineMatch,
        double fontSize,
        LogWriter log = null
      )
    {
      log?.PrintDebug("Starting parsing...");
      Stopwatch sw = Stopwatch.StartNew();

      FlowDocument leftDoc = new FlowDocument();
      FlowDocument rightDoc = new FlowDocument();
      ObservableCollection<OverviewLine> lines = new ObservableCollection<OverviewLine>();
      FontFamily fontFamily = Application.Current.TryFindResource("CodeFont") as FontFamily;
      leftDoc.FontFamily = rightDoc.FontFamily = fontFamily;

      leftText = leftText.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n').Replace("\t", "  ");
      rightText = rightText.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n').Replace("\t", "  ");

      if (string.IsNullOrWhiteSpace(leftText) || string.IsNullOrWhiteSpace(rightText))
      {
        brushDiffForeground = brushDefaultForeground;
        brushDiffBackground = Brushes.Transparent;
      }

      string[] linesLeft = leftText.Split(new string[] { "\n" }, StringSplitOptions.None);
      string[] linesRight = rightText.Split(new string[] { "\n" }, StringSplitOptions.None);

      string lineLeft = string.Empty, lineRight = string.Empty;

      bool offsetTried = false, skipAddToDocument = false, leftEmpty = string.IsNullOrWhiteSpace(leftText), rightEmpty = string.IsNullOrWhiteSpace(rightText);
      bool lastEqual = true;
      bool changed = false;
      bool lineNotEqual = false;
      bool nonSpaceCharFound = false;
      int r, l, lOffsetIndex = -1, rOffsetIndex = -1;

      int maxLineLength = -1;
      int spaceCount = 0;

      string longestLineLeft = linesLeft.Where(x => x.Length == linesLeft.Max(y => y.Length)).FirstOrDefault();
      string longestLineRight = linesRight.Where(x => x.Length == linesRight.Max(y => y.Length)).FirstOrDefault();

      string longestLine = longestLineLeft.Length > longestLineRight.Length ? longestLineLeft : longestLineRight;

      char charLeft = '\0';
      char charRight = '\0';
      bool equal = true;
      bool hasLConsecutiveLines = false;
      bool hasRConsecutiveLines = false;

      LineCount = 0;

      Paragraph paragraphLeft = new Paragraph() { Margin = new Thickness(0), LineHeight = lineHeight, FontFamily = fontFamily };
      Paragraph paragraphRight = new Paragraph() { Margin = new Thickness(0), LineHeight = lineHeight, FontFamily = fontFamily };
      CharSize = MeasureString(longestLine, paragraphLeft, fontSize);

      for (l = 0, r = 0; l < linesLeft.Length || r < linesRight.Length; l++, r++)
      {
        lastEqual = true;
        changed = false;
        lineNotEqual = false;
        offsetTried = false;
        skipAddToDocument = false;

        if (45 == l)
        {

        }
        maxLineLength = Math.Max(linesLeft[Math.Min(l, linesLeft.Length - 1)].Length, linesRight[Math.Min(r, linesRight.Length - 1)].Length);

        paragraphLeft = new Paragraph() { Margin = new Thickness(0), LineHeight = lineHeight, FontFamily = fontFamily };
        paragraphRight = new Paragraph() { Margin = new Thickness(0), LineHeight = lineHeight, FontFamily = fontFamily };

        if (0 < lOffsetIndex)
        {
          try
          {
            do
            {
              AddCompensationLine(linesLeft[l], leftDoc, rightDoc, brushDiffForeground, brushDiffBackground, brushBlankLine);
              lines.Add(new OverviewLine(LineCount++, OverviewLine.Side.Left, OverviewWidth) { LeftLine = linesLeft[l] });
              NumberOfUnequalLines++;
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
              AddCompensationLine(linesRight[r], rightDoc, leftDoc, brushDiffForeground, brushDiffBackground, brushBlankLine);
              lines.Add(new OverviewLine(LineCount++, OverviewLine.Side.Right, OverviewWidth) { RightLine = linesRight[r] });
              NumberOfUnequalLines++;
            } while (++r < rOffsetIndex);
            rOffsetIndex = -1;
          }
          catch (Exception ex)
          {
            log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}");
          }
        }

        if (rightEmpty || linesRight.Length - 1 < r)
        {
          try
          {
            if (linesLeft.Length - 1 < l)
            {
              break;
            }
            AddCompensationLine(linesLeft[l], leftDoc, rightDoc, brushDiffForeground, brushDiffBackground, brushBlankLine);
            lines.Add(new OverviewLine(LineCount++, OverviewLine.Side.Left, OverviewWidth) { LeftLine = linesLeft[l] });
            NumberOfUnequalLines++;
            continue;
          }
          catch (Exception ex)
          {
            log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}");
          }
        }
        else if (leftEmpty || linesLeft.Length - 1 < l)
        {
          try
          {
            if (linesRight.Length - 1 < r)
            {
              break;
            }
            AddCompensationLine(linesRight[r], rightDoc, leftDoc, brushDiffForeground, brushDiffBackground, brushBlankLine);
            lines.Add(new OverviewLine(LineCount++, OverviewLine.Side.Right, OverviewWidth) { RightLine = linesRight[r] });
            NumberOfUnequalLines++;
            continue;
          }
          catch (Exception ex)
          {
            log?.PrintException(ex, $"Variables: l:{l}, r:{r}, lOffsetIndex:{lOffsetIndex}, rOffsetIndex:{rOffsetIndex}");
          }
        }
        else
        {
          lineLeft = linesLeft[l];
          lineRight = linesRight[r];
          spaceCount = 0;
          nonSpaceCharFound = false;
          for (int j = 0, lastIndex = 0; j < maxLineLength; j++)
          {
            charLeft = lineLeft.Length > j ? lineLeft[j] : '\0';
            charRight = lineRight.Length > j ? lineRight[j] : '\0';
            equal = charLeft == charRight;
            if (!equal)
            {
              lineNotEqual = true;
            }
            else if (Char.IsWhiteSpace(charLeft) && !nonSpaceCharFound)
            {
              spaceCount++;
            }
            else
            {
              nonSpaceCharFound = true;
            }

            changed = equal != lastEqual;

            try
            {
              if (!offsetTried && (spaceCount + lineIndexBeforeLineMatch) > j && !equal)
              {
                offsetTried = true;
                lOffsetIndex = rOffsetIndex = 0;

                if (0 < lineRight.Length)
                {
                  lOffsetIndex = Array.IndexOf(linesLeft.SubArray(l, linesLeft.Length - l - 1), linesLeft.SubArray(l, linesLeft.Length - l - 1).Where(x => x.Equals(lineRight, StringComparison.Ordinal)).FirstOrDefault());
                  if (-1 == lOffsetIndex)
                  {
                    lOffsetIndex = Array.IndexOf(linesLeft.SubArray(l, linesLeft.Length - l - 1), linesLeft.SubArray(l, linesLeft.Length - l - 1).Where(x => x.StartsWith(lineRight.Substring(0, Math.Min(j + charsToSearchForOffset, lineRight.Length)), StringComparison.Ordinal)).FirstOrDefault());
                  }
                }

                if (0 < lineLeft.Length)
                {
                  rOffsetIndex = Array.IndexOf(linesRight.SubArray(r, linesRight.Length - r - 1), linesRight.SubArray(r, linesRight.Length - r - 1).Where(x => x.Equals(lineLeft, StringComparison.Ordinal)).FirstOrDefault());
                  if (-1 == rOffsetIndex)
                  {
                    rOffsetIndex = Array.IndexOf(linesRight.SubArray(r, linesRight.Length - r - 1), linesRight.SubArray(r, linesRight.Length - r - 1).Where(x => x.StartsWith(lineLeft.Substring(0, Math.Min(j + charsToSearchForOffset, lineLeft.Length)), StringComparison.Ordinal)).FirstOrDefault());
                  }
                }

                if ((0 < lOffsetIndex || 0 < rOffsetIndex) && lineIndexBeforeLineMatch >= lOffsetIndex && lineIndexBeforeLineMatch >= rOffsetIndex)
                {
                  if (0 < lOffsetIndex && 0 < rOffsetIndex)
                  {
                    if (lOffsetIndex < rOffsetIndex)
                    {
                      rOffsetIndex = 0;
                    }
                    else
                    {
                      lOffsetIndex = 0;
                    }
                  }

                  if (0 < lOffsetIndex)
                  {
                    lOffsetIndex += l;
                  }
                  else
                  {
                    rOffsetIndex += r;
                  }

                  r--;
                  l--;
                  skipAddToDocument = true;
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
                // ToDo: Make the text brushes bindable
                //Binding binding = new Binding((j == maxLineLength && !changed ? equal : lastEqual) ? nameof(brushDefaultForeground) : nameof(brushDiffForeground));
                //Run rLeft = new Run(linesLeft[l].Substring(lastIndex, Math.Min(j - lastIndex, linesLeft[l].Length - lastIndex)));// { Foreground = (j == maxLineLength && !changed ? equal : lastEqual) ? brushDefaultForeground : brushDiffForeground }
                //Run rRight = new Run(linesRight[r].Substring(lastIndex, Math.Min(j - lastIndex, linesRight[r].Length - lastIndex)));// { Foreground = (j == maxLineLength && !changed ? equal : lastEqual) ? brushDefaultForeground : brushDiffForeground }
                //rLeft.SetBinding(Run.ForegroundProperty, binding);
                //rRight.SetBinding(Run.ForegroundProperty, binding);
                //paragraphLeft.Inlines.Add(rLeft);
                //paragraphRight.Inlines.Add(rRight);

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
          NumberOfUnequalLines++;
          lines.Add(new OverviewLine(LineCount++, lineNotEqual ? OverviewLine.Side.Both : OverviewLine.Side.None, OverviewWidth) { LeftLine = lineLeft, RightLine = lineRight });
          leftDoc.Blocks.Add(paragraphLeft);
          rightDoc.Blocks.Add(paragraphRight);
          lineLeft = lineRight = string.Empty;
        }
        if (linesLeft.Length - 1 < l && linesRight.Length - 1 < r)
        {
          break;
        }
      }
      log?.WriteToLog(LogMsgType.Measurement, $"Parsing done: {sw.Elapsed.ToString(@"ss\.fff")}");
      leftDoc.PageWidth = rightDoc.PageWidth = CharSize.Width + 10.5;
      DocumentLeft = leftDoc;
      DocumentRight = rightDoc;
      log?.PrintDebug("Saving compare map list...");
      sw.Restart();

      /* For debug only
      StringBuilder sb = new StringBuilder();
      int idx = 0;
      foreach (OverviewLine item in lines)
      {
        while (item.LineNumber != idx++)
        {
          sb.AppendLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        sb.Append($"({item.LineNumber}) ".PadLeft(10));
        sb.Append($"{item.LeftLine}".PadLeft(100));
        sb.Append(item.To.X == 0 ? " - " : " ! ");
        sb.AppendLine(item.RightLine);
      }
      File.WriteAllText(@"c:\isic\_compare_lines.txt", sb.ToString());
      */

      Lines = lines;
      log?.WriteToLog(LogMsgType.Measurement, $"Done: {sw.Elapsed.ToString(@"ss\.fff")}");
      return (leftDoc, rightDoc);
    }

    private void AddCompensationLine(string line, FlowDocument fdWithText, FlowDocument fdBlank, Brush brushFailForeground, Brush backgroundBrush, Brush brushBlankLine)
    {
      double width = CharSize.Width;
      GeometryGroup gg = new GeometryGroup();
      gg.Children.Add(new RectangleGeometry() { RadiusX = 7, RadiusY = 7 });
      gg.Children.Add(new LineGeometry(new Point(0, 3.5), new Point(3.5, 0)));
      gg.Children.Add(new LineGeometry(new Point(3.5, 7), new Point(7, 3.5)));
      Paragraph paraWithText = new Paragraph() { Margin = new Thickness(0), LineHeight = lineHeight, FontFamily = fdWithText.FontFamily, Background = backgroundBrush };
      Paragraph paraBlankLine = new Paragraph()
      {
        Margin = new Thickness(0),
        LineHeight = lineHeight,
        FontFamily = fdWithText.FontFamily,
        Background = new DrawingBrush()
        {
          Stretch = Stretch.None,
          TileMode = TileMode.Tile,
          Viewport = new Rect(0, 0, 7 / width, 0.5),
          Drawing = new GeometryDrawing()
          {
            Brush = Brushes.Transparent,
            Geometry = gg,
            Pen = new Pen(brushBlankLine, 0.3)
            {
              EndLineCap = PenLineCap.Square,
              StartLineCap = PenLineCap.Square
            }
          }
        }
      };
      paraWithText.Inlines.Add(new Run(line) { Foreground = brushFailForeground });

      paraBlankLine.Inlines.Add(new Run());
      //paraBlankLine.Inlines.Add(new Rectangle()
      //{
      //  Width = width,
      //  Height = paraWithText.FontSize * paraWithText.FontFamily.LineSpacing,
      //  IsEnabled = false,
      //  Focusable = false,
      //  Fill = new DrawingBrush()
      //  {
      //    Stretch = Stretch.None,
      //    TileMode = TileMode.Tile,
      //    Viewport = new Rect(0, 0, 7 / width, 0.5),
      //    Drawing = new GeometryDrawing()
      //    {
      //      Brush = Brushes.Gray,
      //      Geometry = gg,
      //      Pen = new Pen(Brushes.Black, 0.3)
      //      {
      //        EndLineCap = PenLineCap.Square,
      //        StartLineCap = PenLineCap.Square
      //      }
      //    }
      //  }
      //});

      fdWithText.Blocks.Add(paraWithText);
      fdBlank.Blocks.Add(paraBlankLine);
    }

    private Size MeasureString(string candidate, Paragraph paragraph, double fontSize)
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

    public class OverviewLine : IComparable
    {
      internal enum Side
      {
        Left,
        Right,
        Both,
        None
      }

      public double LineNumber { get; set; }
      public Point From { get; set; }
      public Point To { get; set; }
      public string LeftLine { get; set; }
      public string RightLine { get; set; }

      internal OverviewLine(double lineNumber, Side side, double width)
      {
        double olstart = 0;
        double orstart = width / 2;

        LineNumber = lineNumber;

        switch (side)
        {
          case Side.Left:
            From = new Point(0, lineNumber);
            To = new Point(0 + orstart, lineNumber);
            break;
          case Side.Right:
            From = new Point(orstart, lineNumber);
            To = new Point(width, lineNumber);
            break;
          case Side.Both:
            From = new Point(olstart, lineNumber);
            To = new Point(width, lineNumber);
            break;
          case Side.None:
          default:
            From = new Point();
            To = new Point();
            break;
        }
      }

      internal OverviewLine(double fromX, double fromY, double toX, double toY)
      {
        From = new Point(fromX, fromY);
        To = new Point(toX, toY);
      }

      public override string ToString()
      {
        return $"Line: {LineNumber} {From.X}-{To.X}";
      }

      public int CompareTo(object obj)
      {
        return obj is OverviewLine ol && ol != null ? LineNumber.CompareTo(ol.LineNumber) : -1;
      }
    }
  }

  public class DataCompareOptimized : INotifyPropertyChanged
  {
    #region Interface OnPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ObservableCollection<OverviewLine> lines;
    public ObservableCollection<OverviewLine> Lines
    {
      get => lines;
      private set
      {
        lines = value;
        OnPropertyChanged();
      }
    }

    private FlowDocument documentLeft;
    public FlowDocument DocumentLeft
    {
      get => documentLeft;
      private set
      {
        documentLeft = value;
        OnPropertyChanged();
      }
    }

    private FlowDocument documentRight;
    public FlowDocument DocumentRight
    {
      get => documentRight;
      private set
      {
        documentRight = value;
        OnPropertyChanged();
      }
    }

    private int lineCount;
    public int LineCount
    {
      get => lineCount;
      private set
      {
        lineCount = value;
        OnPropertyChanged();
      }
    }

    private int overviewWidth;
    public int OverviewWidth
    {
      get => overviewWidth;
      set
      {
        overviewWidth = value;
        OnPropertyChanged();
      }
    }

    private int numberOfUnequalLines;
    public int NumberOfUnequalLines
    {
      get => numberOfUnequalLines;
      private set
      {
        numberOfUnequalLines = value;
        OnPropertyChanged();
      }
    }
    #endregion

    private Size CharSize;
    private double lineHeight;

    public DataCompareOptimized()
    {
      OverviewWidth = 20;
      lineHeight = 13;
      Lines = new ObservableCollection<OverviewLine>();
      Clear();
    }

    public void Clear()
    {
      Lines.Clear();
      LineCount = 0;
      DocumentLeft?.Blocks?.Clear();
      DocumentRight?.Blocks?.Clear();
    }

    public (FlowDocument fdLeft, FlowDocument fdRight) CompareBytes(
        IEnumerable<byte> leftData,
        IEnumerable<byte> rightData,
        bool formatAsMemoryView,
        Brush brushDefaultForeground,
        Brush brushDiffForeground,
        Brush brushDiffBackground,
        Brush brushBlankLine,
        int charsToSearchForOffset,
        int lineIndexBeforeLineMatch,
        double fontSize,
        LogWriter log = null)
    {
      string strLeft = leftData != null
          ? (formatAsMemoryView
              ? StringFormatHelper.GetFormattedMemoryString(0, leftData.ToArray().GetNullableUIntArray())
              : leftData.ToArray().GetString())
          : string.Empty;

      string strRight = rightData != null
          ? (formatAsMemoryView
              ? StringFormatHelper.GetFormattedMemoryString(0, rightData.ToArray().GetNullableUIntArray())
              : rightData.ToArray().GetString())
          : string.Empty;

      return CompareStrings(strLeft, strRight, brushDefaultForeground, brushDiffForeground, brushDiffBackground, brushBlankLine, charsToSearchForOffset, lineIndexBeforeLineMatch, fontSize, log);
    }

    public (FlowDocument fdLeft, FlowDocument fdRight) CompareStrings(
        string leftText,
        string rightText,
        Brush brushDefaultForeground,
        Brush brushDiffForeground,
        Brush brushDiffBackground,
        Brush brushBlankLine,
        int charsToSearchForOffset,
        int lineIndexBeforeLineMatch,
        double fontSize,
        LogWriter log = null)
    {
      log?.PrintDebug("Starting parsing...");
      Stopwatch sw = Stopwatch.StartNew();

      FlowDocument leftDoc = new FlowDocument();
      FlowDocument rightDoc = new FlowDocument();

      FontFamily fontFamily = Application.Current.TryFindResource("CodeFont") as FontFamily;
      leftDoc.FontFamily = rightDoc.FontFamily = fontFamily;

      leftText = NormalizeString(leftText);
      rightText = NormalizeString(rightText);

      bool leftEmpty = string.IsNullOrWhiteSpace(leftText);
      bool rightEmpty = string.IsNullOrWhiteSpace(rightText);
      if (leftEmpty || rightEmpty)
      {
        brushDiffForeground = brushDefaultForeground;
        brushDiffBackground = Brushes.Transparent;
      }

      string[] linesLeft = leftText.Split('\n');
      string[] linesRight = rightText.Split('\n');

      string longestLine = GetLongestLine(linesLeft, linesRight);
      CharSize = MeasureString(longestLine, new Paragraph { FontFamily = fontFamily }, fontSize);

      Paragraph paragraphLeft = CreateParagraph(fontFamily);
      Paragraph paragraphRight = CreateParagraph(fontFamily);

      for (int l = 0, r = 0; l < linesLeft.Length || r < linesRight.Length; l++, r++)
      {
        if (l >= linesLeft.Length || r >= linesRight.Length)
        {
          HandleOutOfBoundsLines(l, r, linesLeft, linesRight, leftDoc, rightDoc, brushDiffForeground, brushDiffBackground, brushBlankLine);
          continue;
        }

        string lineLeft = linesLeft[l];
        string lineRight = linesRight[r];

        CompareAndAddLines(lineLeft, lineRight, paragraphLeft, paragraphRight, brushDefaultForeground, brushDiffForeground, brushDiffBackground, log);
        leftDoc.Blocks.Add(paragraphLeft);
        rightDoc.Blocks.Add(paragraphRight);
      }

      log?.WriteToLog(LogMsgType.Measurement, $"Parsing done: {sw.Elapsed:hh\\:mm\\:ss\\.fff}");
      leftDoc.PageWidth = rightDoc.PageWidth = CharSize.Width + 10.5;

      return (leftDoc, rightDoc);
    }

    private string NormalizeString(string input)
    {
      return input?
          .Replace("\r\n", "\n")
          .Replace("\n\r", "\n")
          .Replace('\r', '\n')
          .Replace("\t", "  ") ?? string.Empty;
    }

    private string GetLongestLine(string[] linesLeft, string[] linesRight)
    {
      string longestLineLeft = linesLeft.OrderByDescending(line => line.Length).FirstOrDefault() ?? string.Empty;
      string longestLineRight = linesRight.OrderByDescending(line => line.Length).FirstOrDefault() ?? string.Empty;
      return longestLineLeft.Length > longestLineRight.Length ? longestLineLeft : longestLineRight;
    }

    private Paragraph CreateParagraph(FontFamily fontFamily)
    {
      return new Paragraph
      {
        Margin = new Thickness(0),
        LineHeight = lineHeight,
        FontFamily = fontFamily
      };
    }

    private void CompareAndAddLines(
        string lineLeft,
        string lineRight,
        Paragraph paragraphLeft,
        Paragraph paragraphRight,
        Brush brushDefaultForeground,
        Brush brushDiffForeground,
        Brush brushDiffBackground,
        LogWriter log)
    {
      bool lineNotEqual = false;
      for (int i = 0; i < Math.Max(lineLeft.Length, lineRight.Length); i++)
      {
        char charLeft = i < lineLeft.Length ? lineLeft[i] : '\0';
        char charRight = i < lineRight.Length ? lineRight[i] : '\0';
        bool equal = charLeft == charRight;

        Brush foreground = equal ? brushDefaultForeground : brushDiffForeground;
        paragraphLeft.Inlines.Add(new Run(charLeft.ToString()) { Foreground = foreground });
        paragraphRight.Inlines.Add(new Run(charRight.ToString()) { Foreground = foreground });

        if (!equal) lineNotEqual = true;
      }

      if (lineNotEqual)
      {
        paragraphLeft.Background = brushDiffBackground;
        paragraphRight.Background = brushDiffBackground;
      }
    }

    private void HandleOutOfBoundsLines(
        int l,
        int r,
        string[] linesLeft,
        string[] linesRight,
        FlowDocument leftDoc,
        FlowDocument rightDoc,
        Brush brushDiffForeground,
        Brush brushDiffBackground,
        Brush brushBlankLine)
    {
      if (l >= linesLeft.Length)
      {
        AddCompensationLine(linesRight[r], rightDoc, leftDoc, brushDiffForeground, brushDiffBackground, brushBlankLine);
      }
      else if (r >= linesRight.Length)
      {
        AddCompensationLine(linesLeft[l], leftDoc, rightDoc, brushDiffForeground, brushDiffBackground, brushBlankLine);
      }
    }

    private void AddCompensationLine(string line, FlowDocument fdWithText, FlowDocument fdBlank, Brush brushFailForeground, Brush backgroundBrush, Brush brushBlankLine)
    {
      Paragraph paraWithText = CreateParagraph(fdWithText.FontFamily);
      Paragraph paraBlankLine = CreateParagraph(fdWithText.FontFamily);
      paraWithText.Background = backgroundBrush;
      paraWithText.Inlines.Add(new Run(line) { Foreground = brushFailForeground });

      fdWithText.Blocks.Add(paraWithText);
      fdBlank.Blocks.Add(paraBlankLine);
    }

    private Size MeasureString(string candidate, Paragraph paragraph, double fontSize)
    {
      FormattedText formattedText = new FormattedText(
          candidate,
          CultureInfo.CurrentCulture,
          FlowDirection.LeftToRight,
          new Typeface(paragraph.FontFamily, paragraph.FontStyle, paragraph.FontWeight, paragraph.FontStretch),
          fontSize,
          Brushes.Black);
      return new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
    }

    public class OverviewLine : IComparable
    {
      internal enum Side
      {
        Left,
        Right,
        Both,
        None
      }

      public double LineNumber { get; set; }
      public Point From { get; set; }
      public Point To { get; set; }
      public string LeftLine { get; set; }
      public string RightLine { get; set; }

      internal OverviewLine(double lineNumber, Side side, double width)
      {
        double olstart = 0;
        double orstart = width / 2;

        LineNumber = lineNumber;

        switch (side)
        {
          case Side.Left:
            From = new Point(0, lineNumber);
            To = new Point(0 + orstart, lineNumber);
            break;
          case Side.Right:
            From = new Point(orstart, lineNumber);
            To = new Point(width, lineNumber);
            break;
          case Side.Both:
            From = new Point(olstart, lineNumber);
            To = new Point(width, lineNumber);
            break;
          case Side.None:
          default:
            From = new Point();
            To = new Point();
            break;
        }
      }

      public int CompareTo(object obj)
      {
        return obj is OverviewLine ol ? LineNumber.CompareTo(ol.LineNumber) : -1;
      }
    }
  }
}
