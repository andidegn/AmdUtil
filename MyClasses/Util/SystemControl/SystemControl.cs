using System.Management;

namespace AMD.Util.SystemControl
{
	/// <summary>
	/// Contains functions to control some system functions like restart and shutdown
	/// </summary>
	public static class SystemControl
	{
		/// <summary>
		/// Initiates a restart of the system
		/// </summary>
		public static void Restart()
		{
			InitiatePowerCall(2);
		}

		/// <summary>
		/// Initiates a shutdown of the system
		/// </summary>
		public static void Shutdown()
		{
			InitiatePowerCall(1);
		}

		private static void InitiatePowerCall(int flag)
		{
			ManagementBaseObject mboShutdown = null;
			ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
			mcWin32.Get();

			// You can't shutdown without security privileges
			mcWin32.Scope.Options.EnablePrivileges = true;
			ManagementBaseObject mboShutdownParams =
					 mcWin32.GetMethodParameters("Win32Shutdown");

			// Flag 1 means we want to shut down the system. Use "2" to reboot.
			mboShutdownParams["Flags"] = flag.ToString();
			mboShutdownParams["Reserved"] = "0";
			foreach (ManagementObject manObj in mcWin32.GetInstances())
			{
				mboShutdown = manObj.InvokeMethod("Win32Shutdown",
											   mboShutdownParams, null);
			}
		}
	}
}
