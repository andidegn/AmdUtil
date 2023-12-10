using AMD.Util.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AMD.Util.Data
{
  public static class StringFormatHelper
  {
    private static void SetArrValuesNull(byte?[] arr)
    {
      for (int i = 0; i < arr.Length; i++)
      {
        arr[i] = null;
      }
    }

    private static void AppendAscii(StringBuilder sb, byte?[] ascii)
    {
      sb.Append(" ");
      foreach (byte? b in ascii)
      {
        switch (b)
        {
          case var _ when b is null:
            sb.Append(' ');
            break;

          case var _ when 0x7E >= b && 0x20 <= b:
            sb.Append((char)b);
            break;

          default:
            sb.Append('.');
            break;
        }
      }
      SetArrValuesNull(ascii);
      sb.AppendLine();
    }

    private static int PopulateAsciiArray(Endian endian, byte?[] ascii, int lineIndex, uint? value, int byteCount)
    {
      for (int k = 0; k < 4; k++)
      {
        int bitShift = Endian.Big == endian ? 8 * (3 - k) : 8 * k;
        ascii[lineIndex] = (byte)(value != null ? ((value >> bitShift) & 0xFF) : 0);
        lineIndex = (lineIndex + 1) % byteCount;
      }

      return lineIndex;
    }

    /// <summary>
    /// Gets a formatted memory string from a word array
    /// </summary>
    /// <param name="startAddr"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string GetFormattedMemoryString(uint startAddr, uint?[] data, Endian endian = Endian.Big)
    {
      StringBuilder sb = new StringBuilder();
      int byteCount = 16;
      int modulus = byteCount / 4;
      int lineIndex = 0;
      int wordIndex = 0;
      byte?[] ascii = new byte?[byteCount];
      uint addr = startAddr;

      if (Endian.Big == endian)
      {
        sb.AppendLine("_Address_|_0________4________8________C________0123456789ABCDEF");
      }
      else
      {
        sb.AppendLine("_Address_|________0________4________8________C_0123456789ABCDEF");
      }

      SetArrValuesNull(ascii);

      if (data != null)
      {
        AlignAddressAndData(ref data, ref addr, byteCount);

        for (int i = 0; i < data.Length; i++)
        {
          uint? value = data[i];

          /* If first word (which is the address, print separator */
          if (wordIndex == 0)
          {
            sb.Append(addr.ToString("X8"));
            sb.Append(" |");
          }

          sb.Append(" ");

          /* Print value or ? if null */
          if (value != null)
          {
            sb.Append(value.Value.ToString("X8"));
          }
          else
          {
            if ((addr + 4 * wordIndex) >= startAddr)
            {
              sb.Append("????????");
            }
            else
            {
              sb.Append("        ");
            }
          }

          /* If last word but not mod 4, print padding until mod 4 */
          if (i == data.Length - 1 && (wordIndex + 1) % modulus != 0)
          {
            do
            {
              sb.Append("         ");
            } while ((++wordIndex + 1) % modulus != 0);
          }

          lineIndex = PopulateAsciiArray(endian, ascii, lineIndex, value, byteCount);

          if (++wordIndex % modulus == 0)
          {
            AppendAscii(sb, ascii);
            addr += (uint)byteCount;
            wordIndex = 0;
          }
        }
      }
      else
      {
        sb.Append("   No data...");
      }
      return sb.ToString();
    }

    /// <summary>
    /// Gets a formatted memory string from a word array
    /// </summary>
    /// <param name="startAddr"></param>
    /// <param name="data"></param>
    /// <param name="byteCount">Has to be divisible by 8. If not, throw exception</param>
    /// <param name="endian"></param>
    /// <returns></returns>
    public static string GetFormattedMemoryStringByteSeparated(uint startAddr, uint?[] data, int byteCount, Endian endian = Endian.Big)
    {
      if (byteCount % 8 != 0)
      {
        throw new NotSupportedException("byteCount  to be divisible by 8");
      }
      StringBuilder sb = new StringBuilder();
      int modulus = byteCount / 4;
      int lineIndex = 0;
      int wordIndex = 0;
      byte?[] ascii = new byte?[byteCount];
      uint addr = startAddr;

      AppendHeaderLine(endian, sb, byteCount);

      SetArrValuesNull(ascii);
      if (data != null)
      {
        AlignAddressAndData(ref data, ref addr, byteCount);

        for (int i = 0; i < data.Length; i++)
        {
          uint? value = data[i];

          /* If first word (which is the address, print separator */
          if (wordIndex == 0)
          {
            sb.Append(addr.ToString("X8"));
            sb.Append(" |");
          }

          sb.Append(" ");

          /* Print value or ? if null */
          if (value != null)
          {
            for (int j = 0; j < 4; j++)
            {
              byte b = (byte)(value >> ((3 - j) * 8));
              sb.Append(b.ToString("X2"));
              if (3 > j)
              {
                sb.Append(" ");
              }
            }
          }
          else
          {
            if ((addr + 4 * wordIndex) >= startAddr)
            {
              sb.Append("?? ?? ?? ??");
            }
            else
            {
              sb.Append("           ");
            }
          }

          /* If last word but not mod 4, print padding until mod 4 */
          if (i == data.Length - 1 && (wordIndex + 1) % modulus != 0)
          {
            do
            {
              sb.Append("            ");
            } while ((++wordIndex + 1) % modulus != 0);
          }
          lineIndex = PopulateAsciiArray(endian, ascii, lineIndex, value, byteCount);

          if (++wordIndex % modulus == 0)
          {
            AppendAscii(sb, ascii);
            wordIndex = 0;
            addr += (uint)byteCount;

          }
        }
      }
      else
      {
        sb.Append("   No data...");
      }
      return sb.ToString();
    }

    private static void AppendHeaderLine(Endian endian, StringBuilder sb, int byteCount)
    {
      sb.Append("_Address_|");
      for (int i = 0; i < byteCount; i++)
      {
        if (Endian.Big == endian)
        {
          sb.Append($"{i:X}".PadLeft(3, '_'));
        }
        else
        {
          int idx = 4 * (i / 4) + 4;
          sb.Append($"{idx - i - 1 + idx - 4:X}".PadLeft(3, '_'));
        }
      }
      sb.Append('_');
      for (int i = 0; i < byteCount; i++)
      {
        sb.Append($"{(i & 0x0F):X}");
      }
      sb.AppendLine();
    }

    public static string GetByteArrayString(byte[] arr, bool showAsWord, bool littleEndian)
    {
      StringBuilder sb = new StringBuilder();
      if (showAsWord)
      {
        uint tmp = 0;
        for (int i = 0; i < arr.Length; i++)
        {
          int byteNo = i % 4;
          int bitShift = 8 * (littleEndian ? byteNo : 3 - byteNo);
          tmp |= (uint)(arr[i] << bitShift);
          if (byteNo == 3 || i == arr.Length - 1)
          {
            sb.AppendFormat("{0}{1}", tmp.ToString("X8"), i % 31 == 0 ? "\n" : " ");
            tmp = 0;
          }
        }
      }
      else
      {
        for (int i = 0; i < arr.Length; i++)
        {
          sb.AppendFormat("{0}{1}", arr[i].ToString("X2"), (i + 1) % 16 == 0 ? "\n" : " ");
        }
      }
      return sb.ToString();
    }

    public static string GetWordArrayString(uint[] arr, bool showAsByte, bool littleEndian)
    {
      StringBuilder sb = new StringBuilder();
      if (showAsByte)
      {
        for (int i = 0; i < arr.Length; i++)
        {
          for (int j = 3; j >= 0; j--)
          {
            sb.AppendFormat("{0} ", ((arr[i] >> 8 * (littleEndian ? 3 - j : j)) & 0xFF).ToString("X2"));
          }
          if (i < arr.Length - 1 && (i + 1) % 4 == 0)
          {
            sb.AppendLine();
          }
        }
      }
      else
      {
        for (int i = 0; i < arr.Length; i++)
        {
          sb.AppendFormat("{0}{1}", arr[i].ToString("X8"), (i + 1) % 8 == 0 ? "\n" : " ");
        }
      }
      return sb.ToString();
    }

    private static void AlignAddressAndData(ref uint?[] data, ref uint addr, int byteCount)
    {
      if (addr % byteCount != 0)
      {
        uint overflow = addr % (uint)byteCount;
        addr -= overflow;

        List<byte?> bListTemp = new List<byte?>();
        for (int i = 0; i < overflow; i++)
        {
          bListTemp.Add(null);
        }

        foreach (var item in data)
        {
          if (item == null)
          {
            bListTemp.AddRange(new byte?[4]);
          }
          else
          {
            bListTemp.Add((byte?)item);
            bListTemp.Add((byte?)(item >> 8));
            bListTemp.Add((byte?)(item >> 16));
            bListTemp.Add((byte?)(item >> 24));
          }
        }
        List<uint?> uintListTemp = new List<uint?>();
        uint? tmp = null;
        for (int i = 0; i < bListTemp.Count; i++)
        {
          byte? bVal = bListTemp[i];
          int byteOffSet = i % 4;
          if (bVal == null)
          {
            tmp = null;
          }
          else
          {
            tmp |= (uint?)(bVal << (byteOffSet * 8));
          }

          if (byteOffSet == 3)
          {
            uintListTemp.Add(tmp);
            tmp = 0;
          }
        }

        data = uintListTemp.ToArray();
      }
    }

    public static string Repeat(string text, int count)
    {
      return string.Concat(Enumerable.Repeat(text, count)) + Environment.NewLine;
    }

    private class StackEntry
    {
      public int NumberOfCharactersToSkip { get; set; }
      public bool Ignorable { get; set; }

      public StackEntry(int numberOfCharactersToSkip, bool ignorable)
      {
        NumberOfCharactersToSkip = numberOfCharactersToSkip;
        Ignorable = ignorable;
      }
    }

    private static readonly Regex _rtfRegex = new Regex(@"\\([a-z]{1,32})(-?\d{1,10})?[ ]?|\\'([0-9a-f]{2})|\\([^a-z])|([{}])|[\r\n]+|(.)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    private static readonly List<string> destinations = new List<string>
    {
        "aftncn","aftnsep","aftnsepc","annotation","atnauthor","atndate","atnicn","atnid",
        "atnparent","atnref","atntime","atrfend","atrfstart","author","background",
        "bkmkend","bkmkstart","blipuid","buptim","category","colorschememapping",
        "colortbl","comment","company","creatim","datafield","datastore","defchp","defpap",
        "do","doccomm","docvar","dptxbxtext","ebcend","ebcstart","factoidname","falt",
        "fchars","ffdeftext","ffentrymcr","ffexitmcr","ffformat","ffhelptext","ffl",
        "ffname","ffstattext","field","file","filetbl","fldinst","fldrslt","fldtype",
        "fname","fontemb","fontfile","fonttbl","footer","footerf","footerl","footerr",
        "footnote","formfield","ftncn","ftnsep","ftnsepc","g","generator","gridtbl",
        "header","headerf","headerl","headerr","hl","hlfr","hlinkbase","hlloc","hlsrc",
        "hsv","htmltag","info","keycode","keywords","latentstyles","lchars","levelnumbers",
        "leveltext","lfolevel","linkval","list","listlevel","listname","listoverride",
        "listoverridetable","listpicture","liststylename","listtable","listtext",
        "lsdlockedexcept","macc","maccPr","mailmerge","maln","malnScr","manager","margPr",
        "mbar","mbarPr","mbaseJc","mbegChr","mborderBox","mborderBoxPr","mbox","mboxPr",
        "mchr","mcount","mctrlPr","md","mdeg","mdegHide","mden","mdiff","mdPr","me",
        "mendChr","meqArr","meqArrPr","mf","mfName","mfPr","mfunc","mfuncPr","mgroupChr",
        "mgroupChrPr","mgrow","mhideBot","mhideLeft","mhideRight","mhideTop","mhtmltag",
        "mlim","mlimloc","mlimlow","mlimlowPr","mlimupp","mlimuppPr","mm","mmaddfieldname",
        "mmath","mmathPict","mmathPr","mmaxdist","mmc","mmcJc","mmconnectstr",
        "mmconnectstrdata","mmcPr","mmcs","mmdatasource","mmheadersource","mmmailsubject",
        "mmodso","mmodsofilter","mmodsofldmpdata","mmodsomappedname","mmodsoname",
        "mmodsorecipdata","mmodsosort","mmodsosrc","mmodsotable","mmodsoudl",
        "mmodsoudldata","mmodsouniquetag","mmPr","mmquery","mmr","mnary","mnaryPr",
        "mnoBreak","mnum","mobjDist","moMath","moMathPara","moMathParaPr","mopEmu",
        "mphant","mphantPr","mplcHide","mpos","mr","mrad","mradPr","mrPr","msepChr",
        "mshow","mshp","msPre","msPrePr","msSub","msSubPr","msSubSup","msSubSupPr","msSup",
        "msSupPr","mstrikeBLTR","mstrikeH","mstrikeTLBR","mstrikeV","msub","msubHide",
        "msup","msupHide","mtransp","mtype","mvertJc","mvfmf","mvfml","mvtof","mvtol",
        "mzeroAsc","mzeroDesc","mzeroWid","nesttableprops","nextfile","nonesttables",
        "objalias","objclass","objdata","object","objname","objsect","objtime","oldcprops",
        "oldpprops","oldsprops","oldtprops","oleclsid","operator","panose","password",
        "passwordhash","pgp","pgptbl","picprop","pict","pn","pnseclvl","pntext","pntxta",
        "pntxtb","printim","private","propname","protend","protstart","protusertbl","pxe",
        "result","revtbl","revtim","rsidtbl","rxe","shp","shpgrp","shpinst",
        "shppict","shprslt","shptxt","sn","sp","staticval","stylesheet","subject","sv",
        "svb","tc","template","themedata","title","txe","ud","upr","userprops",
        "wgrffmtfilter","windowcaption","writereservation","writereservhash","xe","xform",
        "xmlattrname","xmlattrvalue","xmlclose","xmlname","xmlnstbl",
        "xmlopen"
    };

    private static readonly Dictionary<string, string> specialCharacters = new Dictionary<string, string>
    {
        { "par", "\n" },
        { "sect", "\n\n" },
        { "page", "\n\n" },
        { "line", "\n" },
        { "tab", "\t" },
        { "emdash", "\u2014" },
        { "endash", "\u2013" },
        { "emspace", "\u2003" },
        { "enspace", "\u2002" },
        { "qmspace", "\u2005" },
        { "bullet", "\u2022" },
        { "lquote", "\u2018" },
        { "rquote", "\u2019" },
        { "ldblquote", "\u201C" },
        { "rdblquote", "\u201D" },
    };

    /// <summary>
    /// Strip RTF Tags from RTF Text
    /// </summary>
    /// <param name="inputRtf">RTF formatted text</param>
    /// <returns>Plain text from RTF</returns>
    public static string GetStringFromRtf(string inputRtf)
    {
      if (inputRtf == null)
      {
        return null;
      }

      string returnString;

      var stack = new Stack<StackEntry>();
      bool ignorable = false;              // Whether this group (and all inside it) are "ignorable".
      int ucskip = 1;                      // Number of ASCII characters to skip after a unicode character.
      int curskip = 0;                     // Number of ASCII characters left to skip
      var outList = new List<string>();    // Output buffer.

      MatchCollection matches = _rtfRegex.Matches(inputRtf);

      if (matches.Count > 0)
      {
        foreach (Match match in matches)
        {
          string word = match.Groups[1].Value;
          string arg = match.Groups[2].Value;
          string hex = match.Groups[3].Value;
          string character = match.Groups[4].Value;
          string brace = match.Groups[5].Value;
          string tchar = match.Groups[6].Value;

          if (!string.IsNullOrEmpty(brace))
          {
            curskip = 0;
            if (brace == "{")
            {
              // Push state
              stack.Push(new StackEntry(ucskip, ignorable));
            }
            else if (brace == "}")
            {
              // Pop state
              StackEntry entry = stack.Pop();
              ucskip = entry.NumberOfCharactersToSkip;
              ignorable = entry.Ignorable;
            }
          }
          else if (!string.IsNullOrEmpty(character)) // \x (not a letter)
          {
            curskip = 0;
            if (character == "~")
            {
              if (!ignorable)
              {
                outList.Add("\xA0");
              }
            }
            else if ("{}\\".Contains(character))
            {
              if (!ignorable)
              {
                outList.Add(character);
              }
            }
            else if (character == "*")
            {
              ignorable = true;
            }
          }
          else if (!string.IsNullOrEmpty(word)) // \foo
          {
            curskip = 0;
            if (destinations.Contains(word))
            {
              ignorable = true;
            }
            else if (ignorable)
            {
            }
            else if (specialCharacters.ContainsKey(word))
            {
              outList.Add(specialCharacters[word]);
            }
            else if (word == "uc")
            {
              ucskip = Int32.Parse(arg);
            }
            else if (word == "u")
            {
              int c = Int32.Parse(arg);
              if (c < 0)
              {
                c += 0x10000;
              }
              outList.Add(Char.ConvertFromUtf32(c));
              curskip = ucskip;
            }
          }
          else if (!string.IsNullOrEmpty(hex)) // \'xx
          {
            if (curskip > 0)
            {
              curskip -= 1;
            }
            else if (!ignorable)
            {
              int c = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
              outList.Add(Char.ConvertFromUtf32(c));
            }
          }
          else if (!string.IsNullOrEmpty(tchar))
          {
            if (curskip > 0)
            {
              curskip -= 1;
            }
            else if (!ignorable)
            {
              outList.Add(tchar);
            }
          }
        }
      }
      else
      {
        // Didn't match the regex
        returnString = inputRtf;
      }

      returnString = string.Join(string.Empty, outList.ToArray());

      return returnString;
    }
  }
}