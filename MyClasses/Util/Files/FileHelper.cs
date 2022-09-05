using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AMD.Util.Files
{
	public static class FileHelper
  {
    public static event ProgressChangedEventHandler ProgressChanged;
    private static void OnProgressChanged(int percentage, string text)
    {
      ProgressChanged?.Invoke(null, new ProgressChangedEventArgs(percentage, text));
    }

    public static readonly string LogFileFilter = "Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
    public static readonly string TextFileFilter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
    public static readonly string CsvFileFilter = "CSV File (*.csv)|*.csv|Text File (*.txt)|*.txt|All Files (*.*)|*.*";
    public static readonly string XmlFileFilter = "XML File (*.xml)|*.xml|All Files (*.*)|*.*";

    public static readonly int MAX_PATH = 260;

    /// <summary>
    /// Gets the filepath from OpenFileDialog
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="initialDirectory"></param>
    /// <param name="title"></param>
    /// <param name="initialFileName"></param>
    /// <returns></returns>
    public static string GetLoadFilePath(string filter, string initialDirectory, string title, string initialFileName = null)
    {
      return GetFilePath(new OpenFileDialog(), filter, initialDirectory, title, initialFileName);
    }

    /// <summary>
    /// Gets the filepath from SaveFileDialog
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="initialDirectory"></param>
    /// <param name="title"></param>
    /// <param name="initialFileName"></param>
    /// <returns></returns>
    public static string GetSaveFilePath(string filter, string initialDirectory, string title, string initialFileName = null)
    {
      return GetFilePath(new SaveFileDialog(), filter, initialDirectory, title, initialFileName);
    }

    public static string GetFilePath(FileDialog fd, string filter, string initialDirectory, string title, string initialFileName)
    {
      string filePath = null;
      fd.Title = title;
      fd.FileName = initialFileName;
      fd.Filter = filter;
      if (!string.IsNullOrWhiteSpace(initialDirectory))
      {
        try
        {
          if ((File.GetAttributes(initialDirectory) & FileAttributes.Directory) != FileAttributes.Directory)
          {
            initialDirectory = Path.GetDirectoryName(initialDirectory);
          }
          if (Directory.Exists(initialDirectory))
          {
            fd.InitialDirectory = initialDirectory;
          }
        }
        catch (Exception ex)
        {
          //LogWriter
        }
      }
      if (fd.ShowDialog() == true && !string.IsNullOrWhiteSpace(fd.FileName))
      {
        filePath = fd.FileName;
      }
      if (filePath?.Length > FileHelper.MAX_PATH)
      {
        filePath = null;
      }
      return filePath;
    }

    /// <summary>
    /// Checks if the path is too long, if file exists and if path is null
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool CheckPath(string filePath)
    {
      return File.Exists(filePath) && filePath.Length <= MAX_PATH;
    }

    /// <summary>
    /// Checks if a file path contains any illegal characters
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool IsFilePathLegal(string filePath)
    {
       return filePath.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }

		public static string GetExecutePath(string fileName, int maxDepth = int.MaxValue)
		{
			string path = string.Format(@"c:\Program Files\BurnInTest\{0}", fileName);
      OnProgressChanged(1, $"Checking path: \"{path}\"");
			if (!File.Exists(path))
			{
				path = path = string.Format(@"c:\Program Files (x86)\BurnInTest\{0}", fileName);
        OnProgressChanged(2, $"Checking path: \"{path}\"");
        if (!File.Exists(path))
				{
					path = string.Format(@"{0}\{1}", GetInstallPathFromRegistry("BurnInTest1"), fileName);
          OnProgressChanged(3, $"Checking path: \"{path}\"");
          if (!File.Exists(path))
					{
						path = null;
						LocateExeFromPath(Environment.GetEnvironmentVariable("ProgramFiles"), fileName, ref path, maxDepth);
						if (string.IsNullOrEmpty(path))
						{
							LocateExeFromPath(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), fileName, ref path, maxDepth);
						}
					}
				}
      }

      return path;
		}

		private static void LocateExeFromPath(string startPath, string fileName, ref string path, int maxDepth)
		{
			try
			{
        int ctr = 0;
        string[] files = Directory.GetFiles(startPath);
				foreach (string curFilePath in files)
        {
          OnProgressChanged(100 * ctr++ / files.Length, $"Scanning file: \"{curFilePath}\"");
          if (Path.GetFileName(curFilePath).Equals(fileName))
					{
						path = curFilePath;
						return;
					}
				}
        if (0 < maxDepth--)
        {
          ctr = 0;
          string[] directories = Directory.GetDirectories(startPath);
          foreach (string curDirectoryPath in directories)
          {
            OnProgressChanged(100 * ctr++ / directories.Length, $"Scanning file: \"{curDirectoryPath}\"");
            LocateExeFromPath(curDirectoryPath, fileName, ref path, maxDepth);
          }
        }
			}
			catch
			{
			}
		}

		private static string GetInstallPathFromRegistry(string name)
		{
			using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
			{
				foreach (string skName in rk.GetSubKeyNames())
				{
					using (RegistryKey sk = rk.OpenSubKey(skName))
					{
						try
						{
							if (sk.GetValue("DisplayName") != null)
							{
								if (sk.GetValue("InstallLocation") != null)
								{
									string displayName = Convert.ToString(sk.GetValue("DisplayName"));
									if (displayName.StartsWith(name))
									{
										return Convert.ToString(sk.GetValue("InstallLocation"));
									}
								}
							}
						}
						catch (Exception ex)
						{
						}
					}
				}
				return null;
			}
		}
  }
}
