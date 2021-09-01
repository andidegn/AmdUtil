﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AMD.Util.Files
{
	public static class FileHelper
	{
    public static readonly String LogFileFilter = "Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
    public static readonly String TextFileFilter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
    public static readonly String CsvFileFilter = "CSV File (*.csv)|*.csv|Text File (*.txt)|*.txt|All Files (*.*)|*.*";
    public static readonly String XmlFileFilter = "XML File (*.xml)|*.xml|All Files (*.*)|*.*";

    public static readonly int MAX_PATH = 260;

    /// <summary>
    /// Gets the filepath from OpenFileDialog
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="initialDirectory"></param>
    /// <param name="title"></param>
    /// <param name="initialFileName"></param>
    /// <returns></returns>
    public static String GetLoadFilePath(String filter, String initialDirectory, String title, String initialFileName = null)
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
    public static String GetSaveFilePath(String filter, String initialDirectory, String title, String initialFileName = null)
    {
      return GetFilePath(new SaveFileDialog(), filter, initialDirectory, title, initialFileName);
    }

    public static String GetFilePath(FileDialog fd, String filter, String initialDirectory, String title, String initialFileName)
    {
      String filePath = null;
      fd.Title = title;
      fd.FileName = initialFileName;
      fd.Filter = filter;
      if (!String.IsNullOrWhiteSpace(initialDirectory))
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
      if (fd.ShowDialog() == true && !String.IsNullOrWhiteSpace(fd.FileName))
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
    public static bool CheckPath(String filePath)
    {
      return File.Exists(filePath) && filePath.Length <= MAX_PATH;
    }

    /// <summary>
    /// Checks if a file path contains any illegal characters
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool IsFilePathLegal(String filePath)
    {
       return filePath.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }

		public static string GetExecutePath(String fileName)
		{
			String path = String.Format(@"c:\Program Files\BurnInTest\{0}", fileName);
			if (!File.Exists(path))
			{
				path = path = String.Format(@"c:\Program Files (x86)\BurnInTest\{0}", fileName);
				if (!File.Exists(path))
				{
					path = String.Format(@"{0}\{1}", GetInstallPathFromRegistry("BurnInTest1"), fileName);
					if (!File.Exists(path))
					{
						path = null;
						LocateExeFromPath(Environment.GetEnvironmentVariable("ProgramFiles"), fileName, ref path);
						if (String.IsNullOrEmpty(path))
						{
							LocateExeFromPath(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), fileName, ref path);
						}
					}
				}
			}

			return path;
		}

		private static void LocateExeFromPath(String startPath, String fileName, ref String path)
		{
			try
			{
				foreach (var item in Directory.GetFiles(startPath))
				{
					if (Path.GetFileName(item).Equals(fileName))
					{
						path = item;
						return;
					}
				}
				foreach (var item in Directory.GetDirectories(startPath))
				{
					LocateExeFromPath(item, fileName, ref path);
				}
			}
			catch
			{
			}
		}

		private static String GetInstallPathFromRegistry(String name)
		{
			using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
			{
				foreach (String skName in rk.GetSubKeyNames())
				{
					using (RegistryKey sk = rk.OpenSubKey(skName))
					{
						try
						{
							if (sk.GetValue("DisplayName") != null)
							{
								if (sk.GetValue("InstallLocation") != null)
								{
									String displayName = Convert.ToString(sk.GetValue("DisplayName"));
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
