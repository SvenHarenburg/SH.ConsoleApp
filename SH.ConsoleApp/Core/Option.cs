using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class Option
  {
    public ParameterInfo ParameterInfo { get; set; }
    public CommandOptionAttribute CommandOptionAttribute { get; set; }
  }
}
