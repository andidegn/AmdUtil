using AMD.Util.Display.DDCCI.MCCSCodeStandard;
using AMD.Util.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AMD.Util.Display.DDCCI.Util
{
  public static class DDCHelper
  {

    /// <summary>
    /// Returns the VCP name as stated in the VESA MCCS Standard Version 2.2a
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static string GetVCPName(eVCPCode code)
    {
      return VCPCodeStandard.Instance.GetName(code);
    }

    /// <summary>
    /// Formats a capability string to an indented multiline format
    /// </summary>
    /// <param name="capabilityString"></param>
    /// <returns></returns>
    public static string GetFormattedCapabilityString(string capabilityString)
    {
      StringBuilder sbcsf = new StringBuilder();
      //string cssub = capabilityString.Substring(0, index); // (prot(monitor)type(lcd)SAMSUNGcmds(01 02 03 07 0C E3 F3)
      int indentCnt = 0, indentSize = 2;
      char lastChar = (char)0;
      foreach (char c in capabilityString)
      {
        switch (c)
        {
          case '(':
            if (0 == indentCnt && (char)0 == lastChar)
            {
              sbcsf.AppendLine(c.ToString());
              sbcsf.Append("".PadLeft((indentCnt + 1) * indentSize));
            }
            else if (2 > indentCnt)
            {
              sbcsf.Append($"{Environment.NewLine}{c.ToString().PadLeft(indentCnt * indentSize + 1)}{Environment.NewLine}{"".PadLeft((indentCnt + 1) * indentSize)}");
            }
            else
            {
              sbcsf.Append(c);
            }
            indentCnt++;
            break;

          case ')':
            indentCnt--;
            if (0 > indentCnt)
            {
              indentCnt = 0;
              string s = sbcsf.ToString();
            }
            if (2 > indentCnt)
            {
              sbcsf.Append($"{Environment.NewLine}{c.ToString().PadLeft(indentCnt * indentSize + 1)}{Environment.NewLine}{"".PadLeft(indentCnt * indentSize)}");
            }
            else
            {
              sbcsf.Append(c);
            }
            break;

          case ' ':
            //if (')' != lastChar)
            {
              if (3 > indentCnt)
              {
                sbcsf.Append($"{Environment.NewLine}{"".PadLeft(indentCnt * indentSize)}");
              }
              else
              {
                sbcsf.Append(c);
              }
            }
            break;

          default:
            sbcsf.Append(c);
            break;
        }
        lastChar = c;
      }
      return sbcsf.ToString();
    }

    /// <summary>
    /// Populates a provided VCPCodeList with the codes found in the capabilityString
    /// </summary>
    /// <param name="capabilityString"></param>
    /// <param name="list"></param>
    public static void PopulateVcpCodes(string capabilityString, VCPCodeList list)
    {
      if (null == list)
      {
        list = new VCPCodeList();
      }
      else
      {
        list.Clear();
      }
      if (!string.IsNullOrEmpty(capabilityString))
      {
        // (prot(monitor)type(LCD)model(RTK)cmds(01 02 03 07 0C E3 F3)vcp(02 04 10 12 14(04 05 06 0B) 16 18 1A 52 60(11 12 0F 10) AC AE B2 B6 C0 C6 C8 C9 CA D6(01 02 05) DF E0(00 01 02 FF) E4(00 01 02 03 04) E6 E7 E8 EA F0 F1 F2 F3 FD FE)mswhql(1)asset_eep(40)mccs_ver(2.2))vcpname(E0 (ECDIS (00 Day 01 Dusk 02 Night FF Off)),E4 (External Baud Rate (00 19200 01 9600 02 115200 03 460800 04 921600)),E6 (USB Link Select),E7 (PiP Set and Activate),E8 (PbP Set and Activate),EA (ColorMap Download),F0 (Temperature),F1 (Max Temperature),F2 (Min Temperature),F3 (Backlight runtime),FD (Software Version),FE (Serial number)))
        int index = capabilityString.IndexOf("vcp", StringComparison.OrdinalIgnoreCase);
        if (0 < index)
        {
          int depth = 0;
          bool endOfVcpList = false;
          StringBuilder sb = new StringBuilder(2);

          foreach (char c in capabilityString.Skip(index + 3))
          {
            if (endOfVcpList)
            {
              break;
            }
            switch (c)
            {
              case '(':
                depth++;
                break;

              case ')':
                if (1 > --depth)
                {
                  if (0 < sb.Length)
                  {
                    VCPCode lastCode = new VCPCode(byte.Parse(sb.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                    lastCode.AddPresets(VCPCodeStandard.GetStandardPresets(lastCode.Code));
                    list.Add(lastCode);
                  }
                  endOfVcpList = true;
                }
                break;

              default:
                if (c.IsHexNumber())
                {
                  if (1 == depth)
                  {
                    sb.Append(c);
                    if (sb.Length == 1)
                    {
                      continue;
                    }

                    if (0 < sb.Length)
                    {
                      VCPCode newCode = new VCPCode(byte.Parse(sb.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                      list.Add(newCode);
                      sb.Clear();
                    }
                  }
                  else if (2 == depth)
                  {
                    sb.Append(c);
                    if (sb.Length == 1)
                    {
                      continue;
                    }

                    if (0 < sb.Length)
                    {
                      uint presetValue = uint.Parse(sb.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                      VCPCode latestCode = list.Last();
                      VCPCodePreset preset = (from p in VCPCodeStandard.GetStandardPresets(latestCode.Code)
                                              where p.Value == presetValue
                                              select p).DefaultIfEmpty(null).SingleOrDefault();

                      if (null != preset)
                      {
                        latestCode.AddPreset(preset);
                      }
                      else
                      {
                        latestCode.AddPreset("", presetValue);
                      }
                      sb.Clear();
                    }
                  }
                }
                break;
            }
          }
        }
      }
    }

    public static void PopulateVcpCodeNames(String capabilityString, VCPCodeList list)
    {
      if (!String.IsNullOrEmpty(capabilityString) && null != list)
      {

        int index = capabilityString.IndexOf("vcpname", StringComparison.OrdinalIgnoreCase);
        if (0 < index)
        {

          int depth = 0;
          bool endOfVcpList = false, hasPresets = false; ;
          String name, code;
          StringBuilder sb = new StringBuilder();
          StringBuilder sbPresets = new StringBuilder();
          VCPCode vCPCode = null;

          foreach (char c in capabilityString.Skip(index + 7))
          {
            if (endOfVcpList)
            {
              break;
            }
            switch (c)
            {// e0(color temperature)
              case '(':
                depth++;
                if (2 == depth) // VCP code in sb
                {
                  code = sb.ToString().Trim(' ', ',', '.');
                  if (2 == code.Length && code.IsHexNumber())
                  {
                    vCPCode = list.Get((eVCPCode)byte.Parse(code, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                    sb.Clear();
                  }
                }
                else if (3 == depth)
                {
                  hasPresets = true;
                }
                break;

              case ')':
                depth--;
                if (1 == depth)
                {
                  name = sb.ToString().Trim(' ', ',', '.');
                  if (!String.IsNullOrWhiteSpace(name) && null != vCPCode)
                  {
                    vCPCode.Name = name;
                    sb.Clear();
                  }
                }
                else if (2 == depth)
                {
                  hasPresets = false;
                  string[] parts = sbPresets.ToString().Split();
                  sbPresets.Clear();
                  uint tmpValue = 0;
                  string tmpStr;
                  for (int i = 0; i < parts.Length - 1; i++)
                  {
                    tmpStr = parts[i];
                    if (uint.TryParse(tmpStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out tmpValue))
                    {
                      vCPCode.AddPreset(parts[++i], tmpValue);
                    }
                  }
                }
                else if (1 > depth)
                {
                  endOfVcpList = true;
                }
                break;

              default:
                if (hasPresets)
                {
                  sbPresets.Append(c);
                }
                else
                {
                  sb.Append(c);
                }
                break;
            }
          }
        }
      }
    }
  }
}
