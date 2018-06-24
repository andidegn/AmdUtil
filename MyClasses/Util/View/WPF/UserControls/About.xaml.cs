using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for About.xaml
  /// </summary>
  public partial class About : UserControl
  {
    public About()
    {
      InitializeComponent();
      rtbDescription.Document.Blocks.FirstBlock.LineHeight = 1;
    }

    #region Properties
    public String PropApplicationName
    {
      get
      {
        return lblApplicationName.Content as String;
      }
      set
      {
        lblApplicationName.Content = value;
      }
    }

    public String PropCompanyName
    {
      get
      {
        return lblCopyRightCompany.Content as String;
      }
      set
      {
        lblCopyRightCompany.Content = value;
      }
    }

    public int PropYear
    {
      get
      {
        return (int)lblCopyRightYear.Content;
      }
      set
      {
        lblCopyRightYear.Content = value;
      }
    }

    public Version PropVersion
    {
      get
      {
        return lblVersion.Content as Version;
      }
      set
      {
        lblVersion.Content = value;
      }
    }

    public String PropCodeName
    {
      get
      {
        return lblCodeName.Content as String;
      }
      set
      {
        lblCodeName.Content = value;
      }
    }

    public String PropDeveloper
    {
      get
      {
        return lblDeveloper.Content as String;
      }
      set
      {
        lblDeveloper.Content = value;
      }
    }

    public RichTextBox PropDescription
    {
      get
      {
        return rtbDescription;
      }
    }


    public Brush PropLogoBackground
    {
      get
      {
        return canvasLogoBackground.Background;
      }
      set
      {
        canvasLogoBackground.Background = value;
      }
    }

    public Brush PropInfoBackground
    {
      get
      {
        return canvasInfoBackground.Background;
      }
      set
      {
        canvasInfoBackground.Background = value;
      }
    }

    public ImageSource PropLogoSource
    {
      get { return imgLogo.Source; }
      set { imgLogo.Source = value; }
    }

    public ImageSource PropIconSource
    {
      get { return imgIcon.Source; }
      set { imgIcon.Source = value; }
    }

    public double PropLogoHeight
    {
      get { return rowLogo.Height.Value; }
      set { rowLogo.Height = new GridLength(value); }
    }

    public Uri PropLogoPath
    {
      set { imgLogo.Source = new BitmapImage(value); }
    }

    public Uri PropIconPath
    {
      set { imgIcon.Source = new BitmapImage(value); }
    }

    public Image PropLogo
    {
      get { return imgLogo; }
      set { imgLogo = value; }
    }

    public Image PropIcon
    {
      get { return imgIcon; }
      set { imgIcon = value; }
    }

    public Visibility PropCloseButtonVisibility
    {
      get { return btnClose.Visibility; }
      set { btnClose.Visibility = value; }
    }
    #endregion // Properties

    #region Event Handlers
    /// <summary>
    /// Eventhandler for progress information
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void ExitHandler(object sender, RoutedEventArgs args);
    public event ExitHandler Exit;

    private void ExecuteExit(RoutedEventArgs e)
    {
      Exit?.Invoke(this, e);
    }

    private void Exit_EventHandler(object sender, RoutedEventArgs e)
    {
      if (Exit != null)
      {
        ExecuteExit(e);
      }
      else
      {
        Application.Current.Shutdown();
      }
    }
    #endregion // Event Handlers
  }
}
