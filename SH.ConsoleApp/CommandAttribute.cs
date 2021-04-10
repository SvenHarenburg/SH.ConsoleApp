using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  /// <summary>
  /// Declares a function to be a Command.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class CommandAttribute : CommandAttributeBaseWithDescription
  {
    public string ExampleUsage { get; set; }

    public CommandAttribute() : base()
    {
    }

    public CommandAttribute(string name, string description, string exampleUsage = "") : base(name, description)
    {
      Description = description;
      ExampleUsage = exampleUsage;
    }
  }
}
