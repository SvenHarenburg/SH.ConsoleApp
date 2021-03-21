using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  public static class IHostBuilderExtensions
  {
    public static async Task RunConsoleAppEngineAsync(this IHostBuilder builder, string[] args)
    {
      builder.ConfigureServices((hostContext, services) =>
      {
        services.AddSingleton(services.BuildServiceProvider());
        services.AddSingleton(new CommandLineArgs(args));
        services.AddHostedService<Engine>();
      });

      await builder.RunConsoleAsync();
    }
  }
}
