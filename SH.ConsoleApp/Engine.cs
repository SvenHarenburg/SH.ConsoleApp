using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SH.ConsoleApp.Core;
using SH.ConsoleApp.Input;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  internal class Engine : IHostedService
  {
    private int? _exitCode;

    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly CommandLineArgs _args;
    private readonly IServiceCollection _serviceCollection;
    private readonly IInputParser _inputParser;
    private readonly ICommandTreeBuilder _commandTreeBuilder;
    private readonly ICommandRunner _commandRunner;

    // Constructor has to be public for Dependency Injection to work.
    // The ServiceProvider cannot access internal constructors.
    public Engine(
      ILogger<Engine> logger,
      IHostApplicationLifetime appLifetime,
      CommandLineArgs args,
      IServiceCollection serviceCollection,
      IInputParser inputParser,
      ICommandTreeBuilder commandTreeBuilder,
      ICommandRunner commandRunner)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
      _args = args ?? throw new ArgumentNullException(nameof(args));
      _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
      _inputParser = inputParser ?? throw new ArgumentNullException(nameof(inputParser));
      _commandTreeBuilder = commandTreeBuilder ?? throw new ArgumentNullException(nameof(commandTreeBuilder));
      _commandRunner = commandRunner ?? throw new ArgumentNullException(nameof(commandRunner));
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
            var commandTree = _commandTreeBuilder.BuildBaseTree();
            var availableCommands = commandTree.CommandGroups.SelectMany(q => q.AvailableCommands).ToList();

            // Add the commandTree to the servicecollection so the help-command can access and use it.
            _serviceCollection.AddSingleton(commandTree);

            // Parse input
            var parsedInput = _inputParser.ParseInput(_args.Args, availableCommands);

            // Match input to Command:
            var commandMatch = commandTree.FindCommand(parsedInput);

            if (commandMatch != null)
            {
              // Run command using CommandRunner:
              var runCommandResult = _commandRunner.RunCommand(commandMatch.Command, parsedInput.Options, parsedInput.Arguments);
              if (!runCommandResult.Success)
              {
                AnalyseRunCommandResult(runCommandResult);
              }
            }
            else
            {
              WriteCommandNotFound();
            }

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

    private void AnalyseRunCommandResult(RunCommandResult runCommandResult)
    {

      // TODO: Called when runCommandResult.Success == false. Analyse why and throw exceptions or something.
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogDebug($"Exiting with return code: {_exitCode}");

      // Exitcode may be null if the user cancelled cia Ctrl+C/SIGTERM
      Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
      return Task.CompletedTask;
    }

    private void WriteCommandNotFound()
    {
      Console.WriteLine($"No command could be found for provided arguments. Run \"help\" for a list of commands or \"{{command}} help\" for help with a specific command.");
    }
  }
}
