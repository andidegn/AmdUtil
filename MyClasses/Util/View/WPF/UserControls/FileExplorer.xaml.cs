using AMD.Util.Files;
using AMD.Util.HID;
using AMD.Util.Log;
using AMD.Util.View.WPF.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AMD.Util.View.WPF.UserControls
{

  /// <summary>
  /// Interaction logic for FileExplorerView.xaml
  /// </summary>
  public partial class FileExplorer : UserControl
  {
    #region Events
    /// <summary>
    /// Eventhandler for TreeViewItem double click
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void TviMouseLeftButtonDown(object sender, MouseButtonEventArgs args);
    public event TviMouseLeftButtonDown TviMouseLeftButtonDownEvent;

    private void FireMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
      TviMouseLeftButtonDownEvent?.Invoke(sender, args);
    }

    /// <summary>
    /// Eventhandler for TreeViewItem double click
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void TviMouseDoubleClick(object sender, MouseButtonEventArgs args);
    public event TviMouseDoubleClick TviMouseDoubleClickEvent;

    private void FireMouseDoubleClick(object sender, MouseButtonEventArgs args)
    {
      if (sender is TreeViewItem)
      {
        FileContainer fc = (sender as TreeViewItem).DataContext as FileContainer;
        if (fc != null && (fc.FileType & FileType.IsFileMask) == FileType.IsFileMask)
        {
          args.Handled = true;
          TviMouseDoubleClickEvent?.Invoke(sender, args);
        }
      }
    }

    /// <summary>
    /// Eventhandler for Enter Key up
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void EnterKeyUp(object sender, KeyEventArgs args);
    public event EnterKeyUp EnterKeyUpEvent;

    private void FireEnterKeyUp(object sender, KeyEventArgs args)
    {
      EnterKeyUpEvent?.Invoke(sender, args);
    }

    /// <summary>
    /// Eventhandler for Append File
    /// </summary>
    /// <param name="sender">The object who called the Handler</param>
    /// <param name="args">The arguments of the progress</param>
    public delegate void AppendFile(object sender, RoutedEventArgs args);
    public event AppendFile AppendFileEvent;

    private void FireAppendFile(object sender, RoutedEventArgs args)
    {
      AppendFileEvent?.Invoke(sender, args);
    }
    #endregion // Events

    #region DependencyProperties


    public String RootPath
    {
      get { return (String)GetValue(RootPathProperty); }
      set { SetValue(RootPathProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RootPath.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RootPathProperty =
        DependencyProperty.Register("RootPath", typeof(String), typeof(FileExplorer), new PropertyMetadata(null));


    public bool StartRecursiveSetRoot = false;
    //public bool StartRecursiveSetRoot
    //{
    //  get { return (bool)GetValue(StartRecursiveSetRootProperty); }
    //  set { SetValue(StartRecursiveSetRootProperty, value); }
    //}

    //// Using a DependencyProperty as the backing store for StartRecursiveSetRoot.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty StartRecursiveSetRootProperty =
    //    DependencyProperty.Register("StartRecursiveSetRoot", typeof(bool), typeof(FileExplorer), new PropertyMetadata(false));


    public bool IsMainFileView
    {
      get { return (bool)GetValue(IsMainFileViewProperty); }
      set { SetValue(IsMainFileViewProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IsMainFileView.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsMainFileViewProperty =
        DependencyProperty.Register("IsMainFileView", typeof(bool), typeof(FileExplorer), new PropertyMetadata(false));



    //public Brush BrushForeground
    //{
    //  get { return (Brush)GetValue(BrushForegroundProperty); }
    //  set { SetValue(BrushForegroundProperty, value); }
    //}

    //// Using a DependencyProperty as the backing store for BrushForeground.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty BrushForegroundProperty =
    //    DependencyProperty.Register("BrushForeground", typeof(Brush), typeof(FileExplorer), new PropertyMetadata(0));




    #endregion // DependencyProperties
    private LogWriter log;

    private FileContainer _selectedFile;
    public FileContainer SelectedFile { get; private set; }

    public IList<FileContainer> SelectedFiles
    {
      get
      {
        List<FileContainer> fcSelectedList = new List<FileContainer>();
        GetSelectedFiles(fileContainerCollection, fcSelectedList);
        return fcSelectedList;
      }
    }

    private void GetSelectedFiles(IList<FileContainer> files, IList<FileContainer> outFiles)
    {
      if (outFiles == null)
      {
        outFiles = new List<FileContainer>();
      }
      foreach (FileContainer fcItem in files)
      {
        if (fcItem.FileType == FileType.RegularFile && fcItem.IsValidForAnalysis && fcItem.IsSelectedForAnalysis)
        {
          outFiles.Add(fcItem);
        }
        GetSelectedFiles(fcItem.SubFiles, outFiles);
      }
    }

    public StringCollection FileTypeFilter { get; set; }
    public bool CheckboxSelectEnable { get; set; }
    private TreeViewItem tviSelected { get; set; }

    private List<FileContainer> fileContainerCollection;
    private FileSystemWatcher fileSystemWatcher;
    private String tbTviTextBackup;
    private bool renameStarted;
    private bool isPopulated;

    public FileExplorer()
    {
      fileContainerCollection = new List<FileContainer>();
      FileTypeFilter = new StringCollection();
      InitializeComponent();
      log = LogWriter.Instance;
    }

    #region GUI
    private void SetSelectAllForAnalysis(IList<FileContainer> fileList, bool selected)
    {
      foreach (FileContainer fcItem in fileList)
      {
        if (fcItem.FileType == FileType.RegularFile && fcItem.IsValidForAnalysis)
        {
          fcItem.IsSelectedForAnalysis = selected;
        }
        SetSelectAllForAnalysis(fcItem.SubFiles, selected);
      }
    }

    public bool SetSelectedFile(String filePath)
    {
      return SetSelectedFile(fileContainerCollection, filePath);
    }

    private bool SetSelectedFile(IList<FileContainer> fileList, String filePath)
    {
      bool retVal = false;
      foreach (FileContainer fcItem in fileList)
      {
        if (fcItem.FullPath.Equals(filePath))
        {
          fcItem.IsSelected = true;
          retVal = true;
          break;
        }
        retVal = SetSelectedFile(fcItem.SubFiles, filePath);
      }
      return retVal;
    }
    #endregion // GUI

    #region Population
    public void Refresh()
    {
      Populate(RootPath, StartRecursiveSetRoot);
    }

    public void Populate(String path, bool recursiveFilesOnly)
    {
      String lastRootPath = RootPath;
      if (fileSystemWatcher == null)
      {
        fileSystemWatcher = new FileSystemWatcher();
        fileSystemWatcher.EnableRaisingEvents = false;
        fileSystemWatcher.Created += Fsw_Created;
        fileSystemWatcher.Deleted += Fsw_Deleted;
        fileSystemWatcher.Renamed += Fsw_Renamed;
        fileSystemWatcher.IncludeSubdirectories = true;
        fileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;
      }

      StartRecursiveSetRoot = recursiveFilesOnly;
      RootPath = path;
      tbPath.Text = path;

      fileSystemWatcher.EnableRaisingEvents = false;

      if (!String.IsNullOrWhiteSpace(path))
      {
        if ((File.GetAttributes(path) & FileAttributes.Directory) != FileAttributes.Directory)
        {
          path = Path.GetDirectoryName(path);
        }
        if (!path.EndsWith("\\"))
        {
          path += '\\';
        }
        fileSystemWatcher.Path = path;
        fileSystemWatcher.EnableRaisingEvents = true;
      }
      
      BackgroundWorker bw = new BackgroundWorker();
      StringCollection fileTypeFilter = FileTypeFilter;

      bw.DoWork += (s, e) =>
      {
        PopulateBase(path, fileTypeFilter, recursiveFilesOnly);
      };
      bw.RunWorkerCompleted += (s, e) =>
      {
        tvFileExplorer.ItemsSource = fileContainerCollection;
        tvFileExplorer.Items.Refresh();

        var v = (from t in fileContainerCollection
                 where t.FullPath == lastRootPath
                 select t).FirstOrDefault();

        if (v != null)
        {
          v.IsSelected = true;
        }

        TreeViewItem tviTmp = (tvFileExplorer.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem);
        tviTmp?.Focus();
        tvFileExplorer.Focus();

        if (fileContainerCollection.Count > 0)
        {
          fileContainerCollection.First().IsSelected = true;
        }
      };
      bw.RunWorkerAsync();
      isPopulated = true;
    }

    private void PopulateBase(String path, StringCollection fileTypeFilter, bool recursiveFilesOnly)
    {
      if (String.IsNullOrWhiteSpace(path))
      {
        DriveInfo[] diArr = DriveInfo.GetDrives();
        fileContainerCollection.Clear();
        for (int i = 0; i < diArr.Length; i++)
        {
          DriveInfo di = diArr[i];
          FileContainer fc = GetFileContainer(di.RootDirectory.FullName, fileTypeFilter, StartRecursiveSetRoot, di.Name);
          if (fc != null)
          {
            fc.Index = i;
            fileContainerCollection.Add(fc);
          }
        }
      }
      else
      {
        List<FileSystemInfo> subPathList = new List<FileSystemInfo>();
        if (recursiveFilesOnly)
        {
          subPathList.AddRange(new DirectoryInfo(path).GetFiles("*.*", SearchOption.AllDirectories));
        }
        else
        {
          subPathList.AddRange(new DirectoryInfo(path).GetDirectories());
          subPathList.AddRange(new DirectoryInfo(path).GetFiles());
        }

        List<FileContainer> tmpList = new List<FileContainer>();
        foreach (FileSystemInfo fsi in subPathList)
        {
          FileContainer fc = GetFileContainer(fsi.FullName, fileTypeFilter, StartRecursiveSetRoot, fsi.FullName.Replace(path, ""));
          if (fc != null)
          {
            tmpList.Add(fc);
          }
        }
        if (tmpList.Count > 1000 && MessageBox.Show(String.Format("There are {0} files to be shown. This will slow down the program emencely.\nAre you sure you want to show this many files?", tmpList.Count), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
        {
          return;
        }
        fileContainerCollection.Clear();
        fileContainerCollection.AddRange(tmpList);
      }
      SortFileCollectionAndSetIndex(fileContainerCollection);
    }

    private void SortFileCollectionAndSetIndex(List<FileContainer> fileCollection)
    {
      fileCollection.Sort();
      int index = 0;
      foreach (FileContainer fcItem in fileCollection)
      {
        fcItem.Index = index++;
      }
    }

    private FileContainer GetFileContainer(String path, StringCollection fileTypeFilter, bool populateAllRecursive, String FileNameOverride = null)
    {
      if (Directory.Exists(path) || FileHelper.CheckPath(path))
      {
        FileContainer fc = new FileContainer();
        fc.FileName = FileNameOverride ?? Path.GetFileName(path);
        fc.FullPath = path;

        FileAttributes fa = File.GetAttributes(path);
        if ((fa & FileAttributes.Directory) == FileAttributes.Directory)
        {
          fc.FileSystemInfo = new DirectoryInfo(path);
          fc.IsValidForAnalysis = fc.IsSelectedForAnalysis = false;
          fc.FileType = new DirectoryInfo(path).Parent == null ? FileType.Drive : FileType.Directory;
          if (populateAllRecursive)
          {
            try
            {
              List<FileSystemInfo> subPathList = new List<FileSystemInfo>();
              subPathList.AddRange(new DirectoryInfo(path).GetDirectories());
              subPathList.AddRange(new DirectoryInfo(path).GetFiles());
              foreach (FileSystemInfo fsi in subPathList)
              {
                FileContainer fileContainer = GetFileContainer(fsi.FullName, fileTypeFilter, populateAllRecursive);
                if (fileContainer != null)
                {
                  fileContainer.Parent = fc;
                  fc.SubFiles.Add(fileContainer);
                }
              }
              SortFileCollectionAndSetIndex(fc.SubFiles);
            }
            catch (UnauthorizedAccessException uae)
            {
              log.WriteToLog(uae, "Error accessing file: {0}", path);
            }
          }
          else
          {
            fc.SubFiles.Add(new FileContainer() { FileName = "dummy", FullPath = "dummy", FileType = FileType.RegularFile });
          }
        }
        else
        {
          if (fileTypeFilter.Contains(".*") || fileTypeFilter.Contains(Path.GetExtension(fc.FullPath)))
          {
            fc.FileSystemInfo = new FileInfo(path);
            fc.IsValidForAnalysis = CheckboxSelectEnable;
            fc.IsSelectedForAnalysis = fc.IsValidForAnalysis;
            fc.FileType = FileType.RegularFile;
          }
          else
          {
            fc = null;
          }
        }
        return fc;
      }
      return null;
    }
    #endregion // Population

    #region Navigation
    private void SetAsRoot(bool recursiveFileSearch)
    {
      try
      {
        FileContainer fc = tvFileExplorer.SelectedItem as FileContainer;
        if (fc != null && (fc.FileType & FileType.IsExpandableMask) == FileType.IsExpandableMask)
        {
          RootPath = fc.FullPath;
          Populate(RootPath, recursiveFileSearch);
        }
      }
      catch (Exception ex)
      {
        log.WriteToLog(ex);
      }
    }

    private void NavigateLevelUp()
    {
      if (!String.IsNullOrWhiteSpace(RootPath))
      {
        DirectoryInfo di = new DirectoryInfo(RootPath);
        if (di.Parent != null)
        {
          Populate(Path.GetDirectoryName(RootPath), false);
        }
        else
        {
          Populate(null, false);
        }
      }
    }

    private void RenameFile(FileContainer fileContainer, String newName)
    {
      String newNamePath = Path.Combine(fileContainer.DirectoryPath, newName);
      switch (fileContainer.FileType)
      {
        case FileType.Drive:
          break;
        case FileType.Directory:
          Directory.Move(fileContainer.FullPath, newNamePath);
          break;
        case FileType.RegularFile:
          File.Move(fileContainer.FullPath, newNamePath);
          break;
        case FileType.IsExpandableMask:
          break;
        default:
          break;
      }
    }

    private void TbTviStartEdit()
    {
      FileContainer fc = tviSelected?.DataContext as FileContainer;
      if (fc != null && fc.FileType == FileType.Directory || fc.FileType == FileType.RegularFile)
      {
        TextBox tb = (TextBox)VisualHelper.GetChildDependencyObjectFromVisualTree(tviSelected, typeof(TextBox));
        if (tb != null)
        {
          tbTviTextBackup = tb.Text;
          tb.BorderThickness = new Thickness(1);
          tb.IsReadOnly = false;
          tb.SelectAll();
          renameStarted = true;
          tb.Focus();
        }
      }
    }

    private void TbTviCloseEdit(TextBox tb)
    {
      tb.BorderThickness = new Thickness(0);
      tb.IsReadOnly = true;
      tb.IsReadOnlyCaretVisible = false;
      tb.Select(0, 0);
      renameStarted = false;
      tviSelected.Focus();
    }

    private void TbTviCancelEdit(TextBox tb)
    {
      tb.Text = tbTviTextBackup;
      TbTviCloseEdit(tb);
    }

    public new void Focus()
    {
      base.Focus();
      tvFileExplorer.Focus();
      tviSelected?.Focus();
    }
    #endregion // Navigation

    #region FileSystemWatcher
    private bool UpdateFile(List<FileContainer> fileList, String oldPath, String newPath, StringCollection fileTypeFilter)
    {
      bool retVal = false;
      FileAttributes newFileAttrib = File.GetAttributes(newPath);
      foreach (FileContainer fc in fileList)
      {
        if (fc != null)
        {
          if (fc.FullPath.Equals(oldPath))
          {
            //if (fileTypeFilter.Contains(Path.GetExtension(newPath)))
            //{
              fc.FileName = Path.GetFileName(newPath);
              fc.FullPath = newPath;
              FileAttributes fa = File.GetAttributes(fc.FullPath);
              //fc.Visibility = fileTypeFilter.Count == 0 || fileTypeFilter.Contains(Path.GetExtension(fc.FullPath)) || fa.HasFlag(FileAttributes.Directory) ? Visibility.Visible : Visibility.Collapsed;
              SortFileCollectionAndSetIndex(fileList);
              retVal = true;
              break;
            //}
          }
          else
          {
            if (UpdateFile(fc.SubFiles, oldPath, newPath, fileTypeFilter))
            {
              retVal = true;
              break;
            }
          }
        }
      }
      return retVal;
    }

    private bool RemoveFile(IList<FileContainer> fileList, String fullPath)
    {
      foreach (FileContainer fc in fileList)
      {
        if (fc != null)
        {
          if (fc.FullPath.Equals(fullPath))
          {
            fileList.Remove(fc);
            if (fileList.Count == 0)
            {
              fc.SubFiles.Add(new FileContainer() { FileName = "dummy", FullPath = "dummy", FileType = FileType.RegularFile });
            }
            return true;
          }
          else
          {
            if (RemoveFile(fc.SubFiles, fullPath))
            {
              return true;
            }
          }
        }
      }
      return false;
    }

    private bool AddFile(String fullPath)
    {
      String fullPathDirectoryName = Path.GetDirectoryName(fullPath);
      if (fullPathDirectoryName.Equals(RootPath))
      {
        FileContainer fcNew = GetFileContainer(fullPath, FileTypeFilter, StartRecursiveSetRoot);
        if (fcNew != null)
        {
          fileContainerCollection.Add(fcNew);
          SortFileCollectionAndSetIndex(fileContainerCollection);
          return true;
        }
        return false;
      }
      else
      {
        return AddFile(fileContainerCollection, fullPath);
      }
    }

    private bool AddFile(IList<FileContainer> fileList, String fullPath)
    {
      foreach (FileContainer fc in fileList)
      {
        if (fc.FileType == FileType.Directory)
        {
          if (fc.FullPath.Equals(Path.GetDirectoryName(fullPath)))
          {
            FileContainer fcNew = GetFileContainer(fullPath, FileTypeFilter, StartRecursiveSetRoot);
            if (fcNew != null)
            {
              if (!(fc.SubFiles.Count > 0 && fc.SubFiles[0].FullPath == "dummy"))
              {
                fc.SubFiles.Add(fcNew);
                SortFileCollectionAndSetIndex(fc.SubFiles);
              }
              return true;
            }
          }
          else
          {
            if (AddFile(fc.SubFiles, fullPath))
            {
              return true;
            }
          }
        }
      }
      return false;
    }
    #endregion // FileSystemWatcher

    #region EventHandlers
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (!isPopulated)
      {
        try
        {
          if (!Directory.Exists(RootPath))
          {
            log.WriteToLog(LogMsgType.Error, "Root path not found: \"{0}\"", RootPath);
            RootPath = null;
          }
          if (FileTypeFilter.Count == 0)
          {
             FileTypeFilter.Add(".*");
          }
          Refresh();
        }
        catch (Exception ex)
        {
          log.WriteToLog(ex);
        }
      }
    }

    private void tvi_Expanded(object sender, RoutedEventArgs e)
    {
      e.Handled = true;
      if (!StartRecursiveSetRoot && (sender as TreeViewItem)?.DataContext is FileContainer)
      {
        FileContainer fc = (sender as TreeViewItem).DataContext as FileContainer;
        if (fc.FileType == FileType.Directory || fc.FileType == FileType.Drive)
        {
          try
          {
            if (fc.SubFiles.Count > 0 && fc.SubFiles.First()?.FullPath == "dummy")
            {
              fc.SubFiles.Clear();

              List<FileSystemInfo> subPathList = new List<FileSystemInfo>();
              subPathList.AddRange(new DirectoryInfo(fc.FullPath).GetDirectories());
              subPathList.AddRange(new DirectoryInfo(fc.FullPath).GetFiles());
              foreach (FileSystemInfo fsi in subPathList)
              {
                FileContainer fileContainer = GetFileContainer(fsi.FullName, FileTypeFilter, StartRecursiveSetRoot);
                if (fileContainer != null)
                {
                  fileContainer.Parent = fc;
                  fc.SubFiles.Add(fileContainer);
                }
              }
              SortFileCollectionAndSetIndex(fc.SubFiles);
            }
            fc.IsExpanded = true;
          }
          catch (UnauthorizedAccessException uae)
          {
            log.WriteToLog(uae, "Error accessing file");
          }
        }
      }
    }

    private bool DirectoryExists(String dirPath)
    {
      bool result = false;
      result = dirPath.Equals(RootPath);
      if (!result)
      {
        result = DirectoryExists(dirPath, fileContainerCollection);
      }
      return result;
    }

    private bool DirectoryExists(String dirPath, IList<FileContainer> collection)
    {
      foreach (FileContainer fc in collection)
      {
        if (dirPath.Equals(fc.FullPath))
        {
          return true;
        }
        if (DirectoryExists(dirPath, fc.SubFiles))
        {
          return true;
        }
      }
      return false;
    }

    private void Fsw_Created(object sender, FileSystemEventArgs e)
    {
      //Dispatcher.Invoke(() =>
      //{
      //  if (DirectoryExists(Path.GetDirectoryName(e.FullPath)))
      //  {
      //    AddFile(e.FullPath);
      //    tvFileExplorer.Items.Refresh();
      //  }
      //});
    }

    private void Fsw_Deleted(object sender, FileSystemEventArgs e)
    {
      //Dispatcher.Invoke(() =>
      //{
      //  if (DirectoryExists(Path.GetDirectoryName(e.FullPath)))
      //  {
      //    RemoveFile(fileContainerCollection, e.FullPath);
      //    tvFileExplorer.Items.Refresh();
      //  }
      //});
    }

    private void Fsw_Renamed(object sender, RenamedEventArgs e)
    {
      //Dispatcher.Invoke(() =>
      //{
      //  if (DirectoryExists(Path.GetDirectoryName(e.FullPath)))
      //  {
      //    UpdateFile(fileContainerCollection, e.OldFullPath, e.FullPath, FileTypeFilter);
      //    tvFileExplorer.Items.Refresh();
      //  }
      //});
    }

    private void ccSetAsRoot_Click(object sender, RoutedEventArgs e)
    {
      SetAsRoot(false);
    }

    private void ccSetRecursiveRoot_Click(object sender, RoutedEventArgs e)
    {
      SetAsRoot(true);
    }

    private void ccRenameFile_Click(object sender, RoutedEventArgs e)
    {
      TbTviStartEdit();
    }

    private void ccRefresh_Click(object sender, RoutedEventArgs e)
    {
      Refresh();
    }

    private void ccAppendFile_Click(object sender, RoutedEventArgs e)
    {
      FireAppendFile(sender, e);
    }

    private void btnLevelUp_Click(object sender, RoutedEventArgs e)
    {
      NavigateLevelUp();
    }

    private void tvFileExplorer_KeyDown(object sender, KeyEventArgs e)
    {
      if (!renameStarted)
      {
        switch (e.Key)
        {
          case Key.F5:
            Refresh();
            break;
          case Key.Back:
            if (!renameStarted)
            {
              NavigateLevelUp();
            }
            break;
          case Key.Enter:
            if (Modifier.IsShiftDown)
            {
              SetAsRoot(false);
            }
            else if (Modifier.IsCtrlDown)
            {
              SetAsRoot(true);
            }
            else
            {
              if (tviSelected != null)
              {
                if (SelectedFile != null && SelectedFile.FileType == FileType.RegularFile)
                {
                  FireEnterKeyUp(sender, e);
                }
                else
                {
                  tviSelected.IsExpanded = true;
                }
              }
            }
            break;
          default:
            e.Handled = false;
            break;
        }
      }
    }

    private void tbPath_KeyUp(object sender, KeyEventArgs e)
    {
      TextBox tb = sender as TextBox;
      e.Handled = false;
      if (tb != null)
      {
        switch (e.Key)
        {
          case Key.Enter:
            String path = tbPath.Text.Trim();
            if (String.IsNullOrWhiteSpace(path))
            {
              Populate(null, false);
            }
            else
            {
              //DirectoryInfo di = new DirectoryInfo(path);
              if (Directory.Exists(path))
              {
                Populate(path, Modifier.IsCtrlDown);
              }
              //else if (di.Parent == null)
              //{
              //  Populate(null, false);
              //}
              else
              {
                log.WriteToLog(LogMsgType.Error, "Invalid path or path does not exist: \"{0}\"", path);
              }
            }
            e.Handled = true;
            break;
          case Key.Home:
          case Key.End:
          case Key.PageUp:
          case Key.PageDown:
            ScrollIntoView(tb);
            break;
          default:
            break;
        }
      }
    }

    private void tbPath_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      TextBox tb = sender as TextBox;
      if (tb != null)
      {
        if (e.Key == Key.Back)
        {
          allowPathUpdate = false;
        }
        else
        {
          allowPathUpdate = true;
        }

        if (e.Key == Key.Tab)
        {
          e.Handled = true;
          if (tbPath.SelectedText.Length > 0)
          {
            tbPath.Text = String.Format("{0}\\", tbPath.Text);
            tbPath.CaretIndex = tbPath.Text.Length;
          }
          else
          {
            tviSelected.Focus();
          }
        }
      }
    }

    private bool allowPathUpdate = true;
    private void tbPath_TextChanged(object sender, TextChangedEventArgs e)
    {
      TextBox tb = sender as TextBox;
      if (tb != null)
      {
        String path = tb.Text.Trim();

        try
        {
          DirectoryInfo di = new DirectoryInfo(path);
          DirectoryInfo[] diArr;

          if (String.IsNullOrWhiteSpace(path) || Directory.Exists(path) || (new DirectoryInfo(path).Parent == null && Directory.GetLogicalDrives().Contains(path)))
          {
            diArr = null;
            tb.ToolTip = null;
            tb.Background = null;
          }
          else
          {
            diArr = di.Parent?.GetDirectories();
            tb.ToolTip = "Invalid path or path does not exist";
            tb.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0x20, 0x20));
          }
          if (diArr?.Length > 0)
          {
            int caretIndex = tb.CaretIndex;
            foreach (var item in diArr)
            {
              if (item.FullName.ToUpper().Contains(path.ToUpper()) && allowPathUpdate)
              {
                allowPathUpdate = false;
                tb.Text = item.FullName;
                //tb.SelectedText = item.FullName.Replace(path, "");
                tb.CaretIndex = caretIndex;
                tb.Select(caretIndex, tb.Text.Length - caretIndex);
                break;
              }
            }
          }
        }
        catch
        {

          tb.ToolTip = "Invalid path or path does not exist";
          tb.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0x20, 0x20));
        }
        finally
        {
          allowPathUpdate = true;
        }

      }

      return;
    }

    private void tbPath_GoToEnd(object sender, RoutedEventArgs e)
    {
      TextBox tb = sender as TextBox;
      if (tb != null)
      {
        ScrollIntoView(tb);
      }
    }

    private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      TreeViewItem tvi = sender as TreeViewItem;
      if (tvi != null)
      {
        tvi.Focus();
        tvi.IsSelected = true;
      }
    }

    private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
    {
      if (sender is TreeViewItem)
      {
        tviSelected = sender as TreeViewItem;
        if (tviSelected != null)
        {
          SelectedFile = tviSelected.DataContext as FileContainer;
          tviSelected.BringIntoView();
        }
        e.Handled = true;
      }
      else
      {
        tviSelected = null;
      }
    }

    private void TreeViewItem_KeyUp(object sender, KeyEventArgs e)
    {
      TreeViewItem tvi = sender as TreeViewItem;
      e.Handled = true;
      switch (e.Key)
      {
        case Key.F2:
          TbTviStartEdit();
          break;
        case Key.Delete:
          break;
        case Key.Space:
          if (tvi != null)
          {
            FileContainer fc = tvi.DataContext as FileContainer;
            if (fc != null && fc.IsValidForAnalysis)
            {
              fc.IsSelectedForAnalysis = !fc.IsSelectedForAnalysis;
            }
          }
          break;
        default:
          e.Handled = false;
          break;
      }
    }

    private void tbFileName_KeyUp(object sender, KeyEventArgs e)
    {
      TextBox tb = sender as TextBox;

      e.Handled = true;
      switch (e.Key)
      {
        case Key.Escape:
          TbTviCancelEdit(tb);
          break;
        case Key.Enter:
          RenameFile(tviSelected.DataContext as FileContainer, tb.Text.Trim());
          TbTviCloseEdit(tb);
          break;
        default:
          break;
      }
    }

    private void tbTvi_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = e.Text.IndexOfAny(Path.GetInvalidFileNameChars()) > -1;
    }

    private void tbFileName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      TextBox tb = sender as TextBox;
      if (tb != null && !tb.IsReadOnly)
      {
        TbTviCancelEdit(tb);
      }
    }

    private void tvFileExplorer_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      FileContainer fc = tvFileExplorer.SelectedItem as FileContainer;
      if (Modifier.IsCtrlDown)
      {
        return;
      }

      if (fc != null)
      {
        e.Handled = true;
        switch (e.Key)
        {
          case Key.Down:
            KeyboardNavigateDown(fc, false);
            break;
          case Key.Up:
            KeyboardNavigationUp(fc);
            break;
          default:
            e.Handled = false;
            break;
        }
      }
    }

    private void chkSelectedForAnalysis_Click(object sender, RoutedEventArgs e)
    {
      if (Modifier.IsCtrlDown)
      {
        SetSelectAllForAnalysis(fileContainerCollection, (sender as CheckBox).IsChecked == true);
      }
    }
    #endregion // EventHandlers

    private void ScrollIntoView(TextBox tb)
    {
      //tb.CaretIndex = tb.Text.Length;
      var rect = tb.GetRectFromCharacterIndex(tb.CaretIndex);
      tb.ScrollToHorizontalOffset(rect.Right);
    }

    private void KeyboardNavigationUp(FileContainer fc)
    {
      fc.IsSelected = false;
      FileContainer fcPrev = null;
      if (fc.Index == 0)
      {
        if (fc.Parent != null)
        {
          fcPrev = fc.Parent;
        }
        else
        {
          fcPrev = fc;
        }
      }
      else
      {
        if (fc.Parent != null)
        {
          fcPrev = FileSearchUpVisible(fc.Parent.SubFiles, fc.Index - 1);
        }
        else
        {
          fcPrev = FileSearchUpVisible(fileContainerCollection, fc.Index - 1);
        }
        if (fcPrev != null)
        {
          if (fcPrev.IsExpanded)
          {
            FileContainer fcLastChild = fcPrev;
            do
            {
              fcLastChild = FileSearchDownVisible(fcLastChild.SubFiles, fcLastChild.SubFiles.Count - 1);
            }
            while (fcLastChild != null && (fcLastChild.FullPath == "dummy" || (fcLastChild.IsExpanded && fcLastChild.SubFiles.Count > 0)));

            //FileContainer fcPrevLastChild = FileSearchUpVisible(fcPrev.SubFiles, fcPrev.SubFiles.Count - 1);
            if (fcLastChild != null)
            {
              fcLastChild.IsSelected = true;
              return;
            }
          }
        }
        else
        {
          if (fc.Parent != null)
          {
            fcPrev = fc.Parent;
          }
        }
      }
      if (fcPrev != null)
      {
        fcPrev.IsSelected = true;
      }
    }

    private void KeyboardNavigateDown(FileContainer fc, bool skipExpanded)
    {
      fc.IsSelected = false;
      FileContainer fcNext = null;
      if (fc.IsExpanded && !skipExpanded && (fcNext = FileSearchDownVisible(fc.SubFiles, 0)) != null)
      {
      }
      else
      {
        if (fc.Parent != null)
        {
          fcNext = FileSearchDownVisible(fc.Parent.SubFiles, fc.Index + 1);
          if (fcNext == null)
          {
            KeyboardNavigateDown(fc.Parent, true);
            return;
          }
        }
        else
        {
          fcNext = FileSearchDownVisible(fileContainerCollection, fc.Index + 1);
        }
      }
      if (fcNext != null)
      {
        fcNext.IsSelected = true;
      }
      else
      {
        fc.IsSelected = true;
      }
    }

    private FileContainer FileSearchUpVisible(IList<FileContainer> children, int index)
    {
      FileContainer fcPrev = null;
      for (int i = children.Count - 1; i >= 0; i--)
      {
        fcPrev = children[i];
        if (fcPrev.Index <= index)
        {
          break;
        }
      }
      return fcPrev;
    }

    private FileContainer FileSearchDownVisible(IList<FileContainer> children, int index)
    {
      FileContainer fcNext = null;
      foreach (FileContainer fcItem in children)
      {
        if (fcItem.Index >= index)
        {
          fcNext = fcItem;
          break;
        }
      }
      return fcNext;
    }
  }

  public class FileTypeIsDirOrDriveToBool : IValueConverter
  {
    public static FileTypeIsDirOrDriveToBool Instance = new FileTypeIsDirOrDriveToBool();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is FileType)
      {
        FileType ft = (FileType)value;
        return (ft & FileType.IsExpandableMask) == FileType.IsExpandableMask;
      }
      throw new NotSupportedException(String.Format("Not correct type. Expected \"FileType\", actual \"{0}\"", value.GetType()));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException("Cannot convert back");
    }
  }

  public class FileTypeToImageConverter : IValueConverter
  {
    public static FileTypeToImageConverter Instance = new FileTypeToImageConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      BitmapImage bmi = null;
      try
      {
        if (value is FileType)
        {
          switch ((FileType)value)
          {
            case FileType.RegularFile:
              bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/file_16x16.png"));
              break;
            case FileType.Directory:
              bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/folder_16x16.png"));
              break;
            case FileType.Drive:
              bmi = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/drive_16x16.png"));
              break;
            default:
              break;
          }
        }
      }
      catch { }
      return bmi;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException("Cannot convert back");
    }
  }
}
