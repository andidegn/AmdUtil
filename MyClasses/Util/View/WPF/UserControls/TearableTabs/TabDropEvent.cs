using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.View.WPF.UserControls.TearableTabs
{
  public enum DropLocation
  {
    Center,
    Top,
    Bottom,
    Left,
    Right,
    NA
  }

  public class TabDropEventArgs
  {
    public DropLocation Location { get; set; }
    public TearableTabItem TearableTab { get; set; }

    public TabDropEventArgs(DropLocation location, TearableTabItem tearableTab)
    {
      this.Location = location;
      this.TearableTab = tearableTab;
    }
  }
}
