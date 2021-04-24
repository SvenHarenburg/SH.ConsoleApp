using System;
using System.Collections.Generic;

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
  internal class InputParser : IInputParser
  {
    private const string ArgumentKeyPrefix = "--";
    private const char KeyValueDelimiter = ':';

    private Dictionary<char, string> _escapeCharacters = new Dictionary<char, string>()
    {
      {':',"[--colon--]" }
    };

    public ParsedInput ParseInput(string[] args, List<string> availableCommands)
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

          if (availableCommands.Contains(currentCommandValue))
          {
            result.Command = currentCommandValue;
            commandFound = true;
          }

          continue;
        }

        // Option or Argument:

        // To support colons and other special characters being part of the value of an option or argument, they can be put into quotes.
        // Example: duration:"01:02:03" 
        // Note: The colon is a special character that divides the key and value of an option or argument.
        // 
        // This should result in an option with key duration and a value of 01:02:03.
        // For this to work the Parser will replace colons inside of quotes with a special character sequence
        // and reverse that process when putting the value into the ParsedInput-result.

        var escapedItem = EscapeInputValue(reader.Item);
        var split = escapedItem.Split(KeyValueDelimiter);
        if (split.Length > 2)
        {
          throw new FormatException($"Invalid format. The key or value of the following option or argument contains a colon(:) - '{reader.Item}'. This is not currently supported.");
        }

        // Argument:
        if (StringIsArgumentKey(escapedItem))
        {
          firstArgumentReached = true;
          result.Arguments.Add(RemoveArgumentPrefix(split[0]), split.Length > 1 ? UnescapeInputValue(split[1]) : "");
          continue;
        }

        // Option:
        // An option can only occur between the command and the first argument.
        // If the command has been processed and the first argument has been read, there can be no more options.
        if (!firstArgumentReached)
        {
          result.Options.Add(split[0], split.Length > 1 ? UnescapeInputValue(split[1]) : "");
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
      return value.StartsWith(ArgumentKeyPrefix);
    }

    private string RemoveArgumentPrefix(string value)
    {
      if (value.StartsWith(ArgumentKeyPrefix))
      {
        return value.Substring(ArgumentKeyPrefix.Length, value.Length - ArgumentKeyPrefix.Length);
      }
      return value;
    }

    private string EscapeInputValue(string value)
    {
      // Loop goes from the back to the front so it can just replace the colons with the character sequence
      // while looping through without messing up the loop.
      var quoteSequenceStarted = false;
      for (var i = value.Length - 1; i >= 0; i--)
      {
        var currentChar = value[i];

        if (currentChar == '"')
        {
          quoteSequenceStarted = !quoteSequenceStarted;
          continue;
        }

        if (quoteSequenceStarted && _escapeCharacters.ContainsKey(currentChar))
        {
          value = value.Remove(i, 1); // Remove the character
          value = value.Insert(i, _escapeCharacters[currentChar]); // Insert the escape sequence
        }
      }

      return value;
    }

    private string UnescapeInputValue(string value)
    {
      foreach (var escapeCharacter in _escapeCharacters)
      {
        value = value.Replace(escapeCharacter.Value, escapeCharacter.Key.ToString());
      }

      return value.Replace("\"", ""); // Remove quotes.
    }
  }
}

