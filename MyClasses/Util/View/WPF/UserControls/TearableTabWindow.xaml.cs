using AMD.Util.Log;
using AMD.Util.View.WPF.UserControls;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AMD.Util.View.WPF.UserControls
{
  /// <summary>
  /// Interaction logic for TearableTab.xaml
  /// </summary>
  public partial class TearableTabWindow : Window
  {
    private LogWriter log;
    private bool closingAllowed;

    public ItemCollection Items
    {
      get
      {
        return tcMain.Items;
      }
    }

    public TearableTabWindow()
    {
      log = LogWriter.Instance;
      closingAllowed = false;
      InitializeComponent();

      TitleBar tb = new TitleBar();

      titleBar.ExitButtonApple.Visibility = Visibility.Hidden;

      CollectionViewSource.GetDefaultView(tcMain.Items).CollectionChanged += (s, e) =>
      {
        if (tcMain.Items.Count == 0)
        {
          closingAllowed = true;
          this.Close();
        }
      };
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.WindowState == WindowState.Maximized &&
        this.ActualWidth != 2560 &&
        this.ActualWidth != 1920 &&
        this.ActualWidth != 1024 &&
        this.ActualWidth != 1366 &&
        this.ActualWidth != 1280 &&
        this.ActualWidth != 1600 &&
        this.ActualWidth != 1680 &&
        this.ActualWidth != 2560)
      {
        this.WindowState = WindowState.Normal;
        this.ResizeMode = ResizeMode.CanMinimize;
        this.WindowState = WindowState.Maximized;
      }
      else if (this.WindowState != WindowState.Maximized && this.ResizeMode != ResizeMode.CanResize)
      {
        this.ResizeMode = ResizeMode.CanResize;
      }
    }

    #region Dragable Tabs
    private void DragOverHandler(object sender, DragEventArgs e)
    {
      if (tabControlAdorner != null)
      {
        Point curMousePos = e.GetPosition(this);
        tabControlAdorner.UpdatePosition(new Point(curMousePos.X - 10, curMousePos.Y - 10));
      }
    }

    #region TabItem
    private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      allowTabDrag = true;
    }

    private void TabItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      allowTabDrag = false;
    }

    private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      TabItem tabItem = e.Source as TabItem;

      if (allowTabDrag && tabItem != null)
      {
        if (Mouse.LeftButton == MouseButtonState.Pressed)
        {
          tcStartPoint = e.GetPosition(this);
          StartDrag((sender as FrameworkElement).Parent as TabControl, tabItem);
        }

        e.Handled = true;
      }
    }

    private void TabItem_DragEnter(object sender, DragEventArgs e)
    {
      TabItem tabItem = sender as TabItem;
      //tabItem = e.Source as TabItem;

      if (tabItem != null)
      {
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

    private void TabItem_DragLeave(object sender, DragEventArgs e)
    {
      tiHoverTimer?.Stop();
      tiHoverTimer = null;
    }

    private void TabItem_Drop(object sender, DragEventArgs e)
    {
      var tabItemTarget = e.Source as TabItem;
      var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

      allowTabDrag = false;

      if (tabItemTarget != null && tabItemSource != null)
      {
        tiHoverTimer?.Stop();
        tiHoverTimer = null;
        if (!tabItemTarget.Equals(tabItemSource))
        {
          TabControl tabControlSource = tabItemSource.Parent as TabControl;
          String source = tabControlSource.Name;
          TabControl tabControlTarget = tabItemTarget.Parent as TabControl;
          String target = tabControlTarget.Name;
          if (tabControlSource.Equals(tabControlTarget))
          {
            int sourceIndex = tabControlTarget.Items.IndexOf(tabItemSource);
            int targetIndex = tabControlTarget.Items.IndexOf(tabItemTarget);

            tabControlTarget.Items.Remove(tabItemSource);
            tabControlTarget.Items.Insert(targetIndex, tabItemSource);

            tabControlTarget.Items.Remove(tabItemTarget);
            tabControlTarget.Items.Insert(sourceIndex, tabItemTarget);
          }
          else
          {
            int targetIndex = tabControlTarget.Items.IndexOf(tabItemTarget);

            tabControlSource.Items.Remove(tabItemSource);
            tabControlTarget.Items.Insert(targetIndex, tabItemSource);
          }
          tabItemSource.IsSelected = true;
          e.Handled = true;
        }
      }
    }
    #endregion // TabItem

    #region TabControl
    private TearableTabControlAdorner tabControlAdorner;
    private DispatcherTimer tiHoverTimer;
    private Point tcStartPoint;
    private bool allowTabDrag;

    private void StartDrag(TabControl sender, TabItem itemToDrag)
    {
      if (allowTabDrag)
      {
        UIElement adornerElementTarget = sender;
        AdornerLayer al = AdornerLayer.GetAdornerLayer(adornerElementTarget);
        //AdornerLayer alParent = AdornerLayer.GetAdornerLayer(Owner);

        al.Add(tabControlAdorner = new TearableTabControlAdorner(adornerElementTarget, itemToDrag, 0.5));
        //alParent.Add(tabControlAdorner = new TabControlAdorner(adornerElementTarget, itemToDrag, 0.5));


        //this.Visibility = Visibility.Collapsed;
        //double width = this.Width;
        //double height = this.Height;
        //this.Width = 150;
        //this.Height = 150;

        DragDrop.DoDragDrop(itemToDrag.Parent, itemToDrag, DragDropEffects.All);

        //this.Width = width;
        //this.Height = height;
        //if (isOpen)
        //{
        //  this.Visibility = Visibility.Visible;
        //}

        al.Remove(tabControlAdorner);
        //alParent.Remove(tabControlAdorner);
      }
    }

    private void TabControl_Drop(object sender, DragEventArgs e)
    {
      var tabControlTarget = e.Source as TabControl;
      var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

      allowTabDrag = false;

      if (tabControlTarget != null)
      {
        TabControl tabControlSource = tabItemSource.Parent as TabControl;

        tabControlSource.Items.Remove(tabItemSource);
        tabControlTarget.Items.Insert(tabControlTarget.Items.Count, tabItemSource);
        tabItemSource.IsSelected = true;
        e.Handled = true;
      }
    }
    #endregion // TabControl

    #endregion // Dragable Tabs

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (!closingAllowed)
      {
        e.Cancel = true;
      }
    }
  }
}