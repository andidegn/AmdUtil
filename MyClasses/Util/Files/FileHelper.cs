using AMD.Util.Log;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Validates whether the given path is valid in terms of its format and characters.
    /// This works for both local and network (SMB) paths.
    /// </summary>
    /// <param name="path">The path to validate.</param>
    /// <returns>True if the path is valid; otherwise, false.</returns>
    public static bool IsValidPath(string path)
    {
      try
      {
        // Check if the path is null, empty, or consists only of whitespace
        if (string.IsNullOrWhiteSpace(path))
        {
          return false;
        }

        // Check for invalid characters in the path
        if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
          return false;
        }

        // Ensure the path does not exceed the maximum length
        if (MAX_PATH <= path.Length) // Default MAX_PATH limit
        {
          return false;
        }

        // Handle UNC paths (network shares)
        if (path.StartsWith(@"\\"))
        {
          // UNC paths must have at least two segments after '\\'
          string[] segments = path.TrimStart('\\').Split('\\');
          if (segments.Length < 2)
          {
            return false; // Must have \\ServerName\ShareName
          }
          return true; // Valid UNC path
        }

        // For non-UNC paths, ensure the path has a valid root (e.g., "C:\")
        if (string.IsNullOrWhiteSpace(Path.GetPathRoot(path)) || !Path.IsPathRooted(path))
        {
          return false;
        }

        return true; // The path is valid
      }
      catch (Exception)
      {
        // If an exception occurs, the path is invalid
        return false;
      }
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

    public static void Delete(string path, bool printToLog = true)
    {
      LogWriter log = null;
      if (printToLog)
      {
        log = LogWriter.Instance;
        log.PrintNotification($"Trying to delete file: \"{path}\"");
      }
      try
      {
        if (File.Exists(path))
        {
          if (printToLog) { log.PrintNotification("File found!"); }

          Process.Start(new ProcessStartInfo()
          {
            Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + path + "\"",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            FileName = "cmd.exe"
          });

          if (printToLog) { log.PrintNotification("File flagged for deletion"); }
        }
        else
        {
          if (printToLog) { log.PrintNotification("File NOT found??"); }
        }
      }
      catch (Exception ex)
      {
        if (printToLog) { log.WriteToLog(ex); }
      }
    }
  }
}
