using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class CommandMatch
  {
    public Command Command { get; set; }
    public int NumberOfMissingOptionalOptions { get; set; }
    public int NumberOfMissingOptionalArguments { get; set; }
  }
}
