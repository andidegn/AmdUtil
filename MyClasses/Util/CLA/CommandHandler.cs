using AMD.Util.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.CLA
{
  public class CommandHandler
  {
    #region Private variables
    private LogWriter log;
    private string[] args;
    private IEnumerable<CommandContainer> commandMap;
    #endregion // Private variables

    public CommandHandler(string[] args, IEnumerable<CommandContainer> commandMap)
    {
      log = LogWriter.Instance;
      this.args = args;
      this.commandMap = commandMap;
    }

    public bool Execute()
    {
      bool retVal = true;
      List<CommandContainer> passedCommands = new List<CommandContainer>();
      log.PrintDebug($"Attempting to parse args: \"{string.Join(" ", args)}\"");
      if (null != args && 1 < args.Length)
      {
        if (retVal)
        {
          for (int i = 0; i < args.Length; i++)
          {
            string arg = args[i];
            CommandContainer cc = null;
            if (arg.StartsWith("--"))
            {
              cc = commandMap.Where(x => x.CmdStr.Equals(arg.Substring(2))).DefaultIfEmpty(null).SingleOrDefault() as CommandContainer;
            }
            else if (arg.StartsWith("-"))
            {
              cc = commandMap.Where(x => x.CmdStrShort.Equals(arg.Substring(1))).DefaultIfEmpty(null).SingleOrDefault() as CommandContainer;
            }
            else
            {
              continue;
            }

            if (null != cc)
            {
              if (passedCommands.Contains(cc))
              {
                passedCommands.Remove(cc);
              }
              if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
              {
                cc.CmdParameterValue = args[i + 1];
              }
              passedCommands.Add(cc);
            }
            else
            {
              log.WriteToLog(LogMsgType.Error, $"Argument: {arg} is not recognised");
            }
          }
          passedCommands.Sort();
          foreach (CommandContainer cmdc in passedCommands)
          {
            if (false == (retVal &= cmdc.Execute()))
            {
              log.PrintDebug($"{cmdc.CmdStr} failed");
              break;
            }
          }

          log.WriteToLog(LogMsgType.Debug, $"All args parsed. Args:\n{string.Join(", ", this.args)} Success: {retVal}");
        }
      }
      return retVal;
    }
  }
}
