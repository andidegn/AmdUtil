using AMD.Util.Collections.Dictionary;
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
  public class C_VariableListTypes : INotifyPropertyChanged
  {
    [XmlIgnore]
    public static readonly Type[] IncludedTypes = new Type[]
    {
      typeof(Bit),
      typeof(C_Struct),
      typeof(C_Array),
      typeof(C_Primitive),
      typeof(C_Variable),
      typeof(C_Enum),
      typeof(C_EnumWrapper)
    };

    private ObservableCollection<C_Variable> _collection;
    public ObservableCollection<C_Variable> Collection
    {
      get
      {
        return _collection;
      }
      private set
      {
        _collection = value;
      }
    }

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

    public C_VariableListTypes()
    {
      Collection = new ObservableCollection<C_Variable>();
    }

    #region Indexer
    //public C_Variable this[String name]
    //{
    //  get
    //  {
    //    return from v in ContextCollection
    //           where v.Name == name
    //           select v;
    //  }
    //  set
    //  {
    //    collection.Add(name, value);
    //  }
    //}
    #endregion // Indexer

    public void AddOrUpdate(C_Struct cs, C_VariableListContexts context)
    {
      var structs = (from v in _collection
                     where v is ITypeDef && (v as ITypeDef).TypeDefName == cs.TypeDefName
                     select v).ToList();
      foreach (C_Struct csItem in structs)
      {
        _collection.Remove(csItem);
      }
      UpdateStructInCollection(cs, _collection);
      UpdateStructInCollection(cs, context.ContextCollection);
      _collection.Add(cs);
      UpdateCollectionChanged(CollectionChangeAction.Add, "Name");
      UpdateCollectionChanged(CollectionChangeAction.Add, "Size");
    }

    public void AddOrUpdate(C_Enum ce, C_VariableListContexts context)
    {
      var enums = (from v in _collection
                   where v is ITypeDef && (v as ITypeDef).TypeDefName == ce.TypeDefName
                   select v).ToList();
      foreach (C_Enum ceItem in enums)
      {
        _collection.Remove(ceItem);
      }
      UpdateEnumInCollection(ce, _collection);
      UpdateEnumInCollection(ce, context.ContextCollection);
      _collection.Add(ce);
      UpdateCollectionChanged(CollectionChangeAction.Add, "Name");
      UpdateCollectionChanged(CollectionChangeAction.Add, "Size");
    }

    private void UpdateStructInCollection(C_Struct csNew, ObservableCollection<C_Variable> collection)
    {
      for (int i = 0; i < collection.Count; i++)
      {
        if (collection[i].Type == C_Type.STRUCT)
        {
          if (collection[i].IsArray)
          {
            C_Array csTmp = collection[i] as C_Array;
          }
          else
          {
            C_Struct csTmp = collection[i] as C_Struct;
            if (csTmp.TypeDefName == csNew.TypeDefName)
            {
              C_Variable csNewTmp = csNew.Clone();
              csNewTmp.Name = csTmp.Name;
              csNewTmp.IsArray = csTmp.IsArray;
              csNewTmp.IsPointer = csTmp.IsPointer;
              csNewTmp.DisplayType = csTmp.DisplayType;
              csNewTmp.Address = csTmp.Address;
              csNewTmp.FixedAddr = csTmp.FixedAddr;
              collection[i] = csNewTmp;
            }
          }
          UpdateStructInCollection(csNew, collection[i].Members);
        }
      }
    }

    private void UpdateEnumInCollection(C_Enum ceNew, ObservableCollection<C_Variable> collection)
    {
      for (int i = 0; i < collection.Count; i++)
      {
        if (collection[i].Type == C_Type.ENUM)
        {
          if (collection[i].IsArray)
          {
            C_Array csTmp = collection[i] as C_Array;
            UpdateEnumInCollection(ceNew, csTmp.Members);
          }
          else
          {
            C_Enum ceTmp = collection[i] as C_Enum;
            if (ceTmp.TypeDefName == ceNew.TypeDefName)
            {
              C_Variable ceNewTmp = ceNew.Clone();
              ceNewTmp.Name = ceTmp.Name;
              ceNewTmp.IsArray = ceTmp.IsArray;
              ceNewTmp.IsPointer = ceTmp.IsPointer;
              ceNewTmp.DisplayType = ceTmp.DisplayType;
              collection[i] = ceNewTmp;
            }
          }
        }
        else if (collection[i].Type == C_Type.STRUCT)
        {
          UpdateEnumInCollection(ceNew, (collection[i] as C_Struct).Members);
        }
      }
    }

    public bool Remove(C_Variable cv)
    {
      return _collection.Remove(cv);
    }

    public C_Struct GetEnumType(String typeDefName)
    {
      return (from s in _collection
              where s.Type == C_Type.ENUM &&
                    (s as C_Struct).TypeDefName == typeDefName
              select s).FirstOrDefault() as C_Struct;
    }

    public C_Struct GetStructType(String typeDefName)
    {
      return (from s in _collection
              where s.Type == C_Type.STRUCT &&
                    (s as C_Struct).TypeDefName == typeDefName
              select s).FirstOrDefault() as C_Struct;
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

    public static bool SaveToXml(String path, C_VariableListTypes types)
     {
      String backupPath = String.Format("{0}.bak", path);
      bool saveResult = false;
      try
      {
        XmlSerializer writer = new XmlSerializer(typeof(C_VariableListTypes), IncludedTypes);

        if (!FileHelper.IsFilePathLegal(path))
        {
          return false;
        }
        if (!FileHelper.CheckPath(path))
        {
          Directory.CreateDirectory(Path.GetDirectoryName(path));
          File.Create(path).Close();
        }
        File.Copy(path, backupPath, true);
        using (StreamWriter file = new StreamWriter(path))
        {
          writer.Serialize(file, types);
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

    public bool LoadFromXml(String path, C_VariableListTypes types, C_VariableListContexts context)
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
          var eventHandlers = types.CollectionChanged;
          XmlSerializer reader = new XmlSerializer(typeof(C_VariableListTypes), IncludedTypes);
          using (StreamReader file = new StreamReader(path))
          {
            types = (C_VariableListTypes)reader.Deserialize(file);
          }
          types.CollectionChanged = eventHandlers;
          loadResult = true;
          UpdateCollectionChanged(CollectionChangeAction.Refresh, "All");
          foreach (C_Variable cvItem in types.Collection)
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
