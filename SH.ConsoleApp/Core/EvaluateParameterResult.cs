using System.Collections.Generic;
using System.Reflection;

namespace SH.ConsoleApp.Core
{
  internal class EvaluateParameterResult
  {
    public CommandParameterKind Kind { get; set; }
    public bool IsOptional { get; set; }
    public KeyValuePair<string, string>? RawValue { get; set; }
    public ParameterInfo ParameterInfo { get; set; }
  }
}
