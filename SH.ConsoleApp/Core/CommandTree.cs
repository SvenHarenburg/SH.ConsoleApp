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

              // Options:
              var optionMatch = MatchParameters(
                command.Options.Cast<CommandParameter>().ToList(),
                options);
              if (optionMatch.MissesRequiredParameters) continue;
              if (optionMatch.MoreParametersThanAvailable) continue;

              // Arguments:
              var argumentMatch = MatchParameters(
                command.Arguments.Cast<CommandParameter>().ToList(),
                arguments);
              if (argumentMatch.MissesRequiredParameters) continue;
              if (argumentMatch.MoreParametersThanAvailable) continue;

              // Found a potential match, add it to the list:
              potentialMatches.Add(new CommandMatch()
              {
                Command = command,
                NumberOfMissingOptionalOptions = optionMatch.NumberOfMissingOptionalParameters,
                NumberOfMissingOptionalArguments = argumentMatch.NumberOfMissingOptionalParameters
              });
            }
          }
        }
      }
            
      var bestMatchingCommand = potentialMatches.OrderByDescending(q => q.NumberOfMissingOptionalOptions + q.NumberOfMissingOptionalArguments).FirstOrDefault();
      return bestMatchingCommand;
    }

    private ParameterMatch MatchParameters(List<CommandParameter> commandParameters, List<string> inputParameters)
    {
      var result = new ParameterMatch();

      var argumentDictionary = commandParameters.ToDictionary(q => q.Name, q => q, StringComparer.OrdinalIgnoreCase);
      var fullJoin = inputParameters.FullJoin(commandParameters.Select(q => q.Name),
        x => x.ToLower(),
        y => y.ToLower(),
        x => new { inputParameter = x, commandParameter = "" },
        y => new { inputParameter = "", commandParameter = y },
        (x, y) => new { inputParameter = x, commandParameter = y }
        );

      result.MissesRequiredParameters = fullJoin.Any(
        q => string.IsNullOrWhiteSpace(q.inputParameter)
        && !string.IsNullOrWhiteSpace(q.commandParameter)
        && !argumentDictionary[q.commandParameter].ParameterInfo.IsOptional);

      result.MoreParametersThanAvailable = fullJoin.Any(
        q => string.IsNullOrWhiteSpace(q.commandParameter));

      // Required to find the best matching command.
      // If one has two missing optional parameters and another has only one missing, than the latter matches better.
      result.NumberOfMissingOptionalParameters = fullJoin.Count(
        q => string.IsNullOrWhiteSpace(q.inputParameter)
        && !string.IsNullOrWhiteSpace(q.commandParameter)
        && argumentDictionary[q.commandParameter].ParameterInfo.IsOptional);

      return result;
    }
  }
}
