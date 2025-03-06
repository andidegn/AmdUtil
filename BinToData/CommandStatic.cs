using AMD.Util.AttributeHelper;
using AMD.Util.CLA;
using AMD.Util.Data;
using AMD.Util.Extensions;
using AMD.Util.Files;
using AMD.Util.MyConsole;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AMD.Util.ProcessUtil.ProcessWrapper;

namespace BinToData
{
  internal static class CommandStatic
  {
    internal const int TotalWidth = 113;
    public static InputParameters InputParameters { get; private set; } = new InputParameters();

    internal static string Banner
    {
      get
      {
        string[] lines =
        { 
          //// In Shadow ascii txt
          //@"                   | _)      |                             |  |   ",
          //@"  _` |   __ \   _` |  |   _` |   _ \   _` |   __ \      _` |  |  /",
          //@" (   |  |   |  (   |  |  (   |   __/  (   |  |   |     (   |    < ",
          //@"\__,_| _|  _| \__,_| _| \__,_| \___| \__, | _|  _| _) \__,_| _|\_\",
          //@"                                      |___/                       ",

          //// Rounded ascii txt
          //@"                 _ _     _                        _ _     ",
          //@"                | (_)   | |                      | | |    ",
          //@" _____ ____   __| |_  __| |_____  ____ ____    __| | |  _ ",
          //@"(____ |  _ \ / _  | |/ _  | ___ |/ _  |  _ \  / _  | |_/ )",
          //@"/ ___ | | | ( (_| | ( (_| | ____( (_| | | | |( (_| |  _ ( ",
          //@"\_____|_| |_|\____|_|\____|_____)\___ |_| |_(_)____|_| \_)",
          //@"                                (_____|                   ",
          //@"                                                          ",

          //// Small shadow ascii txt
          @"                   | _)      |                            |  |   ",
          @"   _` |    \    _` |  |   _` |   -_)   _` |    \       _` |  | / ",
          @" \__,_| _| _| \__,_| _| \__,_| \___| \__, | _| _| _) \__,_| _\_\ ",
          @"                                     ____/                       ",

          //// Spliff ascii txt
          //@" _____  _____  _____  ___  _____  _____  _____  _____     _____  __ ___",
          //@"/  _  \/  _  \|  _  \/___\|  _  \/   __\/   __\/  _  \   |  _  \|  |  /",
          //@"|  _  ||  |  ||  |  ||   ||  |  ||   __||  |_ ||  |  | _ |  |  ||  _ <",
          //@"\__|__/\__|__/|_____/\___/|_____/\_____/\_____/\__|__/<_>|_____/|__|__\",
        };

        return ConsoleHelper.GetTextBoxCentered(lines, TotalWidth, 1);
      }
    }

    internal const string Description = "This application takes the binary of a file and formats it to a hex data string.";

    private static string FormatStyles
    {
      get
      {
        IEnumerable<string> entries = Enum.GetValues(typeof(eFormatStyle)).Cast<eFormatStyle>().Select(x => $"\t\t\t\t{((int)x).ToString().PadRight(Enum.GetValues(typeof(eFormatStyle)).Cast<int>().Max().ToString().Length + 1)}= {x.GetAttribute<NameAttribute>().Name}");

        return string.Join(Environment.NewLine, entries);
      }
    }

