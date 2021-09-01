using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AMD.Util.Extensions.WPF;
using AMD.Util.Collections.Dictionary;
using System.Collections.Specialized;

namespace AMD.Util.View.WPF.UserControls.TearableTabs
{
  /// <summary>
  /// Interaction logic for TearableTabSplitHorizontal.xaml
  /// </summary>
  public partial class TearableTabSplitHorizontal : UserControl, ITabControlContainer
  {
    #region Interface
    public ITabControlContainer TabParent { get; set; }

    public bool AddSplitControl(DropLocation location, DropLocation sourceLoaction, TearableTabItem ttItem)
    {
      ITabControlContainer itcc;
      Grid droppedItemParent = null;
      switch (location)
      {
        case DropLocation.Top:
        case DropLocation.Bottom:
          itcc = new TearableTabSplitHorizontal(this);
          break;
        case DropLocation.Left:
        case DropLocation.Right:
          itcc = new TearableTabSplitVertical(this);
          break;
        case DropLocation.Center:
        default:
          throw new NotSupportedException("Center not supported");
      }
      tabs[location] = itcc;
      switch (sourceLoaction)
      {
        case DropLocation.Top:
          if (ttcTop.Items.Contains(ttItem) && 1 == ttcTop.Items.Count)
          {
            return false;
          }
          DetachHandlers(ttcTop);
          gridContentHorizontal.Children.Remove(ttcTop);
          TearableTabControl ttcTopTmp = ttcTop;
          if (this.TabParent == null)
          {

          }
          if (ttcTop == null)
          {
            // ADE 20210319
            return false;
          }
          if (itcc.AddControl(location, ttcTop, ttItem, ref droppedItemParent))
          {
            ttddTop.Visibility = Visibility.Collapsed;
            ttddTop.DetachDetectorElement();
            Grid.SetRow(itcc as UIElement, 0);
            itccTop = itcc;
            ttcTop = null;
            if (null == droppedItemParent)
            {
              gridContentHorizontal.Children.Insert(0, itcc as UIElement);
            }
            else
            {
              droppedItemParent.Children.Add(itcc as UIElement);
            }
          }
          else
          {
            (ttcTopTmp.Parent as Grid).Children.Remove(ttcTopTmp);
            gridContentHorizontal.Children.Insert(0, ttcTopTmp);
          }
          break;
        case DropLocation.Bottom:
          if (ttcBottom.Items.Contains(ttItem) && 1 == ttcBottom.Items.Count)
          {
            return false;
          }
          DetachHandlers(ttcBottom);
          gridContentHorizontal.Children.Remove(ttcBottom);
          TearableTabControl ttcBottomTmp = ttcBottom;
          if (ttcBottom == null)
          {

          }
          if (itcc.AddControl(location, ttcBottom, ttItem, ref droppedItemParent))
          {
            ttddBottom.Visibility = Visibility.Collapsed;
            ttddBottom.DetachDetectorElement();
            Grid.SetRow(itcc as UIElement, 2);
            itccBottom = itcc;
            ttcBottom = null;
            if (null == droppedItemParent)
            {
              gridContentHorizontal.Children.Insert(0, itcc as UIElement);
            }
            else
            {
              droppedItemParent.Children.Add(itcc as UIElement);
            }
          }
          else
          {
            (ttcBottomTmp.Parent as Grid).Children.Remove(ttcBottomTmp);
            gridContentHorizontal.Children.Insert(0, ttcBottomTmp);
          }
          break;
        case DropLocation.Left:
        case DropLocation.Right:
        case DropLocation.Center:
        default:
          throw new NotSupportedException("Center not supported");
      }

      return true;
    }

