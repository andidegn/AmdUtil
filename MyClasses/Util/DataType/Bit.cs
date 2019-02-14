using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.DataType
{
  [Serializable]
  public class Bit
  {
    public byte Value
    {
      get
      {
        return (byte)(BoolValue ? 1 : 0);
      }
    }

    public bool BoolValue { get; set; }

    /// <summary>
    /// Empty constructor explicite to make XmlSerializer work
    /// </summary>
    public Bit() { }

    public Bit(byte value)
    {
      BoolValue = value > 0;
    }

    public Bit(bool value)
    {
      BoolValue = value;
    }

    public Bit(byte value, int bit)
    {
      SetValue(value, bit);
    }

    public Bit(UInt32 value, int bit)
    {
      SetValue(value, bit);
    }

    public void SetValue(byte value, int bit)
    {
      BoolValue = ((value >> bit) & 0x01) > 0;
    }

    public void SetValue(UInt32 value, int bit)
    {
      BoolValue = ((value >> bit) & 0x01) > 0;
    }

    public static Bit[] GetBitArray(UInt32 value, int startBit, int length)
    {
      Bit[] bitArr = new Bit[length];
      for (int i = 0; i < length; i++)
      {
        bitArr[length - i - 1] = new Bit(value, startBit - i);
      }
      return bitArr;
    }

    public override String ToString()
    {
      return BoolValue ? "1" : "0";
    }

    public override bool Equals(object obj)
    {
      return obj is Bit ? BoolValue.Equals((obj as Bit).BoolValue) : false;
    }

    public override int GetHashCode()
    {
      return -1939223833 + BoolValue.GetHashCode();
    }
  }

  public interface IUnionContent
  {
    void SetUnionContent(UInt32 value);
  }
}
