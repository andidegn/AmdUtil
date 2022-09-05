using AMD.Util.Extensions;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Data
{
  public enum IntelHexRecordType
  {
    /// <summary>
    /// Contains data and a 16-bit starting address for the data. The byte count specifies number of data bytes in the record. The example shown to the right has 0B (decimal 11) data bytes (61, 64, 64, 72, 65, 73, 73, 20, 67, 61, 70) located at consecutive addresses beginning at address 0010.
    /// </summary>
    Data = 0x00,

    /// <summary>
    /// Must occur exactly once per file in the last line of the file. The data field is empty (thus byte count is 00) and the address field is typically 0000.
    /// </summary>
    EoF = 0x01,

    /// <summary>
    /// The data field contains a 16-bit segment base address (thus byte count is 02) compatible with 80x86 real mode addressing. The address field (typically 0000) is ignored. The segment address from the most recent 02 record is multiplied by 16 and added to each subsequent data record address to form the physical starting address for the data. This allows addressing up to one megabyte of address space.
    /// </summary>
    ExtendedSegmentAddr = 0x02,

    /// <summary>
    /// For 80x86 processors, specifies the initial content of the CS:IP registers. The address field is 0000, the byte count is 04, the first two bytes are the CS value, the latter two are the IP value.
    /// </summary>
    StartSegmentAddr = 0x03,

    /// <summary>
    /// Allows for 32 bit addressing (up to 4GiB). The address field is ignored (typically 0000) and the byte count is always 02. The two encoded, big endian data bytes specify the upper 16 bits of the 32 bit absolute address for all subsequent type 00 records; these upper address bits apply until the next 04 record. If no type 04 record precedes a 00 record, the upper 16 address bits default to 0000. The absolute address for a type 00 record is formed by combining the upper 16 address bits of the most recent 04 record with the low 16 address bits of the 00 record.
    /// </summary>
    ExtendedLinearAddr = 0x04,

    /// <summary>
    /// The address field is 0000 (not used) and the byte count is 04. The four data bytes represent the 32-bit value loaded into the EIP register of the 80386 and higher CPU.
    /// </summary>
    StartLinearAddr = 0x05,
    MAX = StartLinearAddr
  }

  /// <summary>
  /// Intel Hex file handler
  /// </summary>
  public class IntelHexToBin : List<IntelHexRecord>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="path"></param>
    public IntelHexToBin(string path)
    {
      if (!File.Exists(path))
      {
        throw new FileNotFoundException(string.Format("The file does not exist: {0}", path));
      }

      string[] lines = File.ReadAllLines(path);
      foreach (var item in lines)
      {
        this.Add(new IntelHexRecord(item));
      }
    }

    /// <summary>
    /// Gets a byte array with the compiled firmware byte values
    /// </summary>
    /// <param name="startAddr"></param>
    /// <param name="endAddr"></param>
    /// <param name="fillValue"></param>
    /// <returns></returns>
    public byte[] GetData(uint startAddr, uint endAddr = 0x00, byte fillValue = 0x00, bool pad = false)
    {
      uint segOffset = 0;
      uint extOffset = 0;
      uint? dataOffSet = null;
      uint largestBuffPos = 0;
      byte[] dataArr = null;
      if (endAddr == 0)
      {
        dataArr = new byte[0xFFFFFFFF];
      }
      else
      {
        dataArr = new byte[endAddr - startAddr];
      }
      for (uint i = 0; i < dataArr.Length; i++)
      { 
        dataArr[i] = fillValue;
      }

      foreach (IntelHexRecord record in this)
      {
        switch (record.RecordType)
        {
          case IntelHexRecordType.Data:
            if (null == dataOffSet)
            {
              dataOffSet = record.Address;
            }
            if (0x00 == endAddr || record.Address + segOffset + extOffset < endAddr)
            {
              byte[] recData = record.Data;
              if ((record.Address + segOffset + extOffset + recData.Length) >= startAddr)
              {
                for (uint i = 0; i < recData.Length; i++)
                {
                  uint recPos = segOffset + extOffset + record.Address + i;
                  if ((recPos >= startAddr) && (0x00 == endAddr || recPos < endAddr))
                  {
                    uint buffPos = recPos - startAddr - dataOffSet ?? 0;
                    try
                    {
                      dataArr[buffPos] = recData[i];
                    }
                    catch (Exception ex)
                    {
                      LogWriter.Instance.PrintException(ex);
                      throw;
                    }
                    largestBuffPos = Math.Max(largestBuffPos, buffPos);
                  }
                }
              }
              else
              {

              }
            }
            break;


          case IntelHexRecordType.ExtendedSegmentAddr:
            segOffset = (uint)((record.Data[0] << 12) | (record.Data[1] << 4));
            break;

          case IntelHexRecordType.ExtendedLinearAddr:
            extOffset = (uint)((record.Data[0] << 8) | (record.Data[1] << 16));
            break;

          case IntelHexRecordType.EoF:
          case IntelHexRecordType.StartSegmentAddr:
          case IntelHexRecordType.StartLinearAddr:
          default:
            break;
        }
      }
      return pad ? dataArr : dataArr.SubArray(0, (int)largestBuffPos + 1);
    }

    public void DumpTestFile(string path)
    {
      string values = this.ToString();
      File.WriteAllBytes(path, values.GetBytes());
    }

    public override string ToString()
    {
      int[] typeArr = new int[(int)IntelHexRecordType.MAX + 1];
      StringBuilder sb = new StringBuilder();
      foreach (var item in this)
      {
        if (item.RecordType != IntelHexRecordType.Data)
        {
          sb.AppendLine(item.CompleteRecordGeneratedFromProperties);
        }

        typeArr[(int)item.RecordType]++;
      }

      sb.AppendLine();
      for (int i = 0; i < typeArr.Length; i++)
      {
        sb.AppendFormat("{0}\t {1} records\n", typeArr[i], (IntelHexRecordType)i);
      }

      return sb.ToString();
    }
  }

  /// <summary>
  /// Record object containing relevant properties
  /// </summary>
  public class IntelHexRecord
  {
    #region Private stuff
    private enum RecordByteIndex
    {
      ByteCount = 0,
      Address = 1,
      Type = 3,
      Data = 4
    }

    private byte GetByteIndex(RecordByteIndex index)
    {
      return (byte)index;
    }

    private IntelHexRecordType GetRecordType(byte typeValue)
    {
      if (!Enum.IsDefined(typeof(IntelHexRecordType), (int)typeValue))
      {
        log.WriteToLog(LogMsgType.Debug, "Invalid IntelHexRecordType: {0}", typeValue);
        throw new Exception("Type does not exist");
      }
      IntelHexRecordType type = (IntelHexRecordType)typeValue;
      return type;
    }

    private bool ValidateChecksum(byte checksum, byte[] recordValues)
    {
      return checksum == CalculateChecksum(recordValues);
    }

    private byte CalculateChecksum(byte[] recordValues)
    {
      byte val = 0;
      for (int i = 0; i < recordValues.Length - 1; i++)
      {
        val += recordValues[i];
      }
      return (byte)(0x100 - val);
    }

    private LogWriter log;
    #endregion // Private stuff

    /// <summary>
    /// Number of data bytes. The maximum byte count is 255 (0xFF). 16 (0x10) and 32 (0x20) are commonly used byte counts.
    /// </summary>
    public byte ByteCount { get; private set; }

    /// <summary>
    /// 2 byte address of the data. Normally big endian
    /// </summary>
    public UInt16 Address { get; private set; }

    /// <summary>
    /// Record type of value 0x00-0x05
    /// </summary>
    public IntelHexRecordType RecordType { get; private set; }

    /// <summary>
    /// The data
    /// </summary>
    public byte[] Data { get; private set; }

    /// <summary>
    /// Two's compliment checksum
    /// </summary>
    public byte Checksum { get; private set; }

    /// <summary>
    /// The complete record as given in the constructor
    /// </summary>
    public string CompleteRecord { get; private set; }

    public string CompleteRecordGeneratedFromProperties
    {
      get
      {
        return $"{ByteCount.ToString("X2")}{Address.ToString("X4")}{((byte)RecordType).ToString("X2")}{Data.GetHexString('\0')}{Checksum.ToString("X2")}";
      }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="record"></param>
    public IntelHexRecord(string record)
    {
      log = LogWriter.Instance;
      CompleteRecord = record;
      if (!CompleteRecord.StartsWith(":"))
      {
        log.WriteToLog(LogMsgType.Debug, "Record syntax error: {0}", CompleteRecord);
        throw new FormatException("The record string is not in the correct syntax format");
      }

      byte[] recordValues = CompleteRecord.Substring(1).GetBytesFromHex();
      ByteCount = recordValues[GetByteIndex(RecordByteIndex.ByteCount)];

      if (recordValues.Length != ByteCount + 5)
      {
        log.WriteToLog(LogMsgType.Debug, "Record length error: {0}", CompleteRecord);
        throw new Exception("Record length is not correct");
      }

      int addressIndex = GetByteIndex(RecordByteIndex.Address);
      Address = (UInt16)((recordValues[addressIndex] << 8) | recordValues[addressIndex + 1]);
      RecordType = GetRecordType(recordValues[GetByteIndex(RecordByteIndex.Type)]);
      Data = recordValues.SubArray<byte>(GetByteIndex(RecordByteIndex.Data), ByteCount);
      Checksum = recordValues[recordValues.Length - 1];
      if (!ValidateChecksum(Checksum, recordValues))
      {
        byte calculatedChecksum = CalculateChecksum(recordValues);
        log.WriteToLog(LogMsgType.Debug, "Checksums does not match. Calculated: {0}, expected: {1}, record: {2}", calculatedChecksum, Checksum, CompleteRecord);
        throw new InvalidChecksumException(string.Format("The checksum is not valid. Calculated: {0}, expected: {1}", Checksum, CalculateChecksum(recordValues)));
      }
    }
  }

  /// <summary>
  /// Invalid Checksum exception
  /// </summary>
  public class InvalidChecksumException : Exception
  {
    /// <summary>
    /// Zero argument constructor
    /// </summary>
    public InvalidChecksumException()
    {

    }

    /// <summary>
    /// One argument constructor
    /// </summary>
    /// <param name="Message">Describing message</param>
    public InvalidChecksumException(string Message)
      : base(Message)
    {

    }

    /// <summary>
    /// Two argument constructor
    /// </summary>
    /// <param name="Message">Describing message</param>
    /// <param name="InnerException">Child exception</param>
    public InvalidChecksumException(string Message, Exception InnerException)
      : base(Message, InnerException)
    {

    }
  }
}
