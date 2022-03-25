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
    public static String GetVCPName(eVCPCode code)
    {
      return VCPCodeStandard.Instance.GetName(code);
    }

    /// <summary>
    /// Populates a provided VCPCodeList with the codes found in the capabilityString
    /// </summary>
    /// <param name="capabilityString"></param>
    /// <param name="list"></param>
    public static void PopulateVcpCodes(String capabilityString, VCPCodeList list)
    {
      if (null == list)
      {
        list = new VCPCodeList();
      }
      else
      {
        list.Clear();
      }
      if (!String.IsNullOrEmpty(capabilityString))
      {

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
                sb.Append(c);
                break;
            }
          }
        }
      }
    }
  }
}
