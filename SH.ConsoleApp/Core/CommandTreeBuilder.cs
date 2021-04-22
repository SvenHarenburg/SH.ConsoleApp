using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  /// <summary>
  /// Provides functionality for building a <see cref="CommandTree"/> by searching an <see cref="Assembly"/>.
  /// </summary>
  internal class CommandTreeBuilder : ICommandTreeBuilder
  {
    private readonly Assembly[] _assemblies;
    private CommandTree _commandTree;

    /// <summary>
    /// Initializes a new instance of <see cref="CommandTreeBuilder"/>.
    /// </summary>
    /// <param name="commandGroupAssemblyProvider">An instance of <see cref="ICommandGroupAssemblyProvider"/> 
    /// which will be used to get the Assemblies to be search for <see cref="CommandGroup"/>s and <see cref="Command"/>s.</param>
    public CommandTreeBuilder(ICommandGroupAssemblyProvider commandGroupAssemblyProvider)
    {
      if (commandGroupAssemblyProvider == null) throw new ArgumentNullException(nameof(commandGroupAssemblyProvider));

      _assemblies = commandGroupAssemblyProvider.GetAssemblies();
    }

    /// <summary>
    /// Builds a <see cref="CommandTree"/>-object by searching through the provided Assembly.
    /// Will not fill Options and Arguments of <see cref="Command"/>-objects since those are only needed for the commands a user wants to execute.
    /// </summary>
    /// <returns></returns>
    public CommandTree BuildBaseTree()
    {
      _commandTree = new CommandTree();

      // Could call FillCommands for each CommandGroup from within FillCommandGroups()
      // but I don't like chaining calls in that way. It is easier to understand
      // what happens here if both get called from this function.
      FillCommandGroups();
      FillCommands();

      return _commandTree;
    }

    /// <summary>
    /// Searches the <see cref="Assembly"/> for CommandGroups and fills the <see cref="CommandTree.CommandGroups"/>-List.
    /// </summary>
    private void FillCommandGroups()
    {
      var commandGroupTypes = GetCommandGroupTypes();

      foreach (var type in commandGroupTypes)
      {
        var groupAttribute = type.GetCustomAttribute<CommandGroupAttribute>();

        var commandGroup = new CommandGroup()
        {
          CommandGroupType = type,
          CommandGroupAttribute = groupAttribute
        };
        _commandTree.CommandGroups.Add(commandGroup);
      }
    }

    /// <summary>
    /// Searches the <see cref="Type"/>s of the <see cref="CommandGroup"/>s in the <see cref="CommandTree.CommandGroups"/>-List for Commands
    /// and fills their <see cref="CommandGroup.Commands"/>-List.
    /// </summary>
    private void FillCommands()
    {
      foreach (var commandGroup in _commandTree.CommandGroups)
      {
        var commandMethods = GetCommandMethods(commandGroup.CommandGroupType);

        foreach (var method in commandMethods)
        {
          var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
          var command = new Command()
          {
            CommandMethodInfo = method,
            CommandAttribute = commandAttribute,
            CommandGroup = commandGroup
          };
          commandGroup.Commands.Add(command);
        }

        // Add help-command for every CommandGroup that is not the Help-CommandGroup:
        if (commandGroup.CommandGroupType != typeof(Commands.HelpCommand))
        {
          var helpCommand = new Command();
          helpCommand.CommandMethodInfo = typeof(Commands.HelpCommand).GetMethod(nameof(Commands.HelpCommand.CommandGroupHelp));
          helpCommand.CommandAttribute = new CommandAttribute("help", "Shows help for the CommandGroup.");
          helpCommand.CommandGroup = commandGroup;
          commandGroup.Commands.Add(helpCommand);
        }
      }
    }

    public static void FillOptionsAndArguments(Command command)
    {
      if (command == null) throw new ArgumentNullException(nameof(command));

      FillOptions(command);
      FillArguments(command);
    }

    private static void FillOptions(Command command)
    {
      if (command == null) throw new ArgumentNullException(nameof(command));

      var optionParameters = GetCommandOptions(command.CommandMethodInfo);
      command.Options = optionParameters.Select(q => new Option()
      {
        ParameterInfo = q,
        CommandOptionAttribute = q.GetCustomAttribute<CommandOptionAttribute>()
      }).ToList();
    }

    private static void FillArguments(Command command)
    {
      if (command == null) throw new ArgumentNullException(nameof(command));

      var optionParameters = GetCommandArguments(command.CommandMethodInfo);
      command.Arguments = optionParameters.Select(q => new Argument()
      {
        ParameterInfo = q,
        CommandArgumentAttribute = q.GetCustomAttribute<CommandArgumentAttribute>()
      }).ToList();
    }

    /// <summary>
    /// Searches the <see cref="Assembly"/>-objects for <see cref="Type"/>s that represent a <see cref="CommandGroup"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{Type}"/> containing <see cref="Type"/>s that represent a <see cref="CommandGroup"/>.</returns>
    /// <remarks>A <see cref="Type"/> representing a <see cref="CommandGroup"/> is currently defined by having the <see cref="CommandGroupAttribute"/> applied to it.</remarks>
    private IEnumerable<Type> GetCommandGroupTypes()
    {
      return _assemblies
        .SelectMany(q => q.GetTypes())
        .Where(q => q.GetCustomAttribute<CommandGroupAttribute>() != null);
    }

    /// <summary>
    /// Searches a <see cref="Type"/> for all methods that represent a <see cref="Command"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that should be searched.</param>
    /// <returns>An <see cref="IEnumerable{MethodInfo}"/> containing all <see cref="MethodInfo"/>s that represent a <see cref="Command"/>.</returns>
    /// <remarks>A method representing a <see cref="Command"/> is currently defined by having the <see cref="CommandAttribute"/> applied to it.</remarks>
    private IEnumerable<MethodInfo> GetCommandMethods(Type type)
    {
      if (type == null) throw new ArgumentNullException(nameof(type));
      return type
          .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
          .Where(q => q.GetCustomAttribute<CommandAttribute>() != null);
    }

    /// <summary>
    /// Searches a <see cref="MethodInfo"/>-object for parameters that represent a <see cref="CommandOption"/>.
    /// </summary>
    /// <param name="commandMethod">The <see cref="MethodInfo"/> that should be searched.</param>
    /// <returns>An <see cref="IEnumerable{ParameterInfo}"/> containing all <see cref="ParameterInfo"/>s that represent a <see cref="CommandOption"/>.</returns>
    /// <remarks>A parameter representing a <see cref="CommandOption"/> is currently being defined by having the <see cref="CommandOptionAttribute"/> applied to it.</remarks>
    private static IEnumerable<ParameterInfo> GetCommandOptions(MethodInfo commandMethod)
    {
      if (commandMethod == null) throw new ArgumentNullException(nameof(commandMethod));
      return commandMethod.GetParameters()
        .Where(q => q.GetCustomAttribute<CommandOptionAttribute>() != null);
    }

    /// <summary>
    /// Searches a <see cref="MethodInfo"/>-object for parameters that represent a <see cref="CommandOption"/>.
    /// </summary>
    /// <param name="commandMethod">The <see cref="MethodInfo"/> that should be searched.</param>
    /// <returns>An <see cref="IEnumerable{ParameterInfo}"/> containing all <see cref="ParameterInfo"/>s that represent a <see cref="CommandOption"/>.</returns>
    /// <remarks>A parameter representing a <see cref="CommandOption"/> is currently being defined by having the <see cref="CommandOptionAttribute"/> applied to it.</remarks>
    private static IEnumerable<ParameterInfo> GetCommandArguments(MethodInfo commandMethod)
    {
      if (commandMethod == null) throw new ArgumentNullException(nameof(commandMethod));
      return commandMethod.GetParameters()
        .Where(q => q.GetCustomAttribute<CommandArgumentAttribute>() != null);
    }
  }
}
