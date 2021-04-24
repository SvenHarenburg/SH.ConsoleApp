using System.Collections.Generic;

namespace SH.ConsoleApp.Core
{
  internal interface ICommandRunner
  {
    RunCommandResult RunCommand(Command command, Dictionary<string, string> options, Dictionary<string, string> arguments);
  }
}