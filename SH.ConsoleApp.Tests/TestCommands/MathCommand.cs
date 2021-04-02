using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Tests.TestCommands
{
  /// <summary>
  /// CommandGroup for use in Unit-Tests.
  /// </summary>
  [CommandGroup("math", "Commands for math calculations.")]
  public class MathCommand
  {
    [Command("add", "Adds up two values.")]
    public void Add(
      [CommandOption("value1", "First value of the operation.")] int value1,
      [CommandOption("value2", "Second value of the operation.")] int value2
      )
    {
    }

    [Command("subtract", "Subtracts a value from another.")]
    public void Subtract(
      [CommandOption("value1", "First value of the operation.")] int value1,
      [CommandOption("value2", "Second value of the operation.")] int value2
      )
    {
    }
  }
}
