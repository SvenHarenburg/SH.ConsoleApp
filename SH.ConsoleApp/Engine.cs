using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SH.ConsoleApp.Core;
using SH.ConsoleApp.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  internal class Engine : IHostedService
  {
    private int? _exitCode;

    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IServiceProvider _serviceProvider;
    private readonly CommandLineArgs _args;

    // Constructor has to be public for Dependency Injection to work.
    // The ServiceProvider cannot access internal constructors.
    public Engine(
      ILogger<Engine> logger,
      IHostApplicationLifetime appLifetime,
      IServiceProvider serviceProvider,
      CommandLineArgs args)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _args = args ?? throw new ArgumentNullException(nameof(args));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogDebug($"Starting with arguments: {string.Join(" ", _args.Args)}");

      _appLifetime.ApplicationStarted.Register(() =>
      {
        Task.Run(() =>
        {
          try
          {
            _logger.LogInformation("EngineService => Register");

            // Build command tree:
            // ExecutingAssembly: SH.ConsoleApp to include Commands provided by this library.
            // EntryAssembly: The Assembly referencing SH.ConsoleApp.
            var assemblies = new Assembly[2] { Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() };
            var builder = new CommandTreeBuilder(assemblies);
            var commandTree = builder.BuildBaseTree();
            var availableCommands = commandTree.CommandGroups.SelectMany(q => q.AvailableCommands).ToList();

            // Parse input
            ParsedInput parsedInput = null;
            var parser = new InputParser(availableCommands);
            parsedInput = parser.ParseInput(_args.Args);

            // Match input to Command:
            var commandMatch = commandTree.FindCommand(parsedInput);

            // Run command using CommandRunner:
            var runner = ActivatorUtilities.CreateInstance<CommandRunner>(_serviceProvider);
            runner.RunCommand(commandMatch.Command, parsedInput.Options, parsedInput.Arguments);

            _exitCode = 0;

          }
          catch (Exception ex)
          {
            _logger.LogError($"Unhandled Exception: {ex}");
            _exitCode = 1;
          }
          finally
          {
            _appLifetime.StopApplication();
          }
        });
      });

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogDebug($"Exiting with return code: {_exitCode}");

      // Exitcode may be null if the user cancelled cia Ctrl+C/SIGTERM
      Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
      return Task.CompletedTask;
    }
  }
}
