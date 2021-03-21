using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
  public class CommandOptionAttribute : CommandAttributeBaseWithDescription
  {
    public CommandOptionAttribute() : base()
    {
    }

    public CommandOptionAttribute(string name, string description) : base(name, description)
    {
    }
  }
}
