using AMD.Util.Extensions.WPF;
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

namespace AMD.Util.View.WPF.UserControls.TearableTabs
{
  /// <summary>
  /// Interaction logic for TearableTabSplitVertical.xaml
  /// </summary>
  public partial class TearableTabSplitVertical : UserControl, ITabControlContainer
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

      switch (sourceLoaction)
      {
        case DropLocation.Left:
          if (ttcLeft.Items.Contains(ttItem) && 1 == ttcLeft.Items.Count)
          {
            return false;
          }
          DetachHandlers(ttcLeft);
          gridContentVertical.Children.Remove(ttcLeft);
          TearableTabControl ttcLeftTmp = ttcLeft;
          if (ttcLeft == null)
          {

          }
          if (itcc.AddControl(location, ttcLeft, ttItem, ref droppedItemParent))
          {
            ttddLeft.Visibility = Visibility.Collapsed;
            ttddLeft.DetachDetectorElement();
            Grid.SetColumn(itcc as UIElement, 0);
            itccLeft = itcc;
            ttcLeft = null;
            if (null == droppedItemParent)
            {
              gridContentVertical.Children.Insert(0, itcc as UIElement);
            }
            else
            {
              droppedItemParent.Children.Add(itcc as UIElement);
            }
          }
          else
          {
            (ttcLeftTmp.Parent as Grid).Children.Remove(ttcLeftTmp);
            gridContentVertical.Children.Insert(0, ttcLeftTmp);
          }
          break;
        case DropLocation.Right:
          if (ttcRight.Items.Contains(ttItem) && 1 == ttcRight.Items.Count)
          {
            return false;
          }
          DetachHandlers(ttcRight);
          gridContentVertical.Children.Remove(ttcRight);
          TearableTabControl ttcRightTmp = ttcRight;
          if (ttcRight == null)
          {

          }
          if (itcc.AddControl(location, ttcRight, ttItem, ref droppedItemParent))
          {
            ttddRight.Visibility = Visibility.Collapsed;
            ttddRight.DetachDetectorElement();
            Grid.SetColumn(itcc as UIElement, 2);
            itccRight = itcc;
            ttcRight = null;
            if (null == droppedItemParent)
            {
              gridContentVertical.Children.Insert(0, itcc as UIElement);
            }
            else
            {
              droppedItemParent.Children.Add(itcc as UIElement);
            }
          }
          else
          {
            (ttcRightTmp.Parent as Grid).Children.Remove(ttcRightTmp);
            gridContentVertical.Children.Insert(0, ttcRightTmp);
          }
          break;
        case DropLocation.Top:
        case DropLocation.Bottom:
        case DropLocation.Center:
        default:
          throw new NotSupportedException("Center not supported");
      }
      //try
      //{
      //  gridContentVertical.Children.Insert(0, itcc as UIElement);
      //}
      //catch (Exception ex)
      //{
      //}

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
        case DropLocation.Left:
          ttcLeft = ttcNew;
          ttcRight = existingTabControl;
          break;
        case DropLocation.Right:
          ttcLeft = existingTabControl;
          ttcRight = ttcNew;
          break;
        case DropLocation.Center:
        case DropLocation.Top:
        case DropLocation.Bottom:
        default:
          throw new NotSupportedException("Only left and right locations are supported in Horizontal");
      }

      // This is where it seems to go wrong as well as in TTSH
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
			gridContentVertical.Children.Insert(0, ttcLeft);
			Grid.SetColumn(ttcLeft, 0);
			gridContentVertical.Children.Insert(0, ttcRight);
			Grid.SetColumn(ttcRight, 2);

			AttachHandlers(ttcLeft);
			AttachHandlers(ttcRight);

			ttddLeft.AttachDetectorElement(ttcLeft);
			ttddRight.AttachDetectorElement(ttcRight);

			AttachHandlers(ttddLeft);
			AttachHandlers(ttddRight);

			return true;
		}

		public void Remove(ref ITabControlContainer itcc, ref TearableTabControl ttc, ref TearableTabDropDetector ttdd, TearableTabControl ttcToBeInserted, int insertIndex)
    {
      gridContentVertical.Children.Remove(itcc as UIElement);
      itcc = null;
      ttc = ttcToBeInserted;
      AttachHandlers(ttc);
      ttdd.Visibility = Visibility.Visible;
      ttdd.AttachDetectorElement(ttc);
      gridContentVertical.Children.Insert(0, ttc);
      Grid.SetColumn(ttc, insertIndex);
    }

    public bool RemoveControl(ITabControlContainer sender, FrameworkElement controlToInsertInPlaceOfRemoved)
    {
      if (sender == itccLeft)
      {
        gridContentVertical.Children.Remove(itccLeft as UIElement);
        itccLeft = null;
        if (controlToInsertInPlaceOfRemoved is ITabControlContainer)
        {
          itccLeft = controlToInsertInPlaceOfRemoved as ITabControlContainer;
          gridContentVertical.Children.Insert(0, itccLeft as UIElement);
          Grid.SetColumn(itccLeft as UIElement, 0);
        }
        else if (controlToInsertInPlaceOfRemoved is TearableTabControl)
        {
          ttcLeft = controlToInsertInPlaceOfRemoved as TearableTabControl;
          AttachHandlers(ttcLeft);
          ttddLeft.Visibility = Visibility.Visible;
          ttddLeft.AttachDetectorElement(ttcLeft);
          gridContentVertical.Children.Insert(0, ttcLeft);
          Grid.SetColumn(ttcLeft, 0);
        }
        //Remove(ref itccLeft, ref ttcLeft, ref ttddLeft, controlToInsertInPlaceOfRemoved, 0);
      }
      else if (sender == itccRight)
      {
        gridContentVertical.Children.Remove(itccRight as UIElement);
        itccRight = null;
        if (controlToInsertInPlaceOfRemoved is ITabControlContainer)
        {
          itccRight = controlToInsertInPlaceOfRemoved as ITabControlContainer;
          gridContentVertical.Children.Insert(0, itccRight as UIElement);
          Grid.SetColumn(itccRight as UIElement, 2);
        }
        else if (controlToInsertInPlaceOfRemoved is TearableTabControl)
        {
          ttcRight = controlToInsertInPlaceOfRemoved as TearableTabControl;
          AttachHandlers(ttcRight);
          ttddRight.Visibility = Visibility.Visible;
          ttddRight.AttachDetectorElement(ttcRight);
          gridContentVertical.Children.Insert(0, ttcRight);
          Grid.SetColumn(ttcRight, 2);
          //Remove(ref itccRight, ref ttcRight, ref ttddRight, controlToInsertInPlaceOfRemoved, 0);
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
      if (null != ttcLeft)
      {
        retValue = ttcLeft.DropTab(tabItem);
      }
      else if (null != ttcRight)
      {
        retValue = ttcRight.DropTab(tabItem);
      }
      else if (null != itccLeft)
      {
        retValue = itccLeft.DropTab(tabItem);
      }
      else if (null != itccRight)
      {
        retValue = itccRight.DropTab(tabItem);
      }
      return retValue;
    }

    public void Dispose()
    {
      ttcLeft?.Dispose();
      ttcRight?.Dispose();
      itccLeft?.Dispose();
      itccRight?.Dispose();
    }
    #endregion // Interface

    private TearableTabControl ttcLeft;
    private TearableTabControl ttcRight;

    private ITabControlContainer itccLeft, itccRight;

    public TearableTabSplitVertical(ITabControlContainer parent)
    {
      this.TabParent = parent;
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

    #region EventHanlers
    private void TearableTabDropDetector_TabDrop(object sender, TabDropEventArgs args)
    {
      TearableTabDropDetector ttdd = sender as TearableTabDropDetector;
      if (null != ttdd)
      {
        if (ttdd == ttddLeft)
        {
          if (DropLocation.Center == args.Location)
          {
            ttcLeft.DropTab(args.TearableTab);
          }
          else
          {
            AddSplitControl(args.Location, DropLocation.Left, args.TearableTab);
          }
        }
        else if (ttdd == ttddRight)
        {
          if (DropLocation.Center == args.Location)
          {
            ttcRight.DropTab(args.TearableTab);
          }
          else
          {
            AddSplitControl(args.Location, DropLocation.Right, args.TearableTab);
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
          DetachHandlers(ttcLeft);
          DetachHandlers(ttcRight);

          ttddLeft.DetachDetectorElement();
          ttddRight.DetachDetectorElement();

          DetachHandlers(ttddLeft);
          DetachHandlers(ttddRight);

          if (ttc == ttcLeft)
          {
            ttcLeft = null;
            if (null != ttcRight)
            {
              gridContentVertical.Children.Remove(ttcRight);
              TabParent.RemoveControl(this, ttcRight);
              ttcRight = null;
            }
            else if (null != itccRight)
            {
              gridContentVertical.Children.Remove(itccRight as UIElement);
              itccRight.TabParent = this.TabParent;
              TabParent.RemoveControl(this, itccRight as FrameworkElement);
              itccRight = null;
            }
            else
            {
              (this.Parent as Grid).Children.Remove(this);
              //throw new NullReferenceException("Both ttcRight and itccRight cannot be null");
            }
          }
          else if (ttc == ttcRight)
          {
            if (ttcLeft?.Items.Count == 0)
            {

            }
            ttcRight = null;
            if (null != ttcLeft)
            {
              gridContentVertical.Children.Remove(ttcLeft);
              TabParent.RemoveControl(this, ttcLeft);
              ttcLeft = null;
            }
            else if (null != itccLeft)
            {
              gridContentVertical.Children.Remove(itccLeft as UIElement);
              itccLeft.TabParent = this.TabParent;
              TabParent.RemoveControl(this, itccLeft as FrameworkElement);
              itccLeft = null;
            }
            else
            {
              throw new NullReferenceException("Both ttcLeft and itccLeft cannot be null");
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
