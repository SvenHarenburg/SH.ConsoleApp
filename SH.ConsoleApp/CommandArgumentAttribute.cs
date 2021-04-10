using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  /// <summary>
  /// Declares a parameter to be an Argument passed to the Command.
  /// </summary>
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
