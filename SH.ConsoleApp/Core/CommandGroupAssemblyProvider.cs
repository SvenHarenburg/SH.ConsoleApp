using System.Reflection;

namespace SH.ConsoleApp.Core
{
  public class CommandGroupAssemblyProvider : ICommandGroupAssemblyProvider
  {
    public Assembly[] GetAssemblies()
    {
      // ExecutingAssembly: SH.ConsoleApp to include Commands provided by this library.
      // EntryAssembly: The Assembly referencing SH.ConsoleApp.      
      return new Assembly[2] {
        Assembly.GetExecutingAssembly(),
        Assembly.GetEntryAssembly()
      };
    }
  }
}
