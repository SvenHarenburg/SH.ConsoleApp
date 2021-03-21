using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
  public class CommandArgumentAttribute : CommandAttributeBaseWithDescription
  {

    public CommandArgumentAttribute() : base()
    {
    }

    public CommandArgumentAttribute(string name, string description) : base(name, description)
    {
    }
  }
}
