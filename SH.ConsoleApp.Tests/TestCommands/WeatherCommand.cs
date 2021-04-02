using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SH.ConsoleApp;

namespace SH.ConsoleApp.Tests.TestCommands
{
  /// <summary>
  /// CommandGroup for use in Unit-Tests.
  /// </summary>
  [CommandGroup("weather", "Commands for the weather.")]
  public class WeatherCommand
  {
    [Command("today", "Gets the weather for today and prints it.")]
    public void Today(
      [CommandOption("location", "The location the weather should be printed for.")] string location,
      [CommandArgument("pretty", "Makes it pretty.")] bool pretty
      )
    {
    }

    [Command("tomorrow", "Gets the weather for tomorrow and prints it.")]
    public void Tomorrow(
      [CommandOption("location", "The location the weather should be printed for.")] string location,
      [CommandArgument("pretty", "Makes it pretty.")] bool pretty
      )
    {
    }

    [Command("weekly", "Gets the weather for the week and prints it.")]
    public void Weekly(
      [CommandOption("location", "The location the weather should be printed for.")] string location,
      [CommandArgument("pretty", "Makes it pretty.")] bool pretty
      )
    {
    }
  }
}
