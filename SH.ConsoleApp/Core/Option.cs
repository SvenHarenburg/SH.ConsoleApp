using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class Option : CommandParameter
  {
    public override string Name => CommandOptionAttribute?.Name;
    public CommandOptionAttribute CommandOptionAttribute { get; set; }
  }
}
