using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  public abstract class CommandAttributeBase : Attribute
  {
    public string Name { get; set; }

    public CommandAttributeBase()
    {
    }
    public CommandAttributeBase(string name)
    {
      Name = name;
    }
  }
}