    public bool AddControl(DropLocation location, TearableTabControl existingTabControl, TearableTabItem newTabItem, ref Grid droppedItemParent)
    {
      TearableTabControl ttcNew = new TearableTabControl();
      TearableTabControl newTabItemParent = newTabItem.Parent as TearableTabControl;
      if (!ttcNew.DropTab(newTabItem))
      {

      }
      switch (location)
      {
        case DropLocation.Top:
          ttcTop = ttcNew;
          ttcBottom = existingTabControl;
          break;
        case DropLocation.Bottom:
          ttcTop = existingTabControl;
          ttcBottom = ttcNew;
          break;
        case DropLocation.Center:
        case DropLocation.Left:
        case DropLocation.Right:
        default:
          throw new NotSupportedException("Only top and bottom locations are supported in Horizontal");
      }

      // This is where it seems to go wrong as well as in TTSV
      // Start {
      droppedItemParent = existingTabControl.Parent as Grid;
      if (null != droppedItemParent)
      {
        droppedItemParent.Children.Remove(existingTabControl);
      }
      else
      {

      }
      if (this.IsChildOf(newTabItem))
			{

			}
      // } end
			gridContentHorizontal.Children.Insert(0, ttcTop);
			Grid.SetRow(ttcTop, 0);
			gridContentHorizontal.Children.Insert(0, ttcBottom);
			Grid.SetRow(ttcBottom, 2);

			AttachHandlers(ttcTop);
			AttachHandlers(ttcBottom);

			ttddTop.AttachDetectorElement(ttcTop);
			ttddBottom.AttachDetectorElement(ttcBottom);

			AttachHandlers(ttddTop);
			AttachHandlers(ttddBottom);

			return true;
    }

    public void Remove(ref ITabControlContainer itcc, ref TearableTabControl ttc, ref TearableTabDropDetector ttdd, TearableTabControl ttcToBeInserted, int insertIndex)
    {
      gridContentHorizontal.Children.Remove(itcc as UIElement);
      itcc = null;
      ttc = ttcToBeInserted;
      AttachHandlers(ttc);
      ttdd.Visibility = Visibility.Visible;
      ttdd.AttachDetectorElement(ttc);
      gridContentHorizontal.Children.Insert(0, ttc);
      Grid.SetRow(ttc, insertIndex);
    }

    public bool RemoveControl(ITabControlContainer sender, FrameworkElement controlToInsertInPlaceOfRemoved)
    {
      if (sender == itccTop)
      {
        gridContentHorizontal.Children.Remove(itccTop as UIElement);
        itccTop = null;
        if (controlToInsertInPlaceOfRemoved is ITabControlContainer)
        {
          itccTop = controlToInsertInPlaceOfRemoved as ITabControlContainer;
          gridContentHorizontal.Children.Insert(0, itccTop as UIElement);
          Grid.SetRow(itccTop as UIElement, 0);
        }
        else if (controlToInsertInPlaceOfRemoved is TearableTabControl)
        {
          ttcTop = controlToInsertInPlaceOfRemoved as TearableTabControl;
          AttachHandlers(ttcTop);
          ttddTop.Visibility = Visibility.Visible;
          ttddTop.AttachDetectorElement(ttcTop);
          gridContentHorizontal.Children.Insert(0, ttcTop);
          Grid.SetRow(ttcTop, 0);
        }
        //Remove(ref itccTop, ref ttcTop, ref ttddTop, controlToInsertInPlaceOfRemoved, 0);
      }
      else if (sender == itccBottom)
      {
        gridContentHorizontal.Children.Remove(itccBottom as UIElement);
        itccBottom = null;
        if (controlToInsertInPlaceOfRemoved is ITabControlContainer)
        {
          itccBottom = controlToInsertInPlaceOfRemoved as ITabControlContainer;
          gridContentHorizontal.Children.Insert(0, itccBottom as UIElement);
          Grid.SetRow(itccBottom as UIElement, 2);
        }
        else if (controlToInsertInPlaceOfRemoved is TearableTabControl)
        {
          ttcBottom = controlToInsertInPlaceOfRemoved as TearableTabControl;
          AttachHandlers(ttcBottom);
          ttddBottom.Visibility = Visibility.Visible;
          ttddBottom.AttachDetectorElement(ttcBottom);
          gridContentHorizontal.Children.Insert(0, ttcBottom);
          Grid.SetRow(ttcBottom, 2);
          //Remove(ref itccBottom, ref ttcBottom, ref ttddBottom, controlToInsertInPlaceOfRemoved, 0);
        }
      }
      else
      {
        throw new NotSupportedException("Child contorl not found");
      }
      return true;
    }

    public bool DropTab(TearableTabItem tabItem)
    {
      bool retValue = false;
      if (null != ttcTop)
      {
        retValue = ttcTop.DropTab(tabItem);
      }
      else if (null != ttcBottom)
      {
        retValue = ttcBottom.DropTab(tabItem);
      }
      else if (null != itccTop)
      {
        retValue = itccTop.DropTab(tabItem);
      }
      else if (null != itccBottom)
      {
        retValue = itccBottom.DropTab(tabItem);
      }
      return retValue;
    }

