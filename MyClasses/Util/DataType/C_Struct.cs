using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AMD.Util.DataType
{
  public class C_Struct : C_Variable, ITypeDef, IMemberCollection, INotifyPropertyChanged
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
              return String.Format("(0x{0:X8})", Value);
            }
            return "Error, addr not U32";
          }
        }
        return null;
      }
    }

    [XmlIgnore]
    public String FormattedValueStruct
    {
      get
      {
        StringBuilder sb = new StringBuilder("= (");
        for (int i = 0; i < Members.Count; i++)
        {
          C_Variable member = Members[i];
          sb.AppendFormat("{0} = {1}", member.Name, member.FormattedValue);
          if (i < Members.Count - 1)
          {
            sb.Append(", ");
          }
          else
          {
            sb.Append(")");
          }
        }
        return sb.ToString();
      }
    }
    #endregion // XmlIgnore

    private String _typeDefName;
    public String TypeDefName
    {
      get
      {
        return _typeDefName;
      }
      set
      {
        _typeDefName = value;
        UpdatePropertyChangedTypeDefName();
      }
    }

    public override UInt32 SizeInBits
    {
      get
      {
        UInt32 size = 0;
        foreach (C_Variable cv in Members)
        {
          if (cv == null)
          {
            throw new NullReferenceException("No members should ever be null");
          }
          size += cv.ActualSizeInBits;
        }
        return size;
      }
    }

    public override UInt32 ActualSizeInBits
    {
      get
      {
        UInt32 size = 0;
        if (IsPointer)
        {
          size = 0x20;
        }
        else
        {
          foreach (C_Variable cv in Members)
          {
            if (cv == null)
            {
              throw new NullReferenceException("No members should ever be null");
            }
            size += cv.ActualSizeInBits;
          }
        }
        return size;
      }
    }

    public C_Struct()
      : base()
    {
      Members = new ObservableCollection<C_Variable>();
      Type = C_Type.STRUCT;
      DisplayType = C_DisplayType.SubHeader;
      Members.CollectionChanged += (s, e) =>
      {
        UpdatePropertyChangedSize();
      };
    }

    public C_Variable GetMember(String name)
    {
      return (from v in Members
              where v.Name == name
              select v).SingleOrDefault();
    }

    public new int CompareTo(object obj)
    {
      if (!String.IsNullOrWhiteSpace(TypeDefName))
      {
        if (obj is ITypeDef)
        {
          return TypeDefName.CompareTo((obj as ITypeDef).TypeDefName);
        }
        if (obj is C_Variable)
        {
          return TypeDefName.CompareTo((obj as C_Variable).Name);
        }
      }
      return -1;
    }

    public override C_Variable Clone()
    {
      ObservableCollection<C_Variable> members = new ObservableCollection<C_Variable>();
      C_Struct csTmp = new C_Struct()
      {
        Name = this.Name,

        TypeDefName = this.TypeDefName,
        DisplayType = this.DisplayType,
        Parent = this.Parent,
        Value = this.Value,
        IsArray = this.IsArray,
        IsPointer = this.IsPointer,
        Passed = this.Passed,
        CheckEqual = this.CheckEqual,
        Type = this.Type,
        Members = members,
        FixedAddr = this.FixedAddr
      };
      foreach (C_Variable cvItem in Members)
      {
        C_Variable cvItemClone = cvItem.Clone();
        cvItemClone.Parent = csTmp;
        members.Add(cvItemClone);
      }
      csTmp.SetAddress(Address);
      return csTmp;
    }

    public override bool ValidateValue(C_Variable cv)
    {
      if (cv is C_Struct)
      {
        C_Struct cs = cv as C_Struct;
        CheckEqual = cv.CheckEqual;
        if (IsPointer && IsSelected && cv.IsSelected)
        {
          if (Value != null)
          {
            if (cv.CheckEqual)
            {
              Passed = cv.Passed = Value.Equals(cv.Value);
            }
            else
            {
              Passed = cv.Passed = !Value.Equals(cv.Value);
            }
          }
          else
          {
            Passed = false;
          }
        }
        else
        {
          Passed = cv.Passed = true;
        }
        //Passed = IsPointer && IsSelected && cv.IsSelected ? Value != null ? Value.Equals(cv.Value) : false : true;
        for (int i = 0; i < Math.Min(Members.Count, cs.Members.Count); i++)
        {
          Passed &= Members[i].ValidateValue(cs.Members[i]);
          if (Passed == false || Members[i].IsExpanded || cs.Members[i].IsExpanded/* || !(Members[i].IsSelected && cs.Members[i].IsSelected)*/)
          {
            IsExpanded = true;
          }
        }
        cs.Passed = Passed;
      }
      else
      {
        Passed = IsSelected && cv.IsSelected ? false : true;
      }
      return Passed == true;
    }

    public override bool Equals(object obj)
    {
      C_Struct cs = obj as C_Struct;
      //return cs != null && TypeDefName == cs.TypeDefName && Name == cs.Name;
      if (cs != null)
      {
        bool typeDefNameEqual = TypeDefName == cs.TypeDefName;
        bool nameEqual = Name == cs.Name;
        bool membersEqual = Members.SequenceEqual(cs.Members);
        return typeDefNameEqual & nameEqual & membersEqual;
      }
      return false;
    }

    public override int GetHashCode()
    {
      var hashCode = -1128648959;
      hashCode = hashCode * -1521134295 + base.GetHashCode();
      hashCode = hashCode * -1521134295 + TypeDefName.GetHashCode();
      return hashCode;
    }

    public override String ToString()
    {
      return String.Format("{0} {1} ({2:X8})", TypeDefName, Name, Address);
    }
  }
}
