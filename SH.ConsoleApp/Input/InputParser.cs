using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Input
{

  /// <summary>
  /// 
  /// </summary>
  /// <remarks>
  /// SH 01.02.2021: Ideally the InputParser would just be parsing and would not try to match the input to a command in the availableCommands. 
  ///                Problem is, that an input can start with multiple single words where those words can be either a command or an option.
  ///                To decide wether a word is part of the command or is an option, the parser needs to know which commands are available.
  /// </remarks>
  internal class InputParser
  {
    private const string _argumentKeyPrefix = "--";
    private List<string> _availableCommands;

    public InputParser(List<string> availableCommands)
    {
      _availableCommands = availableCommands;
    }

    public ParsedInput ParseInput(string[] args)
    {
      // Sample Input:
      // "{Command} {Option1} {Option2:}:{Option2Value} {--Argument1}:{Value1} {--Argument2}:{Value2}"
      // The values can be optional.

      var result = new ParsedInput();
      var commandFound = false;

      // Saves the current command value since it can be composed of multiple strings.
      var currentCommandValue = "";

      // Will be set to true when the first option is processed.
      // The input is invalid if there are any items after the first option that are not an option-key or an option-value.
      var firstArgumentReached = false;

      var reader = new ArrayReader<string>(args);
      while (reader.Read())
      {

        // Command:
        if (!commandFound)
        {
          if (!string.IsNullOrWhiteSpace(currentCommandValue))
          {
            currentCommandValue += $" ";
          }
          currentCommandValue += reader.Item;

          if (_availableCommands.Contains(currentCommandValue))
          {
            result.Command = currentCommandValue;
            commandFound = true;
          }

          continue;
        }

        // Option or Argument:
        var split = reader.Item.Split(':');
        if (split.Length > 2)
        {
          throw new FormatException($"Invalid format. The key or value of the following option or argument contains a colon(:) - '{reader.Item}'. This is not currently supported.");
        }

        // Argument:
        if (StringIsArgumentKey(reader.Item))
        {
          firstArgumentReached = true;
          result.Arguments.Add(RemoveArgumentPrefix(split[0]), split.Length > 1 ? split[1] : "");
          continue;
        }

        // Option:
        // An option can only occur between the command and the first argument.
        // If the command has been processed and the first argument has been read, there can be no more options.
        if (!firstArgumentReached)
        {
          result.Options.Add(split[0], split.Length > 1 ? split[1] : "");
          continue;
        }

        throw new Exception("Invalid input");
      }

      if (string.IsNullOrWhiteSpace(result.Command))
      {
        throw new CommandNotFoundException($"Command not found: {currentCommandValue}", currentCommandValue);
      }

      return result;
    }

    private bool StringIsArgumentKey(string value)
    {
      return value.StartsWith(_argumentKeyPrefix);
    }

    private string RemoveArgumentPrefix(string value)
    {
      if (value.StartsWith(_argumentKeyPrefix))
      {
        return value.Substring(_argumentKeyPrefix.Length, value.Length - _argumentKeyPrefix.Length);
      }
      return value;

    }
  }
}

