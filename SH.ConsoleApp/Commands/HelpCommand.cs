using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Commands
{
  [CommandGroup("help")]
  public class HelpCommand
  {
    [Command("","description")]
    public void Execute()
    {
      Console.WriteLine("HelpCommand=>Execute");
    }
  }
}
