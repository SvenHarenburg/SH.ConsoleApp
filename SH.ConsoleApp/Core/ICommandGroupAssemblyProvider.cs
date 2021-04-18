using System.Reflection;

namespace SH.ConsoleApp.Core
{
  public interface ICommandGroupAssemblyProvider
  {
    Assembly[] GetAssemblies();
  }
}
