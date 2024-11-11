using AMD.Util.Collections.Dictionary;
using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AMD.Util.View.WPF.UserControls
{
  [Serializable()]
  public class LogFilterCollection : SerializableDictionary<LogMsgType, bool>
  {
    public LogFilterCollection()
      : base() { }
    protected LogFilterCollection(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {

    }
  }
}
