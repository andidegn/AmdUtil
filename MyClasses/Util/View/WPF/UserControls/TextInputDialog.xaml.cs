using AMD.Util.Display;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for TextInputDialog.xaml
  /// </summary>
  public partial class TextInputDialog : Window
	{
		public enum ButtonAssignments
		{
			NotSet,
			YesNo,
			OkCancel
		}

		public Brush BackgroundBrush
		{
			get { return (Brush)GetValue(BackgroundBrushProperty); }
			set { SetValue(BackgroundBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BackgroundBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BackgroundBrushProperty =
				DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(TextInputDialog), new PropertyMetadata(new LinearGradientBrush(new GradientStopCollection()
				{
					new GradientStop(Color.FromArgb(0xFF, 0x2E, 0x2E, 0x2E), 0.0),
          new GradientStop(Color.FromArgb(0xFF, 0x3E, 0x3E, 0x3E), 0.3),
          new GradientStop(Color.FromArgb(0xFF, 0x3E, 0x3E, 0x3E), 1.0)
				})));

		public Brush TextBoxBackgroundBrush
		{
			get { return (Brush)GetValue(TextBoxBackgroundBrushProperty); }
			set { SetValue(TextBoxBackgroundBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for TextBoxBackgroundBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TextBoxBackgroundBrushProperty =
				DependencyProperty.Register("TextBoxBackgroundBrush", typeof(Brush), typeof(TextInputDialog), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xAA, 0x17, 0x17, 0x17))));

		public Brush ForegroundBrush
		{
			get { return (Brush)GetValue(ForegroundBrushProperty); }
			set { SetValue(ForegroundBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ForegroundBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ForegroundBrushProperty =
				DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(TextInputDialog), new PropertyMetadata(Brushes.White));

		public int MaxCharacters
		{
			get { return (int)GetValue(MaxCharactersProperty); }
			set { SetValue(MaxCharactersProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MaxCharacters.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaxCharactersProperty =
				DependencyProperty.Register("MaxCharacters", typeof(int), typeof(TextInputDialog), new PropertyMetadata(255));

    public static bool SizeToContentOnNextShow { get; set; }

		public string PropTitle
		{
			get
			{
				return lblTitle.Text;
			}
			set
			{
				lblTitle.Text = value;
			}
		}

		private TextInputDialog()
		{
			InitializeComponent();
			tbText.Focus();
    }

    public static string Show()
    {
      return ShowCore(null, string.Empty, string.Empty, 0xFF, 0, null, null, null);
    }

    public static string Show(string caption)
    {
      return ShowCore(null, caption, string.Empty, 0xFF, 0, null, null, null);
    }

    public static string Show(string caption, string existingText)
    {
      return ShowCore(null, caption, existingText, 0xFF, 0, null, null, null);
    }

    public static string Show(string caption, string existingText, int maxChars)
    {
      return ShowCore(null, caption, existingText, maxChars, 0, null, null, null);
    }

    public static string Show(string caption, string existingText, int maxChars, int minHeight)
    {
      return ShowCore(null, caption, existingText, maxChars, minHeight, null, null, null);
    }

    public static string Show(string caption, string existingText, int maxChars, int minHeight, Brush foreground, Brush background, Brush textBoxBackground)
    {
      return ShowCore(null, caption, existingText, maxChars, minHeight, foreground, background, textBoxBackground);
    }

    public static string Show(string caption, string existingText, int maxChars, Brush foreground, Brush background, Brush textBoxBackground)
    {
      return ShowCore(null, caption, existingText, maxChars, 0, foreground, background, textBoxBackground);
    }

    public static string Show(string caption, string existingText, Brush foreground, Brush background, Brush textBoxBackground)
    {
      return ShowCore(null, caption, existingText, 0xFF, 0, foreground, background, textBoxBackground);
    }

    public static string Show(string caption, Brush foreground, Brush background, Brush textBoxBackground)
    {
      return ShowCore(null, caption, string.Empty, 0xFF, 0, foreground, background, textBoxBackground);
    }




    public static string Show(Window owner)
		{
			return ShowCore(owner, string.Empty, string.Empty, 0xFF, 0, null, null, null);
    }

    public static string Show(Window owner, string caption)
    {
      return ShowCore(owner, caption, string.Empty, 0xFF, 0, null, null, null);
    }

    public static string Show(Window owner, string caption, string existingText)
    {
      return ShowCore(owner, caption, existingText, 0xFF, 0, null, null, null);
    }

    public static string Show(Window owner, string caption, string existingText, int maxChars)
    {
      return ShowCore(owner, caption, existingText, maxChars, 0, null, null, null);
    }

    public static string Show(Window owner, string caption, string existingText, int maxChars, int minHeight)
    {
      return ShowCore(owner, caption, existingText, maxChars, minHeight, null, null, null);
    }

    public static string Show(Window owner, string caption, string existingText, int maxChars, int minHeight, Brush foreground, Brush background, Brush textBoxBackground)
    {
      return ShowCore(owner, caption, existingText, maxChars, minHeight, foreground, background, textBoxBackground);
    }

    public static string Show(Window owner, string caption, string existingText, int maxChars, Brush foreground, Brush background, Brush textBoxBackground)
    {
      return ShowCore(owner, caption, existingText, maxChars, 0, foreground, background, textBoxBackground);
    }

    public static string Show(Window owner, string caption, string existingText, Brush foreground, Brush background, Brush textBoxBackground)
    {
      return ShowCore(owner, caption, existingText, 0xFF, 0, foreground, background, textBoxBackground);
    }

    public static string Show(Window owner, string caption, Brush foreground, Brush background, Brush textBoxBackground)
    {
      return ShowCore(owner, caption, string.Empty, 0xFF, 0, foreground, background, textBoxBackground);
    }

    private static string ShowCore(Window owner, string caption, string existingText, int maxChars, int minHeight, Brush foreground, Brush background, Brush textBoxBackground)
		{
			TextInputDialog tid = new TextInputDialog()
			{
				MaxCharacters = maxChars,
				MinHeight = minHeight
			};

			tid.lblTitle.Text = caption;
			tid.tbText.Text = existingText;

			if (null != foreground)
			{
				tid.ForegroundBrush = foreground;
			}

			if (null !=  background)
			{
				tid.BackgroundBrush = background;
			}

			if (null !=  textBoxBackground)
			{
				tid.TextBoxBackgroundBrush = textBoxBackground;
			}

			tid.Loaded += (s, e) =>
			{
				if (null != owner)
				{
					double refTop, refLeft;
					switch (owner.WindowState)
					{
						case WindowState.Minimized:
						case WindowState.Maximized:
							var sc = ScreenUtil.GetContainedScreen(owner.Left, owner.Top);
							refTop = sc.Bounds.Top;
							refLeft = sc.Bounds.Left;
							break;

						case WindowState.Normal:
						default:
							refTop = owner.Top;
							refLeft = owner.Left;
							break;
					}

					tid.Top = refTop + (owner.ActualHeight - tid.ActualHeight) / 2;
					tid.Left = refLeft + (owner.ActualWidth - tid.ActualWidth) / 2;
				}
			};

      if (SizeToContentOnNextShow)
      {
        tid.SizeToContent = SizeToContent.WidthAndHeight;
        SizeToContentOnNextShow = false;
      }
      else
      {
        tid.SizeToContent = SizeToContent.Height;
      }

			if (true == tid.ShowDialog())
			{
				return tid.tbText.Text;
			}
			return null;
    }

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1)
			{
				DragMove();
			}
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				DialogResult = false;
			}
		}

		private void tbText_TextChanged(object sender, TextChangedEventArgs e)
		{
			//lblCharUsed.Text = (sender as TextBox).Text.Length.ToString();
		}

    private void btnLeft_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void btnRight_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
