using System;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace AMD.Util.Permissions
{
  public class FastPrincipal : System.Security.Principal.WindowsPrincipal
  {
    private const int ERROR_INSUFFICIENT_BUFFER = 122;  //from winerror.h
    private const int SecurityImpersonation = 2;        //SECURITY_IMPERSONATION_LEVEL enum from winnt.h
    private static SidCache s_sidCache = null;
    private static object s_cacheLock = new object();

    private class SidCache : System.Collections.Hashtable
    {
      public SidCache() : base()
      {  //ensure we clean up our native heap memory
        System.AppDomain.CurrentDomain.DomainUnload += new EventHandler(DomainUnload);
      }

      private void DomainUnload(Object sender, EventArgs e)
      {
        this.Clear();
      }

      public override void Clear()
      {
        foreach (Sid sid in base.Values)
        {
          sid.Dispose();
        }
        base.Clear();
      }
    }

    private class Sid : IDisposable
    {
      IntPtr _sidvalue;
      int _length;

      public Sid(IntPtr sid, int length)
      {
        _sidvalue = sid;
        _length = length;
      }

      public int Length { get { return _length; } }

      public IntPtr Value { get { return _sidvalue; } }

      public Sid Copy()
      {
        Sid newSid = new Sid(IntPtr.Zero, 0);
        byte[] buffer = new byte[this.Length];
        newSid._sidvalue = Marshal.AllocHGlobal(this.Length);
        newSid._length = this.Length;
        Marshal.Copy(this.Value, buffer, 0, this.Length);
        Marshal.Copy(buffer, 0, newSid._sidvalue, this.Length);

        return newSid;
      }

      public void Dispose()
      {
        if (_sidvalue != IntPtr.Zero)
        {
          Marshal.FreeHGlobal(_sidvalue);
          _sidvalue = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
      }
    }//Sid

    public FastPrincipal(WindowsIdentity ntIdentity) : base(ntIdentity) { }


    public override bool IsInRole(string role)
    {
      Sid sid = null;
      bool ret = false;
      try
      {
        sid = GetSid(role);
        if (sid != null && sid.Value != IntPtr.Zero)
        {
          ret = IsSidInToken(((WindowsIdentity)base.Identity).Token, sid);
        }
      }
      catch {/*Don’t allow exceptions to bubble back up*/}
      finally
      {
        if (sid != null)
        {
          sid.Dispose();
        }
      }

      return ret;
    }

    private bool IsSidInToken(IntPtr token, Sid sid)
    {
      IntPtr impersonationToken = IntPtr.Zero;
      bool inToken = false;

      try
      {
        if (DuplicateToken(token, SecurityImpersonation, ref impersonationToken))
        {
          CheckTokenMembership(impersonationToken, sid.Value, out inToken);
        }
      }
      finally
      {
        if (impersonationToken != IntPtr.Zero)
        {
          CloseHandle(impersonationToken);
        }
      }

      return inToken;
    }

    private Sid GetSid(string role)
    {
      Sid sid = null;

      sid = GetSidFromCache(role);
      if (sid == null)
      {
        sid = ResolveNameToSid(role);
        if (sid != null)
        {
          AddSidToCache(role, sid);
        }
      }
      return sid;
    }

    private Sid GetSidFromCache(string role)
    {
      Sid sid = null;

      if (s_sidCache != null)
      {
        lock (s_cacheLock)
        {
          Sid cachedsid = (Sid)s_sidCache[role.ToUpper()];
          if (cachedsid != null)
          {
            sid = cachedsid.Copy();
          }
        }
      }
      return sid;
    }

    private void AddSidToCache(string role, Sid sid)
    {
      if (s_sidCache == null)
      {
        lock (s_cacheLock)
        {
          if (s_sidCache == null)
          {
            s_sidCache = new SidCache();
          }
        }
      }

      lock (s_cacheLock)
      {
        if (!s_sidCache.Contains(role))
        {
          s_sidCache.Add(role.ToUpper(), sid.Copy());
        }
      }
    }


    private Sid ResolveNameToSid(string name)
    {
      bool ret = false;
      Sid sid = null;
      IntPtr psid = IntPtr.Zero;
      IntPtr domain = IntPtr.Zero;
      int sidLength = 0;
      int domainLength = 0;
      int sidType = 0;

      try
      {
        ret = LookupAccountName(null,
                      name,
                      psid,
                      ref sidLength,
                      domain,
                      ref domainLength,
                      out sidType);


        if (ret == false &&
          Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
        {
          psid = Marshal.AllocHGlobal(sidLength);

          //LookupAccountName only works on Unicode systems so to ensure
          //we allocate a LPWSTR
          domain = Marshal.AllocHGlobal(domainLength * 2);

          ret = LookupAccountName(null,
                        name,
                        psid,
                        ref sidLength,
                        domain,
                        ref domainLength,
                        out sidType);



        }

        if (ret == true)
        {
          sid = new Sid(psid, sidLength);
        }

      }
      finally
      {
        if (domain != IntPtr.Zero)
        {
          Marshal.FreeHGlobal(domain);
        }
      }

      return sid;

    }

    #region /********IMPORTS***********/
    //only works on Unicode systems so we ar safe to go with an Auto CharSet
    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private extern static bool LookupAccountName(string lpSystemName,
                             string lpAccountName,
                             IntPtr Sid, ref int cbSid,
                             IntPtr ReferencedDomainName, ref int cbReferencedDomainName,
                             out int peUse);

    [DllImport("advapi32.dll")]
    public extern static bool CheckTokenMembership(IntPtr TokenHandle, IntPtr SidToCheck, out bool IsMember);


    [DllImport("advapi32.dll")]
    public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
                          int SECURITY_IMPERSONATION_LEVEL,
                          ref IntPtr DuplicateTokenHandle);

    [DllImport("kernel32.dll")]
    public extern static bool CloseHandle(IntPtr Handle);

    #endregion

  }//FastPrincipal

  public static class Permission
  {
    public static bool IsRunAsAdmin()
    {
      WindowsIdentity id = WindowsIdentity.GetCurrent();
      WindowsPrincipal principal = new WindowsPrincipal(id);

      return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static string GetCurrentUserOnMachine
    {
      get
      {
        return string.Format("{0}@{1} on {2}", Environment.UserName, Environment.UserDomainName, Environment.MachineName);
      }
    }

    //[PrincipalPermission(SecurityAction.Demand, Role = @"INNOSCAN\Domain Admins")]
    public static bool HasAccess(string role)
    {
      AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

      WindowsIdentity curIdentity = WindowsIdentity.GetCurrent();
      WindowsPrincipal myPrincipal = new WindowsPrincipal(curIdentity);
      if (myPrincipal.Identity.IsAuthenticated)
      {

        //if (debugStr != null)
        //{
        //	List<string> groups = new List<string>();
        //	StringBuilder sb = new StringBuilder();
        //	foreach (IdentityReference irc in curIdentity.Groups)
        //	{
        //		groups.Add(((NTAccount)irc.Translate(typeof(NTAccount))).Value);
        //		sb.AppendLine(string.Format("Is in {0} - {1}", ((NTAccount)irc.Translate(typeof(NTAccount))).Value, myPrincipal.IsInRole(((NTAccount)irc.Translate(typeof(NTAccount))).Value)));
        //	}

        //	sb.AppendFormat("is in {0} - {1}\n", role, myPrincipal.IsInRole(role));

        //	debugStr = string.Format("Name:\t\t{0}\nAuth:\t{1}\nBuiltinAdmin:\t{2}\nGroups:\n\t\t{3}", curIdentity.Name, curIdentity.IsAuthenticated, myPrincipal.IsInRole(role) ? "True" : "False", string.Join(string.Format(",{0}\t\t", Environment.NewLine), groups.ToArray()));
        //}
        return new FastPrincipal(WindowsIdentity.GetCurrent()).IsInRole(role);
        return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(role);
      }
      return false;
    }

    public static bool HasAccess(params string[] roles)
    {
      foreach (string role in roles)
      {
        if (HasAccess(role))
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public static bool IsMemberOf(string groupName, string domainName)
    {
      PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName);
      UserPrincipal user = UserPrincipal.FindByIdentity(ctx, Environment.UserName);
      GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);

      if (null != user && null != group)
      {
        if (user.IsMemberOf(group))
        {
          return true;
        }
      }
      return false;
    }
  }

  public enum LogonType
  {
    LOGON32_LOGON_INTERACTIVE = 2,
    LOGON32_LOGON_NETWORK = 3,
    LOGON32_LOGON_BATCH = 4,
    LOGON32_LOGON_SERVICE = 5,
    LOGON32_LOGON_UNLOCK = 7,
    LOGON32_LOGON_NETWORK_CLEARTEXT = 8, // Win2K or higher
    LOGON32_LOGON_NEW_CREDENTIALS = 9 // Win2K or higher
  };

  public enum LogonProvider
  {
    LOGON32_PROVIDER_DEFAULT = 0,
    LOGON32_PROVIDER_WINNT35 = 1,
    LOGON32_PROVIDER_WINNT40 = 2,
    LOGON32_PROVIDER_WINNT50 = 3
  };

  public enum ImpersonationLevel
  {
    SecurityAnonymous = 0,
    SecurityIdentification = 1,
    SecurityImpersonation = 2,
    SecurityDelegation = 3
  }

  class Win32NativeMethods
  {
    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern int LogonUser(string lpszUserName,
       string lpszDomain,
       string lpszPassword,
       int dwLogonType,
       int dwLogonProvider,
       ref IntPtr phToken);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int DuplicateToken(IntPtr hToken,
        int impersonationLevel,
        ref IntPtr hNewToken);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool RevertToSelf();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool CloseHandle(IntPtr handle);
  }

  /// <summary>
  /// Allows code to be executed under the security context of a specified user account.
  /// </summary>
  /// <remarks> 
  ///
  /// Implements IDispose, so can be used via a using-directive or method calls;
  ///  ...
  ///
  ///  var imp = new Impersonator( "myUsername", "myDomainname", "myPassword" );
  ///  imp.UndoImpersonation();
  ///
  ///  ...
  ///
  ///   var imp = new Impersonator();
  ///  imp.Impersonate("myUsername", "myDomainname", "myPassword");
  ///  imp.UndoImpersonation();
  ///
  ///  ...
  ///
  ///  using ( new Impersonator( "myUsername", "myDomainname", "myPassword" ) )
  ///  {
  ///   ...
  ///   
  ///   ...
  ///  }
  ///
  ///  ...
  /// </remarks>
  public class Impersonator : IDisposable
  {
    private WindowsImpersonationContext _wic;

    /// <summary>
    /// Begins impersonation with the given credentials, Logon type and Logon provider.
    /// </summary>
    /// <param name = "userName" > Name of the user.</param>
    /// <param name = "domainName" > Name of the domain.</param>
    /// <param name = "password" > The password. <see cref = "System.string" /></ param >
    /// <param name="logonType">Type of the logon.</param>
    /// <param name = "logonProvider" > The logon provider. <see cref = "Mit.Sharepoint.WebParts.EventLogQuery.Network.LogonProvider" /></ param >
    public Impersonator(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
    {
      Impersonate(userName, domainName, password, logonType, logonProvider);
    }

    /// <summary>
    /// Begins impersonation with the given credentials.
    /// </summary>
    /// <param name = "userName" > Name of the user.</param>
    /// <param name = "domainName" > Name of the domain.</param>
    /// <param name = "password" > The password. <see cref = "System.string" /></ param >
    public Impersonator(string userName, string domainName, string password)
    {
      Impersonate(userName, domainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Impersonator"/> class.
    /// </summary>
    public Impersonator()
    { }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      UndoImpersonation();
    }

    /// <summary>
    /// Impersonates the specified user account.
    /// </summary>
    /// <param name = "userName" > Name of the user.</param>
    /// <param name = "domainName" > Name of the domain.</param>
    /// <param name = "password" > The password. <see cref = "System.string" /></ param >
    public void Impersonate(string userName, string domainName, string password)
    {
      Impersonate(userName, domainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
    }

    /// <summary>
    /// Impersonates the specified user account.
    /// </summary>
    /// <param name = "userName" > Name of the user.</param>
    /// <param name = "domainName" > Name of the domain.</param>
    /// <param name = "password" > The password. <see cref = "System.string" /></param >
    /// <param name="logonType">Type of the logon.</param>
    /// <param name = "logonProvider" > The logon provider. <see cref = "Mit.Sharepoint.WebParts.EventLogQuery.Network.LogonProvider" /></param >
    public void Impersonate(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
    {
      UndoImpersonation();

      IntPtr logonToken = IntPtr.Zero;
      IntPtr logonTokenDuplicate = IntPtr.Zero;
      try
      {
        // revert to the application pool identity, saving the identity of the current requestor
        _wic = WindowsIdentity.Impersonate(IntPtr.Zero);

        // do logon & impersonate
        if (Win32NativeMethods.LogonUser(userName, domainName, password, (int)logonType, (int)logonProvider, ref logonToken) != 0)
        {
          if (Win32NativeMethods.DuplicateToken(logonToken, (int)ImpersonationLevel.SecurityImpersonation, ref logonTokenDuplicate) != 0)
          {
            var wi = new WindowsIdentity(logonTokenDuplicate);
            wi.Impersonate(); // discard the returned identity context (which is the context of the application pool)
          }
          else
          {
            throw new Win32Exception(Marshal.GetLastWin32Error());
          }
        }
        else
        {
          throw new Win32Exception(Marshal.GetLastWin32Error());
        }
      }
      finally
      {
        if (logonToken != IntPtr.Zero)
        {
          Win32NativeMethods.CloseHandle(logonToken);
        }

        if (logonTokenDuplicate != IntPtr.Zero)
        {
          Win32NativeMethods.CloseHandle(logonTokenDuplicate);
        }
      }
    }

    /// <summary>
    /// Stops impersonation.
    /// </summary>
    private void UndoImpersonation()
    {
      // restore saved requestor identity
      if (_wic != null)
      {
        _wic.Undo();
      }
      _wic = null;
    }
  }
}
