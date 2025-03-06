using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.MyConsole
{
  public enum eQuestionResult
  {
    Yes = ConsoleKey.Y,
    No = ConsoleKey.N,
    Cancel = ConsoleKey.C,
    Unknown = ConsoleKey.NoName
  }

  public enum eAnswerOptions
  {
    YesNo,
    YesNoCancel
  }

  public static class ConsoleHelper
  {
    public const ConsoleColor defaultColor = ConsoleColor.White;

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

      if (curWidth >= totalLineWidth - 2)
      {
        return $"{startChar}{input}{endChar}";
      }

      int totalPadding = totalLineWidth - 2 - curWidth; // -2 for borders
      int leftPadding = totalPadding / 2;
      int rightPadding = totalPadding - leftPadding;

      string centeredString = new string(paddingChar, leftPadding) + input + new string(paddingChar, rightPadding);

      return $"{startChar}{centeredString}{endChar}";
    }

    public static string GetTextBoxWithHeaderCentered(string header, string text, int totalWidth, string newLine = "\n")
    {
      const char spaceChar = ' ', lineChar = '_', startEndChar = '|';

      StringBuilder sb = new StringBuilder();

      sb.Append($"{GetCenteredString("", totalWidth, spaceChar, spaceChar, lineChar)}{newLine}");
      sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
      sb.Append($"{GetCenteredString(header, totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
      sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, lineChar)}{newLine}");

      if (!string.IsNullOrWhiteSpace(text))
      {
        sb.Append($"{GetCenteredString("", totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
        var words = text.Split(new char[] { ' ' }).ToList();
        List<string> line = new List<string>();
        int lineLength = 0;

        for (int i = 0; i < words.Count; i++)
        {
          string word = words[i];
          words.RemoveAt(i);

          string newWord = string.Empty;
          char prevC = (char)0;
          foreach (char c in word)
          {
            if ('\n' == c || '\r' == c)
            {
              if (!string.IsNullOrEmpty(newWord))
              {
                words.Insert(i++, newWord);
                newWord = string.Empty;
              }
              if (('\r' == c && '\n' == prevC) || ('\n' == c && '\r' == prevC))
              {
                continue;
              }
              words.Insert(i++, Environment.NewLine);
            }
            else
            {
              newWord += c;
            }
            prevC = c;
          }
          if (!string.IsNullOrEmpty(word))
          {
            words.Insert(i, newWord);
          }
        }

        foreach (string word in words)
        {
          lineLength += word.Length + 1;
          if (totalWidth - 4 < lineLength || Environment.NewLine == word)
          {
            sb.Append($"{GetCenteredString(string.Join(" ", line), totalWidth, startEndChar, startEndChar, spaceChar)}{newLine}");
            line.Clear();
            lineLength = word.Length;
          }
          if (Environment.NewLine != word)
          {
            line.Add(word);
          }
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

    public static string GetLoadingBar(double percentage, int length = 10)
    {
      int progressCharIndex = length * (int)percentage / 100;
      StringBuilder sb = new StringBuilder("[");

      for (int i = 0; i < length; i++)
      {
        sb.Append($"{(i < progressCharIndex ? "■" : " ")}");
      }
      sb.Append("]");
      return sb.ToString();
    }

    public static void ResetColor()
    {
      Console.ForegroundColor = defaultColor;
    }

    public static void ResetConsole()
    {
      Console.ResetColor();
    }

    public static void Print(string text, ConsoleColor color)
    {
      ConsoleColor currentColor = Console.ForegroundColor;
      Console.ForegroundColor = color;
      Console.Write(text);
      Console.ForegroundColor = currentColor;
    }

    public static void Print(string text)
    {
      Print(text, defaultColor);
    }

    public static void NewLine()
    {
      Console.WriteLine();
    }

    public static void PrintLine(string text, ConsoleColor color)
    {
      ConsoleColor currentColor = Console.ForegroundColor;
      Console.ForegroundColor = color;
      Console.WriteLine(text);
      Console.ForegroundColor = currentColor;
    }

    public static void PrintLine(string text)
    {
      PrintLine(text, defaultColor);
    }

    public static void PrintErrorLine(string text)
    {
      PrintLine(text, ConsoleColor.Red);
    }

    public static eQuestionResult AskYNQuestion(string question)
    {
      return AskQuestion(question, eAnswerOptions.YesNo);
    }

    public static eQuestionResult AskYNCQuestion(string question)
    {
      return AskQuestion(question, eAnswerOptions.YesNoCancel);
    }

    public static eQuestionResult AskQuestion(string question, eAnswerOptions answerOptions)
    {
      Print($"{question} ", ConsoleColor.Magenta);
      switch (answerOptions)
      {
        case eAnswerOptions.YesNo:
          Print("(Yes/No)> ", ConsoleColor.Cyan);
          return (eQuestionResult)ReadQuestionReplyKey(ConsoleKey.Y, ConsoleKey.N).Key;

        case eAnswerOptions.YesNoCancel:
          Print("(Yes/No/Cancel)> ", ConsoleColor.Cyan);
          return (eQuestionResult)ReadQuestionReplyKey(ConsoleKey.Y, ConsoleKey.N, ConsoleKey.C).Key;

        default:
          break;
      }
      return eQuestionResult.Unknown;
    }

    public static ConsoleKeyInfo ReadQuestionReplyKey(params ConsoleKey[] keys)
    {
      ConsoleKeyInfo key;
      do
      {
        key = Console.ReadKey();
        if (!keys.Contains(key.Key))
        {
          Console.Write("\b");
          continue;
        }
        Console.WriteLine();
        break;
      } while (true);
      return key;
    }
  }
}
