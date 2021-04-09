using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class Argument : CommandParameter
  {
    public override string Name => CommandArgumentAttribute?.Name;
    public CommandArgumentAttribute CommandArgumentAttribute { get; set; }
  }
}
