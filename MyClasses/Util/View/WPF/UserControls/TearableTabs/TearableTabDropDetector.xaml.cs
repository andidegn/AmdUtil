using AMD.Util.Extensions.WPF;
using AMD.Util.Log;
using AMD.Util.View.WPF.UserControls.TearableTabs;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AMD.Util.View.WPF.UserControls.TearableTabs
{
  /// <summary>
  /// Interaction logic for TearableTabControl.xaml
  /// </summary>
  public partial class TearableTabDropDetector : UserControl
  {
    private LogWriter log;
    private Point mousePosition;
    private DropLocation dropLocation;
    private TearableTabSharedHelper sharedData;
    //private MainLocation currLocation;

    #region DependencyProperties

    public Brush HightlightColor
    {
      get { return (Brush)GetValue(HightlightColorProperty); }
      set { SetValue(HightlightColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HightlightColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HightlightColorProperty =
        DependencyProperty.Register("HightlightColor", typeof(Brush), typeof(TearableTabDropDetector), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0x20, 0xFF))));


    private FrameworkElement detectorElement;
    
    #endregion // DependencyProperties

    #region External EventHandlers
    public delegate void TabDropEvent(object sender, TabDropEventArgs args);
    public event TabDropEvent TabDrop;
    private void OnTabDropChanged(TearableTabItem droppedTab)
    {
      TabDrop?.Invoke(this, new TabDropEventArgs(dropLocation, droppedTab));
    }
    #endregion // External EventHandlers

    public TearableTabDropDetector()
    {
      log = LogWriter.Instance;
      sharedData = TearableTabSharedHelper.Instance;
      //currLocation = new MainLocation() { ColumnIndex = 0, RowIndex = 0 };
      InitializeComponent();
    }

    public TearableTabDropDetector(FrameworkElement detectorElement)
      : this()
    {
      this.detectorElement = detectorElement;
    }

    private void HideHighlight()
    {
      foreach (UIElement item in gridOuter.Children)
      {
        if (item is Grid)
        {
          Grid grid = item as Grid;
          grid.Opacity = 0;
        }
      }
    }

    public void AttachDetectorElement(FrameworkElement detectorElement)
    {
      DetachDetectorHandlers(this.detectorElement);
      this.detectorElement = detectorElement;
      AttachDetectorHandlers(this.detectorElement);
    }

    public void DetachDetectorElement()
    {
      DetachDetectorHandlers(this.detectorElement);
    }

    private void DetachDetectorHandlers(FrameworkElement detectorElement)
    {
      if (null != detectorElement)
      {
        detectorElement.DragOver -= DetectorElement_DragOver;
        detectorElement.DragLeave -= DetectorElement_DragLeave;
        detectorElement.PreviewDrop -= DetectorElement_PreviewDrop;
        detectorElement.Drop -= DetectorElement_Drop;
      }
    }

    private void AttachDetectorHandlers(FrameworkElement detectorElement)
    {
      if (null != detectorElement)
      {
        detectorElement.DragOver += DetectorElement_DragOver;
        detectorElement.DragLeave += DetectorElement_DragLeave;
        detectorElement.PreviewDrop += DetectorElement_PreviewDrop;
        detectorElement.Drop += DetectorElement_Drop;
      }
    }

    private void DetectorElement_DragOver(object sender, DragEventArgs e)
    {
      mousePosition = e.GetPosition(sender as FrameworkElement);
      double vertical = detectorElement.ActualWidth / 3;
      double horizontal = detectorElement.ActualHeight / 3;
      double opacity = 0.5;
      dropLocation = DropLocation.NA;

      HideHighlight();
      TearableTabItem tabItemTarget = e.Source as TearableTabItem;
      if (this.IsChildOf(tabItemTarget))
      {

      }
      TearableTabItem tabItemSource = e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem;
      if (this.IsChildOf(tabItemSource))
      {

      }
      if (mousePosition.Y > 25)
      {
        if (mousePosition.X < vertical)
        {
          gridLeft.Opacity = opacity;
          dropLocation = DropLocation.Left;
        }
        else if (mousePosition.X > detectorElement.ActualWidth - vertical)
        {
          gridRight.Opacity = opacity;
          dropLocation = DropLocation.Right;
        }
        else if (mousePosition.Y < horizontal)
        {
          gridTop.Opacity = opacity;
          dropLocation = DropLocation.Top;
        }
        else if (mousePosition.Y > detectorElement.ActualHeight - horizontal)
        {
          gridBottom.Opacity = opacity;
          dropLocation = DropLocation.Bottom;
        }
        else
        {
          gridTop.Opacity = opacity;
          gridBottom.Opacity = opacity;
          dropLocation = DropLocation.Center;
        }
      }
    }

    private void DetectorElement_DragLeave(object sender, DragEventArgs e)
    {
      HideHighlight();
    }

    private void DetectorElement_PreviewDrop(object sender, DragEventArgs e)
    {
      e.Handled = true;
      HideHighlight();
      TearableTabItem droppedItem = e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem;
      if (DropLocation.NA == dropLocation)
      {
        e.Handled = false;
      }
      else
      {
        if (DropLocation.Center != dropLocation)
        {
          if (null != droppedItem)
					{
						OnTabDropChanged(e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem);
					}
				}
				else if (this.detectorElement is TearableTabControl)
        {
          (detectorElement as TearableTabControl)?.DropTab(droppedItem);
        }
      }
    }

    private void DetectorElement_Drop(object sender, DragEventArgs e)
    {
    }

    //private enum Orientation
    //{
    //  Horizontal,
    //  Vertical
    //}

    //private class MainLocation
    //{
    //  internal int RowIndex { get; set; }
    //  internal int ColumnIndex { get; set; }
    //  internal DropLocation Location { get; set; }
    //  internal TearableTabDropDetector ParentContainer { get; set; }

    //  internal MainLocation()
    //  {
    //  }

    //  internal void Teardown(TearableTabDropDetector itemToRemove)
    //  {
    //    //ParentContainer?.RemoveTearableTabItemFromRow(itemToRemove, Location);
    //  }
    //}

    //private void gridContent_PreviewDrop(object sender, DragEventArgs e)
    //{
    //  HideHighlight();
    //  TearableTabItem droppedTab = e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem;
    //  if (null != droppedTab)
    //  {
    //    OnTabDropChanged(droppedTab);
    //  }
    //  e.Handled = true;
    //}
  }
}