using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
	}
}
