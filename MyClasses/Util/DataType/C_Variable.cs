using AMD.Util.Collections.Dictionary;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AMD.Util.DataType
{
  public enum C_Type
  {
    BOOL =    0x00,
    BIT =     0x01,
    U8 =      0x02,
    S8 =      0x03,
    U16 =     0x04,
    S16 =     0x05,
    U32 =     0x06,
    S32 =     0x07,
    U64 =     0x08,
    S64 =     0x09,
    ENUM =    0xF0,
    STRUCT =  0xF1
  }

  public enum C_DisplayType
  {
    Paragraph = 0,
    Enum,
    Array,
    Pointer,
    MainHeader,
    SubHeader
  }

  public interface ITypeDef : IComparable
  {
    String TypeDefName { get; set; }
  }

  public interface IMemberCollection
  {
    ObservableCollection<C_Variable> Members { get; set; }
  }

  public interface IVariableBase { }

  public abstract class C_Variable : INotifyPropertyChanged, IComparable, IVariableBase
  {
    #region XmlIgnore
    [XmlIgnore]
    private bool? _passed;
    [XmlIgnore]
    public bool? Passed
    {
      get
      {
        return _passed;
      }
      set
      {
        _passed = value;
        UpdatePropertyChangedPassed();
      }
    }

    [XmlIgnore]
    private bool _isExpanded;
    [XmlIgnore]
    public bool IsExpanded
    {
      get
      {
        return _isExpanded;
      }
      set
      {
        _isExpanded = value;
        UpdatePropertyChangedIsExpanded();
      }
    }

    [XmlIgnore]
    private String _toolTip;
    [XmlIgnore]
    public String ToolTip
    {
      get
      {
        return _toolTip;
      }
      set
      {
        _toolTip = value;
        UpdatePropertyChangedToolTip();
      }
    }

    [XmlIgnore]
    public abstract UInt32 SizeInBits
    {
      get;
    }

    [XmlIgnore]
    public abstract UInt32 ActualSizeInBits
    {
      get;
    }

    [XmlIgnore]
    public UInt32 Size
    {
      get
      {
        {
          return SizeInBits / 8;
        }
      }
    }

    [XmlIgnore]
    public UInt32 ActualSize
    {
      get
      {
        return ActualSizeInBits / 8;
      }
    }

    [XmlIgnore]
    public C_Variable Parent { get; set; }

    [XmlIgnore]
    public abstract String FormattedValue { get; }

    [XmlIgnore]
    private static SerializableDictionary<C_Type, UInt32> SizeInBitsLUT = new SerializableDictionary<C_Type, UInt32>
    {
      { C_Type.BIT,         0x01 },
      { C_Type.U8,          0x08 },
      { C_Type.S8,          0x08 },
      { C_Type.U16,         0x10 },
      { C_Type.S16,         0x10 },
      { C_Type.U32,         0x20 },
      { C_Type.S32,         0x20 },
      { C_Type.U64,         0x40 },
      { C_Type.S64,         0x40 },
      { C_Type.ENUM,        0x20 },
      { C_Type.BOOL,        0x20 },
      { C_Type.STRUCT,      0x00 }
    };
    #endregion // XmlIgnore

    private ObservableCollection<C_Variable> _members;
    public ObservableCollection<C_Variable> Members
    {
      get
      {
        return _members;
      }
      set
      {
        _members = value;
        UpdatePropertyChangedMembers();
      }
    }

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

    private UInt32 _address;
    public UInt32 Address
    {
      get
      {
        return _address;
      }
      set
      {
        _address = value;
        UpdatePropertyChangedAddress();
      }
    }

    private C_Type _type;
    public C_Type Type
    {
      get
      {
        return _type;
      }
      set
      {
        _type = value;
        UpdatePropertyChangedType();
        UpdatePropertyChangedSize();
      }
    }

    private object _value;
    public object Value
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

    private bool _checkEqual;
    public bool CheckEqual
    {
      get
      {
        return _checkEqual;
      }
      set
      {
        _checkEqual = value;
        UpdatePropertyChangedCheckEqual();
      }
    }

    private C_DisplayType _displayType;
    public C_DisplayType DisplayType
    {
      get
      {
        return _displayType;
      }
      set
      {
        _displayType = value;
        if (DisplayType == C_DisplayType.MainHeader)
        {
          _isExpanded = true;
        }
        UpdatePropertyChangedDisplayType();
      }
    }

    private bool _isArray;
    public bool IsArray
    {
      get
      {
        return _isArray;
      }
      set
      {
        _isArray = value;
        UpdatePropertyChangedIsArray();
      }
    }

    private bool _isPointer;
    public bool IsPointer
    {
      get
      {
        return _isPointer;
      }
      set
      {
        _isPointer = value;
        if (_isPointer)
        {
          DisplayType = C_DisplayType.Pointer;
        }
        UpdatePropertyChangedSize();
        UpdatePropertyChangedIsPointer();
      }
    }

    private bool _fixedAddr;
    public bool FixedAddr
    {
      get
      {
        return _fixedAddr;
      }
      set
      {
        _fixedAddr = value;
        UpdatePropertyChangedFixedAddr();
      }
    }

    private bool _isSelected;
    public bool IsSelected
    {
      get
      {
        return _isSelected;
      }
      set
      {
        if (value != _isSelected)
        {
          _isSelected = value;
          if (_isSelected)
          {
            if (Parent != null)
            {
              Parent.SetSelected(true, true);
            }
          }
          foreach (C_Variable cvItem in Members)
          {
            cvItem.IsSelected = value;
          }
          UpdatePropertyChangedIsSelected();
        }
      }
    }

    private void SetSelected(bool value, bool parentAsWell)
    {
      _isSelected = value;
      if (parentAsWell && Parent != null)
      {
        Parent.SetSelected(value, parentAsWell);
      }
      UpdatePropertyChangedIsSelected();
    }

    #region PropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected void UpdatePropertyChanged(String name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void UpdatePropertyChangedMembers()
    {
      UpdatePropertyChanged("Members");
    }

    protected void UpdatePropertyChangedValue()
    {
      UpdatePropertyChanged("Value");
      UpdatePropertyChanged("FormattedValue");
    }

    protected void UpdatePropertyChangedCheckEqual()
    {
      UpdatePropertyChanged("CheckEqual");
    }

    protected void UpdatePropertyChangedAddress()
    {
      UpdatePropertyChanged("Address");
    }

    protected void UpdatePropertyChangedPassed()
    {
      UpdatePropertyChanged("Passed");
    }

    protected void UpdatePropertyChangedIsExpanded()
    {
      UpdatePropertyChanged("IsExpanded");
    }

    protected void UpdatePropertyChangedIsSelected()
    {
      UpdatePropertyChanged("IsSelected");
    }

    protected void UpdatePropertyChangedSize()
    {
      UpdatePropertyChanged("Size");
    }

    protected void UpdatePropertyChangedType()
    {
      UpdatePropertyChanged("Type");
      //UpdatePropertyChanged("TypeDefName"));
    }

    protected void UpdatePropertyChangedEnumWrapper()
    {
      UpdatePropertyChanged("EnumWrapper");
    }

    protected void UpdatePropertyChangedDisplayType()
    {
      UpdatePropertyChanged("DisplayType");
    }

    protected void UpdatePropertyChangedName()
    {
      UpdatePropertyChanged("Name");
    }

    protected void UpdatePropertyChangedIsArray()
    {
      UpdatePropertyChanged("IsArray");
    }

    protected void UpdatePropertyChangedIsPointer()
    {
      UpdatePropertyChanged("IsPointer");
    }

    protected void UpdatePropertyChangedFixedAddr()
    {
      UpdatePropertyChanged("FixedAddr");
    }

    protected void UpdatePropertyChangedArrayLength()
    {
      UpdatePropertyChanged("ArrayLength");
    }

    protected void UpdatePropertyChangedTypeDefName()
    {
      UpdatePropertyChanged("TypeDefName");
    }

    protected void UpdatePropertyChangedEnumValues()
    {
      UpdatePropertyChanged("EnumValues");
    }

    protected void UpdatePropertyChangedToolTip()
    {
      UpdatePropertyChanged("ToolTip");
    }
    #endregion // PropertyChanged

    public C_Variable()
    {
      _isSelected = true;
      _isArray = false;
      _checkEqual = true;
    }

    #region Static functions
    public static String GetHierarchicalString(ObservableCollection<C_Variable> cvList, String indent = "")
    {
      StringBuilder sb = new StringBuilder();
      foreach (C_Variable cv in cvList)
      {
        if (cv is IMemberCollection)
        {
          sb.AppendFormat("{0}{1} [{2}]\n{0:X8} {{\n", indent, cv.Name, cv.Address);
          sb.AppendFormat("{0}\n", GetHierarchicalString((cv as IMemberCollection).Members, String.Format("{0}  ", indent)));
        }
        else
        {
          sb.AppendFormat("{0}{1} = {2}\n", indent, cv.Name, cv.FormattedValue);
        }
      }
      sb.AppendFormat("{0}}}", indent.Substring(indent.Length / 2));
      return sb.ToString();
    }
    #endregion // Static functions

    #region Static Functions
    public static UInt32 GetSizeOf(C_Variable cv)
    {
      return SizeInBitsLUT[cv.Type];
    }
    #endregion // Static Functions

    #region Functions
    public void SetAddress(UInt32 addr)
    {
      _address = addr;
      UpdatePropertyChangedAddress();
    }

    public void SetParent()
    {
      foreach (C_Variable cvItem in Members)
      {
        cvItem.SetParent();
        cvItem.Parent = this;
      }
    }

    private void SetParent(C_Variable cv)
    {
      foreach (C_Variable cvItem in cv.Members)
      {
        cvItem.Parent = cv;
        SetParent(cvItem);
      }
    }

    /// <summary>
    /// Gets a range of data. Non-accessed data is represented with null
    /// </summary>
    /// <param name="memory"></param>
    /// <param name="startAddr"></param>
    /// <param name="length">Length in bytes</param>
    /// <param name="timeInNS"></param>
    /// <returns></returns>
    public UInt32?[] GetRangeWords(Dictionary<UInt32, SortedDictionary<long, UInt32>> memory, UInt32 startAddr, UInt32 length, long timeInNS = -1)
    {
      UInt32?[] retArr = new UInt32?[length / 4];
      UInt32 tmpVal;
      for (UInt32 i = 0; i < length / 4; i++)
      {
        if (TryGet(memory, out tmpVal, startAddr + i * 4, timeInNS))
        {
          retArr[i] = tmpVal;
        }
        else
        {
          retArr[i] = null;
        }
      }
      return retArr;
    }

    /// <summary>
    /// TryGet for trying to get value at specific time and address
    /// </summary>
    /// <param name="memory"></param>
    /// <param name="value"></param>
    /// <param name="addr"></param>
    /// <param name="timeInNS"></param>
    /// <returns></returns>
    private bool TryGet(Dictionary<UInt32, SortedDictionary<long, UInt32>> memory, out UInt32 value, UInt32 addr, long timeInNS = -1)
    {
      value = 0;
      bool retVal = false;
      try
      {
        if (memory.ContainsKey(addr))
        {
          if (timeInNS >= 0)
          {
            foreach (var item in memory[addr])
            {
              if (item.Key <= timeInNS)
              {
                value = item.Value;
                retVal = true;
              }
              else
              {
                break;
              }
            }
          }
          else
          {
            value = memory[addr].Last().Value;
            retVal = true;
          }
        }
      }
      catch (Exception ex)
      {
        LogWriter.Instance.WriteToLog(ex);
      }
      return retVal;
    }

    public void Populate(Dictionary<UInt32, SortedDictionary<long, UInt32>> memory, 
                         UInt32 addr, 
                         long timeInNS = -1, 
                         bool littleEndian = true, 
                         ICollection<String> output = null)
    {
      int byteIndex = 0;
      int bitIndex = 0;
      UInt32 addrIndex = 0;
      UInt32 sizeInBytes = Math.Max(ActualSizeInBits / 8, 1);
      if (sizeInBytes % 4 != 0)
      {
        sizeInBytes = sizeInBytes - (sizeInBytes % 4) + 4;
      }
      Populate(memory, this, addr, GetRangeWords(memory, addr, sizeInBytes, timeInNS), ref addrIndex, ref byteIndex, ref bitIndex, timeInNS, littleEndian, output);
    }

    private void Populate(Dictionary<UInt32, SortedDictionary<long, UInt32>> memory, 
                          UInt32 addrBase, 
                          UInt32?[] mem, 
                          ref UInt32 addrOffset, 
                          ref int byteIndex, 
                          ref int bitIndex, 
                          long timeInNS, 
                          bool littleEndian = true,
                          ICollection<String> output = null)
    {
      Populate(memory, this, addrBase, mem, ref addrOffset, ref byteIndex, ref bitIndex, timeInNS, littleEndian, output);
    }

    private void Populate(Dictionary<UInt32, SortedDictionary<long, UInt32>> memory, 
                          C_Variable variableObj, 
                          UInt32 addrBase, 
                          UInt32?[] mem, 
                          ref UInt32 addrOffset, 
                          ref int byteIndex, 
                          ref int bitIndex, 
                          long timeInNS, 
                          bool littleEndian = true, 
                          ICollection<String> output = null)
    {
      UInt32 word = 0;
      int shiftCount = 0;

      if (!littleEndian)
      {
        throw new NotImplementedException("Big Endian is not currently supported");
      }

      try
      {
        word = mem[addrOffset] ?? 0;
        variableObj.Passed = null;

        if (variableObj.IsPointer)
        {
          UInt32 addrBasePtr = word;
          int byteIndexPtr = 0;
          int bitIndexPtr = 0;
          UInt32 addrIndexPtr = 0;
          UInt32 sizeInBytesPtr = Math.Max(variableObj.SizeInBits / 8, 1);
          if (sizeInBytesPtr % 4 != 0)
          {
            sizeInBytesPtr = sizeInBytesPtr - (sizeInBytesPtr % 4) + 4;
          }
          variableObj.Value = addrBasePtr;
          UInt32?[] data = GetRangeWords(memory, addrBasePtr, sizeInBytesPtr, timeInNS);
          foreach (C_Variable cvItem in variableObj.Members)
          {
            cvItem.Populate(memory, addrBasePtr, data, ref addrIndexPtr, ref byteIndexPtr, ref bitIndexPtr, timeInNS, littleEndian, output);
          }
          addrOffset++;
        }
        else
        {
          if (variableObj.IsArray)
          {
            if (byteIndex > 0 && bitIndex == 0)
            {
              bitIndex = byteIndex * 8;
            }
            C_Array caTmp = variableObj as C_Array;
            if (caTmp.ArrayLength != caTmp.Members.Count)
            {
              throw new Exception("Array length mismatch!!!");
            }
            for (int i = 0; i < caTmp.ArrayLength; i++)
            {
              caTmp.Members[i].Populate(memory, addrBase, mem, ref addrOffset, ref byteIndex, ref bitIndex, timeInNS, littleEndian, output);
              caTmp.Members[i].Name = String.Format("({0})", i);
            }
            caTmp.ToolTip = caTmp.FormattedValueArray;
          }
          else
          {
            switch (variableObj.Type)
            {
              case C_Type.BIT:
                (variableObj as C_Primitive).Value = new Bit(word, bitIndex++);
                if ((byteIndex = ((bitIndex) / 8)) > 3)
                {
                  bitIndex = 0;
                  byteIndex = 0;
                  addrOffset++;
                }
                break;
              case C_Type.U8:
              case C_Type.S8:
                shiftCount = 8 * byteIndex;
                (variableObj as C_Primitive).Value = (byte)(word >> shiftCount);
                if ((byteIndex = (byteIndex + 1) % 4) == 0)
                {
                  addrOffset++;
                }
                break;
              case C_Type.U16:
              case C_Type.S16:
                UInt16 val16;
                if (byteIndex == 0)
                {
                  val16 = (UInt16)word;
                  byteIndex += 2;
                }
                else
                {
                  shiftCount = 8 * byteIndex;
                  val16 = (UInt16)(word >> shiftCount);
                  if ((byteIndex = (byteIndex + 1) % 4) == 0)
                  {
                    word = mem[++addrOffset] ?? 0;
                    shiftCount = 0x20 - shiftCount;
                    val16 |= (UInt16)(word << shiftCount);
                  }
                  byteIndex++;
                  if (byteIndex % 4 == 0)
                  {
                    byteIndex = 0;
                    addrOffset++;
                  }
                }
                C_Primitive cp16 = (variableObj as C_Primitive);
                cp16.Value = val16;
                break;
              case C_Type.BOOL:
              case C_Type.U32:
              case C_Type.S32:
                UInt32 val32;
                if (byteIndex == 0)
                {
                  val32 = word;
                  addrOffset++;
                }
                else
                {
                  shiftCount = 8 * byteIndex;
                  val32 = word >> shiftCount;
                  word = mem[++addrOffset] ?? 0;
                  val32 |= word << (0x20 - shiftCount);
                }
                C_Primitive cp32 = (variableObj as C_Primitive);
                cp32.Value = val32;
                break;
              case C_Type.U64:
              case C_Type.S64:
                UInt64 val64;
                if (byteIndex == 0)
                {
                  val64 = word;
                  val64 |= (UInt64)(mem[++addrOffset] ?? 0) << 0x20;
                  addrOffset++;
                }
                else
                {
                  shiftCount = 8 * byteIndex;
                  val64 = word >> shiftCount;
                  shiftCount = 0x20 - shiftCount;
                  word = mem[++addrOffset] ?? 0;
                  val64 |= (UInt64)word << shiftCount;
                  word = mem[++addrOffset] ?? 0;
                  val64 |= (UInt64)word << (shiftCount + 0x20);
                }

                if (!littleEndian)
                {
                  byte[] bArrTmp = BitConverter.GetBytes(val64);
                  Array.Reverse(bArrTmp);
                  val64 = BitConverter.ToUInt64(bArrTmp, 0);
                }

                C_Primitive cp64 = (variableObj as C_Primitive);
                cp64.Value = val64;
                break;
              case C_Type.ENUM:
                C_Enum ceEnum = (variableObj as C_Enum);
                shiftCount = 8 * (littleEndian ? byteIndex : 3 - byteIndex);
                ceEnum.Value = word >> shiftCount;
                if (byteIndex != 0)
                {
                  byteIndex = 4 - byteIndex;
                  shiftCount = 8 * (littleEndian ? byteIndex : 3 - byteIndex);
                  word = mem[++addrOffset] ?? 0;
                  ceEnum.Value = (UInt32)ceEnum.Value | (word >> shiftCount);
                }
                else
                {
                  addrOffset++;
                }
                break;
              case C_Type.STRUCT:
                C_Struct csTmp = variableObj as C_Struct;
                csTmp.SetAddress(addrBase + addrOffset * 4);
                foreach (C_Variable cvItem in csTmp.Members)
                {
                  cvItem.Populate(memory, addrBase, mem, ref addrOffset, ref byteIndex, ref bitIndex, timeInNS, littleEndian, output);
                }
                csTmp.ToolTip = csTmp.FormattedValueStruct;
                
                break;
              default:
                break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogWriter.Instance.WriteToLog(ex, "Error parsing: {0}", variableObj.Name);
        output?.Add(String.Format("Parsing error: {0}\n{1}", ex.Message, ex.StackTrace));
      }
    }

    public override int GetHashCode()
    {
      var hashCode = -1128608277;
      if (Name != null)
      {
        hashCode = hashCode * -1521134295 + Name.GetHashCode();
      }
      hashCode = hashCode * -1521134295 + Address.GetHashCode();
      hashCode = hashCode * -1521134295 + Size.GetHashCode();
      hashCode = hashCode * -1521134295 + Type.GetHashCode();
      hashCode = hashCode * -1521134295 + IsArray.GetHashCode();
      hashCode = hashCode * -1521134295 + IsPointer.GetHashCode();
      hashCode = hashCode * -1521134295 + DisplayType.GetHashCode();
      hashCode = hashCode * -1521134295 + FixedAddr.GetHashCode();
      return hashCode;
    }
    #endregion // Function

    #region Abstract Functions
    public abstract C_Variable Clone();

    public abstract bool ValidateValue(C_Variable cv);

    public abstract override String ToString();

    public abstract override bool Equals(object obj);

    public int CompareTo(object obj)
    {
      if (obj is C_Variable && !String.IsNullOrWhiteSpace(Name))
      {
        return Name.CompareTo((obj as C_Variable).Name);
      }
      return -1;
    }
    #endregion // Abstract Functions
  }  
}
