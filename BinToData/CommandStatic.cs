using AMD.Util.AttributeHelper;
using AMD.Util.CLA;
using AMD.Util.Data;
using AMD.Util.Extensions;
using AMD.Util.Files;
using AMD.Util.MyConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AMD.Util.ProcessUtil.ProcessWrapper;

namespace BinToData
{
  internal static class CommandStatic
  {
    public static InputParameters InputParameters { get; private set; } = new InputParameters();

    internal static string PrintNameBanner(int totalWidth)
    {
      string[] lines =
      {
        @"                   | _)      |                             |  |   ",
        @"  _` |   __ \   _` |  |   _` |   _ \   _` |   __ \      _` |  |  /",
        @" (   |  |   |  (   |  |  (   |   __/  (   |  |   |     (   |    < ",
        @"\__,_| _|  _| \__,_| _| \__,_| \___| \__, | _|  _| _) \__,_| _|\_\",
        @"                                      |___/                       "
      };
      return ConsoleHelper.GetTextBoxCentered(lines, totalWidth, 1);
    }
    private static string ArrayStyles
    {
      get
      {
        IEnumerable<string> entries = Enum.GetValues(typeof(eFormatStyle)).Cast<eFormatStyle>().Select(x => $"\t\t\t\t{((int)x).ToString().PadRight(Enum.GetValues(typeof(eFormatStyle)).Cast<int>().Max().ToString().Length + 1)}= {x.GetAttribute<NameAttribute>().Name}");

        return string.Join(Environment.NewLine, entries);
      }
    }

    private static List<CommandContainer> commandMap;
    public static List<CommandContainer> CommandMap
    {
      get
      {
        if (commandMap is null)
        {
          commandMap = new List<CommandContainer>()
          {
            new CommandContainer(0,     PREFIX,             nameof(PREFIX),             "p",  $"The prefix of the individual data: [PREFIX]XX. Eg. 0x01. - [Default: '0x']", "Note: Does not work with Memory Views"),
            new CommandContainer(10,    SEPARATOR,          nameof(SEPARATOR),          "s",  "The separator between data: XX[SEPARATOR]YY. Eg. ', ' = 01, 02. - [Default: ', ']", "Note: Does not work with Memory Views"),
            new CommandContainer(20,    DATA_LINE_WIDTH,    nameof(DATA_LINE_WIDTH),    "w",  "The width of the line of data in number of data points. - [Default: 8]", $"Note: Does not work with {eFormatStyle.MemoryWord.GetAttribute<NameAttribute>().Name}"),
            new CommandContainer(30,    INPUT,              nameof(INPUT),              "i",  $"Input path",  true, "Note: Required!"),
            new CommandContainer(40,    FORMAT,             nameof(FORMAT),             "f",  $"Selects formatting:\n{ArrayStyles}. - [Default: {eFormatStyle.NoFormat.GetAttribute<NameAttribute>().Name}]"),
            new CommandContainer(45,    TAB_SIZE,           nameof(TAB_SIZE),           "ts", $"Tab size. Only used when --{nameof(FORMAT)} is set to an array style. - [Default: 2]"),
            new CommandContainer(999,   COPY_TO_CLIPBOARD,  nameof(COPY_TO_CLIPBOARD),  "c",  $"Copies the output data to the clipboard"),
            new CommandContainer(1000,  HELP,               nameof(HELP),               "?",  "Displays this help screen."),
          };
        }
        return commandMap;
      }
    }

    //public static readonly List<CommandContainer> CommandMap = new List<CommandContainer>()
    //{
    //  new CommandContainer(0,     PREFIX,             nameof(PREFIX),             "p",  $"The prefix of the individual data: [PREFIX]XX. Eg. 0x01. - [Default: '0x']", "Note: Does not work with Memory Views"),
    //  new CommandContainer(10,    SEPARATOR,          nameof(SEPARATOR),          "s",  "The separator between data: XX[SEPARATOR]YY. Eg. ', ' = 01, 02. - [Default: ', ']", "Note: Does not work with Memory Views"),
    //  new CommandContainer(20,    DATA_LINE_WIDTH,    nameof(DATA_LINE_WIDTH),    "w",  "The width of the line of data in number of data points. - [Default: 8]", $"Note: Does not work with {eFormatStyle.MemoryWord.GetAttribute<NameAttribute>().Name}"),
    //  new CommandContainer(30,    INPUT,              nameof(INPUT),              "i",  $"Input path",  true, "Note: Required!"),
    //  new CommandContainer(40,    FORMAT,             nameof(FORMAT),             "f",  $"Selects formatting:\n{ArrayStyles}. - [Default: {eFormatStyle.NoFormat.GetAttribute<NameAttribute>().Name}]"),
    //  new CommandContainer(45,    TAB_SIZE,           nameof(TAB_SIZE),           "ts", $"Tab size. Only used when --{nameof(FORMAT)} is set to an array style. - [Default: 2]"),
    //  new CommandContainer(999,   COPY_TO_CLIPBOARD,  nameof(COPY_TO_CLIPBOARD),  "c",  $"Copies the output data to the clipboard"),
    //  new CommandContainer(1000,  HELP,               nameof(HELP),               "?",  "Displays this help screen."),
    //};

