using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class CommandTree
  {
    public List<CommandGroup> CommandGroups { get; } = new List<CommandGroup>();

    public CommandMatch FindCommand(string commandName, List<string> options, List<string> arguments)
    {
      var potentialMatches = new List<CommandMatch>();

      foreach (var group in CommandGroups)
      {
        // Find CommandGroup by name:
        var groupName = group.CommandGroupAttribute.Name;
        if (commandName.StartsWith(groupName, StringComparison.OrdinalIgnoreCase))
        {

          // Find Command by name:
          foreach (var command in group.Commands)
          {
            var combinedCommandName = $"{groupName} {command.CommandAttribute.Name}".Trim();
            if (string.Equals(combinedCommandName, commandName, StringComparison.OrdinalIgnoreCase))
            {
              // Since the name fits, we now need to fill options and arguments to find potential matches:
              CommandTreeBuilder.FillOptionsAndArguments(command);


              // Find Command by options and arguments:

              /*
               * Rules:
               * - Command must match all options
               * - Command must match all arguments
               * - Command can have additional optional and arguments
               */

              // -------------------------------
              // Options:
              // -------------------------------
              var optionDictionary = command.Options.ToDictionary(q => q.CommandOptionAttribute.Name, q => q, StringComparer.OrdinalIgnoreCase);
              var optionFullJoin = options.FullJoin(command.Options.Select(q => q.CommandOptionAttribute.Name),
                x => x.ToLower(),
                y => y.ToLower(),
                x => new { InputOption = x, CommandOption = "" },
                y => new { InputOption = "", CommandOption = y },
                (x, y) => new { InputOption = x, CommandOption = y }
                );

              var missesRequiredOptions = optionFullJoin.Any(
                q => string.IsNullOrWhiteSpace(q.InputOption)
                && !string.IsNullOrWhiteSpace(q.CommandOption)
                && !optionDictionary[q.CommandOption].ParameterInfo.IsOptional);
              if (missesRequiredOptions) continue;

              var moreOptionsThanAvailable = optionFullJoin.Any(
                q => string.IsNullOrWhiteSpace(q.CommandOption));
              if (moreOptionsThanAvailable) continue;

              // Required to find the best matching command.
              // If one has two missing optional options and another has only one missing, than the other matches better.
              var numberOfMissingOptionalOptions = optionFullJoin.Count(
                q => string.IsNullOrWhiteSpace(q.InputOption)
                && !string.IsNullOrWhiteSpace(q.CommandOption)
                && optionDictionary[q.CommandOption].ParameterInfo.IsOptional);

              // -------------------------------
              // Arguments:
              // -------------------------------
              var argumentDictionary = command.Arguments.ToDictionary(q => q.CommandArgumentAttribute.Name, q => q, StringComparer.OrdinalIgnoreCase);
              var argumentFullJoin = arguments.FullJoin(command.Arguments.Select(q => q.CommandArgumentAttribute.Name),
                x => x.ToLower(),
                y => y.ToLower(),
                x => new { InputArgument = x, CommandArgument = "" },
                y => new { InputArgument = "", CommandArgument = y },
                (x, y) => new { InputArgument = x, CommandArgument = y }
                );

              var missesRequiredArguments = argumentFullJoin.Any(
                q => string.IsNullOrWhiteSpace(q.InputArgument)
                && !string.IsNullOrWhiteSpace(q.CommandArgument)
                && !argumentDictionary[q.CommandArgument].ParameterInfo.IsOptional);
              if (missesRequiredArguments) continue;

              var moreArgumentsThanAvailable = argumentFullJoin.Any(
                q => string.IsNullOrWhiteSpace(q.CommandArgument));
              if (moreArgumentsThanAvailable) continue;

              // Required to find the best matching command.
              // If one has two missing Argumental Arguments and another has only one missing, than the other matches better.
              var numberOfMissingOptionalArguments = argumentFullJoin.Count(
                q => string.IsNullOrWhiteSpace(q.InputArgument)
                && !string.IsNullOrWhiteSpace(q.CommandArgument)
                && argumentDictionary[q.CommandArgument].ParameterInfo.IsOptional);
              if (missesRequiredArguments) continue;

              potentialMatches.Add(new CommandMatch()
              {
                Command = command,
                NumberOfMissingOptionalOptions = numberOfMissingOptionalOptions,
                NumberOfMissingOptionalArguments = numberOfMissingOptionalArguments
              });
            }
          }
        }
      }

      var bestMatchingCommand = potentialMatches.OrderByDescending(q => q.NumberOfMissingOptionalOptions + q.NumberOfMissingOptionalArguments).FirstOrDefault();
      return bestMatchingCommand;
    }
  }
}
