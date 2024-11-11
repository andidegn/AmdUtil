using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace AMD.Util.Data
{
	public static class StreamHelper
	{
		public static string GetEmbeddedResource(string path, Assembly asm = null)
    {
      Assembly assembly = asm ?? Assembly.GetCallingAssembly();
      using (Stream stream = assembly.GetManifestResourceStream(GetResourcePath(path, asm)))
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					return sr.ReadToEnd();
				}
			}
    }

    public static string GetResourcePath(string absPath, Assembly asm = null)
    {
      Assembly assembly = asm ?? Assembly.GetEntryAssembly();
      foreach (var item in assembly.GetManifestResourceNames())
      {
        if (item.ToUpper().Contains(absPath.ToUpper()))
        {
          absPath = item;
          break;
        }
      }

      return absPath;
    }

    public static string DeployResource(string absPath, string deployPath, Assembly asm = null)
    {
      Assembly assembly = asm ?? Assembly.GetEntryAssembly();
      absPath = GetResourcePath(absPath, asm);
      using (Stream stream = assembly.GetManifestResourceStream(absPath))
      {
        Directory.CreateDirectory(Path.GetDirectoryName(deployPath));
        FileStream fs = File.Create(deployPath);
        stream.Seek(0, SeekOrigin.Begin);
        stream.CopyTo(fs);
        fs.Close();
        Thread.Sleep(100);
      }
      return absPath;
    }
  }
}
