using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  internal class CommandLineArgs
  {
    public string[] Args { get; set; }
    public CommandLineArgs()
    {
    }
    public CommandLineArgs(string[] args)
    {
      Args = args;
    }
  }
}
