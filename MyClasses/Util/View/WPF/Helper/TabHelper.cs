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
        double adjuster = 6d / Math.Max(1, ic.Count);
        foreach (TabItem ti in ic)
        {
          TabControl tc = ti.Parent as TabControl;
          if (tc != null)
          {
            double curMar = ti.Margin.Left + ti.Margin.Right;
            ti.Width = Math.Max(0, (double)(tc.ActualWidth / tc.Items.Count) - adjuster);
          }
        }
      }
    }
  }
}
