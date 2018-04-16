using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Data
{
  public static class ValueHelper
  {
    public static class AmdBitConverter
    {
      public static UInt32 ToUInt32(byte[] bytes, int index, bool littleEndian = true)
      {
        UInt32 retVal = 0;
        int length = Math.Min(index + 4, bytes.Length);
        if (littleEndian)
        {
          for (int i = index; i < length; i++)
          {
            retVal |= (UInt32)(bytes[i] << i * 8);
          }
        }
        else
        {
          for (int i = index; i < length; i++)
          {
            retVal |= (UInt32)(bytes[i] << (24 - (i * 8)));
          }
        }
        return retVal;
      }
    }
  }
}
