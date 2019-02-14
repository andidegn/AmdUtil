using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AMD.Util.DataType
{
  public class C_Array : C_Variable, IMemberCollection
  {
    #region XmlIgnore
    [XmlIgnore]
    public override String FormattedValue
    {
      get
      {
        if (IsPointer)
        {
          if (Value != null)
          {
            if (Value is UInt32)
            {
              return String.Format("(0x{0:X8}) [{1}]", Value, ArrayLength);
            }
            return "Error, addr not U32";
          }
        }
        return String.Format("[{0}]", ArrayLength);
      }
    }

    [XmlIgnore]
    public String FormattedValueArray
    {
      get
      {
        String ret = String.Empty;
        switch (Type)
        {
          case C_Type.BIT:
            //ret = String.Format("[{0}]", String.Join(", ", Members.Select(x => x.FormattedValue)));
            ret = GetBitArrayString(new List<Bit>(Members.Select(x => x.Value as Bit)));
            break;
          case C_Type.BOOL:
          case C_Type.U8:
          case C_Type.S8:
          case C_Type.U16:
          case C_Type.S16:
          case C_Type.U32:
          case C_Type.S32:
          case C_Type.U64:
          case C_Type.S64:
            ret = String.Format("{{{0}}}", String.Join(", ", Members.Select(x => x.FormattedValue)));
            break;
          default:
            ret = String.Format("Error: {0}", Type);
            break;
        }
        return ret;
      }
    }
    #endregion // XmlIgnore

    private UInt32 _arrayLength;
    public UInt32 ArrayLength
    {
      get
      {
        return _arrayLength;
      }
      set
      {
        _arrayLength = value;
        UpdatePropertyChangedArrayLength();
      }
    }

    public override UInt32 SizeInBits
    {
      get
      {
        if (Type == C_Type.STRUCT)
        {
          UInt32 _size = 0;
          foreach (C_Variable cvItem in Members)
          {
            _size += cvItem.ActualSizeInBits;
          }
          return _size;
        }
        else
        {
          return ArrayLength * GetSizeOf(this);
        }
      }
    }

    public override UInt32 ActualSizeInBits
    {
      get
      {
        if (IsPointer)
        {
          return 0x20;
        }
        else if (Type == C_Type.STRUCT)
        {
          UInt32 _size = 0;
          foreach (C_Variable cvItem in Members)
          {
            _size += cvItem.ActualSizeInBits;
          }
          return _size;
        }
        else
        {
          return ArrayLength * GetSizeOf(this);
        }
      }
    }

    #region FormattedArray
    private String GetBitArrayString(IList<Bit> arr)
    {
      UInt32 value = 0;
      StringBuilder sb = new StringBuilder();
      for (int i = arr.Count - 1; i >= 0; i--)
      {
        sb.AppendFormat("{0}{1}", arr[i].ToString(), (i) % 8 == 0 && i > 0 ? " " : "");
        value |= (UInt32)(arr[i].Value << i);
      }
      return String.Format("{0, -8} bits = ({1})", value.ToString("X"), sb.ToString());
    }

    private String GetByteArrayString(byte[] arr, bool showAsWord, bool littleEndian = true)
    {
      StringBuilder sb = new StringBuilder();
      if (showAsWord)
      {
        UInt32 tmp = 0;
        for (int i = 0; i < arr.Length; i++)
        {
          int byteNo = i % 4;
          int bitShift = 8 * (littleEndian ? byteNo : 3 - byteNo);
          tmp |= (UInt32)(arr[i] << bitShift);
          if (byteNo == 3 || i == arr.Length - 1)
          {
            sb.AppendFormat("{0}{1}", tmp.ToString("X8"), i % 31 == 0 ? "\n" : " ");
            tmp = 0;
          }
        }
        //int indexOfSpace = 0;
        //foreach (var item in arr)
        //{
        //  sb.AppendFormat("{0}{1}", item.ToString("X2"), ++indexOfSpace % 4 == 0 ? " " : "");
        //}
      }
      else
      {
        for (int i = 0; i < arr.Length; i++)
        {
          sb.AppendFormat("{0}{1}", arr[i].ToString("X2"), (i + 1) % 16 == 0 ? "\n" : " ");
        }
        //foreach (var item in arr)
        //{
        //  sb.AppendFormat("{0} ", item.ToString("X2"));
        //}
      }
      return sb.ToString();
    }

    private String GetWordArrayString(UInt32[] arr, bool showAsByte, bool littleEndian = true)
    {
      StringBuilder sb = new StringBuilder();
      if (showAsByte)
      {
        for (int i = 0; i < arr.Length; i++)
        {
          for (int j = 3; j >= 0; j--)
          {
            sb.AppendFormat("{0} ", ((arr[i] >> 8 * (littleEndian ? 3 - j : j)) & 0xFF).ToString("X2"));
          }
          if (i < arr.Length - 1 && (i + 1) % 4 == 0)
          {
            sb.AppendLine();
          }
        }
      }
      else
      {
        for (int i = 0; i < arr.Length; i++)
        {
          sb.AppendFormat("{0}{1}", arr[i].ToString("X8"), (i + 1) % 8 == 0 ? "\n" : " ");
        }
      }
      return sb.ToString();
    }
    #endregion // FormattedArray

    public C_Array()
      : base()
    {
      DisplayType = C_DisplayType.Array;
      Members = new ObservableCollection<C_Variable>();
      IsArray = true;
    }

    public override C_Variable Clone()
    {
      ObservableCollection<C_Variable> members = new ObservableCollection<C_Variable>();
      C_Array caTmp = new C_Array()
      {
        ArrayLength = this.ArrayLength,
        Name = this.Name,
        Type = this.Type,
        Members = members,
        DisplayType = this.DisplayType,
        Parent = this.Parent,
        IsArray = this.IsArray,
        IsPointer = this.IsPointer,
        Passed = this.Passed,
        CheckEqual = this.CheckEqual,
        FixedAddr = this.FixedAddr
      };
      foreach (C_Variable cvItem in Members)
      {
        C_Variable cvItemClone = cvItem.Clone();
        cvItemClone.Parent = caTmp;
        members.Add(cvItemClone);
      }
      caTmp.SetAddress(Address);
      return caTmp;
    }

    public override bool ValidateValue(C_Variable cv)
    {
      if (cv is C_Array)
      {
        C_Array ca = cv as C_Array;
        Passed = true;
        CheckEqual = cv.CheckEqual;
        for (int i = 0; i < Math.Min(Members.Count, ca.Members.Count); i++)
        {
          Passed &= Members[i].ValidateValue(ca.Members[i]);
          if (Passed == false || Members[i].IsExpanded || ca.Members[i].IsExpanded/* || !(Members[i].IsSelected && ca.Members[i].IsSelected)*/)
          {
            IsExpanded = true;
          }
        }
        cv.Passed = Passed;
      }
      else
      {
        Passed = IsSelected && cv.IsSelected ? false : true;
      }
      return Passed == true;
    }

    public override bool Equals(object obj)
    {
      C_Array ca = obj as C_Array;
      return ca != null && Name == ca.Name && (Members != null && ca.Members != null ? Members.SequenceEqual(ca.Members) : Members == ca.Members);
    }

    public override int GetHashCode()
    {
      var hashCode = -1129514919;
      hashCode = hashCode * -1521134295 + base.GetHashCode();
      hashCode = hashCode * -1521134295 + ArrayLength.GetHashCode();
      return hashCode;
    }

    public override String ToString()
    {
      return String.Format("{0}[{1}] {2}", Type, ArrayLength, Name);
    }
  }
}
