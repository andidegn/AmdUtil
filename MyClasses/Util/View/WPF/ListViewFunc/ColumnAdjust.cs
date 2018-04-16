using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AMD.Util.View.WPF.ListViewFunc
{
	public static class ColumnAdjust
	{
		public static void AutoSizeGridViewColumns(ListView listView)
		{
			GridView gridView = listView.View as GridView;
			if (gridView != null)
			{
				foreach (var column in gridView.Columns)
				{
					if (double.IsNaN(column.Width))
					{
						column.Width = column.ActualWidth;
						column.Width = double.NaN;
					}
				}
			}
		}
	}

	public static class ColumnSort
	{
		private static GridViewColumnHeader listViewSortCol = null;
		private static SortAdorner listViewSortAdorner = null;

		public static void ListViewColumnClickSort(ListView lv, GridViewColumnHeader column)
		{
			String sortBy = column.Tag.ToString();
			if (listViewSortCol != null)
			{
				AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
				lv.Items.SortDescriptions.Clear();
			}

			ListSortDirection newDir = ListSortDirection.Ascending;
			if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
			{
				newDir = ListSortDirection.Descending;
			}

			listViewSortCol = column;
			listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
			AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
			lv.Items.SortDescriptions.Clear();
			lv.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
		}
	}

	public class SortAdorner : Adorner
	{
		private static Geometry ascGeometry =
				Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

		private static Geometry descGeometry =
				Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

		public ListSortDirection Direction { get; private set; }

		public SortAdorner(UIElement element, ListSortDirection dir)
				: base(element)
		{
			this.Direction = dir;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if (AdornedElement.RenderSize.Width < 20)
			{
				return;
			}

			TranslateTransform transform = new TranslateTransform(AdornedElement.RenderSize.Width - 15, (AdornedElement.RenderSize.Height - 5) / 2);
			drawingContext.PushTransform(transform);

			Geometry geometry = ascGeometry;
			if (this.Direction == ListSortDirection.Descending)
			{
				geometry = descGeometry;
			}
			drawingContext.DrawGeometry(Brushes.Black, null, geometry);

			drawingContext.Pop();
		}
	}
}
