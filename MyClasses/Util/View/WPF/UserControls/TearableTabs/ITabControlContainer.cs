using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AMD.Util.View.WPF.UserControls.TearableTabs
{
  public interface ITabControlContainer : IDisposable
  {
    ITabControlContainer TabParent { get; set; }
    bool AddSplitControl(DropLocation location, DropLocation sourceLocation, TearableTabItem ttItem);
    bool AddControl(DropLocation location, TearableTabControl existingTabControl, TearableTabItem newTabItem, ref Grid droppedItemParent);
    bool RemoveControl(ITabControlContainer sender, FrameworkElement controlToInsertInPlaceOfRemoved);
    bool DropTab(TearableTabItem tabItem);
  }
}
