using AMD.Util.Extensions.WPF;
using AMD.Util.View.WPF.Helper;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System;
using System.Collections.Generic;
using AMD.Util.View.WPF.UserControls.TearableTabs;
using System.Windows.Media;

namespace AMD.Util.View.WPF.UserControls
{
  public class TearableTabControl : TabControl, IDisposable
  {
    #region Interface
    public void Dispose()
    {
      foreach (var item in this.Items)
      {
        (item as IDisposable)?.Dispose();
      }
    }
    #endregion // Interface

    #region External EventHandlers
    public delegate void ItemsChangedEvent(object sender, NotifyCollectionChangedEventArgs e);
    public event ItemsChangedEvent ItemsChanged;
    private void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      ItemsChanged?.Invoke(this, e);
    }
    #endregion // External EventHandlers

    private static TearableTabSharedHelper sharedData;

    static TearableTabControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TearableTabControl), new FrameworkPropertyMetadata(typeof(TearableTabControl)));
    }

    public TearableTabControl()
    {
      sharedData = TearableTabSharedHelper.Instance;

      this.Background = Brushes.Purple;

      this.AllowDrop = true;
      this.SizeChanged += TearableTabControl_SizeChanged;
      this.Drop += TearableTabControl_Drop;
      this.Loaded += TearableTabControl_Loaded;
      CollectionViewSource.GetDefaultView(this.Items).CollectionChanged += TearableTabControl_CollectionChanged;
    }

    private void TearableTabControl_Loaded(object sender, RoutedEventArgs e)
    {
      UpdateSharedData(this.Items);
    }

    private void TearableTabControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      TabHelper.AdjustTabItemWidth(sender as ItemCollection);
      if (null != e.NewItems)
      {
        UpdateSharedData(e.NewItems);
      }
      OnItemsChanged(e);
    }

    private void TearableTabControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      TabHelper.AdjustTabItemWidth(this.Items);
    }

    private void UpdateSharedData(IEnumerable enumerable)
    {
      foreach (var item in enumerable)
      {
        if (item is TearableTabItem)
        {
          (item as TearableTabItem).SharedData = TearableTabControl.sharedData;
        }
      }
    }

    internal bool DropTab(TearableTabItem tabItemSource)
    {
      bool retVal = false;
      try
      {
        TearableTabControl tabControlTarget = this;

        if (tabItemSource != null)
        {
          TearableTabControl tabControlSource = tabItemSource.Parent as TearableTabControl;
          if (tabControlTarget != tabControlSource && !tabControlTarget.IsChildOf(tabItemSource))
          {
            if (null != tabControlSource && tabControlSource.Items.Contains(tabItemSource))
            {
              tabControlSource.Items.Remove(tabItemSource);
              // Should this be disposed if no items remain???
            }
            tabControlTarget.Items.Insert(tabControlTarget.Items.Count, tabItemSource);
            tabItemSource.IsSelected = true;
            retVal = true;
          }
          else
          {
            Log.LogWriter.Instance.WriteToLog(Log.LogMsgType.Error, "Cannot add tab to internal tab control");
          }
        }
      }
      catch (Exception ex)
      {
      }
      return retVal;
    }



    #region TearableTabWindow
    //private void CreateTearableTab(TearableTabItem tabItem, String name = null)
    //{
    //  (tabItem.Parent as TearableTabControl)?.Items.Remove(tabItem);
    //  TearableTabWindow tt = new TearableTabWindow()
    //  {
    //    Name = name ?? String.Format("tearableTab{0}", tearableTabs.Count)
    //  };
    //  tearableTabs.Add(tt);
    //  tt.Items.Add(tabItem);
    //  tt.KeyUp += Window_KeyUp;
    //  tt.Show();
    //}

    //private bool AddToTearableTab(TearableTabItem tabItem, String tabControlName, int tabIndex)
    //{
    //  TearableTabWindow tt = TearableTabWindow.Where(x => x.Name == tabControlName).FirstOrDefault();
    //  bool retValue = false;
    //  if (tt != null)
    //  {
    //    tt.Items.Insert(tabIndex, tabItem);
    //    retValue = true;
    //  }
    //  return retValue;
    //}
    #endregion // TearableTabWindow

    #region EventHandlers

    private void TearableTabControl_Drop(object sender, DragEventArgs e)
    {
      if (DropTab(e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem))
      {
        e.Handled = true;
      }
    }
    #endregion // EventHandlers
  }
}