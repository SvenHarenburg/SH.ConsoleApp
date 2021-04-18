using System.Collections.Generic;

namespace SH.ConsoleApp.Input
{
  public interface IInputParser
  {
    ParsedInput ParseInput(string[] args, List<string> availableCommands);
  }
}