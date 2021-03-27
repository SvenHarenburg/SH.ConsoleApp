using SH.ConsoleApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Commands
{
  [CommandGroup("help", "General help command.")]
  internal class HelpCommand
  {
    private readonly CommandTree _commandTree;

    public HelpCommand(CommandTree commandTree)
    {
      _commandTree = commandTree ?? throw new ArgumentNullException(nameof(commandTree));
    }

    [Command("", "description")]
    public void Execute()
    {
      var appName = Assembly.GetEntryAssembly().GetName().Name;
      Console.WriteLine($"Available commands:");

      var dictionary = _commandTree.CommandGroups.ToDictionary(key => key.Name, value => value.CommandGroupAttribute.Description);
      ConsoleUtilities.WriteDictionary(dictionary, 5);

      Console.WriteLine();
      Console.WriteLine($"Run \"{appName} [command] help\" to get help for a specific command.");
    }

    public void CommandGroupHelp(CommandGroup commandGroup)
    {
      if (commandGroup == null) throw new ArgumentNullException(nameof(commandGroup));
      var appName = Assembly.GetEntryAssembly().GetName().Name;

      Console.WriteLine($"Command group: {commandGroup.Name}");
      Console.WriteLine($"Available commands:");

      // Print all the commands except the help command itself.
      // Use the MethodHandle to compare.
      var currentMethodHandle = MethodBase.GetCurrentMethod().MethodHandle;
      var dictionary = commandGroup.Commands
        .Where(q => !q.CommandMethodInfo.MethodHandle.Equals(currentMethodHandle))
        .ToDictionary(key => key.CommandAttribute.Name, value => value.CommandAttribute.Description);
      ConsoleUtilities.WriteDictionary(dictionary, 5);

      Console.WriteLine();
      //Console.WriteLine($"Run \"{appName} [command] help\" to get help for a specific command.");
    }



    [Command("test1", "description")]
    public void Execute1() { }


    [Command("test2", "description")]
    public void Execute2() { }


    [Command("test3", "description")]
    public void Execute3() { }
  }


}
