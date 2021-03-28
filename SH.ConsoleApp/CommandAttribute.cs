using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class CommandAttribute : CommandAttributeBaseWithDescription
  {
    public CommandAttribute() : base()
    {
    }

    public CommandAttribute(string name, string description) : base(name, description)
    {
      Description = description;
    }
  }
}
