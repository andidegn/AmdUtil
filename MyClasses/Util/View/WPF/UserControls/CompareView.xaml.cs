using AMD.Util.Data;
using AMD.Util.Extensions;
using AMD.Util.Extensions.WPF;
using AMD.Util.HID;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for CompareView.xaml
  /// </summary>
  public partial class CompareView : UserControl
  {
    public CornerRadius CornerRadius
    {
      get { return (CornerRadius)GetValue(CornerRadiusProperty); }
      set { SetValue(CornerRadiusProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CompareView), new PropertyMetadata(new CornerRadius(5)));

    public Brush InnerBorderBrush
    {
      get { return (Brush)GetValue(InnerBorderBrushProperty); }
      set { SetValue(InnerBorderBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for InnerBorderBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty InnerBorderBrushProperty =
        DependencyProperty.Register("InnerBorderBrush", typeof(Brush), typeof(CompareView), new PropertyMetadata(Brushes.Gray));

    public Visibility CompareMapVisibility
    {
      get { return (Visibility)GetValue(CompareMapVisibilityProperty); }
      set { SetValue(CompareMapVisibilityProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CompareMapVisibility.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CompareMapVisibilityProperty =
        DependencyProperty.Register("CompareMapVisibility", typeof(Visibility), typeof(CompareView), new PropertyMetadata(Visibility.Visible));

    public Visibility LineNumberVisibility
    {
      get { return (Visibility)GetValue(LineNumberVisibilityProperty); }
      set { SetValue(LineNumberVisibilityProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LineNumberVisibility.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LineNumberVisibilityProperty =
        DependencyProperty.Register("LineNumberVisibility", typeof(Visibility), typeof(CompareView), new PropertyMetadata(Visibility.Visible));

    public Brush LineNumberBackground
    {
      get { return (Brush)GetValue(LineNumberBackgroundProperty); }
      set { SetValue(LineNumberBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LineNumberBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LineNumberBackgroundProperty =
        DependencyProperty.Register("LineNumberBackground", typeof(Brush), typeof(CompareView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x20, 0xFF, 0xAF, 0x00))));

    public Brush ControlBackground
    {
      get { return (Brush)GetValue(ControlBackgroundProperty); }
      set { SetValue(ControlBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ControlBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ControlBackgroundProperty =
        DependencyProperty.Register("ControlBackground", typeof(Brush), typeof(CompareView), new PropertyMetadata(Brushes.White));

    public Brush DiffForeground
    {
      get { return (Brush)GetValue(DiffForegroundProperty); }
      set { SetValue(DiffForegroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DiffForeground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DiffForegroundProperty =
        DependencyProperty.Register("DiffForeground", typeof(Brush), typeof(CompareView), new PropertyMetadata(Brushes.Red));

    public Brush DiffBackground
    {
      get { return (Brush)GetValue(DiffBackgroundProperty); }
      set { SetValue(DiffBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DiffBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DiffBackgroundProperty =
        DependencyProperty.Register("DiffBackground", typeof(Brush), typeof(CompareView), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x20, 0xFF, 0x00, 0x00))));


    public Brush MissingLine
    {
      get { return (Brush)GetValue(MissingLineProperty); }
      set { SetValue(MissingLineProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MissingLine.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MissingLineProperty =
        DependencyProperty.Register("MissingLine", typeof(Brush), typeof(CompareView), new PropertyMetadata(Brushes.Gray));

    public Brush CompareMapBackground
    {
      get { return (Brush)GetValue(CompareMapBackgroundProperty); }
      set { SetValue(CompareMapBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CompareMapBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CompareMapBackgroundProperty =
        DependencyProperty.Register("CompareMapBackground", typeof(Brush), typeof(CompareView), new PropertyMetadata(Brushes.White));

    public Brush CompareMapBorderBrush
    {
      get { return (Brush)GetValue(CompareMapBorderBrushProperty); }
      set { SetValue(CompareMapBorderBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CompareMapBorderBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CompareMapBorderBrushProperty =
        DependencyProperty.Register("CompareMapBorderBrush", typeof(Brush), typeof(CompareView), new PropertyMetadata(Brushes.DarkGray));

    public Brush CompareMapThumbBorderBrush
    {
      get { return (Brush)GetValue(CompareMapThumbBorderBrushProperty); }
      set { SetValue(CompareMapThumbBorderBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CompareMapThumbBorderBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CompareMapThumbBorderBrushProperty =
        DependencyProperty.Register("CompareMapThumbBorderBrush", typeof(Brush), typeof(CompareView), new PropertyMetadata(Brushes.Gray));



    public string LineUnderMouse
    {
      get { return (string)GetValue(LineUnderMouseProperty); }
      set { SetValue(LineUnderMouseProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LineUnderMouse.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LineUnderMouseProperty =
        DependencyProperty.Register("LineUnderMouse", typeof(string), typeof(CompareView), new PropertyMetadata(string.Empty));

    public DataCompare DataCompare
    {
      get { return (DataCompare)GetValue(DataCompareProperty); }
      set { SetValue(DataCompareProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DataCompare.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DataCompareProperty =
        DependencyProperty.Register("DataCompare", typeof(DataCompare), typeof(CompareView), new PropertyMetadata(null));









    public string CurrentLine
    {
      get
      {
        string retVal = string.Empty;
        if (rtbLeft.IsKeyboardFocused)
        {
          retVal = rtbLeft.GetCurrentLine();
        }
        else if (rtbRight.IsKeyboardFocused)
        {
          retVal = rtbRight.GetCurrentLine();
        }
        return retVal;
      }
    }



    private LogWriter log;

    //private DataCompare DataCompare;
    private DataCompareOptimized dataCompareOptimized;

    private FlowDocument fdLeft;
    private FlowDocument fdRight;

    private Brush defaultForeground;
    private Brush diffForeground;
    private Brush diffBackground;
    private Brush blankLine;

    private int charsToSearchForOffset;
    private int lineIndexBeforeLineMatch;

    public CompareView()
    {
      log = LogWriter.Instance;
      fdLeft = new FlowDocument();
      fdRight = new FlowDocument();
      DataCompare = new DataCompare();
      dataCompareOptimized = new DataCompareOptimized();

      InitializeComponent();
    }

    #region Byte Collection Compare
    public void Compare(IEnumerable<byte> dataLeft, IEnumerable<byte> dataRight, bool formatAsMemoryView, int charsToSearchForOffset = 5, int lineIndexBeforeLineMatch = 4)
    {
      Compare(dataLeft, dataRight, formatAsMemoryView, Foreground, DiffForeground, DiffBackground, MissingLine, charsToSearchForOffset, lineIndexBeforeLineMatch);
    }

    public void Compare(IEnumerable<byte> dataLeft, IEnumerable<byte> dataRight, bool formatAsMemoryView, Brush defaultForeground, Brush diffForeground, Brush diffBackground, Brush blankLine, int charsToSearchForOffset = 5, int lineIndexBeforeLineMatch = 4)
    {
      Clear();

      UpdateBrushes(defaultForeground, diffForeground, diffBackground, blankLine);
      this.charsToSearchForOffset = charsToSearchForOffset;
      this.lineIndexBeforeLineMatch = lineIndexBeforeLineMatch;

      (fdLeft, fdRight) = DataCompare.CompareBytes(dataLeft, dataRight, formatAsMemoryView, defaultForeground, diffForeground, diffBackground, blankLine, charsToSearchForOffset, lineIndexBeforeLineMatch, rtbLeft.FontSize, log);
      PostCompare(fdLeft.GetRawText(), fdRight.GetRawText());
    }
    #endregion // Byte Collection Compare

    #region String Compare
    public void Compare(string strLeft, string strRight, int charsToSearchForOffset = 5, int lineIndexBeforeLineMatch = 4)
    {
      Compare(strLeft, strRight, Foreground, DiffForeground, DiffBackground, MissingLine, charsToSearchForOffset, lineIndexBeforeLineMatch);
    }

    public void Compare(string strLeft, string strRight, Brush defaultForeground, Brush diffForeground, Brush diffBackground, Brush blankLine, int charsToSearchForOffset = 5, int lineIndexBeforeLineMatch = 4)
    {
      Clear();

      UpdateBrushes(defaultForeground, diffForeground, diffBackground, blankLine);
      this.charsToSearchForOffset = charsToSearchForOffset;
      this.lineIndexBeforeLineMatch = lineIndexBeforeLineMatch;

      //Stopwatch sw = new Stopwatch();long t1, t2;
      //GC.Collect();
      //sw.Start();
      (fdLeft, fdRight) = DataCompare.CompareStrings(strLeft, strRight, defaultForeground, diffForeground, diffBackground, blankLine, charsToSearchForOffset, lineIndexBeforeLineMatch, rtbLeft.FontSize, log);
      //sw.Stop();
      //log.Print(LogMsgType.Measurement, $"DataCompare: {t1 = sw.ElapsedMilliseconds} ms");
      //GC.Collect();
      //sw.Start();
      //(fdLeft, fdRight) = dataCompareOptimized.CompareStrings(strLeft, strRight, defaultForeground, diffForeground, diffBackground, blankLine, charsToSearchForOffset, lineIndexBeforeLineMatch, rtbLeft.FontSize, log);
      //sw.Stop();
      //log.Print(LogMsgType.Measurement, $"DataCompareOptimized: {t2 = sw.ElapsedMilliseconds} ms");
      PostCompare(strLeft, strRight);
    }
    #endregion // String Compare

    private void UpdateBrushes(Brush defaultForeground, Brush diffForeground, Brush diffBackground, Brush blankLine)
    {
      this.defaultForeground = defaultForeground;
      this.diffForeground = diffForeground;
      this.diffBackground = diffBackground;
      this.blankLine = blankLine;
    }

    public void SwapLeftAndRight()
    {
      string strLeft = rtbRight.Tag.ToString();
      string strRight = rtbLeft.Tag.ToString();

      (fdLeft, fdRight) = DataCompare.CompareStrings(strLeft, strRight, defaultForeground, diffForeground, diffBackground, blankLine, charsToSearchForOffset, lineIndexBeforeLineMatch, rtbLeft.FontSize, log);
      PostCompare(strLeft, strRight);

      rtbLeft.Tag = strLeft;
      rtbRight.Tag = strRight;
    }

    private void PostCompare(string leftOrgText, string rightOrgText)
    {
      rtbLeft.Document = fdLeft;
      rtbRight.Document = fdRight;
      //rtbLeft.Width = 0 < fdLeft.PageWidth ? fdLeft.PageWidth : double.NaN;
      //rtbRight.Width = 0 < fdRight.PageWidth ? fdRight.PageWidth : double.NaN;

      rtbLeft.Tag = leftOrgText;
      rtbRight.Tag = rightOrgText;

      SetWidths();
    }

    public void Clear()
    {
      DataCompare.Clear();
    }

    private void SlCompareMap_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      Slider sl1 = sender as Slider;
      var v1 = sl1.IsMouseOver;
      var v2 = Mouse.LeftButton;
      if (sender is Slider sl && sl.IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed)
      {
        svLeft.ScrollToVerticalOffset(sl.Value);
      }
    }

    private void SvLn_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      e.Handled = true;
    }

    private void Sv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      ScrollViewer sv = null;
      if (sender is ScrollViewer svv && svv != null)
      {
        sv = svv;
      }
      else if (sender is RichTextBox rtb)
      {
        sv = rtb.GetChildOfType<ScrollViewer>();
      }
      if (sv != null)
      {
        string name = sv.Name;
        if (sv != null)
        {
          e.Handled = true;
          if (Modifier.IsShiftDown)
          {
            sv.ScrollToHorizontalOffset(sv.HorizontalOffset - e.Delta / 3);
          }
          else
          {
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta / 3);
            //if (sv.VerticalOffset >= sv.ScrollableHeight || sv.VerticalOffset == 0)
            //{
            //  e.Handled = false;
            //}
          }
        }
      }
    }

    private void Sv_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (!Modifier.IsCtrlDown)
      {
        if (sender == svLeft)
        {
          svRight.ScrollToVerticalOffset(e.VerticalOffset);
          svRight.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
        else if (sender == svRight)
        {
          svLeft.ScrollToVerticalOffset(e.VerticalOffset);
          svLeft.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
        svLeftln.ScrollToVerticalOffset(e.VerticalOffset);
        svRightln.ScrollToVerticalOffset(e.VerticalOffset);
        e.Handled = false;
      }
      else
      {
        if (sender == svLeft)
        {
          svLeftln.ScrollToVerticalOffset(e.VerticalOffset);
        }
        else
        {
          svRightln.ScrollToVerticalOffset(e.VerticalOffset);
        }
      }
      slCompareMap.Value = e.VerticalOffset;
      //if (IsLoaded && !changeFromMap)
      //{
      //  slCompareMap.Value = slCompareMap.Maximum * e.VerticalOffset / svLeft.ScrollableHeight;
      //}
      //if (!Modifier.IsCtrlDown)
      //{
      //  if (sender == svLeft)
      //  {
      //    svRight.ScrollToVerticalOffset(e.VerticalOffset);
      //    svRight.ScrollToHorizontalOffset(e.HorizontalOffset);
      //  }
      //  else
      //  {
      //    svLeft.ScrollToVerticalOffset(e.VerticalOffset);
      //    svLeft.ScrollToHorizontalOffset(e.HorizontalOffset);
      //  }
      //  e.Handled = false;
      //}
    }

    private void CommandBinding_Copy(object sender, ExecutedRoutedEventArgs e)
    {
      if (sender is RichTextBox rtb && !string.IsNullOrWhiteSpace(rtb.Selection.Text))
      {
        StringBuilder sb = new StringBuilder();
        string originalText = (rtb.Tag as string).Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n').Replace("\t", "  ");
        string selectedText = rtb.Selection.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n').Replace("\t", "  ");

        string[] lines = selectedText.Split(new string[] { "\n" }, StringSplitOptions.None);
        if (1 == lines.Length)
        {
          sb.AppendLine(lines[0]);
        }
        else if (true == originalText?.Contains(lines[1]))
        {
          originalText = originalText.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace('\r', '\n');
          string[] orgLines = originalText.Split(new string[] { "\n" }, StringSplitOptions.None);
          int negOffset = 0;
          foreach (string line in lines)
          {
            if (!string.IsNullOrWhiteSpace(line))
            {
              break;
            }
            negOffset++;
          }

          int orgIndex = Array.IndexOf(orgLines, orgLines.Where(x => x.Contains(lines[negOffset])).FirstOrDefault()) - negOffset;

          for (int i = 0; i < lines.Length && orgIndex < orgLines.Length; i++, orgIndex++)
          {
            if (0 > orgIndex || !lines[i].Equals(orgLines[orgIndex]))
            {
              if (string.IsNullOrWhiteSpace(lines[i]))
              {
                if (0 <= orgIndex)
                {
                  orgIndex--;
                }
                continue;
              }
            }

            sb.AppendLine(lines[i]);
          }
        }
        try
        {
          Clipboard.SetDataObject(sb.ToString(), true);
        }
        catch { }
      }
    }

    private double GetLeftWidth()
    {
      double offset = Visibility.Visible == svLeft.ComputedVerticalScrollBarVisibility ? 10 : 0;
      return Math.Max(svLeft.ActualWidth - offset, fdLeft.PageWidth);
    }

    private double GetRightWidth()
    {
      double offset = Visibility.Visible == svRight.ComputedVerticalScrollBarVisibility ? 10 : 0;
      return Math.Max(svRight.ActualWidth - offset, fdRight.PageWidth);
    }

    private async void SetWidths()
    {
      if (IsLoaded)
      {
        int retries = 10;
        rtbLeft.Width = GetLeftWidth();
        rtbRight.Width = GetRightWidth();
        await Task.Delay(50);
        while (0 >= svLeft.ScrollableHeight && 0 < retries--)
        {
          await Task.Delay(50);
        }
        if (0 < svLeft.ScrollableHeight)
        {
          slCompareMap.MinHeight = slCompareMap.ActualHeight * svLeft.ViewportHeight / (svLeft.ViewportHeight + svLeft.ScrollableHeight);
        }
      }
    }

    private void Sv_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      SetWidths();
    }

    private void GridSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      columnLeft.Width = columnRight.Width = new GridLength(1, GridUnitType.Star);
    }

    private void CvLocal_Loaded(object sender, RoutedEventArgs e)
    {
      SetWidths();
    }
    private void Rtb_MouseMove(object sender, MouseEventArgs e)
    {
      if (sender is RichTextBox rtb)
      {
        try
        {
          Point mousePosition = e.GetPosition(rtb);

          TextPointer pointer = rtb.GetPositionFromPoint(mousePosition, true);

          if (pointer != null)
          {
            TextPointer lineStart = pointer.GetLineStartPosition(0);
            TextPointer lineEnd = pointer.GetLineStartPosition(1);

            if (lineEnd == null)
            {
              lineEnd = rtb.Document.ContentEnd;
            }

            TextRange textRange = new TextRange(lineStart, lineEnd);
            string lineText = textRange.Text;

            LineUnderMouse = lineText.Trim();
          }
        }
        catch (Exception ex)
        {

        }
      }
    }

    private void slCompareMap_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && sender is Slider s && true)
      {
        // Not working atm. Offset of box not calculated correctly yet
        Point position = e.GetPosition(s);
        double halfMax = s.Maximum / 2;
        double ratio = (halfMax - s.Value) / halfMax;
        double offset = ratio * (s.MinHeight + 4) / 2;
        double d = 1.0d / (s.ActualHeight + offset) * position.Y;
        double p = s.Maximum * d;
        s.Value = p;
      }
    }
  }
}
