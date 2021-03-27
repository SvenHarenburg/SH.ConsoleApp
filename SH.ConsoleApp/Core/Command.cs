using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class Command
  {
    public MethodInfo CommandMethodInfo { get; set; }
    public CommandAttribute CommandAttribute { get; set; }
    public List<Option> Options { get; set; } = new List<Option>();
    public List<Argument> Arguments { get; set; } = new List<Argument>();

    // Reference to the CommandGroup this Command belongs to.
    public CommandGroup CommandGroup { get; set; }    
  }
}
