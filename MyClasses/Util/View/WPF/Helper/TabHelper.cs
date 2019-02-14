using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AMD.Util.View.WPF.Helper
{
  /// <summary>
  /// Helper class for tab control and tab item
  /// </summary>
  public static class TabHelper
  {
    /// <summary>
    /// Function which adjusts the width of the tabs in an ItemCollection
    /// </summary>
    /// <param name="ic"></param>
    public static void AdjustTabItemWidth(ItemCollection ic)
    {
      if (ic != null)
      {
        double adjuster = 10d / Math.Max(1, ic.Count);
        //if (5 > ic.Count)
        //{
        //  adjuster = 2;
        //}
        //else if (11 > ic.Count)
        //{
        //  adjuster = 1;
        //}
        //else if (21 > ic.Count)
        //{
        //  adjuster = 0.4;
        //}
        //else if (41 > ic.Count)
        //{
        //  adjuster = 0.2;
        //}
        //else
        //{
        //  adjuster = 0.1;
        //}
        foreach (TabItem ti in ic)
        {
          TabControl tc = ti.Parent as TabControl;
          if (tc != null)
          {
            double curMar = ti.Margin.Left + ti.Margin.Right;
            ti.Width = Math.Max(0, (double)(tc.ActualWidth / tc.Items.Count) - adjuster);
            //ti.Width = Math.Max(0, (double)(tc.ActualWidth / tc.Items.Count) - 0.1);
          }
        }
      }
    }
  }
}
