using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Input
{
  /// <summary>
  /// The exception that is thrown by the <see cref="InputParser"/> when the input cannot be matched to any of the available commands.
  /// </summary>
  [Serializable]
  public class CommandNotFoundException : Exception
  {
    /// <summary>
    ///// The command which could not be found.
    /// </summary>
    public string Command { get; set; }
    public CommandNotFoundException() { }
    public CommandNotFoundException(string message) : base(message) { }
    public CommandNotFoundException(string message, Exception inner) : base(message, inner) { }
    protected CommandNotFoundException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    /// <summary>
    /// Initializes a new instance of <see cref="CommandNotFoundException"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="command">The command which could not be found.</param>
    public CommandNotFoundException(string message, string command) : base(message)
    {
      Command = command;
    }
  }
}
