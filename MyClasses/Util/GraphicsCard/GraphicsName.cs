using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace AMD.Util.GraphicsCard
{
	public class GraphicsName
	{
		/// <summary>
		/// Wrap the graphics card name retrieval
		/// </summary>
		/// <returns></returns>
		public static string GetGCName()
		{
			return GetGraphicsCardNameWMI() ?? GetGraphicsCardNameRegistry();
		}

		/// <summary>
		/// Attempts to retrieve the graphics card name from the WMI
		/// </summary>
		/// <returns>Graphics card name or null</returns>
		public static string GetGraphicsCardNameWMI()
		{
			try
			{
				ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");

				foreach (ManagementObject obj in searcher.Get())
				{
					return (string)obj.GetPropertyValue("Name");
				}
				return null;
			}
			catch (Exception ex)
			{
				return null;
			}

		}
		/// <summary>
		/// Attempts to retrieve the graphics card name from the registry
		/// </summary>
		/// <returns>Graphics card name</returns>
		public static string GetGraphicsCardNameRegistry()
		{
			string retval = "Unknown";
			object result = null;
			var key = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "");

			//Select the path within the hive
			var subkey = key.OpenSubKey("HARDWARE\\DEVICEMAP\\VIDEO");

			//If the subkey is null, it means that the path within the hive doesn't exist
			if (subkey != null)
			{
				//Read the key
				result = subkey.GetValue("\\Device\\Video0");
				string gCardString = (string)result;
				string[] gCardstringSplit = gCardString.Split('\\');
				gCardString = "";
				for (int i = 3; i < gCardstringSplit.Length; i++)
				{
					if (gCardString.Length > 0)
					{
						gCardString += "\\";
					}
					gCardString += gCardstringSplit[i];
				}

				subkey = key.OpenSubKey(gCardString);
				if (subkey != null)
				{
					result = subkey.GetValue("Device Description");
					if (result != null)
					{
						retval = (string)result;
					}
				}
			}
			return retval;
		}
	}
}