    private static string Outputs
    {
      get
      {
        IEnumerable<string> entries = Enum.GetValues(typeof(eOutput)).Cast<eOutput>().Select(x => $"\t\t\t\t{((int)x).ToString().PadRight(Enum.GetValues(typeof(eOutput)).Cast<int>().Max().ToString().Length + 1)}= {x.GetAttribute<NameAttribute>().Name}");

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
            new CommandContainer(40,    INPUT,              nameof(INPUT),              "i",      $"Input path",  true),
            new CommandContainer(10,    PREFIX,             nameof(PREFIX),             "p",      $"The prefix of the individual data: [PREFIX]XX. Eg. 0x01. - [Default: '0x']", "Note: Does not work with Memory Views"),
            new CommandContainer(20,    SEPARATOR,          nameof(SEPARATOR),          "s",      "The separator between data: XX[SEPARATOR]YY. Eg. ', ' = 01, 02. - [Default: ', ']", "Note: Does not work with Memory Views"),
            new CommandContainer(30,    DATA_LINE_WIDTH,    nameof(DATA_LINE_WIDTH),    "w",      "The width of the line of data in number of data points. - [Default: 8]", $"Note: Does not work with {eFormatStyle.MemoryWord.GetAttribute<NameAttribute>().Name}"),
            new CommandContainer(50,    FORMAT,             nameof(FORMAT),             "f",      $"Selects formatting:\n{FormatStyles}"),
            new CommandContainer(55,    ARRAY_NAME,         nameof(ARRAY_NAME),         "an",     $"Name of the array", false, $"Note: Only used when --{nameof(FORMAT)} is set to an array style. - [Default: <NAME>]"),
            new CommandContainer(56,    TAB_SIZE,           nameof(TAB_SIZE),           "ts",     $"Tab size", false, $"Note: Only used when --{nameof(FORMAT)} is set to an array style. - [Default: 2]"),
            new CommandContainer(500,   OUTPUT,             nameof(OUTPUT),             "o",      $"How to output the data:\n{Outputs}", false, "Note: Bitmask is supported"),
            new CommandContainer(499,   OUTPUT_PATH,        nameof(OUTPUT_PATH),        "op",     $"Path to output file", false, $"Note: Required if --{nameof(OUTPUT)} is set to {eOutput.File.GetAttribute<NameAttribute>().Name}"),
            new CommandContainer(1000,  HELP,               nameof(HELP),               "?",      "Displays this help screen."),
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

    private static bool AreBitsValid(int value, Type enumType)
    {
      int allValidBits = 0;
      foreach (int enumValue in Enum.GetValues(enumType))
      {
        allValidBits |= enumValue;
      }

      return (value & ~allValidBits) == 0;
    }

    public static bool IsValidVariableName(string name, bool isCSharp = true)
    {
      string[] CSharpKeywords = new[]
      {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
        "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
        "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
        "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint",
        "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias",
        "ascending", "async", "await", "by", "descending", "dynamic", "equals", "from", "get", "global", "group",
        "into", "join", "let", "nameof", "on", "orderby", "partial", "remove", "select", "set", "value", "var",
        "when", "where", "yield"
      };
      Regex IdentifierRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

      if (string.IsNullOrWhiteSpace(name))
      {
        return false;
      }

      if (!IdentifierRegex.IsMatch(name))
      {
        return false;
      }

      if (isCSharp && CSharpKeywords.Contains(name))
      {
        return false;
      }

      return true;
    }

    internal static bool IsHelpCommand(string[] args)
    {
      return args.AsQueryable().Contains("-?") ||
             args.AsQueryable().Contains("--help");
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

    internal static void ARRAY_NAME(CommandParam param)
    {
      string trimmedInput = param.Input.Trim().Replace(' ', '_').Replace('-', '_');
      if (IsValidVariableName(trimmedInput))
      {
        InputParameters.ArrayName = trimmedInput;
        param.Success = true;
      }
      else
      {
        param.ErrorMessage = $"\"{param.Input}\" is not a valid array name";
        param.Success = false;
      }
    }

    internal static void OUTPUT(CommandParam param)
    {
      if (int.TryParse(param.Input, out int output) && AreBitsValid(output, typeof(eOutput)))// Enum.IsDefined(typeof(eOutput), output))
      {
        InputParameters.Output = (eOutput)output;

        if (InputParameters.Output.HasFlag(eOutput.File) && string.IsNullOrWhiteSpace(InputParameters.OutputPath))
        {
          param.ErrorMessage = $"No --{nameof(OUTPUT_PATH)} found. This is required if --{nameof(OUTPUT)} has {eOutput.File.GetAttribute<NameAttribute>().Name} flag";
          param.Success = false;

        }
        else
        {
          param.Success = true;
        }
      }
      else
      {
        param.ErrorMessage = $"\"{param.Input}\" is not a valid array style";
        param.Success = false;
      }
    }

    internal static void OUTPUT_PATH(CommandParam param)
    {
      if (FileHelper.IsValidPath(param.Input))
      {
        InputParameters.OutputPath = param.Input;
        param.Success = true;
      }
      else
      {
        param.ErrorMessage = $"\"{param.Input}\" is not a valid --{nameof(OUTPUT_PATH)}";
        param.Success = false;
      }
    }

    private static void HELP(CommandParam param)
    {
      // Only here for framework completion
      param.Success = true;
    }
  }
}
