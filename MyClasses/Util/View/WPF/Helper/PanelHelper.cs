using AMD.Util.View.WPF.Spinners;
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

    public static void Loading(Panel contentControl, bool loading, Control loadingControl = null)
    {
      if (loading)
      {
        Control lc = null;
        double width = contentControl.ActualWidth;
        double height = contentControl.ActualHeight;
        lc = loadingControl ?? new DuckSpinner();
        lc.MaxWidth = Math.Min(600, 0 == width ? 600 : width);
        lc.MaxHeight = Math.Min(600, 0 == height ? 600 : height);
        contentControl.Children.Add(new Canvas() { Background = Brushes.Black, Opacity = 0.4 });
        contentControl.Children.Add(lc);
        contentControl.Visibility = Visibility.Visible;
      }
      else
      {
        contentControl.Visibility = Visibility.Hidden;
        contentControl.Children.Clear();
      }
    }
  }
}
