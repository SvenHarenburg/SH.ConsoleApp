using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class CommandAttribute : CommandAttributeBase
  {
    public string Description { get; set; }

    public CommandAttribute() : base()
    {
    }

    public CommandAttribute(string name, string description) : base(name)
    {
      Description = description;
    }
  }
}
