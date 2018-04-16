using System.Windows.Input;

namespace AMD.Util.HID
{
	public static class Modifier
	{
		public static bool IsShiftDown
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LeftShift) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift);
			}
		}

		public static bool IsCtrlDown
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl);
			}
		}

		public static bool IsAltDown
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LeftAlt) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightAlt);
			}
		}
	}
}
