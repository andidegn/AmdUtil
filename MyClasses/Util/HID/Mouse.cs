using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AMD.Util.HID
{
	public class MouseUtil
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref Win32Point pt);

		[StructLayout(LayoutKind.Sequential)]
		internal struct Win32Point
		{
			public Int32 X;
			public Int32 Y;
		};

		public static Point GetMousePosition()
		{
			Win32Point w32Mouse = new Win32Point();
			GetCursorPos(ref w32Mouse);
			return new Point(w32Mouse.X, w32Mouse.Y);
		}

		public static void SetMousePosition(int x, int y)
		{
			System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);
		}

		public static bool IsOver(Control sender, String name)
		{
			return (sender.Template.FindName(name, sender) as UIElement).IsMouseOver;
		}

		public static bool IsOver(Control sender, Control isOver)
		{
			return IsOver(sender, isOver.Name);
		}
	}
}
