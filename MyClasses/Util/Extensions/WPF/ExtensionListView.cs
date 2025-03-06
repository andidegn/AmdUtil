using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.UI.Xaml.Controls.Primitives;

namespace AMD.Util.Extensions.WPF
{
  public static class ExtensionListView
  {
    public static void ScrollIntoViewCenter(this ListView listView, object scrollToItem)
    {
      if (listView is null || scrollToItem is null)
      {
        return;
      }
      listView.UpdateLayout();
      listView.ScrollIntoView(scrollToItem);

      ScrollViewer sv = GetScrollViewer(listView);
      if (sv is null)
      {
        return;
      }

      double offset = sv.ExtentHeight * listView.SelectedIndex / listView.Items.Count - sv.ViewportHeight / 2;

      if (double.IsNaN(offset))
      {
        offset = 0;
      }
      sv.ScrollToVerticalOffset(offset);
    }

    private static ScrollViewer GetScrollViewer(DependencyObject dependencyObject)
    {
      if (dependencyObject is ScrollViewer viewer)
      {
        return viewer;
      }

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
      {
        var child = VisualTreeHelper.GetChild(dependencyObject, i);
        var result = GetScrollViewer(child);
        if (result != null)
        {
          return result;
        }
      }

      return null;
    }
  }
}
