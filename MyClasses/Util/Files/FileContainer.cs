using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace AMD.Util.Files
{
  public enum FileType
  {
    Drive             = 0x03, // 0011
    Directory         = 0x05, // 0101
    RegularFile       = 0x08, // 1000

    IsExpandableMask  = 0x01, // 1000
    IsFileMask        = 0x08, // 0001
  }

  public class FileContainer : IComparable, INotifyPropertyChanged
  {
    private bool _isSelectedForAnalysis;
    public bool IsSelectedForAnalysis
    {
      get
      {
        return _isSelectedForAnalysis;
      }
      set
      {
        _isSelectedForAnalysis = value;
        UpdatePropertyChangedIsSelectedForAnalysis();
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
        _isSelected = value;
        UpdatePropertyChangedIsSelected();
      }
    }

    private bool _isValidForAnalysis;
    public bool IsValidForAnalysis
    {
      get
      {
        return _isValidForAnalysis;
      }
      set
      {
        _isValidForAnalysis = value;
        UpdatePropertyChangedIsValidForAnalysis();
      }
    }
    public bool IsExpanded { get; set; }
    public int Index { get; set; }

    public String FileName { get; set; }
    public String FullPath { get; set; }
    public FileType FileType { get; set; }
    public FileContainer Parent { get; set; }
    public FileSystemInfo FileSystemInfo { get; set; }
    public List<FileContainer> SubFiles { get; set; }
    public long FileSize
    {
      get
      {
        if (FileSystemInfo is FileInfo && null != FileSystemInfo)
        {
          return (FileSystemInfo as FileInfo).Length;
        }
        return -1;
      }
    }

    public String FileSizeText
    {
      get
      {
        long fs = FileSize;
        return fs < 0 ? String.Empty : fs.ToString();
      }
    }

    public String ToolTipString
    {
      get
      {
        if (FileSystemInfo is FileInfo)
        {
          FileInfo fi = FileSystemInfo as FileInfo;
          return String.Format("Creation Date: {0}\nModified Date: {1}\nSize: {2:0.000}MB", fi.CreationTime, fi.LastWriteTime, (double)(fi.Length / 1048576f));
        }
        else
        {
          return String.Format("Creation Date: {0}\nModified Date: {1}", FileSystemInfo.CreationTime, FileSystemInfo.LastWriteTime);
        }
      }
    }

    #region PropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected void UpdatePropertyChanged(String name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected void UpdatePropertyChangedIsSelected()
    {
      UpdatePropertyChanged("IsSelected");
    }

    protected void UpdatePropertyChangedIsSelectedForAnalysis()
    {
      UpdatePropertyChanged("IsSelectedForAnalysis");
    }

    protected void UpdatePropertyChangedIsValidForAnalysis()
    {
      UpdatePropertyChanged("IsValidForAnalysis");
    }
    #endregion // PropertyChanged

    public String DirectoryPath
    {
      get
      {
        return Path.GetDirectoryName(FullPath);
      }
    }

    public FileContainer()
    {
      SubFiles = new List<FileContainer>();
    }

    public override bool Equals(object obj)
    {
      FileContainer fcObj = obj as FileContainer;
      return fcObj != null ? FullPath.Equals(fcObj.FullPath, StringComparison.CurrentCultureIgnoreCase) : false;
    }

    public override int GetHashCode()
    {
      return FullPath.GetHashCode();
    }

    public int CompareTo(object obj)
    {
      if (obj is FileContainer)
      {
        FileContainer fc = obj as FileContainer;
        int result = FileType.CompareTo(fc.FileType);
        if (result != 0)
        {
          return result;
        }
        return FileName.CompareTo(fc.FileName);
      }
      else
      {
        throw new Exception("Wrong type");
      }
    }
  }
}
