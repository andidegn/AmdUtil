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
  public partial class AboutWindow : Window
  {
    public AboutWindow()
    {
      InitializeComponent();
      Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, (s, e) =>
      {
        this.Close();
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


    #region Properties
    public String PropApplicationName
    {
      get
      {
        return about.lblApplicationName.Content as String;
      }
      set
      {
        about.lblApplicationName.Content = value;
      }
    }

    public String PropCompanyName
    {
      get
      {
        return about.lblCopyRightCompany.Content as String;
      }
      set
      {
        about.lblCopyRightCompany.Content = value;
      }
    }

    public int PropYear
    {
      get
      {
        return (int)about.lblCopyRightYear.Content;
      }
      set
      {
        about.lblCopyRightYear.Content = value;
      }
    }

    public Version PropVersion
    {
      get
      {
        return about.lblVersion.Content as Version;
      }
      set
      {
        about.lblVersion.Content = value;
      }
    }

    public String PropCodeName
    {
      get
      {
        return about.lblCodeName.Content as String;
      }
      set
      {
        about.lblCodeName.Content = value;
      }
    }

    public String PropDeveloper
    {
      get
      {
        return about.lblDeveloper.Content as String;
      }
      set
      {
        about.lblDeveloper.Content = value;
      }
    }

    public RichTextBox PropDescription
    {
      get
      {
        return about.rtbDescription;
      }
    }

    public Brush PropLogoBackground
    {
      get
      {
        return about.canvasLogoBackground.Background;
      }
      set
      {
        about.canvasLogoBackground.Background = value;
      }
    }

    public Brush PropInfoBackground
    {
      get
      {
        return about.canvasInfoBackground.Background;
      }
      set
      {
        about.canvasInfoBackground.Background = value;
      }
    }

    public ImageSource PropLogoSource
    {
      get { return about.imgLogo.Source; }
      set { about.imgLogo.Source = value; }
    }

    public ImageSource PropIconSource
    {
      get { return about.imgIcon.Source; }
      set { about.imgIcon.Source = value; }
    }

    public double PropLogoHeight
    {
      get { return about.rowLogo.Height.Value; }
      set { about.rowLogo.Height = new GridLength(value); }
    }

    public Uri PropLogoPath
    {
      set { about.imgLogo.Source = new BitmapImage(value); }
    }

    public Uri PropIconPath
    {
      set { about.imgIcon.Source = new BitmapImage(value); }
    }

    public Image PropLogo
    {
      get { return about.imgLogo; }
      set { about.imgLogo = value; }
    }

    public Image PropIcon
    {
      get { return about.imgIcon; }
      set { about.imgIcon = value; }
    }

    public Visibility PropCloseButtonVisibility
    {
      get { return about.btnClose.Visibility; }
      set { about.btnClose.Visibility = value; }
    }

    public Grid PropGridBottomCustom
    {
      get
      {
        return about.PropGridBottomCustom;
      }
    }
    #endregion // Properties

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
      {
        //this.DialogResult = true;
        this.Close();
      }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 1)
      {
        DragMove();
      }
    }

    private void about_Exit(object sender, RoutedEventArgs args)
    {
      this.Close();
    }
  }
}
