using AMD.Util.Sort;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace AMD.Util.DataType
{
  public class C_Enum : C_Variable, ITypeDef
  {
    #region XmlIgnore
    [XmlIgnore]
    public override String FormattedValue
    {
      get
      {
        if (IsPointer)
        {
          return String.Format("(0x{0:X8})", Value);
        }
        return enumWrapper?.ToString();
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

    private C_EnumWrapper enumWrapper;
    public C_EnumWrapper EnumWrapper
    {
      get
      {
        if (enumWrapper == null)
        {
          enumWrapper = new C_EnumWrapper();
        }
        return enumWrapper;
      }
      set
      {
        if (value != null)
        {
          enumWrapper = value;
          UpdatePropertyChangedValue();
          UpdatePropertyChangedEnumWrapper();
        }
        else
        {

        }
      }
    }

    [XmlIgnore]
    public UInt32 Value
    {
      get
      {
        return enumWrapper != null ? enumWrapper.Value : 0;
      }
      set
      {
        C_EnumWrapper ev = (from e in EnumValues
                            where e.Value == value
                            select e).SingleOrDefault();
        if (ev == null)
        {
          ev = new C_EnumWrapper("Value not found in enum", value);
        }
        enumWrapper = ev.Clone() as C_EnumWrapper;
        UpdatePropertyChangedValue();
        UpdatePropertyChangedEnumWrapper();
      }
    }

    private ObservableCollection<C_EnumWrapper> _enumValues;
    public ObservableCollection<C_EnumWrapper> EnumValues
    {
      get
      {
        return _enumValues;
      }
      set
      {
        _enumValues = value;
        UpdatePropertyChangedEnumValues();
      }
    }

    public C_Enum()
      : base()
    {
      EnumValues = new ObservableCollection<C_EnumWrapper>();
      Type = C_Type.ENUM;
      DisplayType = C_DisplayType.Enum;
      Members = new ObservableCollection<C_Variable>();
    }

    public void AddOrUpdateEnumValue(String name, UInt32 value)
    {
      C_EnumWrapper cew = new C_EnumWrapper(name, value);
      AddOrUpdateEnumValue(cew);
    }

    public void AddOrUpdateEnumValue(C_EnumWrapper cew)
    {
      var values = (from v in EnumValues
                    where v.Name == cew.Name || v.Value == cew.Value
                    select v).ToList();
      foreach (C_EnumWrapper cewItem in values)
      {
        EnumValues.Remove(cewItem);
      }
      EnumValues.Add(cew);
      BubbleSortIList<C_EnumWrapper>.Sort(EnumValues);
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

    public override C_Variable Clone()
    {
      ObservableCollection<C_EnumWrapper> enumValuesClone = new ObservableCollection<C_EnumWrapper>();
      ObservableCollection<C_Variable> members = new ObservableCollection<C_Variable>();
      foreach (C_EnumWrapper cewItem in EnumValues)
      {
        enumValuesClone.Add(cewItem.Clone());
      }
      C_Enum ceTmp = new C_Enum()
      {
        enumWrapper = this.EnumWrapper?.Clone(),
        EnumValues = enumValuesClone,
        Name = this.Name,
        TypeDefName = this.TypeDefName,
        Members = members,
        DisplayType = this.DisplayType,
        Parent = this.Parent,
        //Value = this.Value,
        IsArray = this.IsArray,
        IsPointer = this.IsPointer,
        Passed = this.Passed,
        CheckEqual = this.CheckEqual,
        Type = this.Type,
        FixedAddr = this.FixedAddr
      };
      foreach (C_Variable cvItem in Members)
      {
        C_Variable cvItemClone = cvItem.Clone();
        cvItemClone.Parent = ceTmp;
        members.Add(cvItemClone);
      }
      ceTmp.SetAddress(Address);
      return ceTmp;
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
      //Passed = IsSelected && cv.IsSelected ? cv.Passed = this.Equals(cv) : true;

      if (Passed == false)
      {
        IsExpanded = true;
      }

      return Passed == true;
    }

    public override bool Equals(object obj)
    {
      C_Enum ca = obj as C_Enum;
      return ca != null && TypeDefName == ca.TypeDefName && Name == ca.Name && (EnumWrapper == null ? ca.EnumWrapper == null : EnumWrapper.Equals(ca.EnumWrapper));
    }

    public override int GetHashCode()
    {
      var hashCode = -1129657248;
      hashCode = hashCode * -1521134295 + base.GetHashCode();
      hashCode = hashCode * -1521134295 + TypeDefName.GetHashCode();
      hashCode = hashCode * -1521134295 + EnumWrapper.GetHashCode();
      return hashCode;
    }

    public override String ToString()
    {
      return String.Format("{0} {1} = {2}", TypeDefName, Name, enumWrapper);
    }
  }

  public class C_EnumWrapper : IComparable, INotifyPropertyChanged
  {
    private String _name;
    public String Name
    {
      get
      {
        return _name;
      }
      set
      {
        _name = value;
        UpdatePropertyChangedName();
      }
    }

    private UInt32 _value;
    public UInt32 Value
    {
      get
      {
        return _value;
      }
      set
      {
        _value = value;
        UpdatePropertyChangedValue();
      }
    }

    #region PropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected void UpdatePropertyChanged(String name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected void UpdatePropertyChangedName()
    {
      UpdatePropertyChanged("Name");
    }

    protected void UpdatePropertyChangedValue()
    {
      UpdatePropertyChanged("Value");
    }
    #endregion // PropertyChanged

    //public static bool operator ==(C_EnumWrapper cew1, C_EnumWrapper cew2)
    //{
    //  return cew1 == null ? cew2 == null : cew1.Equals(cew2);
    //}

    //public static bool operator !=(C_EnumWrapper cew1, C_EnumWrapper cew2)
    //{
    //  return !(cew1 == null ? cew2 == null : cew1.Equals(cew2));
    //}

    public C_EnumWrapper()
    {
      Name = "NO_NAME";
    }

    public C_EnumWrapper(String name, UInt32 value)
    {
      if (name == null)
      {
        throw new NullReferenceException("Name cannot be null");
      }
      Name = name;
      Value = value;
    }

    public int CompareTo(object obj)
    {
      if (!(obj is C_EnumWrapper))
      {
        throw new Exception(String.Format("C_EnumWrapper/CompareTo wrong type: {0}", obj.GetType()));
      }
      return Value.CompareTo((obj as C_EnumWrapper).Value);
    }

    public override bool Equals(object obj)
    {
      C_EnumWrapper ce = obj as C_EnumWrapper;
      return ce != null ? Name == ce.Name & Value == ce.Value : false;
    }

    public override String ToString()
    {
      return String.Format("{0} (0x{1:X} {2} {3})", Name, Value, '\u2a6f', Value);
    }

    public C_EnumWrapper Clone()
    {
      return new C_EnumWrapper(this.Name, this.Value);
    }

    public override int GetHashCode()
    {
      return Name.GetHashCode() * 13 + Value.GetHashCode() * 13;
    }
  }
}
