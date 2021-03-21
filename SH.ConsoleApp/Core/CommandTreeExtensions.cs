using SH.ConsoleApp.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal static class CommandTreeExtensions
  {
    public static CommandMatch FindCommand(this CommandTree commandTree, ParsedInput input)
    {
      return commandTree.FindCommand(input.Command, input.Options.Keys.ToList(), input.Arguments.Keys.ToList());
    }
  }
}
