using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SH.ConsoleApp.Core;
using SH.ConsoleApp.Input;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  public static class IHostBuilderExtensions
  {
    /// <summary>
    /// Start the ConsoleApp-Engine which will then parse the input, try to find a matching Command and execute it.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args">The arguments passed to the console application.</param>
    /// <returns>An awaitable <see cref="Task"/>.</returns>
    public static async Task RunConsoleAppEngineAsync(this IHostBuilder builder, string[] args)
    {
      builder.ConfigureServices((hostContext, services) =>
      {
        services.AddSingleton(services);
        services.AddSingleton(new CommandLineArgs(args));
        services.TryAddTransient<ICommandGroupAssemblyProvider, CommandGroupAssemblyProvider>();
        services.TryAddTransient<IInputParser, InputParser>();
        services.AddHostedService<Engine>();
      });

      await builder.RunConsoleAsync();
    }
  }
}
