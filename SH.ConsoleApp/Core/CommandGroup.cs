using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class CommandGroup
  {
    public Type CommandGroupType { get; set; }
    public CommandGroupAttribute CommandGroupAttribute { get; set; }
    public List<Command> Commands { get; set; } = new List<Command>();

    public string Name => CommandGroupAttribute.Name;

    public List<string> AvailableCommands => Commands
      .Select(q => $"{Name} {(string.IsNullOrWhiteSpace(q.CommandAttribute.Name) ? "" : q.CommandAttribute.Name) }".Trim())
      .Distinct()
      .ToList();
  }
}
