namespace SH.ConsoleApp.Core
{
  internal class ParameterMatch
  {
    public bool MissesRequiredParameters { get; set; }
    public bool MoreParametersThanAvailable { get; set; }
    public int NumberOfMissingOptionalParameters { get; set; }
  }
}
