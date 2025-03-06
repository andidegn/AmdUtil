using AMD.Util.AttributeHelper;
using System.IO;

namespace BinToData
{
  internal enum eFormatStyle : int
  {
    [Name("No Format (Default)")]
    NoFormat    = 0,
    [Name("Memory View 8 bit")]
    MemoryByte  = 1,
    [Name("Memory View 32 bit")]
    MemoryWord  = 2,
    [Name("C Style byte array")]
    C           = 3,
    [Name("C# Style byte array")]
    CSharp      = 4
  }

  internal enum eOutput : int
  {
    [Name("Console (Default)")]
    Console           = 0b00000001,
    [Name("Clipboard")]
    Clipboard         = 0b00000010,
    [Name("File")]
    File              = 0b00000100
  }

  internal class InputParameters
  {
    public string Prefix { get; set; }
    public string Separator { get; set; }
    public int DataLineWidth { get; set; }
    public string InputPath { get; set; }
    public string OutputPath { get; set; }
    public eFormatStyle Format { get; set; }
    public string ArrayName { get; set; }
    public int TabSize { get; set; }
    public eOutput Output { get; set; }

    public InputParameters()
    {
      Prefix = "0x";
      Separator = ", ";
      DataLineWidth = 8;
      InputPath = null;
      OutputPath = null;
      ArrayName = null;
      Format = eFormatStyle.NoFormat;
      TabSize = 2;
      Output = eOutput.Console;
    }
  }
}
