namespace SH.ConsoleApp.Core
{
  internal enum CommandParameterKind
  {
    /// <summary>
    /// The parameter is neither marked as a CommandOption nor as a CommandArgument.
    /// </summary>
    Unknown,

    /// <summary>
    /// The parameter is marked as a CommandOption with the <see cref="CommandOptionAttribute"/>.
    /// </summary>
    Option,

    /// <summary>
    /// The parameter is marked as a CommandArgument with the <see cref="CommandArgumentAttribute"/>.
    /// </summary>
    Argument
  }
}
