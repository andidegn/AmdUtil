using AMD.Util.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      return VCPCodeStandard.VCPNameLUT[(byte)code];
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
                    list.Add(new VCPCode(byte.Parse(sb.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo)));
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
                      list.Add(new VCPCode(byte.Parse(sb.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo)));
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
                      list.Last().Presets.Add(byte.Parse(sb.ToString(), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
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
          bool endOfVcpList = false;
          String name, code;
          StringBuilder sb = new StringBuilder();
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
                if (2 == ++depth) // VCP code in sb
                {
                  code = sb.ToString().Trim(' ', ',', '.');
                  if (2 == code.Length && code.IsHexNumber())
                  {
                    vCPCode = list.Get((eVCPCode)byte.Parse(code, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                    sb.Clear();
                  }
                }
                break;

              case ')':
                if (1 == --depth)
                {
                  name = sb.ToString().Trim(' ', ',', '.');
                  if (!String.IsNullOrWhiteSpace(name) && null != vCPCode)
                  {
                    vCPCode.Name = name;
                    sb.Clear();
                  }
                }
                else if (1 > depth)
                {
                  endOfVcpList = true;
                }
                break;

              default:
                sb.Append(c);
                break;
            }
          }
        }
      }
    }
  }
}
