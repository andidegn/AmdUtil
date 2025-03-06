using AMD.Util.Collections.Dictionary;
using AMD.Util.Data;
using AMD.Util.View.WPF.UserControls.TearableTabs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for TearableTabSplitControl.xaml
  /// </summary>
  public partial class TearableTabSplitControl : UserControl, ITabControlContainer
  {
    public string Name { get; set; }

    public ITabControlContainer TabParent { get; set; }
    #region Interface
    public bool AddSplitControl(DropLocation location, DropLocation sourceLocation, TearableTabItem ttItem)
    {
      if (ttcMain.Items.Contains(ttItem) && 1 == ttcMain.Items.Count)
      {
        return false;
      }
      switch (location)
      {
        case DropLocation.Top:
        case DropLocation.Bottom:
          itccCenter = new TearableTabSplitHorizontal(this);
          break;
        case DropLocation.Left:
        case DropLocation.Right:
          itccCenter = new TearableTabSplitVertical(this);
          break;
        case DropLocation.Center:
        default:
          throw new NotSupportedException("Center not supported");
      }
      tabs[location] = itccCenter;

      gridContentSplitControl.Children.Remove(ttcMain);
      ttddMain.Visibility = Visibility.Collapsed;
      ttddMain.DetachDetectorElement();
      Grid tmpGrid = null;
      itccCenter.AddControl(location, ttcMain, ttItem, ref tmpGrid);
      gridContentSplitControl.Children.Insert(0, itccCenter as UIElement);
      ttcMain = null;
      return true;
    }

    public bool AddControl(DropLocation location, TearableTabControl ttControl, TearableTabItem newTabItem, ref Grid droppedItemParent)
    {
      return true;
    }

    public bool RemoveControl(ITabControlContainer sender, FrameworkElement controlToInsertInPlaceOfRemoved)
    {
      gridContentSplitControl.Children.Remove(itccCenter as UIElement);
      itccCenter = null;
      if (controlToInsertInPlaceOfRemoved is ITabControlContainer)
      {
        itccCenter = controlToInsertInPlaceOfRemoved as ITabControlContainer;
        gridContentSplitControl.Children.Insert(0, itccCenter as UIElement);
        Grid.SetRow(itccCenter as UIElement, 0);
      }
      else if (controlToInsertInPlaceOfRemoved is TearableTabControl)
      {
        gridContentSplitControl.Children.Remove(sender as UIElement);
        ttddMain.Visibility = Visibility.Visible;
        ttddMain.AttachDetectorElement(controlToInsertInPlaceOfRemoved);
        ttcMain = controlToInsertInPlaceOfRemoved as TearableTabControl;
        if (ttcMain.Parent != null)
        {
          (ttcMain.Parent as Grid).Children.Remove(ttcMain);
        }
        gridContentSplitControl.Children.Insert(0, ttcMain);
        //gridContent.Children.Insert(0, controlToInsertInPlaceOfRemoved as UIElement);
      }
      return true;
    }

    public bool DropTab(TearableTabItem tabItem)
    {
      bool retValue = false;
      if (null != ttcMain)
      {
        retValue = ttcMain.DropTab(tabItem);
      }
      else if (null != itccCenter)
      {
        retValue = itccCenter.DropTab(tabItem);
      }
      return retValue;
    }

    public void Dispose()
    {
      ttcMain?.Dispose();
      itccCenter?.Dispose();
    }
    #endregion // Interface

    private ITabControlContainer itccCenter;
    private SerializableDictionary<DropLocation, ITabControlContainer> tabs;

    public TearableTabSplitControl()
    {
      tabs = new SerializableDictionary<DropLocation, ITabControlContainer>();
      InitializeComponent();
    }

    public MemoryStream GetSerializedStreamOfAllContent()
    {
      Type[] types =
      {
        typeof(TearableTabControl),
        typeof(TearableTabControlAdorner),
        typeof(TearableTabDropDetector),
        typeof(TearableTabItem),
        typeof(TearableTabSharedHelper),
        typeof(TearableTabSplitControl),
        typeof(TearableTabSplitHorizontal),
        typeof(TearableTabSplitVertical)
      };
      return SerializationHelper.Serialize(tabs, types);
    }

    public override void OnApplyTemplate()
    {
      ttddMain.AttachDetectorElement(ttcMain);
    }

    public override String ToString()
    {
      return this.Name;
    }

    #region EventHandlers
    private void ttddMain_TabDrop(object sender, TabDropEventArgs args)
    {
      if (DropLocation.Center == args.Location || 0 == ttcMain.Items.Count)
      {
        ttcMain.DropTab(args.TearableTab);
      }
      else
      {
        AddSplitControl(args.Location, DropLocation.NA, args.TearableTab);
      }
    }
    #endregion // EventHandlers
  }
}
