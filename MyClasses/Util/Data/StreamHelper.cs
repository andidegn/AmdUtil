using System;
using System.IO;
using System.Reflection;

namespace AMD.Util.Data
{
	public static class StreamHelper
	{
		public static String GetEmbeddedResource(String path)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			String absPath = path;
			foreach (var item in assembly.GetManifestResourceNames())
			{
				if (item.ToUpper().Contains(path.ToUpper()))
				{
					absPath = item;
					break;
				}
			}

			using (Stream stream = assembly.GetManifestResourceStream(absPath))
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					return sr.ReadToEnd();
				}
			}
		}
	}
}
