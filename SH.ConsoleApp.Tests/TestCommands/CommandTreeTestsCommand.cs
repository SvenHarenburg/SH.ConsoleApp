using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SH.ConsoleApp;

namespace SH.ConsoleApp.Tests.TestCommands
{

  [CommandGroup("CommandTreeTestsCommand", "")]
  public class CommandTreeTestsCommand
  {
    [Command(nameof(NoOptionsOrArguments), "")]
    public void NoOptionsOrArguments()
    {
    }

    [Command(nameof(WithOptionsNoArguments), "")]
    public void WithOptionsNoArguments(
      [CommandOption("option1", "")] string option1,
      [CommandOption("option2", "")] string option2
      )
    {
    }

    [Command(nameof(NoOptionsWithArguments), "")]
    public void NoOptionsWithArguments(
      [CommandArgument("argument1", "")] string argument1,
      [CommandArgument("argument2", "")] string argument2
      )
    {
    }

    [Command(nameof(WithOptionsAndArguments), "")]
    public void WithOptionsAndArguments(
      [CommandOption("option1", "")] string option1,
      [CommandOption("option2", "")] string option2,
      [CommandArgument("argument1", "")] string argument1,
      [CommandArgument("argument2", "")] string argument2
      )
    {
    }

    [Command("MultiplePotentialCommands", "")]
    public void MultiplePotentialCommands1(
      [CommandOption("option1", "")] string option1,
      [CommandArgument("argument1", "")] string argument1,
      [CommandOption("option2", "")] string option2 = ""
      )
    {
    }

    [Command("MultiplePotentialCommands", "")]
    public void MultiplePotentialCommands2(
      [CommandOption("option1", "")] string option1,
      [CommandArgument("argument1", "")] string argument1,
      [CommandOption("option2", "")] string option2 = "",
      [CommandOption("option3", "")] string option3 = "",
      [CommandArgument("argument2", "")] string argument2 = ""
      )
    {
    }

  }
}
