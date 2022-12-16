using AMD.Util.Display.DDCCI.MCCSCodeStandard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AMD.Util.Display.DDCCI.Util
{
  public class VCPCodeList : BindingList<VCPCode>, INotifyPropertyChanged
  {
    #region Interface OnPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string capabilityString;
    public string CapabilityString
    {
      get
      {
        return capabilityString;
      }
      set
      {
        capabilityString = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(CapabilityStringFormatted));
      }
    }

    private string capabilityStringFormatted;
    public string CapabilityStringFormatted
    {
      get
      {
        return capabilityStringFormatted;
      }
    }
    #endregion // Interface OnPropertyChanged

    public VCPCodeList()
    {
    }

    public VCPCode this[eVCPCode code]
    {
      get
      {
        return this.Get(code);
      }
      set
      {
        this.Add(value);
      }
    }

    public void Populate(String capabilityString)
    {
      DDCHelper.PopulateVcpCodes(capabilityString, this);
      DDCHelper.PopulateVcpCodeNames(capabilityString, this);
    }

    public new bool Add(VCPCode vcpCode)
    {
      bool result = true;
      if (this.Contains(vcpCode))
      {
        this.Remove(vcpCode);
      }
      base.Add(vcpCode);
      return result;
    }

    private void VCPCodeList_ListChanged(object sender, ListChangedEventArgs e)
    {
      throw new NotImplementedException();
    }

    public bool Remove(eVCPCode code)
    {
      return this.Remove(this.Get(code));
    }

    public VCPCode Get(eVCPCode code)
    {
      return (from v in this
              where v.Code == code
              select v).SingleOrDefault();
    }

    public bool Contains(eVCPCode code)
    {
      return 0 != (from v in this
                   where v.Code == code
                   select v).Count();
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      foreach (VCPCode VCP in this)
      {
        sb.AppendLine(VCP.ToString());
      }
      return sb.ToString();
    }
  }

  public class VCPCodePreset : IComparable, INotifyPropertyChanged
  {
    #region Interface OnPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private VCPCode parentVCPCode;
    public VCPCode ParentVCPCode
    {
      get
      {
        return this.parentVCPCode;
      }
      private set
      {
        this.parentVCPCode = value;
        OnPropertyChanged();
      }
    }

    private uint value;
    public uint Value
    {
      get
      {
        return this.value;
      }
      private set
      {
        this.value = value;
        OnPropertyChanged();
      }
    }

    private string name;
    public string Name
    {
      get
      {
        return this.name;
      }
      private set
      {
        this.name = value;
        OnPropertyChanged();
      }
    }

    private bool isSelected;
    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        this.isSelected = value;
        OnPropertyChanged();
      }
    }
    #endregion // Interface OnPropertyChanged

    internal VCPCodePreset(VCPCodePreset old, VCPCode parentVCPCode, bool isSelected)
      : this(parentVCPCode, old.Value, old.Name, isSelected)
    {

    }

    public VCPCodePreset(VCPCode parentVCPCode, uint value, string name = "", bool isSelected = false)
    {
      this.ParentVCPCode = parentVCPCode;
      this.Value = value;
      this.Name = name;
      this.IsSelected = isSelected;
    }


    public override bool Equals(object obj)
    {
      bool retVal = false;
      if (obj is VCPCodePreset)
      {
        VCPCodePreset other = (VCPCodePreset)obj;
        retVal = Value.Equals(other.Value) && ParentVCPCode.Equals(other.ParentVCPCode);
      }
      return retVal;
    }

    public int CompareTo(object obj)
    {
      int retVal = -1;
      if (obj is VCPCodePreset)
      {
        VCPCodePreset other = (VCPCodePreset)obj;
        retVal = Value.CompareTo(other.Value);
      }
      return retVal;
    }
  }

  public class VCPCode : INotifyPropertyChanged
  {
    #region Interface OnPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private uint _originalValue;
    public uint OriginalValue
    {
      get
      {
        return _originalValue;
      }
      set
      {
        _originalValue = value;
        OnPropertyChanged();
      }
    }
    private uint _currentValue;
    public uint CurrentValue
    {
      get
      {
        return _currentValue;
      }
      set
      {
        _currentValue = value;
        if (HasPresets)
        {
          for (int i = 0; i < Presets.Count; i++)
          {
            Presets[i].IsSelected = value == Presets[i].Value;
          }
          OnPropertyChanged(nameof(Presets));
        }
        OnPropertyChanged();
      }
    }
    private uint _maximumValue;
    public uint MaximumValue
    {
      get
      {
        return _maximumValue;
      }
      set
      {
        _maximumValue = value;
        OnPropertyChanged();
      }
    }

    private string _name;

    public string Name
    {
      get
      {
        return string.IsNullOrWhiteSpace(_name) ? DDCHelper.GetVCPName(Code) : _name;
      }
      set
      {
        _name = value;
        OnPropertyChanged();
      }
    }
    #endregion // Interface OnPropertyChanged
    public eVCPCode Code { get; set; }
    public eVCPCodeType Type { get; set; }
    public eVCPCodeFunction Function { get; set; }
    public List<VCPCodePreset> Presets { get; private set; }

    public string Description { get; set; }
    public bool HasPresets
    {
      get
      {
        return 0 < Presets?.Count();
      }
    }

    public string CodeString
    {
      get
      {
        return $"{Enum.GetName(typeof(eVCPCode), Code)} ({(byte)Code})";
      }
    }

    public byte CodeByte
    {
      get
      {
        return (byte)Code;
      }
    }

    public VCPCode()
    {
      Presets = new List<VCPCodePreset>();
    }

    public VCPCode(byte code, string name = "", string description = "", params VCPCodePreset[] presets) : this((eVCPCode)code, name, description, presets) { }

    public VCPCode(eVCPCode code, string name = "", string description = "", params VCPCodePreset[] presets)
      : this()
    {
      this.Code = code;
      AddPresets(presets);
      this.Name = name;
      this.Description = description;
    }

    public void AddPreset(VCPCodePreset preset)
    {
      AddPreset(preset.Name, preset.Value);
    }

    public void AddPreset(string name, uint value)
    {
      VCPCodePreset preset = new VCPCodePreset(this, value, name, value == CurrentValue);
      if (Presets.Contains(preset))
      {
        Presets.Remove(preset);
      }
      Presets.Add(preset);
      Presets.Sort();
      OnPropertyChanged(nameof(Presets));
    }

    public void AddPresets(IEnumerable<VCPCodePreset> collection)
    {
      this.Presets.Clear();
      foreach (VCPCodePreset preset in collection)
      {
        AddPreset(preset.Name, preset.Value);
      }
    }

    public override bool Equals(object obj)
    {
      bool retVal = false;
      if (obj is VCPCode)
      {
        retVal = Code.Equals((obj as VCPCode).Code);
      }
      return retVal;
    }

    public override string ToString()
    {
      return $"{Name} [{((byte)Code).ToString("X2")}] {Type} org: {OriginalValue:X4}, max: {MaximumValue:X4}, cur: {CurrentValue:X4}";
    }
  }
}

//(prot(monitor) type(lcd)model(rtk)cmds(01 02 03 07 0c e3 f3)vcp(04 10 12 16 18 1a 60(0f 10) d6(01 04) e0 e1 e2 e3 e5 e6 eb f0 f1 f2 f3 f4 f5 f7 f8 f9 fd fe)mswhql(1)asset_eep(40)mccs_ver(2.2))vcpname(e0 (color temperature), e1 (power off), e2 (backlight), e3 (keypad lock) e5(buzzer) e6(usb link select) eb(port select)f0(temperature) f1(min temperature) f2(max temperature) f3(unit runt-time) f4(backlight run-time) f5(service run-time)f7(backlight adjustment control) f8(buzzer output 60-85 db level) f9(buzzer output level)fd(software version) fe(serial number)))