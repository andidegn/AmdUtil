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
    public string ErrorMessage { get; set; }

    public CommandParam(string input)
    {
      Input = input;
      Success = false;
      ErrorMessage = null;
    }
  }

  public class CommandContainer : IComparable
  {
    private LogWriter log;

    public int Index { get; private set; }
    public Action<CommandParam> Command { get; private set; }
    public string CmdStr { get; private set; }
    public string CmdStrShort { get; private set; }
    public string Description { get; private set; }
    public string Note { get; private set; }
    public string CmdParameterValue { get; internal set; }
    public CommandParam CmdParameter { get; private set; }
    public bool Required { get; private set; }
    public string PrintName
    {
      get
      {
        string first = $"{CmdStrShort},";
        return $"-{first.PadRight(4)} --{CmdStr}";
      }
    }

    public CommandContainer(int index, Action<CommandParam> command, string cmdStr, string cmdStrShort, string description)
      : this(index, command, cmdStr, cmdStrShort, description, false, null) { }

    public CommandContainer(int index, Action<CommandParam> command, string cmdStr, string cmdStrShort, string description, bool required)
      : this(index, command, cmdStr, cmdStrShort, description, required, null) { }

    public CommandContainer(int index, Action<CommandParam> command, string cmdStr, string cmdStrShort, string description, string note)
      : this(index, command, cmdStr, cmdStrShort, description, false, note) { }

      public CommandContainer(int index, Action<CommandParam> command, string cmdStr, string cmdStrShort, string description, bool required, string note)
    {
      log = LogWriter.Instance;
      Index = index;
      Command = command;
      CmdStr = cmdStr.ToLower();
      CmdStrShort = cmdStrShort;
      Description = description;
      Required = required;
      Note = note;
    }

    public bool Execute()
    {
      bool retVal = false;
      if (null != Command)
      {
        try
        {
          CmdParameter = new CommandParam(CmdParameterValue);
          Command(CmdParameter);
          retVal = CmdParameter.Success;
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
      int stdWidth = Required ? 28 : 30;
      int noteWidth = 30;
      string retVal = $"{PrintName.PadRight(stdWidth).PadLeft(stdWidth + indent, ' ')}{(Required ? "* " : string.Empty)}{Description}";
      if (!string.IsNullOrWhiteSpace(Note))
      {
        retVal += $"\n{string.Empty.PadLeft(noteWidth + indent - 6, ' ')}{Note}";
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
