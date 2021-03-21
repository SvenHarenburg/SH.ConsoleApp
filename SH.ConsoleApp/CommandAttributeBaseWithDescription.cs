using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  public abstract class CommandAttributeBaseWithDescription : CommandAttributeBase
  {
    public string Description { get; set; }

    public CommandAttributeBaseWithDescription() : base()
    {
    }

    public CommandAttributeBaseWithDescription(string name, string description) : base(name)
    {
      Description = description;
    }
  }
}
