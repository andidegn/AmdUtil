﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.GraphicsCard
{
  using AMD.Util.Display.Edid;
  using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Text;

	public static class EdidHelper
	{
		#region Windows API stuff
		static Guid GUID_CLASS_MONITOR = new Guid(0x4d36e96e, 0xe325, 0x11ce, 0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18);
		const int DIGCF_PRESENT = 0x00000002;
		const int ERROR_NO_MORE_ITEMS = 259;

		[DllImport("advapi32.dll", SetLastError = true)]
		static extern uint RegEnumValue(
			  UIntPtr hKey,
			  uint dwIndex,
			  StringBuilder lpValueName,
			  ref uint lpcValueName,
			  IntPtr lpReserved,
			  IntPtr lpType,
			  IntPtr lpData,
			  ref int lpcbData);

		[Flags()]
		public enum DisplayDeviceStateFlags : int
		{
			/// <summary>The device is part of the desktop.</summary>
			AttachedToDesktop = 0x1,
			MultiDriver = 0x2,
			/// <summary>The device is part of the desktop.</summary>
			PrimaryDevice = 0x4,
			/// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
			MirroringDriver = 0x8,
			/// <summary>The device is VGA compatible.</summary>
			VGACompatible = 0x10,
			/// <summary>The device is removable; it cannot be the primary display.</summary>
			Removable = 0x20,
			/// <summary>The device has more display modes than its output devices support.</summary>
			ModesPruned = 0x8000000,
			Remote = 0x4000000,
			Disconnect = 0x2000000
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct DISPLAY_DEVICE
		{
			[MarshalAs(UnmanagedType.U4)]
			public int cb;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string DeviceName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceString;
			[MarshalAs(UnmanagedType.U4)]
			public DisplayDeviceStateFlags StateFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceID;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceKey;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SP_DEVINFO_DATA
		{
			public int cbSize;
			public Guid ClassGuid;
			public uint DevInst;
			public IntPtr Reserved;
		}

		[DllImport("setupapi.dll")]
		internal static extern IntPtr SetupDiGetClassDevsEx(IntPtr ClassGuid,
			[MarshalAs(UnmanagedType.LPStr)]String enumerator,
			IntPtr hwndParent, Int32 Flags, IntPtr DeviceInfoSet,
			[MarshalAs(UnmanagedType.LPStr)]String MachineName, IntPtr Reserved);

		[DllImport("setupapi.dll", SetLastError = true)]
		internal static extern Int32 SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet,
			Int32 MemberIndex, ref SP_DEVINFO_DATA DeviceInterfaceData);

		[DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern UIntPtr SetupDiOpenDevRegKey(
			IntPtr hDeviceInfoSet,
			ref SP_DEVINFO_DATA deviceInfoData,
			int scope,
			int hwProfile,
			int parameterRegistryValueKind,
			int samDesired);

		[DllImport("user32.dll")]
		public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern int RegCloseKey(
			UIntPtr hKey);
		#endregion

		private static List<EDID> edidList;

		public static List<EDID> GetEDID(bool forceRecheck = false)
		{
			if (forceRecheck || null == edidList)
			{
				edidList = new List<EDID>();
				IntPtr pGuid = Marshal.AllocHGlobal(Marshal.SizeOf(GUID_CLASS_MONITOR));
				Marshal.StructureToPtr(GUID_CLASS_MONITOR, pGuid, false);
				IntPtr hDevInfo = SetupDiGetClassDevsEx(
					pGuid,
					null,
					IntPtr.Zero,
					DIGCF_PRESENT,
					IntPtr.Zero,
					null,
					IntPtr.Zero);

				DISPLAY_DEVICE dd = new DISPLAY_DEVICE();
				dd.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
				UInt32 dev = 0;

				string DeviceID;
				bool bFoundDevice = false;
				while (EnumDisplayDevices(null, dev, ref dd, 0) && !bFoundDevice)
				{
					DISPLAY_DEVICE ddMon = new DISPLAY_DEVICE();
					ddMon.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
					UInt32 devMon = 0;

					while (EnumDisplayDevices(dd.DeviceName, devMon, ref ddMon, 0) && !bFoundDevice)
					{
						if ((ddMon.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) != 0 && (ddMon.StateFlags & DisplayDeviceStateFlags.MirroringDriver) == 0)
						{
							bFoundDevice = GetActualEDID(out DeviceID, edidList);
						}
						devMon++;

						ddMon = new DISPLAY_DEVICE();
						ddMon.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
					}

					dd = new DISPLAY_DEVICE();
					dd.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
					dev++;
				}
			}

			return edidList;
		}

		const int DICS_FLAG_GLOBAL = 0x00000001;
		const int DIREG_DEV = 0x00000001;
		const int KEY_READ = 0x20019;
		private static bool GetActualEDID(out string DeviceID, List<EDID> lsi)
		{
			IntPtr pGuid = Marshal.AllocHGlobal(Marshal.SizeOf(GUID_CLASS_MONITOR));
			Marshal.StructureToPtr(GUID_CLASS_MONITOR, pGuid, false);
			IntPtr hDevInfo = SetupDiGetClassDevsEx(
				pGuid,
				null,
				IntPtr.Zero,
				DIGCF_PRESENT,
				IntPtr.Zero,
				null,
				IntPtr.Zero);

			DeviceID = string.Empty;

			if (null == hDevInfo)
			{
				Marshal.FreeHGlobal(pGuid);
				return false;
			}

			for (int i = 0; Marshal.GetLastWin32Error() != ERROR_NO_MORE_ITEMS; ++i)
			{
				SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();
				devInfoData.cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));

				if (SetupDiEnumDeviceInfo(hDevInfo, i, ref devInfoData) > 0)
				{
					UIntPtr hDevRegKey = SetupDiOpenDevRegKey(
						hDevInfo,
						ref devInfoData,
						DICS_FLAG_GLOBAL,
						0,
						DIREG_DEV,
						KEY_READ);

					if (hDevRegKey == null)
						continue;

          EDID si = PullEDID(hDevRegKey);
					if (si != null)
					{
						lsi.Add(si);
					}
					RegCloseKey(hDevRegKey);
				}
			}

			Marshal.FreeHGlobal(pGuid);

			return true;
		}

		public const int ERROR_SUCCESS = 0;
		private static EDID PullEDID(UIntPtr hDevRegKey)
		{
			//ScreenInformation si = null;
			EDID edid = null;
			StringBuilder valueName = new StringBuilder(128);
			uint ActualValueNameLength = 128;

			byte[] EDIdata = new byte[1024];
			IntPtr pEDIdata = Marshal.AllocHGlobal(EDIdata.Length);
			Marshal.Copy(EDIdata, 0, pEDIdata, EDIdata.Length);

			int size = 1024;
			for (uint i = 0, retValue = ERROR_SUCCESS; retValue != ERROR_NO_MORE_ITEMS; i++)
			{
				retValue = RegEnumValue(
					hDevRegKey, i,
					valueName, ref ActualValueNameLength,
					IntPtr.Zero, IntPtr.Zero, pEDIdata, ref size); // EDIdata, pSize);

				string data = valueName.ToString();
				if (retValue != ERROR_SUCCESS || !data.Contains("EDID"))
					continue;

				if (size < 1)
					continue;

				byte[] actualData = new byte[size];
				Marshal.Copy(pEDIdata, actualData, 0, size);
				string hex = System.Text.Encoding.ASCII.GetString(actualData);
				edid = new EDID(actualData);
				//si = new ScreenInformation
				//{
				//	Manufacturer = hex.Substring(90, 17).Trim().Replace("\0", string.Empty).Replace("?", string.Empty),
				//	Model = hex.Substring(108, 17).Trim().Replace("\0", string.Empty).Replace("?", string.Empty),
				//	RawEdid = actualData
				//};
			}

			Marshal.FreeHGlobal(pEDIdata);
			return edid;
		}
	}

  public class ScreenInformation
  {
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string FullEDID 
		{ 
			get
			{
				return null != RawEdid ? BitConverter.ToString(RawEdid) : string.Empty;
      }
		}
    public byte[] RawEdid { get; set; }
  }
}
