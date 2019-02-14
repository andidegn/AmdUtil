using AMD.Util.Files;
using AMD.Util.Log;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AMD.Util.DataType
{
  public class C_VariableListContexts : INotifyPropertyChanged
  {
    [XmlIgnore]
    private static readonly Type[] includedTypes = new Type[]
    {
      typeof(Bit),
      typeof(C_Struct),
      typeof(C_Array),
      typeof(C_Primitive),
      typeof(C_Variable),
      typeof(C_Enum),
      typeof(C_EnumWrapper)
    };

    private ObservableCollection<C_Variable> _contextCollection;
    public ObservableCollection<C_Variable> ContextCollection
    {
      get
      {
        return _contextCollection;
      }
      set
      {
        _contextCollection = value;
      }
    }

    public static Type[] IncludedTypes => includedTypes;

    #region PropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    private void UpdatePropertyChanged(String name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void UpdatePropertyChangedRomStack()
    {
      UpdatePropertyChanged("RomStack");
    }
    #endregion // PropertyChanged

    #region Events
    /// <summary>
    /// Eventhandler for finished programming
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments</param>
    public delegate void CollectionChangedHandler(object sender, CollectionChangeEventArgs args);
    public event CollectionChangedHandler CollectionChanged;

    private void UpdateCollectionChanged(CollectionChangeAction action, String element)
    {
      CollectionChanged?.Invoke(this, new CollectionChangeEventArgs(action, element));
    }
    #endregion // Events

    public C_VariableListContexts()
    {
      ContextCollection = new ObservableCollection<C_Variable>();
    }

    public void Add(C_Variable cv)
    {
      _contextCollection.Add(cv);
    }

    public C_Variable Get(String name)
    {
      return (from v in ContextCollection
              where v.Name == name
              select v).FirstOrDefault();
    }

    public bool Contains(String name)
    {
      return Get(name) != null;
    }

    public bool Remove(C_Variable cv)
    {
      return _contextCollection.Remove(cv);
    }

    public void SetExpanded(C_Variable source, C_Variable target)
    {
      if (source.Name == target.Name)
      {
        target.IsExpanded = source.IsExpanded;

        if (source is IMemberCollection && target is IMemberCollection)
        {
          for (int i = 0; i < Math.Min((source as IMemberCollection).Members.Count, (target as IMemberCollection).Members.Count); i++)
          {
            SetExpanded((source as IMemberCollection).Members[i], (target as IMemberCollection).Members[i]);
          }
        }

      }
      else
      {

      }
    }

    public static bool SaveToXml(C_VariableListContexts context, String path)
     {
      String backupPath = String.Format("{0}.bak", path);
      bool saveResult = false;
      try
      {
        if (!FileHelper.IsFilePathLegal(path))
        {
          return false;
        }
        XmlSerializer writer = new XmlSerializer(typeof(C_VariableListContexts), IncludedTypes);

        if (!FileHelper.CheckPath(path))
        {
          Directory.CreateDirectory(Path.GetDirectoryName(path));
          File.Create(path).Close();
        }
        File.Copy(path, backupPath, true);
        using (StreamWriter file = new StreamWriter(path))
        {
          writer.Serialize(file, context);
        }
        saveResult = true;
      }
      catch (Exception ex)
      {
        try
        {
          File.Copy(path, String.Format("{0}.err", path), true);
          File.Copy(backupPath, path, true);
        }
        catch (Exception ex1)
        {
          LogWriter.Instance.WriteToLog(ex1, "Error restoring backup: {0}", backupPath);
        }
        LogWriter.Instance.WriteToLog(ex, "Error saving {0}", path);
      }
      return saveResult;
    }

    public static bool LoadFromXml(C_VariableListContexts context, String path)
    {
      bool loadResult = false;
      try
      {
        if (!FileHelper.IsFilePathLegal(path))
        {
          return false;
        }
        if (FileHelper.CheckPath(path))
        {
          var eventHandlers = context.CollectionChanged;
          XmlSerializer reader = new XmlSerializer(typeof(C_VariableListContexts), IncludedTypes);
          using (StreamReader file = new StreamReader(path))
          {
            context = (C_VariableListContexts)reader.Deserialize(file);
          }
          context.CollectionChanged = eventHandlers;
          loadResult = true;
          context.UpdateCollectionChanged(CollectionChangeAction.Refresh, "All");
          foreach (C_Variable cvItem in context.ContextCollection)
          {
            cvItem.SetParent();
          }
          foreach (C_Variable cvItem in context.ContextCollection)
          {
            cvItem.SetParent();
          }
        }
      }
      catch (Exception ex)
      {
        LogWriter.Instance.WriteToLog(ex, "Error loading C_VariableList.xml");
      }
      return loadResult;
    }
  }
}
