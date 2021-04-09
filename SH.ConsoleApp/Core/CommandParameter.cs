using System.Reflection;

namespace SH.ConsoleApp.Core
{
  internal abstract class CommandParameter
  {
    public abstract string Name { get; }
    public ParameterInfo ParameterInfo { get; set; }
  }
}
