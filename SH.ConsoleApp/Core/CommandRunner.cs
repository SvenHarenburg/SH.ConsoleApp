using Microsoft.Extensions.DependencyInjection;
using SH.ConsoleApp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  /// <summary>
  /// 
  /// </summary>
  /// <remarks>If there is ever any will to rewrite anything in this project then this class should be the place to start.</remarks>
  internal class CommandRunner : ICommandRunner
  {
    private IServiceProvider _serviceProvider;

    /// <summary>
    /// Stores the <see cref="Command"/> which has been passed to the latest call of <see cref="RunCommand(Command, Dictionary{string, string}, Dictionary{string, string})"/>.
    /// </summary>
    private Command _currentCommand;

    /// <summary>
    /// Stores the Options passed to the latest call of <see cref="RunCommand(Command, Dictionary{string, string}, Dictionary{string, string})"/>.
    /// </summary>
    private Dictionary<string, string> _currentOptions;

    /// <summary>
    /// Stores the Arguments passed to the latest call of <see cref="RunCommand(Command, Dictionary{string, string}, Dictionary{string, string})"/>.
    /// </summary>
    private Dictionary<string, string> _currentArguments;

    /// <summary>
    /// Stores the parameter values that should be passed to the Command-method for the current call of <see cref="RunCommand(Command, Dictionary{string, string}, Dictionary{string, string})"/>.
    /// </summary>
    private List<object> _currentParameterValues;

    public CommandRunner(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public RunCommandResult RunCommand(Command command, Dictionary<string, string> options, Dictionary<string, string> arguments)
    {
      var result = new RunCommandResult();
      _currentCommand = command;
      _currentOptions = options ?? new Dictionary<string, string>();
      _currentArguments = arguments ?? new Dictionary<string, string>();
      _currentParameterValues = new List<object>();

      foreach (var parameterInfo in command.CommandMethodInfo.GetParameters())
      {
        if (HandleSpecialParameters(parameterInfo)) continue;

        var evaluateParameterResult = EvaluateParameter(parameterInfo);
        if (evaluateParameterResult.RawValue.HasValue)
        {
          // Has value:
          var rawValue = (KeyValuePair<string, string>)evaluateParameterResult.RawValue;
          var converter = TypeDescriptor.GetConverter(parameterInfo.ParameterType);
          var isValid = converter.IsValid(rawValue.Value);
          if (isValid)
          {
            var typedValue = converter.ConvertTo(rawValue.Value, parameterInfo.ParameterType);
            _currentParameterValues.Add(typedValue);
          }
          else
          {
            // Value is not convertable to parameter type:
            switch (evaluateParameterResult.Kind)
            {
              case CommandParameterKind.Option:
                result.InvalidOptions.Add(rawValue.Key, rawValue.Value);
                break;

              case CommandParameterKind.Argument:
                result.InvalidArguments.Add(rawValue.Key, rawValue.Value);
                break;

              case CommandParameterKind.Unknown:
                // Should not happen: If Kind is Unknown then there should not be a value. Just in case:
                throw new NotImplementedException($"Internal Error: {nameof(evaluateParameterResult.Kind)} is {evaluateParameterResult.Kind} but {nameof(evaluateParameterResult.RawValue.Value)} is not null!");
            }
          }
        }
        else
        {
          // Does not have value:
          if (evaluateParameterResult.IsOptional)
          {
            // Is Optional so it does not need a value.
            _currentParameterValues.Add(Type.Missing);
          }
          else
          {
            if (evaluateParameterResult.Kind == CommandParameterKind.Unknown)
            {
              // Non-optional Unknown parameter, not marked as Option or Argument.
              result.NonOptionalUnknownParameters.Add(evaluateParameterResult.ParameterInfo);
            }
            else
            {
              // Should not happen: If Kind is NOT Unknown then there has to be a value. Just in case:
              throw new NotImplementedException($"Internal Error: {nameof(evaluateParameterResult.Kind)} is {evaluateParameterResult.Kind} and {nameof(evaluateParameterResult.RawValue.Value)} is null!");
            }
          }
        }
      }

      if (CanRunCommand(result))
      {
        RunCommand();
        result.Success = true;
      }
      else
      {
        result.Success = false;
      }

      return result;
    }

    private void RunCommand()
    {
      // Instantiate instance of CommandGroup Type:
      var instance = ActivatorUtilities.CreateInstance(_serviceProvider, _currentCommand.CommandMethodInfo.DeclaringType);

      // Invoke the Command:
      _currentCommand.CommandMethodInfo.Invoke(instance, _currentParameterValues.ToArray());
    }

    public bool CanRunCommand(RunCommandResult result)
    {
      if (result.InvalidOptions.Any()) return false;
      if (result.InvalidArguments.Any()) return false;
      if (result.NonOptionalUnknownParameters.Any()) return false;
      return true;
    }

    /// <summary>
    /// Evaluate wether the Commandmethod-parameter is an option, an argument or neither(=Unknown).
    /// </summary>
    /// <param name="parameterInfo">The <see cref="ParameterInfo"/> corresponding to the Commandmethod-parameter</param>
    /// <returns>Returns the results in an instance of <see cref="EvaluateParameterResult"/>.</returns>
    private EvaluateParameterResult EvaluateParameter(ParameterInfo parameterInfo)
    {
      var result = new EvaluateParameterResult()
      {
        ParameterInfo = parameterInfo
      };

      var rawValue = _currentOptions.FirstOrDefault(q => string.Equals(q.Key, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));
      if (!rawValue.IsDefault())
      {
        result.Kind = CommandParameterKind.Option;
        result.RawValue = rawValue;
      }
      else
      {
        rawValue = _currentArguments.FirstOrDefault(q => string.Equals(q.Key, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));
        if (!rawValue.IsDefault())
        {
          result.Kind = CommandParameterKind.Argument;
          result.RawValue = rawValue;
        }
        else
        {
          result.RawValue = null;
          result.Kind = CommandParameterKind.Unknown;
          result.IsOptional = parameterInfo.IsOptional;
        }
      }

      return result;
    }

    /// <summary>
    /// Checks <see cref="ParameterInfo.ParameterType"/> for special types such as the <see cref="CommandGroup"/>-Type.
    /// Such parameters will be handled differently than normal Options or Arguments.
    /// </summary>
    /// <param name="parameterInfo">The <see cref="ParameterInfo"/> for the Commandmethod-parameter.</param>
    /// <returns><see cref="true"/> if the parameter is special and has been handled, false if not.</returns>
    private bool HandleSpecialParameters(ParameterInfo parameterInfo)
    {
      // Special parameter of type CommandGroup, just pass the CommandGroup-instance.
      // This is utilized by the HelpCommand to output help for a CommandGroup.
      if (parameterInfo.ParameterType == typeof(CommandGroup))
      {
        _currentParameterValues.Add(_currentCommand.CommandGroup);
        return true;
      }

      return false;
    }
  }
}
