using System.Collections.Generic;

namespace SH.ConsoleApp.Input
{
  internal interface IInputParser
  {
    ParsedInput ParseInput(string[] args, List<string> availableCommands);
  }
}