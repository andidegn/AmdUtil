using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AMD.Util.Extensions.WPF
{
	public static class ExtensionUIElement
	{
		public static void ForceRedraw(this UIElement element)
		{
			Action emptyAction = delegate { };
			element.Dispatcher.Invoke(DispatcherPriority.Render, emptyAction);
    }

    public static bool IsChildOf(this FrameworkElement c, FrameworkElement parent)
    {
      return c.Parent == parent || (null != c.Parent ? (c.Parent as FrameworkElement).IsChildOf(parent) : false);
    }
  }
}
