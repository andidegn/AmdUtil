using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace AMD.Util.DataType
{
  public class C_Primitive : C_Variable
  {
    #region XmlIgnore
    [XmlIgnore]
    public override String FormattedValue
    {
      get
      {
        String ret = String.Empty;
        char alsoEqual = '\u2a6f';
        if (Value != null)
        {
          if (IsPointer)
          {
            ret = String.Format("(0x{0:X8})", Value);
          }
          else
          {
            switch (Type)
            {
              case C_Type.BOOL:
                ret = (UInt32)Value > 0 ? "True" : "False";
                break;
              case C_Type.BIT:
                ret = (Value as Bit).ToString();
                break;
              case C_Type.U8:
              case C_Type.S8:
                ret = String.Format("0x{0} {1} {2}", ((byte)Value).ToString("X2"), alsoEqual, Value);
                break;
              case C_Type.U16:
              case C_Type.S16:
                ret = String.Format("0x{0} {1} {2}", ((UInt16)Value).ToString("X4"), alsoEqual, Value);
                break;
              case C_Type.U32:
              case C_Type.S32:
                ret = String.Format("0x{0} {1} {2}", ((UInt32)Value).ToString("X8"), alsoEqual, Value);
                break;
              case C_Type.U64:
              case C_Type.S64:
                ret = String.Format("0x{0} {1} {2}", ((UInt64)Value).ToString("X16"), alsoEqual, Value);
                break;
              default:
                ret = String.Format("Error: {0}", Type);
                break;
            }
          }
        }
        else
        {
          ret = "null";
        }
        return ret;
      }
    }
    #endregion // XmlIgnore

    public override UInt32 SizeInBits
    {
      get
      {
        return GetSizeOf(this);
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
        return GetSizeOf(this);
      }
    }

    public C_Primitive()
      : base()
    {
      DisplayType = C_DisplayType.Paragraph;
      Members = new ObservableCollection<C_Variable>();
    }

    public override C_Variable Clone()
    {
      ObservableCollection<C_Variable> members = new ObservableCollection<C_Variable>();
      C_Primitive cpTmp = new C_Primitive()
      {
        Type = this.Type,
        Name = this.Name,
        Value = this.Value,
        Members = members,
        DisplayType = this.DisplayType,
        Parent = this.Parent,
        Passed = this.Passed,
        CheckEqual = this.CheckEqual,
        IsArray = this.IsArray,
        IsPointer = this.IsPointer,
        FixedAddr = this.FixedAddr
      };
      foreach (C_Variable cvItem in Members)
      {
        C_Variable cvItemClone = cvItem.Clone();
        cvItemClone.Parent = cpTmp;
        members.Add(cvItemClone);
      }
      cpTmp.SetAddress(Address);
      return cpTmp;
    }

    public override bool ValidateValue(C_Variable cv)
    {
      CheckEqual = cv.CheckEqual;
      if (IsSelected && cv.IsSelected)
      {
        if (cv.CheckEqual)
        {
          Passed = cv.Passed = this.Equals(cv);
        }
        else
        {
          Passed = cv.Passed = !this.Equals(cv);
        }
      }
      else
      {
        Passed = cv.Passed = true;
      }
      if (Passed == false/* || !(IsSelected && cv.IsSelected)*/)
      {
        IsExpanded = true;
      }
      return Passed == true;
    }

    public override bool Equals(object obj)
    {
      C_Primitive cp = obj as C_Primitive;
      if (cp != null)
      {
        bool nameEqual = Name == cp.Name;

        bool valueEqual;

        if (Value != null && cp.Value != null)
        {
          valueEqual = Value.Equals(cp.Value);
        }
        else
        {
          valueEqual = Value == cp.Value;
        }
        return nameEqual & valueEqual;
      }
      else
      {
        return false;
      }
    }

    public override int GetHashCode()
    {
      var hashCode = -1121648537;
      hashCode = hashCode * -1521134295 + base.GetHashCode();
      if (Value != null)
      {
        hashCode = hashCode * -1521134295 + Value.GetHashCode();
      }
      return hashCode;
    }

    public override String ToString()
    {
      return String.Format("{0} {1} = {2}", Type, Name, FormattedValue);
    }
  }
}
