using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AMD.Util.CH341
{
	public class CH341NativeFunctions
	{
		public enum EEPROM_TYPE
		{
			ID_24C01,
			ID_24C02,
			ID_24C04,
			ID_24C08,
			ID_24C16,
			ID_24C32,
			ID_24C64,
			ID_24C128,
			ID_24C256,
			ID_24C512,
			ID_24C1024,
			ID_24C2048,
			ID_24C4096
		}

		/// <summary>
		/// Attempts to open the device
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static long CH341OpenDevice(int i);

		/// <summary>
		/// Attempts to close the device
		/// </summary>
		/// <param name="iIndex"></param>
		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static void CH341CloseDevice(int iIndex);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static long CH341GetVersion();



		//[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		//public extern static long CH341DriverCommand(int iIndex, mPWIN32_COMMAND ioCommand);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static long CH341GetDrvVersion();

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341ResetDevice(int iIndex);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341GetDeviceDescr(int iIndex, StringBuilder oBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341GetConfigDescr(int iIndex, StringBuilder oBuffer, ref int ioLength);

		//[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		//public extern static bool CH341SetIntRoutine(int iIndex, mPCH341_INT_ROUTINE iIntRoutine);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341ReadInter(int iIndex, ref int iStatus);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341AbortInter(int iIndex);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341SetParaMode(int iIndex, int iMode);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341InitParallel(int iIndex, int iMode);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341ReadData0(int iIndex, byte[] oBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341ReadData1(int iIndex, byte[] oBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341AbortRead(int iIndex);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341WriteData0(int iIndex, byte[] iBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341WriteData1(int iIndex, byte[] iBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341AbortWrite(int iIndex);


		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341GetStatus(int iIndex, ref int iStatus);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341ReadI2C(int iIndex, byte iDevice, byte iAddr, byte oByte);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341WriteI2C(int iIndex, byte iDevice, byte iAddr, byte iByte);


		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341EppReadData(int iIndex, byte[] oBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341EppReadAddr(int iIndex, byte[] oBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341EppWriteData(int iIndex, byte[] iBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341EppWriteAddr(int iIndex, byte[] iBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341EppSetAddr(int iIndex, byte iAddr);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]

		public extern static bool CH341MemReadAddr0(int iIndex, byte[] oBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341MemReadAddr1(int iIndex, byte[] oBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341MemWriteAddr0(int iIndex, byte[] iBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341MemWriteAddr1(int iIndex, byte[] iBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]

		public extern static bool CH341SetExclusive(int iIndex, int iExclusive);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341SetTimeout(int iIndex, int iWriteTimeout, int iReadTimeout);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341ReadData(int iIndex, byte[] oBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341WriteData(int iIndex, byte[] iBuffer, ref int ioLength);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static StringBuilder CH341GetDeviceName(int iIndex); // Not working return is PVOID

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static long CH341GetVerIC(int iIndex);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341FlushBuffer(int iIndex);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341WriteRead(int iIndex, int iWriteLength, byte[] iWriteBuffer, int iReadStep, int iReadTimes, ref int oReadLength, byte[] oReadBuffer);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341SetStream(int iIndex, int iMode);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341SetDelaymS(int iIndex, int iDelay);

		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341StreamI2C(int iIndex, int iWriteLength, byte[] iWriteBuffer, int iReadLength, byte[] oReadBuffer);











		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341ReadEEPROM(int iIndex, EEPROM_TYPE iEepromID, int iAddr, int iLength, [In, Out] byte[] oBuffer);


		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341WriteEEPROM(int iIndex, EEPROM_TYPE iEepromID, int iAddr, int iLength, [In, Out] byte[] iBuffer);





		[DllImport("ch341dll.DLL", CallingConvention = CallingConvention.Winapi)]
		public extern static bool CH341GetInput(int iIndex, [In, Out] byte[] iStatus);
	}
}