    public void Dispose()
    {
      ttcTop?.Dispose();
      ttcBottom?.Dispose();
      itccTop?.Dispose();
      itccBottom?.Dispose();
    }
    #endregion // Interface

    private TearableTabControl ttcTop;
    private TearableTabControl ttcBottom;

    private ITabControlContainer itccTop, itccBottom;
    private SerializableDictionary<DropLocation, ITabControlContainer> tabs;

    public TearableTabSplitHorizontal(ITabControlContainer parent)
    {
      this.TabParent = parent;
      tabs = new SerializableDictionary<DropLocation, ITabControlContainer>();
      InitializeComponent();
    }

    #region AttachHandlers
    private void AttachHandlers(TearableTabControl ttc)
    {
      if (null != ttc)
      {
        ttc.ItemsChanged += TearableTabControl_ItemsChanged;
      }
    }

    private void AttachHandlers(TearableTabDropDetector ttdd)
    {
      if (null != ttdd)
      {
        ttdd.TabDrop += TearableTabDropDetector_TabDrop;
      }
    }

    private void DetachHandlers(TearableTabControl ttc)
    {
      if (null != ttc)
      {
        ttc.ItemsChanged -= TearableTabControl_ItemsChanged;
      }
    }

    private void DetachHandlers(TearableTabDropDetector ttdd)
    {
      if (null != ttdd)
      {
        ttdd.TabDrop -= TearableTabDropDetector_TabDrop;
      }
    }
    #endregion // AttachHandlers

    public override String ToString()
    {
      return String.Format("Parent: {0}, this: {1}", base.Parent?.ToString(), this.Name);
    }

    #region EventHandlers
    private void TearableTabDropDetector_TabDrop(object sender, TabDropEventArgs args)
    {
      TearableTabDropDetector ttdd = sender as TearableTabDropDetector;
      if (null != ttdd)
      {
        if (ttdd == ttddTop)
        {
          if (DropLocation.Center == args.Location)
          {
            ttcTop.DropTab(args.TearableTab);
          }
          else
          {
            AddSplitControl(args.Location, DropLocation.Top, args.TearableTab);
          }
        }
        else if (ttdd == ttddBottom)
        {
          if (DropLocation.Center == args.Location)
          {
            ttcBottom.DropTab(args.TearableTab);
          }
          else
          {
            AddSplitControl(args.Location, DropLocation.Bottom, args.TearableTab);
          }
        }
        else
        {
          new NotSupportedException("Object not supported");
        }
      }
    }

    private void TearableTabControl_ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      TearableTabControl ttc = sender as TearableTabControl;
      if (null != ttc)
      {
        if (0 == ttc.Items.Count)
        {
          DetachHandlers(ttcTop);
          DetachHandlers(ttcBottom);

          ttddTop.DetachDetectorElement();
          ttddBottom.DetachDetectorElement();

          DetachHandlers(ttddTop);
          DetachHandlers(ttddBottom);
          
          if (ttc == ttcTop)
          {
            ttcTop = null;
            if (null != ttcBottom)
            {
              gridContentHorizontal.Children.Remove(ttcBottom);
              TabParent.RemoveControl(this, ttcBottom);
              ttcBottom = null;
            }
            else if (null != itccBottom)
            {
              gridContentHorizontal.Children.Remove(itccBottom as UIElement);
              itccBottom.TabParent = this.TabParent;
              TabParent.RemoveControl(this, itccBottom as FrameworkElement);
              itccBottom = null;
            }
            else
            {
              throw new NullReferenceException("Both tccBottom and itccBottom cannot be null");
            }
          }
          else if (ttc == ttcBottom)
          {
            if (ttcTop?.Items.Count == 0)
            {

            }
            ttcBottom = null;
            if (null != ttcTop)
            {
              gridContentHorizontal.Children.Remove(ttcTop);
              TabParent.RemoveControl(this, ttcTop);
							ttcTop = null;
						}
            else if (null != itccTop)
            {
              gridContentHorizontal.Children.Remove(itccTop as UIElement);
              itccTop.TabParent = this.TabParent;
              TabParent.RemoveControl(this, itccTop as FrameworkElement);
              itccTop = null;
            }
            else
            {
              throw new NullReferenceException("Both tccTop and itccTop cannot be null");
            }
          }
          else
          {
            throw new NotSupportedException("Child object not supported");
          }
        }
      }
    }
    #endregion // EventHandlers
  }
}
