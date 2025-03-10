﻿using AMD.Util.CLA;
using AMD.Util.Data;
using AMD.Util.Extensions;
using AMD.Util.Log;
using AMD.Util.MyConsole;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BinToData
{
  internal class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      LogWriter.Instance.Enable = false;

      CommandHandler ch = new CommandHandler(CommandStatic.CommandMap);

      if (CommandStatic.IsHelpCommand(args))
      {
        PrintHelp(ch);
        return;
      }

      if (ch.Execute(args))
      {
        foreach (string errMsg in ch.ErrorMessages)
        {
          Out.Error(errMsg);
        }
        GenerateData();
      }
      else
      {
        PrintHelp(ch);
        var formattedArgs = args.Select(arg => arg.Contains(" ") ? $"\"{arg}\"" : arg);
        Out.Error($"Error parsing CLA: \"{string.Join(" ", formattedArgs)}\"");
        foreach (string errMsg in ch.ErrorMessages)
        {
          Out.Error(errMsg);
        }
        Console.ReadKey();
      }
    }

    private static void GenerateData()
    {
      InputParameters ip = CommandStatic.InputParameters;

      byte[] data = File.ReadAllBytes(ip.InputPath);

      string formattedData = null;

      switch (ip.Format)
      {
        case eFormatStyle.NoFormat:
          formattedData = data.GetHexString(ip.Separator, ip.Prefix, ip.DataLineWidth);
          break;
        case eFormatStyle.MemoryByte:
          formattedData = StringFormatHelper.GetFormattedMemoryStringByteSeparated(0, data.GetNullableUIntArray(), ip.DataLineWidth);
          break;
        case eFormatStyle.MemoryWord:
          formattedData = StringFormatHelper.GetFormattedMemoryString(0, data.GetNullableUIntArray());
          break;
        case eFormatStyle.C:
          formattedData = GetCStyleArray(ip.ArrayName, data.GetHexString(ip.Separator, ip.Prefix, ip.DataLineWidth), ip.TabSize);
          break;
        case eFormatStyle.CSharp:
        default:
          formattedData = GetCSharpStyleArray(ip.ArrayName, data.GetHexString(ip.Separator, ip.Prefix, ip.DataLineWidth), ip.TabSize);
          break;
      }

      if (ip.Output.HasFlag(eOutput.File))
      {
        try
        {
          string dirPath = Path.GetDirectoryName(ip.OutputPath);
          if (!Directory.Exists(dirPath))
          {
            Directory.CreateDirectory(dirPath);
          }
          File.WriteAllText(ip.OutputPath, formattedData);
        }
        catch (Exception ex)
        {
          Out.Error($"Error writing to file: {ex.Message}");
        }
      }

      if (ip.Output.HasFlag(eOutput.Clipboard))
      {
        try
        {
          Clipboard.SetText(formattedData);
          Out.Print("Data successfully copied...");
          //Clipboard.SetDataObject(formattedData);
          //Clipboard.SetData(DataFormats.Text, formattedData);
        }
        catch (Exception ex)
        {
          Out.Error($"Error sending to Clipboard: {ex.Message}");
          throw;
        }
      }

      if (ip.Output.HasFlag(eOutput.Console))
      {
        Out.Print(formattedData);
      }
    }

    private static string GetCSharpStyleArray(string arrayName, string formattedData, int tabSize)
    {
      return $"byte[] {(arrayName ?? "<NAME>")} = new byte[]\n{GetArrayBody(formattedData, tabSize)}";
    }

    private static string GetCStyleArray(string arrayName, string formattedData, int tabSize)
    {
      return $"uint8_t {(arrayName ?? "<NAME>")}[] = \n{GetArrayBody(formattedData, tabSize)}";
    }

    private static string GetArrayBody(string formattedData, int tabSize)
    {
      string[] lines = formattedData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
      string indentStr = new string(' ', tabSize);
      StringBuilder sb = new StringBuilder("{\n");
      foreach (string line in lines)
      {
        sb.AppendLine($"{indentStr}{line}");
      }
      sb.AppendLine("};");
      return sb.ToString();
    }

    private static void PrintHelp(CommandHandler ch)
    {
      ConsoleHelper.Print(CommandStatic.Banner, ConsoleColor.Blue);
      ch.PrintHelp(CommandStatic.Description);
    }
  }
}
