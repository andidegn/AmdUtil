using AMD.Util.AttributeHelper;

namespace BinToData
{
  internal enum eFormatStyle : int
  {
    [Name("No Format")]
    NoFormat    = 0,
    [Name("Memory View u8")]
    MemoryByte  = 1,
    [Name("Memory View u32")]
    MemoryWord  = 2,
    [Name("C Style array")]
    C           = 3,
    [Name("C# Style array")]
    CSharp      = 4
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
    public bool CopyToClipboard { get; set; }

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
      CopyToClipboard = false;
    }
  }
}
