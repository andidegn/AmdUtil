using System.Windows.Input;

namespace AMD.Util.HID
{
	public static class Modifier
	{
		public static bool IsShiftDown
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			}
		}

		public static bool IsCtrlDown
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			}
		}

		public static bool IsAltDown
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
			}
		}

		public static bool IsWindowsDown
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin);
			}
		}
	}
}
