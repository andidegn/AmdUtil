using AMD.Util.Colour;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class ColorPickerDialog : Window
  {
    #region Events
    /// <summary>
    /// Eventhandler SelectedBrushChanged event
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void SelectedBrushChangedHandler(object sender, BrushChangedEventArgs args);
    public event SelectedBrushChangedHandler SelectedBrushChanged;

    private void UpdateSelectedBrushChanged()
    {
      SelectedBrushChanged?.Invoke(this, new BrushChangedEventArgs(colorPicker.Resources["OriginalBrush"] as SolidColorBrush, SelectedBrush));
    }
    #endregion // Events
    #region Properties
    public Brush Background
    {
      get { return (Brush)GetValue(BackgroundProperty); }
      set { SetValue(BackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BackgroundProperty =
        DependencyProperty.Register("Background", typeof(Brush), typeof(ColorPickerDialog), new PropertyMetadata(Brushes.GhostWhite));


    public Brush SelectedBrush
    {
      get { return (Brush)GetValue(SelectedBrushProperty); }
      set { SetValue(SelectedBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedBrushProperty =
        DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(ColorPickerDialog), new PropertyMetadata(Brushes.Transparent));


    public Brush OriginalBrush
    {
      get { return (Brush)GetValue(OriginalBrushProperty); }
      set { SetValue(OriginalBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OriginalBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OriginalBrushProperty =
        DependencyProperty.Register("OriginalBrush", typeof(Brush), typeof(ColorPickerDialog), new PropertyMetadata(Brushes.Transparent));


    #endregion // Properties

    public ColorPickerDialog()
    {
      InitializeComponent();
      this.DataContext = this;
      Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, (s, e) =>
      {
        this.CloseDialog();
      });
      CenterWindowOnScreen();
    }

    private void CenterWindowOnScreen()
    {
      double screenWidth = SystemParameters.PrimaryScreenWidth;
      double screenHeight = SystemParameters.PrimaryScreenHeight;
      double windowWidth = this.Width;
      double windowHeight = this.Height;
      this.Left = (screenWidth / 2) - (windowWidth / 2);
      this.Top = (screenHeight / 2) - (windowHeight / 2);
    }


    private void CloseDialog()
    {
      if (null != this.DialogResult)
      {
        this.DialogResult = true;
      }
      this.Close();
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
      {
        CloseDialog();
      }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      return;
      if (e.ClickCount == 1)
      {
        DragMove();
      }
    }

    private void TitleBar_Exit(object sender, RoutedEventArgs args)
    {
      CloseDialog();
    }

    private void colorPicker_SelectedBrushChanged(object sender, BrushChangedEventArgs args)
    {
      UpdateSelectedBrushChanged();
    }
  }
}
