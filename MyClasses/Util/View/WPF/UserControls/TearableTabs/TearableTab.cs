using AMD.Util.HID;
using AMD.Util.View.WPF.Helper;
using AMD.Util.View.WPF.UserControls.TearableTabs;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace AMD.Util.View.WPF.UserControls
{
  public class TearableTabItem : TabItem, IDisposable
  {
    #region Interface
    public void Dispose()
    {
      (Content as IDisposable)?.Dispose();
    }
    #endregion // Interface

    #region DependencyProperties

    public bool Closeable
    {
      get { return (bool)GetValue(CloseableProperty); }
      set { SetValue(CloseableProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Closeable.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CloseableProperty =
        DependencyProperty.Register("Closeable", typeof(bool), typeof(TearableTabItem), new PropertyMetadata(true, CloseablePropertyChanged));

    private static void CloseablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Button button = (d as TearableTabItem)?.GetTemplateChild("PART_Close") as Button;
      if (null != button)
      {
        button.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
      }
    }
    #endregion // DependencyProperties

    #region External EventHandlers
    public delegate void AllowTabDropEvent(object sender, bool allowDrop);
    public event AllowTabDropEvent AllowTabDrop;
    private void AllowTabDropChanged(bool allowDrop)
    {
      AllowTabDrop?.Invoke(this, allowDrop);
    }
    #endregion // External EventHandlers

    internal TearableTabSharedHelper SharedData { private get; set; }

    private DispatcherTimer tiHoverTimer;

    static TearableTabItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TearableTabItem), new FrameworkPropertyMetadata(typeof(TearableTabItem)));
    }

    public TearableTabItem()
    {
      AllowDrop = true;

      PreviewMouseLeftButtonDown += TabItem_PreviewMouseLeftButtonDown;
      PreviewMouseLeftButtonUp += TabItem_PreviewMouseLeftButtonUp;
      PreviewMouseMove += TabItem_PreviewMouseMove;

      DragEnter += TabItem_DragEnter;
      DragOver += TabItem_DragOver;
      DragLeave += TabItem_DragLeave;
      PreviewDrop += TabItem_Drop;

      this.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(Mouse_OutsideWindowLeftUp), true);
      Mouse.Capture(this, CaptureMode.SubTree);
    }

    private void Mouse_OutsideWindowLeftUp(object sender, MouseButtonEventArgs e)
    {

    }

    public override void OnApplyTemplate()
    {
      Button button = this.GetTemplateChild("PART_Close") as Button;
      if (null != button)
      {
        button.Click += (s, e) =>
        {
          if (button.IsMouseOver)
          {
            TearableTabControl parent = this.Parent as TearableTabControl;
            if (null != parent)
            {
              parent.Items.Remove(this);
            }
            this.Dispose();
          }
        };
      }
    }

    #region Dragable Tabs

    private void TabItem_DragOver(object sender, DragEventArgs e)
    {
      if (null != SharedData)
      {
        Point curMousePos = e.GetPosition((UIElement)this.Parent);
        if (!SharedData.IsAdornerElement(this.Parent))
        {
          UIElement data = e.Data.GetData(typeof(TearableTabItem)) as UIElement;
          SharedData.SetAdornerLayer(this.Parent as UIElement, data);
        }
        SharedData.UpdateAdornerPosition(curMousePos.X - this.ActualWidth / 2, curMousePos.Y);
      }
    }

    #region TabItem
    private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (MouseUtil.IsOver(this, "Border") && null != SharedData)
      {
        SharedData.AllowTabDrag = true;
        e.Handled = false;
      }
    }

    private void TabItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (null != SharedData)
      {
        SharedData.AllowTabDrag = false;
        e.Handled = false;
      }
    }

    private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      TearableTabItem tabItem = e.Source as TearableTabItem;

      if (tabItem != null)
      {
        if (Mouse.LeftButton == MouseButtonState.Pressed)
        {
          if (typeof(Button) != Mouse.DirectlyOver.GetType())
          {
            if (null != SharedData)
            {
              SharedData.AdornerStartPoint = e.GetPosition(this);
            }

            SharedData?.StartDrag((sender as FrameworkElement).Parent as TearableTabControl, tabItem);
          }
        }

        e.Handled = true;
      }
    }

    private void TabItem_DragEnter(object sender, DragEventArgs e)
    {
      TearableTabItem tabItem = sender as TearableTabItem;

      //if (true || MouseUtil.IsOver(this, "Border") && null != SharedData)
      //{
      //  TearableTabItem tabItemSource = e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem;
      //  TearableTabControl tabControlTarget = this.Parent as TearableTabControl;
      //  TearableTabControl tabControlSource = tabItemSource.Parent as TearableTabControl;
      //  if (null != tabItemSource && null != tabControlTarget)
      //  {
      //    if (null != tabControlSource)
      //    {
      //      tabControlSource.Items.Remove(tabItemSource);
      //    }
      //    int index = tabControlTarget.Items.IndexOf(this);
      //    if (0 != index) { }
      //    tabControlTarget.Items.Insert(0, tabItemSource);
      //    //tabControlTarget.Items.Insert(tabControlTarget.Items.IndexOf(tabItem), tabItemSource);
      //  }
      //}

      if (null != SharedData && SharedData.AllowTabDrag && tabItem != null)
      {
        e.Handled = true;
        if (!tabItem.IsSelected)
        {
          if (tiHoverTimer == null)
          {
            tiHoverTimer = new DispatcherTimer();
            tiHoverTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            tiHoverTimer.Tick += (s, e1) =>
            {
              tabItem.IsSelected = true;
              tiHoverTimer.Stop();
              tiHoverTimer = null;
            };
          }
          tiHoverTimer.Start();
        }
      }
    }

    private void TabItem_AdjustWidth(object sender, NotifyCollectionChangedEventArgs e)
    {
      TabHelper.AdjustTabItemWidth(sender as ItemCollection);
    }

    private void TabControl_AdjustWidthOfTabItems(object sender, SizeChangedEventArgs e)
    {
      TabHelper.AdjustTabItemWidth((sender as TearableTabControl).Items as ItemCollection);
    }

    private void TabItem_DragLeave(object sender, DragEventArgs e)
    {
      //TearableTabItem tabItemSource = e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem;
      //TearableTabControl tabControlSource = tabItemSource.Parent as TearableTabControl;
      //if (null != tabItemSource && null != tabControlSource)
      //{
      //  tabControlSource.Items.Remove(tabItemSource);
      //}
      tiHoverTimer?.Stop();
      tiHoverTimer = null;
    }

    private void TabItem_Drop(object sender, DragEventArgs e)
    {
      try
      {
        TearableTabItem tabItemTarget = e.Source as TearableTabItem;
        TearableTabItem tabItemSource = e.Data.GetData(typeof(TearableTabItem)) as TearableTabItem;

        if (null != SharedData)
        {
          SharedData.AllowTabDrag = false;
        }

        if (tabItemTarget != null && tabItemSource != null)
        {
          tiHoverTimer?.Stop();
          tiHoverTimer = null;
          if (!tabItemTarget.Equals(tabItemSource))
          {
            TearableTabControl tabControlSource = tabItemSource.Parent as TearableTabControl;
            String source = tabControlSource.Name;
            TearableTabControl tabControlTarget = tabItemTarget.Parent as TearableTabControl;
            String target = tabControlTarget.Name;
            if (tabControlSource.Equals(tabControlTarget))
            {
              int sourceIndex = tabControlTarget.Items.IndexOf(tabItemSource);
              int targetIndex = tabControlTarget.Items.IndexOf(tabItemTarget);

              tabControlTarget.Items.Remove(tabItemSource);
              tabControlTarget.Items.Insert(targetIndex, tabItemSource);

              //tabControlTarget.Items.Remove(tabItemTarget);
              //tabControlTarget.Items.Insert(sourceIndex, tabItemTarget);
            }
            else
            {
              int targetIndex = Math.Min(tabControlTarget.Items.Count, tabControlTarget.Items.IndexOf(tabItemTarget));
              int one = tabControlTarget.Items.IndexOf(tabItemTarget);

              tabControlSource.Items.Remove(tabItemSource);
              tabControlTarget.Items.Insert(targetIndex, tabItemSource);
            }
            tabItemSource.IsSelected = true;
          }
          e.Handled = true;
        }
      }
      catch (Exception ex)
      {
      }
    }
    #endregion // TabItem

    #region TabControl

    private void SetTabControlItemsChangedEventHandler()
    {
      foreach (var item in VisualHelper.FindVisualChildren<TearableTabControl>(this))
      {
        TabHelper.AdjustTabItemWidth(item.Items);
        var view = CollectionViewSource.GetDefaultView(item.Items);
        view.CollectionChanged += TabItem_AdjustWidth;
        item.SizeChanged += TabControl_AdjustWidthOfTabItems;
      }
    }
    #endregion // TabControl

    #endregion // Dragable Tabs
  }
}
