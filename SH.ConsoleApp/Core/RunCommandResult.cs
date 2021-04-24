using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class RunCommandResult
  {
    public Dictionary<string, string> InvalidOptions { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> InvalidArguments { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Contains <see cref="ParameterInfo"/>-objects of parameters on a Command-method that are not marked as either a CommandOption or a CommandArgument.
    /// Unmarked parameters have to be optional. Otherwise this case is not allowed.
    /// </summary>
    public List<ParameterInfo> NonOptionalUnknownParameters { get; set; } = new List<ParameterInfo>();

    public bool Success { get; set; }
  }
}
