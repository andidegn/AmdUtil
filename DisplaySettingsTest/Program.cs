using AMD.Util.Extensions;
using AMD.Util.MyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplaySettingsTest
{
  internal class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Available Displays:");
      int idx = 0;

      var displayList = DisplaySettings.ListDisplays().ToList();

      foreach (var item in displayList)
      {
        Console.WriteLine(item);
      }

      //foreach (Monitor mon in MonitorList.Instance.List)
      //{
      //  Console.WriteLine($"{idx++}: {mon.Name} - {mon.mInfo.szDeviceName}");
      //}

      Console.WriteLine("\nEnter the display device number to modify:");
      string displayId = Console.ReadLine();
      string displayName = null;
      if (int.TryParse(displayId, out int id) && id < displayList.Count)
      {
        displayName = displayList[id];
      }
      else
      {
        Console.WriteLine("Error in input");
        return;
      }
      //if (eQuestionResult.Yes != ConsoleHelper.AskYNQuestion($"Is \"{displayName}\" the correct monitor?"))
      //{
      //  return;
      //}

      Console.WriteLine("\nEnter the desired width:");
      int width = int.Parse(Console.ReadLine());

      Console.WriteLine("\nEnter the desired height:");
      int height = int.Parse(Console.ReadLine());

      Console.WriteLine("\nEnter the desired refresh rate (Hz):");
      int frequency = int.Parse(Console.ReadLine());

      bool success = DisplaySettings.SetDisplayResolution(displayName, width, height, frequency);
      Console.WriteLine(success ? "Resolution changed successfully!" : "Failed to change resolution.");
      Console.ReadKey();
    }
  }
}
