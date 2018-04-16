using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AMD.Util.View.WPF.Helper
{
	public static class PanelHelper
	{
		public static void AddToGrid(Grid g, UIElement uiElement, int row = 0, int col = 0, int rowSpan = 1, int colSpan = 1, bool clearChildren = false)
		{
			UIElement e = uiElement;
			if (clearChildren)
			{
				g.Children.Clear();
			}
			Grid.SetRow(e, row);
			Grid.SetColumn(e, col);
			Grid.SetRowSpan(e, rowSpan);
			Grid.SetColumnSpan(e, colSpan);
			g.Children.Add(e);
    }

    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
      if (depObj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
          if (child != null && child is T)
          {
            yield return (T)child;
          }

          foreach (T childOfChild in FindVisualChildren<T>(child))
          {
            yield return childOfChild;
          }
        }
      }
    }
  }
}
