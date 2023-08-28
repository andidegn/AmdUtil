using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AMD.Util.Extensions
{
  public static class ExtensionKey
  {
    public static System.Windows.Forms.Keys ToFormsKeys(this Key key)
    {
      // Put special case logic here if there's a key you need but doesn't map...  
      try
      {
        return (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), key.ToString());
      }
      catch
      {
        // There wasn't a direct mapping...    
        return System.Windows.Forms.Keys.None;
      }
    }

    public static Key ToInputKey(this System.Windows.Forms.Keys key)
    {
      // Put special case logic here if there's a key you need but doesn't map...  
      try
      {
        return (Key)Enum.Parse(typeof(Key), key.ToString());
      }
      catch
      {
        // There wasn't a direct mapping...    
        return Key.None;
      }
    }
  }
}
