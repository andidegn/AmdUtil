using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;

namespace AMD.Util.Collections
{
  public class BitArray : IEnumerable<bool>
  {
    public int Size { get; private set; }

    private uint[] data32;
    private ulong[] data64;

    private int bitSize => (Environment.Is64BitProcess ? 64 : 32);

    public BitArray(int size)
    {
      Size = size;
      if (Environment.Is64BitProcess)
      {
        data64 = new ulong[size / bitSize];
      }
      else
      {
        data32 = new uint[size / bitSize];
      }
    }

    public void Set(int index, bool value)
    {
      if (value)
      {
        SetHigh(index);
      }
      else
      {
        SetLow(index);
      }
    }

    public void SetHigh(int index)
    {
      if (Environment.Is64BitProcess)
      {
        data64[index / bitSize] |= 1ul << (index % bitSize);
      }
      else
      {
        data32[index / bitSize] |= 1u << (index % bitSize);
      }
    }

    public void SetLow(int index)
    {
      if (Environment.Is64BitProcess)
      {
        data64[index / bitSize] &= ~(1ul << (index % bitSize));
      }
      else
      {
        data32[index / bitSize] &= ~(1u << (index % bitSize));
      }
    }

    public bool Get(int index)
    {
      if (Environment.Is64BitProcess)
      {
        return 1 < (data64[index / bitSize] & 1ul << (index % bitSize));
      }
      return 1 < (data32[index / bitSize] & 1u << (index % bitSize));
    }

    #region GetArray
    public byte[] GetByteArray()
    {
      return GetDataArray(typeof(byte[])) as byte[];
    }

    public short[] GetShortArray()
    {
      return GetDataArray(typeof(short[])) as short[];
    }

    public int[] GetIntArray()
    {
      return GetDataArray(typeof(int[])) as int[];
    }

    public long[] GetLongArray()
    {
      return GetDataArray(typeof(long[])) as long[];
    }

    public sbyte[] GetSByteArray()
    {
      return GetDataArray(typeof(sbyte[])) as sbyte[];
    }

    public ushort[] GetUShortArray()
    {
      return GetDataArray(typeof(ushort[])) as ushort[];
    }

    public uint[] GetUIntArray()
    {
      return GetDataArray(typeof(uint[])) as uint[];
    }

    public ulong[] GetULongArray()
    {
      return GetDataArray(typeof(ulong[])) as ulong[];
    }

    //public byte[] GetByteArray()
    //{
    //  byte[] dstArr;
    //  Array srcArr;
    //  if (Environment.Is64BitProcess)
    //  {
    //    srcArr = data64;
    //  }
    //  else
    //  {
    //    srcArr = data32;
    //  }
    //  dstArr = new byte[srcArr.Length * bitSize];
    //  Buffer.BlockCopy(srcArr, 0, dstArr, 0, dstArr.Length);
    //  return dstArr;
    //}

    public Array GetDataArray(Type t)
    {
      if (!t.IsArray)
      {
        return null;
      }
      Array srcArr, dstArr;
      int size = -1;
      if (Environment.Is64BitProcess)
      {
        srcArr = data64;
      }
      else
      {
        srcArr = data32;
      }
      var v1 = t.GetElementType();
      var tc = Type.GetTypeCode(v1);
      switch (Type.GetTypeCode(t.GetElementType()))
      {
        case TypeCode.Int16:
        case TypeCode.UInt16:
          size = 16;
          break;

        case TypeCode.Int32:
        case TypeCode.UInt32:
          size = 32;
          break;

        case TypeCode.Int64:
        case TypeCode.UInt64:
          size = 64;
          break;

        case TypeCode.Byte:
        case TypeCode.Char:
        case TypeCode.SByte:
        default:
          size = 8;
          break;
      }

      dstArr = Activator.CreateInstance(t, bitSize * srcArr.Length / size) as Array;
      Buffer.BlockCopy(srcArr, 0, dstArr, 0, srcArr.Length * bitSize / 8);
      return dstArr;
    }
    #endregion // GetArray

    public IEnumerator<bool> GetEnumerator()
    {
      for (int i = 0; i < Size; i++)
      {
        yield return Get(i);
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
