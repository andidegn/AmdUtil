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
    public List<string> ErrorMessages { get; private set; }

    #region Private variables
    private LogWriter log;
    private List<CommandContainer> commandMap;
    #endregion // Private variables

    public CommandHandler(IEnumerable<CommandContainer> commands = null)
    {
      log = LogWriter.Instance;
      ErrorMessages = new List<string>();
      commandMap = commands?.ToList() ?? new List<CommandContainer>();
    }

    public void AddCommand(CommandContainer command)
    {
      commandMap.Add(command);
    }

    public void AddCommandRange(IEnumerable<CommandContainer> commands)
    {
      commandMap.AddRange(commands);
    }

    public bool Execute(string[] args)
    {
      bool retVal = true;
      List<CommandContainer> passedCommands = new List<CommandContainer>();
      List<CommandContainer> required = commandMap.Where(x => true == x.Required).ToList();
      log.PrintDebug($"Attempting to parse args: \"{string.Join(" ", args)}\"");
      if (null != args && 0 < args.Length)
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

          foreach (CommandContainer cmdc in required)
          {
            if (!passedCommands.Contains(cmdc))
            {
              string errMsg = $"--{cmdc.CmdStr} (-{cmdc.CmdStrShort}) is required";
              log.PrintDebug(errMsg);
              ErrorMessages.Add(errMsg);
              retVal &= false;
            }
          }
          if (retVal)
          {
            foreach (CommandContainer cmdc in passedCommands)
            {
              if (false == (retVal &= cmdc.Execute()))
              {
                string errMsg = $"{cmdc.CmdStr} failed. {cmdc.CmdParameter.ErrorMessage}";
                log.PrintDebug(errMsg);
                ErrorMessages.Add(errMsg);
                break;
              }
            }
          }

          log.WriteToLog(LogMsgType.Debug, $"All args parsed. Args:\n{string.Join(", ", args)} Success: {retVal}");
        }
      }
      else
      {
        ErrorMessages.Add("No arguments given");
        retVal = false;
      }
      return retVal;
    }
  }
}
