using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for CompareWindow.xaml
  /// </summary>
  public partial class CompareWindow : Window
  {
    public Brush MainBackground
    {
      get { return (Brush)GetValue(MainBackgroundProperty); }
      set { SetValue(MainBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MainBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MainBackgroundProperty =
        DependencyProperty.Register("MainBackground", typeof(Brush), typeof(CompareWindow), new PropertyMetadata(Brushes.GhostWhite));


    public Brush TextBoxBackground
    {
      get { return (Brush)GetValue(TextBoxBackgroundProperty); }
      set { SetValue(TextBoxBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TextBoxBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextBoxBackgroundProperty =
        DependencyProperty.Register("TextBoxBackground", typeof(Brush), typeof(CompareWindow), new PropertyMetadata(Brushes.White));


    public Brush DiffForeground
    {
      get { return (Brush)GetValue(DiffForegroundProperty); }
      set { SetValue(DiffForegroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DiffForeground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DiffForegroundProperty =
        DependencyProperty.Register("DiffForeground", typeof(Brush), typeof(CompareWindow), new PropertyMetadata(Brushes.Red));

    public Brush DiffBackground
    {
      get { return (Brush)GetValue(DiffBackgroundProperty); }
      set { SetValue(DiffBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DiffBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DiffBackgroundProperty =
        DependencyProperty.Register("DiffBackground", typeof(Brush), typeof(CompareWindow), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x20, 0xFF, 0x00, 0x00))));

    public Brush InnerBorderBrush
    {
      get { return (Brush)GetValue(InnerBorderBrushProperty); }
      set { SetValue(InnerBorderBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for InnerBorderBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty InnerBorderBrushProperty =
        DependencyProperty.Register("InnerBorderBrush", typeof(Brush), typeof(CompareWindow), new PropertyMetadata(Brushes.Gray));

    public Brush MissingLine
    {
      get { return (Brush)GetValue(MissingLineProperty); }
      set { SetValue(MissingLineProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MissingLine.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MissingLineProperty =
        DependencyProperty.Register("MissingLine", typeof(Brush), typeof(CompareWindow), new PropertyMetadata(Brushes.Gray));


    private string strLeft, strRight;

    public CompareWindow()
    {
      InitializeComponent();
    }

    public CompareWindow(string strLeft, string strRight, int charsToSearchForOffset = 5, int lineIndexBeforeLineMatch = 4)
      : this()
    {
      Compare(strLeft, strRight, charsToSearchForOffset, lineIndexBeforeLineMatch);
    }

    public void Compare(string strLeft, string strRight, int charsToSearchForOffset = 5, int lineIndexBeforeLineMatch = 4)
    {
      CompareView.DataCompare.LineIndexBeforeLineMatch = lineIndexBeforeLineMatch;
      CompareView.Compare(strLeft, strRight, Foreground, DiffForeground, DiffBackground, MissingLine, charsToSearchForOffset);
    }

    private void Exit()
    {
      this.Close();
    }

    private void Titlebar_Exit(object sender, RoutedEventArgs args)
    {
      Exit();
    }

    private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (Key.Escape == e.Key)
      {
        Exit();
      }
    }

    private void btnSwap_Click(object sender, RoutedEventArgs e)
    {
      CompareView.SwapLeftAndRight();
    }
  }
}
