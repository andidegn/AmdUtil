using AMD.Util.HID;
using AMD.Util.Log;
using AMD.Util.View.WPF.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMD.Util.View.WPF.UserControls
{
  internal enum DropLocation
  {
    Center,
    Top,
    Bottom,
    Left,
    Right
  }
  /// <summary>
  /// Interaction logic for TearableTabControl.xaml
  /// </summary>
  public partial class TearableTabDropDetector : UserControl
  {
    private LogWriter log;
    private Point mousePosition;
    private DropLocation dropLocation;
    private TearableTabSharedHelper sharedData;
    private MainLocation currLocation;

    public Brush HightlightColor
    {
      get { return (Brush)GetValue(HightlightColorProperty); }
      set { SetValue(HightlightColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HightlightColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HightlightColorProperty =
        DependencyProperty.Register("HightlightColor", typeof(Brush), typeof(TearableTabDropDetector), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x40, 0x00, 0x20, 0xFF))));



    public TearableTabDropDetector()
    {
      log = LogWriter.Instance;
      sharedData = TearableTabSharedHelper.Instance;
      currLocation = new MainLocation() { ColumnIndex = 0, RowIndex = 0 };
      InitializeComponent();
      AttachTcMain();
    }

    private void AttachTcMain()
    {
      tcMain.PreviewDrop += TcMain_PreviewDrop;
      tcMain.ItemsChanged += TcMain_ItemsChanged;
    }

    public void AddTab(TearableTabItem tearableTabItem)
    {
      if (null != tearableTabItem)
      {
        if (null == tcMain)
        {
          AttachTcMain();
        }
        tcMain.Items.Add(tearableTabItem);
      }
    }

    internal bool DropTab(TearableTabItem tabItemSource)
    {
      if (null == tcMain)
      {
        AttachTcMain();
      }
      return tcMain.DropTab(tabItemSource);
    }

    private void HideHighlight()
    {
      foreach (UIElement item in gridContent.Children)
      {
        if (item is Grid)
        {
          Grid grid = item as Grid;
          grid.Opacity = 0;
        }
      }
    }

    private void TcMain_ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (0 == tcMain.Items.Count)
      {
        currLocation.Teardown(this);
      }
    }

    private void gridContent_DragOver(object sender, DragEventArgs e)
    {
      mousePosition = e.GetPosition(sender as FrameworkElement);
      double vertical = gridContent.ActualWidth / 3;
      double horizontal = gridContent.ActualHeight / 3;
      double opacity = 0.5;
      dropLocation = DropLocation.Center;

      HideHighlight();

      if (null != tcMain)
      {
        if (mousePosition.X < vertical)
        {
          gridLeft.Opacity = opacity;
          dropLocation = DropLocation.Left;
        }
        else if (mousePosition.X > gridContent.ActualWidth - vertical)
        {
          gridRight.Opacity = opacity;
          dropLocation = DropLocation.Right;
        }
        else if (mousePosition.Y < horizontal)
        {
          gridTop.Opacity = opacity;
          dropLocation = DropLocation.Top;
        }
        else if (mousePosition.Y > gridContent.ActualHeight - horizontal)
        {
          gridBottom.Opacity = opacity;
          dropLocation = DropLocation.Bottom;
        }
      }
    }

    private void gridContent_DragLeave(object sender, DragEventArgs e)
    {
      HideHighlight();
    }

    private void TcMain_PreviewDrop(object sender, DragEventArgs e)
    {
      HideHighlight();
      if (DropLocation.Center != dropLocation)
      {
        e.Handled = true;
        CreateNewTearableTabControl(e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem, dropLocation);
      }
      else
      {
      }
    }

    private void CreateNewTearableTabControl(TearableTabItem tabItemSource, DropLocation dropLocation)
    {
      if (null != tabItemSource)
      {
        TearableTabDropDetector tmtc = new TearableTabDropDetector();
        if (tmtc.DropTab(tabItemSource))
        {
          switch (dropLocation)
          {
            case DropLocation.Top:
              AddTearableTabItemToTop(tmtc);
              break;
            case DropLocation.Bottom:
              AddTearableTabItemToBottom(tmtc);
              break;
            case DropLocation.Left:
              break;
            case DropLocation.Right:
              break;
            case DropLocation.Center:
              AttachTcMain();
              tcMain.DropTab(tabItemSource);
              break;
            default:
              break;
          }
        }
      }
    }

    private void RemoveTearableTabItemFromRow(TearableTabDropDetector tmtc, DropLocation location)
    {
      int rowLocation = Grid.GetRow(tmtc);
      int rowLocationAdjust = location == DropLocation.Top ? 1 : -1;
      if (-1 < rowLocation)
      {
        UIElement gridSplitter = null;
        foreach (UIElement uiItem in gridOuter.Children)
        {
          if (rowLocation + rowLocationAdjust == Grid.GetRow(uiItem))
          {
            gridSplitter = uiItem;
            break;
          }
        }
        if (null != gridSplitter)
        {
          gridOuter.Children.Remove(tmtc);
          gridOuter.Children.Remove(gridSplitter);
          if (DropLocation.Top == location)
          {
            currLocation.RowIndex -= 2;
            gridOuter.RowDefinitions.RemoveAt(rowLocation);
            gridOuter.RowDefinitions.RemoveAt(rowLocation);
            foreach (UIElement uiElement in gridOuter.Children)
            {
              int uiRow = Grid.GetRow(uiElement);
              if (rowLocation + rowLocationAdjust <= uiRow)
              {
                Grid.SetRow(uiElement, uiRow - 2);
              }
            }
          }
          else
          {
            gridOuter.RowDefinitions.RemoveAt(rowLocation - 1);
            gridOuter.RowDefinitions.RemoveAt(rowLocation - 1);
          }
        }
      }
    }

    private void AddTearableTabItemToBottom(TearableTabDropDetector tmtc)
    {
      tmtc.currLocation.Location = DropLocation.Bottom;
      tmtc.currLocation.ParentContainer = this;
      gridOuter.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5) });
      gridOuter.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
      PanelHelper.AddToGrid(gridOuter, new GridSplitter() { HorizontalAlignment = HorizontalAlignment.Stretch }, currLocation.RowIndex + 1, 0, 1, gridOuter.ColumnDefinitions.Count);
      PanelHelper.AddToGrid(gridOuter, tmtc, currLocation.RowIndex + 2, 0, 1, gridOuter.ColumnDefinitions.Count);
      foreach (var item in gridOuter.Children)
      {
        int rowNr = Grid.GetRow(item as UIElement);
      }
    }

    private void AddTearableTabItemToTop(TearableTabDropDetector tmtc)
    {
      tmtc.currLocation.Location = DropLocation.Top;
      tmtc.currLocation.ParentContainer = this;
      gridOuter.RowDefinitions.Insert(currLocation.RowIndex, new RowDefinition() { Height = new GridLength(gridContent.ActualHeight / 2) });
      gridOuter.RowDefinitions.Insert(currLocation.RowIndex + 1, new RowDefinition() { Height = new GridLength(5) });

      foreach (UIElement uiElement in gridOuter.Children)
      {
        int uiRow = Grid.GetRow(uiElement);
        if (currLocation.RowIndex <= uiRow)
        {
          Grid.SetRow(uiElement, uiRow + 2);
        }
      }
      PanelHelper.AddToGrid(gridOuter, tmtc, currLocation.RowIndex, 0, 1, gridOuter.ColumnDefinitions.Count);
      PanelHelper.AddToGrid(gridOuter, new GridSplitter() { HorizontalAlignment = HorizontalAlignment.Stretch }, currLocation.RowIndex + 1, 0, 1, gridOuter.ColumnDefinitions.Count);
      currLocation.RowIndex += 2;
    }

    private enum Orientation
    {
      Horizontal,
      Vertical
    }

    private class MainLocation
    {
      internal int RowIndex { get; set; }
      internal int ColumnIndex { get; set; }
      internal DropLocation Location { get; set; }
      internal TearableTabDropDetector ParentContainer { get; set; }

      internal MainLocation()
      {
      }

      internal void Teardown(TearableTabDropDetector itemToRemove)
      {
        ParentContainer?.RemoveTearableTabItemFromRow(itemToRemove, Location);
      }
    }
  }
}