using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  /// <summary>
  /// Declares a class to be a Commandgroup containing one or more Commands.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class CommandGroupAttribute : CommandAttributeBaseWithDescription
  {
    public CommandGroupAttribute() : base()
    {
    }

    public CommandGroupAttribute(string name, string description) : base(name, description)
    {
    }
  }
}
