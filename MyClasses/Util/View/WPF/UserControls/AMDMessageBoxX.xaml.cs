using AMD.Util.Display;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AMD.Util.View.WPF.UserControls
{
  public sealed partial class AMDMessageBoxX : Window
  {

    public Visibility IconVisibility
    {
      get { return (Visibility)GetValue(IconVisibilityProperty); }
      set { SetValue(IconVisibilityProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IconVisibility.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IconVisibilityProperty =
        DependencyProperty.Register("IconVisibility", typeof(Visibility), typeof(AMDMessageBoxX), new PropertyMetadata(Visibility.Collapsed));


    public ControlTemplate Icon
    {
      get { return (ControlTemplate)GetValue(IconProperty); }
      set { SetValue(IconProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(ControlTemplate), typeof(AMDMessageBoxX), new PropertyMetadata(default(ControlTemplate)));

    public string Caption
    {
      get { return (string)GetValue(CaptionProperty); }
      set { SetValue(CaptionProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(AMDMessageBoxX), new PropertyMetadata(default(string)));

    public string Message
    {
      get { return (string)GetValue(MessageProperty); }
      set { SetValue(MessageProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register("Message", typeof(string), typeof(AMDMessageBoxX), new PropertyMetadata(default(string)));

    public string CustomButtonText1
    {
      get { return (string)GetValue(CustomButtonText1Property); }
      set { SetValue(CustomButtonText1Property, value); }
    }

    // Using a DependencyProperty as the backing store for CustomButtonText1.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CustomButtonText1Property =
        DependencyProperty.Register("CustomButtonText1", typeof(string), typeof(AMDMessageBoxX), new PropertyMetadata(null));

    public string CustomButtonText2
    {
      get { return (string)GetValue(CustomButtonText2Property); }
      set { SetValue(CustomButtonText2Property, value); }
    }

    // Using a DependencyProperty as the backing store for CustomButtonText2.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CustomButtonText2Property =
        DependencyProperty.Register("CustomButtonText2", typeof(string), typeof(AMDMessageBoxX), new PropertyMetadata(null));

    public string CustomButtonText3
    {
      get { return (string)GetValue(CustomButtonText3Property); }
      set { SetValue(CustomButtonText3Property, value); }
    }

    // Using a DependencyProperty as the backing store for CustomButtonText3.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CustomButtonText3Property =
        DependencyProperty.Register("CustomButtonText3", typeof(string), typeof(AMDMessageBoxX), new PropertyMetadata(null));

    public Brush MainBackground
    {
      get { return (Brush)GetValue(MainBackgroundProperty); }
      set { SetValue(MainBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MainBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MainBackgroundProperty =
        DependencyProperty.Register("MainBackground", typeof(Brush), typeof(AMDMessageBoxX), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x10, 0x10, 0x10))));

    public Brush MainForeground
    {
      get { return (Brush)GetValue(MainForegroundProperty); }
      set { SetValue(MainForegroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MainForeground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MainForegroundProperty =
        DependencyProperty.Register("MainForeground", typeof(Brush), typeof(AMDMessageBoxX), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xF0, 0xFF, 0xFF, 0xFF))));

    public Brush BracketBrush
    {
      get { return (Brush)GetValue(BracketBrushProperty); }
      set { SetValue(BracketBrushProperty, value); }
    }

    // Using a DependencyProperty as the backing store for BracketBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BracketBrushProperty =
        DependencyProperty.Register("BracketBrush", typeof(Brush), typeof(AMDMessageBoxX), new PropertyMetadata(Brushes.White));









    public string Yes { get; set; } = "Yes";
    public string No { get; set; } = "No";
    public string Cancel { get; set; } = "Cancel";

    public static bool SizeToContentOnNextShow { get; set; }
    public static new Brush Background { get; set; } = new SolidColorBrush(Color.FromArgb(0xFF, 0x10, 0x10, 0x10));
    public static new Brush Foreground { get; set; } = new SolidColorBrush(Color.FromArgb(0xF0, 0xFF, 0xFF, 0xFF));
    public static Brush BracketBorderBrush { get; set; } = Brushes.White;


    public MessageBoxResult Result { get; set; }

    private MessageBoxButton buttons;

    private AMDMessageBoxX()
		{
      MainBackground = Background;
      MainForeground = Foreground;
      BracketBrush = BracketBorderBrush;
			InitializeComponent();
			//tbUser.Focus();
			//this.Top = top - this.Height / 2;
			//this.Left = left - this.Width / 2;
			//MessageBox.Show()

    }
    #region Show
    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
    {
      return ShowCore(null, messageBoxText, caption, button, icon, defaultResult);
    }

    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
    {
      return ShowCore(null, messageBoxText, caption, button, icon, defaultResult);
    }

    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
      return ShowCore(null, messageBoxText, caption, button, icon, MessageBoxResult.None);
    }

    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
    {
      return ShowCore(null, messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.None);
    }

    public static MessageBoxResult Show(string messageBoxText, string caption)
    {
      return ShowCore(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
    }

    public static MessageBoxResult Show(string messageBoxText)
    {
      return ShowCore(null, messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
    }

    public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
    {
      return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult);
    }

    public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
    {
      return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult);
    }

    public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
    {
      return ShowCore(owner, messageBoxText, caption, button, icon, MessageBoxResult.None);
    }

    public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button)
    {
      return ShowCore(owner, messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.None);
    }

    public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
    {
      return ShowCore(owner, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
    }

    public static MessageBoxResult Show(Window owner, string messageBoxText)
    {
      return ShowCore(owner, messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None);
    }



    public static int Show(string messageBoxText, string caption, MessageBoxImage icon, int defaultResult, params string[] customButtonTexts)
    {
      return ShowCore(null, messageBoxText, caption, icon, defaultResult, customButtonTexts);
    }

    public static int Show(string messageBoxText, string caption, MessageBoxImage icon, params string[] customButtonTexts)
    {
      return ShowCore(null, messageBoxText, caption, icon, -1, customButtonTexts);
    }

    public static int Show(string messageBoxText, string caption, params string[] customButtonTexts)
    {
      return ShowCore(null, messageBoxText, caption, MessageBoxImage.None, -1, customButtonTexts);
    }

    public static int Show(Window owner, string messageBoxText, string caption, MessageBoxImage icon, int defaultResult, params string[] customButtonTexts)
    {
      return ShowCore(owner, messageBoxText, caption, icon, defaultResult, customButtonTexts);
    }

    public static int Show(Window owner, string messageBoxText, string caption, MessageBoxImage icon, params string[] customButtonTexts)
    {
      return ShowCore(owner, messageBoxText, caption, icon, -1, customButtonTexts);
    }

    public static int Show(Window owner, string messageBoxText, string caption, params string[] customButtonTexts)
    {
      return ShowCore(owner, messageBoxText, caption, MessageBoxImage.None, -1, customButtonTexts);
    }
    #endregion // Show

    private void SetButtonFocusBasedOnDefaultResult(MessageBoxResult result, MessageBoxButton buttons)
    {
      if (MessageBoxResult.None == result)
      {
        switch (buttons)
        {
          case MessageBoxButton.OK:
          case MessageBoxButton.OKCancel:
            btnOk.Focus();
            break;
          case MessageBoxButton.YesNoCancel:
          case MessageBoxButton.YesNo:
          default:
            btnYes.Focus();
            break;
        }
        return;
      }
      switch (buttons)
      {
        case MessageBoxButton.OK:
          btnOk.Focus();
          break;

        case MessageBoxButton.OKCancel:
          switch (result)
          {
            case MessageBoxResult.OK:
              btnOk.Focus();
              break;
            case MessageBoxResult.Cancel:
              btnCancel.Focus();
              break;
          }
          break;

        case MessageBoxButton.YesNo:
          switch (result)
          {
            case MessageBoxResult.Yes:
              btnYes.Focus();
              break;
            case MessageBoxResult.No:
              btnNo.Focus();
              break;
          }
          break;

        case MessageBoxButton.YesNoCancel:
          switch (result)
          {
            case MessageBoxResult.Yes:
              btnYes.Focus();
              break;
            case MessageBoxResult.No:
              btnNo.Focus();
              break;
            case MessageBoxResult.Cancel:
              btnCancel.Focus();
              break;
          }
          break;
        default:
          break;
      }
    }

    private void SetButtonFocusBasedOnDefaultResult(int result)
    {
      switch (result)
      {
        case 1:
          btnYes.Focus();
          break;

        case 2:
          btnNo.Focus();
          break;

        default:
          btnCancel.Focus();
          break;
      }
    }

    [SecurityCritical]
    internal static MessageBoxResult ShowCore(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
    {
      MessageBoxResult result = MessageBoxResult.None;

      if (!IsValidMessageBoxButton(button))
      {
        throw new InvalidEnumArgumentException("button", (int)button, typeof(MessageBoxButton));
      }

      if (!IsValidMessageBoxImage(icon))
      {
        throw new InvalidEnumArgumentException("icon", (int)icon, typeof(MessageBoxImage));
      }

      if (!IsValidMessageBoxResult(defaultResult))
      {
        throw new InvalidEnumArgumentException("defaultResult", (int)defaultResult, typeof(MessageBoxResult));
      }

      AMDMessageBoxX amdmb = new AMDMessageBoxX()
      {
        Caption = caption,
        Message = messageBoxText,
        buttons = button,
        WindowStartupLocation = null == owner ? WindowStartupLocation.CenterScreen : WindowStartupLocation.Manual
      };
      GridLength iconBorders = MessageBoxImage.None == icon ? new GridLength(0) : new GridLength(60);
      amdmb.cdIconLeft.Width = amdmb.cdIconRight.Width = iconBorders;
      amdmb.SetButtons(button);
      amdmb.SetIcon(icon);
      amdmb.Loaded += (s, e) =>
      {
        if (null != owner)
        {
          double refTop, refLeft;
          switch (owner.WindowState)
          {
            case WindowState.Minimized:
            case WindowState.Maximized:
              var sc = ScreenUtil.GetContainedScreen(owner.Left + owner.Width / 2, owner.Top + owner.Height / 2);
              refTop = sc is null ? owner.Top : sc.Bounds.Top;
              refLeft = sc is null ? owner.Left : sc.Bounds.Left;
              break;

            case WindowState.Normal:
            default:
              refTop = owner.Top;
              refLeft = owner.Left;
              break;
          }

          amdmb.Top = refTop + (owner.ActualHeight - amdmb.ActualHeight) / 2;
          amdmb.Left = refLeft + (owner.ActualWidth - amdmb.ActualWidth) / 2;
        }
        amdmb.SetButtonFocusBasedOnDefaultResult(defaultResult, button);
      };

      if (SizeToContentOnNextShow)
      {
        amdmb.SizeToContent = SizeToContent.WidthAndHeight;
      }
      else
      {
        SizeToContentOnNextShow = false;
        amdmb.SizeToContent = SizeToContent.Height;
      }

      if (true == amdmb.ShowDialog())
      {
        result = amdmb.Result;
      }

      return result;
    }

    [SecurityCritical]
    internal static int ShowCore(Window owner, string messageBoxText, string caption, MessageBoxImage icon, int defaultResult, params string[] customButtonTexts)
    {
      int result = -1;

      if (!IsValidMessageBoxImage(icon))
      {
        throw new InvalidEnumArgumentException("icon", (int)icon, typeof(MessageBoxImage));
      }

      if (customButtonTexts.Length <= defaultResult)
      {
        throw new InvalidEnumArgumentException("defaultResult", (int)defaultResult, typeof(int));
      }

      AMDMessageBoxX amdmb = new AMDMessageBoxX()
      {
        Caption = caption,
        Message = messageBoxText,
        WindowStartupLocation = null == owner ? WindowStartupLocation.CenterScreen : WindowStartupLocation.Manual
      };
      GridLength iconBorders = MessageBoxImage.None == icon ? new GridLength(0) : new GridLength(60);
      amdmb.cdIconLeft.Width = amdmb.cdIconRight.Width = iconBorders;
      amdmb.SetButtons(customButtonTexts);
      amdmb.SetIcon(icon);
      amdmb.Loaded += (s, e) =>
      {
        if (null != owner)
        {
          double refTop, refLeft;
          switch (owner.WindowState)
          {
            case WindowState.Minimized:
            case WindowState.Maximized:
              var sc = ScreenUtil.GetContainedScreen(owner.Left + owner.Width / 2, owner.Top + owner.Height / 2);
              refTop = sc is null ? owner.Top : sc.Bounds.Top;
              refLeft = sc is null ? owner.Left : sc.Bounds.Left;
              break;

            case WindowState.Normal:
            default:
              refTop = owner.Top;
              refLeft = owner.Left;
              break;
          }
          amdmb.Top = refTop + (owner.ActualHeight - amdmb.ActualHeight) / 2;
          amdmb.Left = refLeft + (owner.ActualWidth - amdmb.ActualWidth) / 2;
        }
        amdmb.SetButtonFocusBasedOnDefaultResult(defaultResult);
      };

      if (SizeToContentOnNextShow)
      {
        amdmb.SizeToContent = SizeToContent.WidthAndHeight;
        SizeToContentOnNextShow = false;
      }
      else
      {
        amdmb.SizeToContent = SizeToContent.Height;
      }

      if (true == amdmb.ShowDialog())
      {
        switch (amdmb.Result)
        {
          case MessageBoxResult.Cancel:
            result = 2;
            break;

          case MessageBoxResult.Yes:
            result = 0;
            break;

          case MessageBoxResult.No:
            result = 1;
            break;

          case MessageBoxResult.OK:
          case MessageBoxResult.None:
          default:
            result = -1;
            break;
        }
      }

      return result;
    }

    #region Validation
    private static bool IsValidMessageBoxButton(MessageBoxButton value)
    {
      if (value != 0 && value != MessageBoxButton.OKCancel && value != MessageBoxButton.YesNo)
      {
        return value == MessageBoxButton.YesNoCancel;
      }

      return true;
    }

    private static bool IsValidMessageBoxImage(MessageBoxImage value)
    {
      if (value != MessageBoxImage.Asterisk && value != MessageBoxImage.Hand && value != MessageBoxImage.Exclamation && value != MessageBoxImage.Hand && value != MessageBoxImage.Asterisk && value != 0 && value != MessageBoxImage.Question && value != MessageBoxImage.Hand)
      {
        return value == MessageBoxImage.Exclamation;
      }

      return true;
    }

    private static bool IsValidMessageBoxResult(MessageBoxResult value)
    {
      if (value != MessageBoxResult.Cancel && value != MessageBoxResult.No && value != 0 && value != MessageBoxResult.OK)
      {
        return value == MessageBoxResult.Yes;
      }

      return true;
    }

    private static bool IsValidMessageBoxOptions(MessageBoxOptions value)
    {
      int num = -3801089;
      if (((uint)value & (uint)num) == 0)
      {
        return true;
      }

      return false;
    }
    #endregion // Validation

    private void SetButtons(MessageBoxButton buttons)
		{
			switch (buttons)
			{
				case MessageBoxButton.OKCancel:
          btnOk.Visibility = Visibility.Visible;
          btnCancel.Visibility = Visibility.Visible;
					break;

				case MessageBoxButton.YesNo:
          btnYes.Visibility = Visibility.Visible;
          btnNo.Visibility = Visibility.Visible;
					break;

				case MessageBoxButton.YesNoCancel:
          btnYes.Visibility = Visibility.Visible;
          btnNo.Visibility = Visibility.Visible;
          btnCancel.Visibility = Visibility.Visible;
					break;

        case MessageBoxButton.OK:
				default:
          btnOk.Visibility = Visibility.Visible;
					break;
      }
    }
    private void SetButtons(params string[] names)
    {
      switch (names.Length)
      {
        case 1:
          CustomButtonText1 = names[0];
          btnYes.Visibility = Visibility.Visible;
          break;

        case 2:
          CustomButtonText1 = names[0];
          CustomButtonText2 = names[1];
          btnYes.Visibility = Visibility.Visible;
          btnNo.Visibility = Visibility.Visible;
          break;

        default:
          CustomButtonText1 = names[0];
          CustomButtonText2 = names[1];
          CustomButtonText3 = names[2];
          btnYes.Visibility = Visibility.Visible;
          btnNo.Visibility = Visibility.Visible;
          btnCancel.Visibility = Visibility.Visible;
          break;
      }
    }

    private void SetIcon(MessageBoxImage icon)
    {
      IconVisibility = Visibility.Visible;
      switch (icon)
      {
        //case MessageBoxImage.Error:
        //case MessageBoxImage.Stop:
        case MessageBoxImage.Hand:
          Icon = IconCollection.Instance[IconCollection.Icon.CrossInRedCircle];
          break;

        case MessageBoxImage.Question:
          Icon = IconCollection.Instance[IconCollection.Icon.QuestionMarkInBlueCircle];
          break;

        //case MessageBoxImage.Warning:
        case MessageBoxImage.Exclamation:
          Icon = IconCollection.Instance[IconCollection.Icon.ExclamationInYellowTriangle];
          break;

        //case MessageBoxImage.Information:
        case MessageBoxImage.Asterisk:
          Icon = IconCollection.Instance[IconCollection.Icon.ExclamationInBlueCircle];
          break;

        case MessageBoxImage.None:
        default:
          IconVisibility = Visibility.Collapsed;
          break;
      }
    }

    private void Exit()
    {
      Close();
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
      switch (e.Key)
      {
        case Key.Escape:
          Result = MessageBoxResult.None;
          DialogResult = false;
          Exit();
          break;

        case Key.Y:
          if (MessageBoxButton.YesNo == buttons || MessageBoxButton.YesNoCancel == buttons)
          {
            ButtonPress(MessageBoxResult.Yes);
          }
          break;

        case Key.N:
          if (MessageBoxButton.YesNo == buttons || MessageBoxButton.YesNoCancel == buttons)
          {
            ButtonPress(MessageBoxResult.No);
          }
          break;

        case Key.C:
          if (MessageBoxButton.OKCancel == buttons || MessageBoxButton.YesNoCancel == buttons)
          {
            ButtonPress(MessageBoxResult.Cancel);
          }
          break;

        case Key.O:
          if (MessageBoxButton.OK == buttons || MessageBoxButton.OKCancel == buttons)
          {
            ButtonPress(MessageBoxResult.OK);
          }
          break;

        default:
          break;
      }
		}

    private void ButtonPress(MessageBoxResult result)
    {
      Result = result;
      DialogResult = true;
      Exit();
    }

    private void btnYes_Click(object sender, RoutedEventArgs e)
    {
      ButtonPress(MessageBoxResult.Yes);
    }

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
      ButtonPress(MessageBoxResult.OK);
    }

    private void btnNo_Click(object sender, RoutedEventArgs e)
    {
      ButtonPress(MessageBoxResult.No);
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      ButtonPress(MessageBoxResult.Cancel);
    }
  }
}
