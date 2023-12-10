using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.CLA
{
  public class CommandParam
  {
    public string Input { get; set; }
    public bool Success { get; set; }

    public CommandParam(string input)
    {
      Input = input;
      Success = false;
    }
  }

  public class CommandContainer : IComparable
  {
    private LogWriter log;

    public int Index { get; set; }
    public Action<CommandParam> Command { get; set; }
    public string CmdStr { get; set; }
    public string CmdStrShort { get; set; }
    public string Description { get; set; }
    public string Note { get; set; }
    public string CmdParameterValue { get; set; }
    public string PrintName
    {
      get
      {
        string first = $"{CmdStrShort},";
        return $"-{first.PadRight(4)} --{CmdStr}";
      }
    }

    public CommandContainer(int index, Action<CommandParam> command, string cmdStr, string cmdStrShort, string description, string note = null)
    {
      log = LogWriter.Instance;
      Index = index;
      Command = command;
      CmdStr = cmdStr.ToLower();
      CmdStrShort = cmdStrShort;
      Description = description;
      Note = note;
    }

    public bool Execute()
    {
      bool retVal = false;
      if (null != Command)
      {
        try
        {
          CommandParam param = new CommandParam(CmdParameterValue);
          Command(param);
          retVal = param.Success;
        }
        catch (Exception ex)
        {
          log.WriteToLog(ex);
          retVal = false;
        }
      }
      return retVal;
    }

    public bool IsThisCommand(string arg)
    {
      string trimmedArg = arg.Trim('-', ' ');
      return trimmedArg.Equals(CmdStr, StringComparison.InvariantCultureIgnoreCase) ||
             trimmedArg.Equals(CmdStrShort, StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      return IsThisCommand((obj as CommandContainer).CmdStr);
    }

    public string ToConsoleString(int indent)
    {
      int stdWidth = 30;
      string retVal = $"{PrintName.PadRight(stdWidth).PadLeft(stdWidth + indent, ' ')}{Description}";
      if (!string.IsNullOrWhiteSpace(Note))
      {
        retVal += $"\n{"".PadLeft(stdWidth + indent - 6, ' ')}{Note}";
      }
      return retVal;
    }

    public override string ToString()
    {
      return ToConsoleString(0);
    }

    public int CompareTo(object obj)
    {
      return Index.CompareTo((obj as CommandContainer).Index);
    }
  }
}
