using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.MyConsole
{
  public static class ConsoleHelper
  {
    public static void ClearLine()
    {
      try
      {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
      }
      catch { } // This will throw exception if run from Keil and Run Independant is not ticked
    }

    public static string GetCenteredString(string input, int totalLineWidth, char startChar, char endChar, char paddingChar)
    {
      int curWidth = input.Length;
      if (totalLineWidth > curWidth)
      {
        int rest = (int)(totalLineWidth - curWidth + 3);
        input = input.PadLeft(totalLineWidth - rest / 2, paddingChar).PadRight(totalLineWidth - 2, paddingChar);
      }
      return $"{startChar}{input}{endChar}";
    }

    public static string GetTextBoxWithHeaderCentered(string header, string text, int totalWidth, string newLine = null)
    {
      char spaceChar = ' ', lineChar = '_', startEndChar = '|';
      if (string.IsNullOrWhiteSpace(newLine))
      {
        newLine = "\n";
      }
      StringBuilder sb = new StringBuilder();

      sb.Append($"{GetCenteredString("", totalWidth, spaceChar, spaceChar, lineChar)}{newLine}");
      sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
      sb.Append($"{GetCenteredString(header, totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
      sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, lineChar)}{newLine}");

      if (!string.IsNullOrWhiteSpace(text))
      {
        sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
        string[] words = text.Split(new char[] { ' ' });
        List<string> line = new List<string>();
        int lineLength = 0;
        for (int i = 0; i < words.Length; i++)
        {
          string word = words[i];
          lineLength += word.Length + 1;
          if (totalWidth - 4 < lineLength)
          {
            sb.Append($"{GetCenteredString(string.Join(" ", line), totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
            line.Clear();
            lineLength = word.Length;
          }
          line.Add(word);
        }
        // Handle last line
        sb.Append($"{GetCenteredString(string.Join(" ", line), totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
        sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, lineChar)}{newLine}");
      }

      return sb.ToString();
    }

    public static string GetTextBoxCentered(string[] lines, int totalWidth, int numOfEmptyLinesAbove = 0, int numOfEmptyLinesBelow = 0, string newLine = null)
    {
      char spaceChar = ' ', lineChar = '_', startEndChar = '|';
      if (string.IsNullOrWhiteSpace(newLine))
      {
        newLine = "\n";
      }
      StringBuilder sb = new StringBuilder();

      sb.Append($"{GetCenteredString("", totalWidth, spaceChar, spaceChar, lineChar)}{newLine}");
      for (int i = 0; i < numOfEmptyLinesAbove; i++)
      {
        sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
      }

      if (null != lines)
      {
        foreach (string line in lines)
        {
          sb.Append($"{GetCenteredString(line, totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
        }
      }

      for (int i = 0; i < numOfEmptyLinesBelow; i++)
      {
        sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
      }

      sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, lineChar)}{newLine}");

      return sb.ToString();
    }
  }
}