    private static string partNoAndVersion;
    public static string PartNoAndVersion
    {
      get
      {
        AssemblyName assem = Assembly.GetEntryAssembly().GetName();
        return $"{assem.Name} v{assem.Version}";
      }
    }

    public static string HeaderAndDescription
    {
      get
      {
        int totalDescriptionWidth = 53;
        return
          ConsoleHelper.GetTextBoxWithHeaderCentered(PartNoAndVersion, $"This application takes the binary of a file and formats it to a hex data string.", totalDescriptionWidth);
      }
    }

    internal static string ValidArguments
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        foreach (CommandContainer cc in CommandMap)
        {
          sb.AppendLine(cc.ToConsoleString(2));
        }
        return sb.ToString();
      }
    }

    internal static string Help
    {
      get
      {
        return $"{HeaderAndDescription}\n\nValid arguments:\n{StringFormatHelper.Repeat("-", 53)}\n{ValidArguments}";
      }
    }

    internal static bool IsHelpCommand(string[] args)
    {
      return args.Length == 1 &&
        (args[0].Equals("-?", StringComparison.InvariantCultureIgnoreCase) ||
         args[0].Equals("--help", StringComparison.InvariantCultureIgnoreCase));
    }

    private static string GetCommandShort(string commandName)
    {
      return CommandMap.First(x => x.CmdStr.Equals(commandName, StringComparison.OrdinalIgnoreCase)).CmdStrShort;
    }

    internal static void PREFIX(CommandParam param)
    {
      InputParameters.Prefix = param.Input;
      param.Success = true;
    }

    internal static void SEPARATOR(CommandParam param)
    {
      InputParameters.Separator = param.Input;
      param.Success = true;
    }

    internal static void DATA_LINE_WIDTH(CommandParam param)
    {
      if (int.TryParse(param.Input, out int result))
      {
        InputParameters.DataLineWidth = result;
        param.Success = true;
      }
      else
      {
        param.ErrorMessage = $"\"{param.Input}\" is not a valid number";
        param.Success = false;
      }
    }

    internal static void INPUT(CommandParam param)
    {
      if (File.Exists(param.Input))
      {
        InputParameters.InputPath = param.Input;
        param.Success = true;
      }
      else
      {
        param.ErrorMessage = $"\"{param.Input}\" does not exist";
        param.Success = false;
      }
    }

    //internal static void OUTPUT(CommandParam param)
    //{
    //  if (FileHelper.IsFilePathLegal(param.Input))
    //  {
    //    InputParameters.OutputPath = param.Input;
    //    param.Success = true;
    //  }
    //}

    internal static void TAB_SIZE(CommandParam param)
    {
      if (int.TryParse(param.Input, out int tabSize))
      {
        InputParameters.TabSize = tabSize;
        param.Success = true;
      }
      else
      {
        param.ErrorMessage = $"\"{param.Input}\" is not a valid tab size";
        param.Success = false;
      }
    }

    internal static void FORMAT(CommandParam param)
    {
      if (int.TryParse(param.Input, out int enIdx) && Enum.IsDefined(typeof(eFormatStyle), enIdx))
      {
        InputParameters.Format = (eFormatStyle)enIdx;
        param.Success = true;
      }
      else
      {
        param.ErrorMessage = $"\"{param.Input}\" is not a valid array style";
        param.Success = false;
      }
    }

    internal static void COPY_TO_CLIPBOARD(CommandParam param)
    {
      InputParameters.CopyToClipboard = true;
      param.Success = true;
    }

    private static void HELP(CommandParam param)
    {
      // Only here for framework completion
      param.Success = true;
    }
  }
}
