using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Input
{
  internal class ParsedInput
  {
    public string Command { get; set; }
    public Dictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
  }
}
