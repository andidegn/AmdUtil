using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Versioning {
	/// <summary>
	/// Class containing the running version of the uploader
	/// </summary>
	public class RunningVersion {
		/// <summary>
		/// The major version number
		/// </summary>
		public static int VERSION_MAJOR { get; set; }
		/// <summary>
		/// The minor version number
		/// </summary>
		public static int VERSION_MINOR { get; set; }
		/// <summary>
		/// The build number
		/// </summary>
		public static int VERSION_BUILD { get; set; }
		/// <summary>
		/// The revision number
		/// </summary>
		public static int VERSION_REVISION { get; set; }

		/// <summary>
		/// Get the current running version number
		/// </summary>
		/// <returns>The current running version</returns>
		public static System.Version GetRunningVersion {
			get {
				System.Version version;
				if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) {
					version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
				} else {
					version = new System.Version(VERSION_MAJOR, VERSION_MINOR, VERSION_BUILD, VERSION_REVISION);
				}
				return version;
			}
		}

		/// <summary>
		/// Sets the static version
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="build"></param>
		/// <param name="revision"></param>
		public static void SetStaticVersion(int major, int minor, int build, int revision) {
			VERSION_MAJOR = major;
			VERSION_MINOR = minor;
			VERSION_BUILD = build;
			VERSION_REVISION = revision;
		}

    /// <summary>
    /// Sets the static version
    /// </summary>
    /// <param name="version"></param>
    public static void SetStaticVersion(Version version)
    {
      VERSION_MAJOR = version.Major;
      VERSION_MINOR = version.Minor;
      VERSION_BUILD = version.Build;
      VERSION_REVISION = version.Revision;
    }
	}
}
