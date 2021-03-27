using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Core
{
  internal class CommandRunner
  {
    private IServiceProvider _serviceProvider;
    public CommandRunner(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public void RunCommand(Command command, Dictionary<string, string> options, Dictionary<string, string> arguments)
    {
      var instance = ActivatorUtilities.CreateInstance(_serviceProvider, command.CommandMethodInfo.DeclaringType);
      var parameters = new List<object>();
      var invalidOptions = new Dictionary<string, string>();
      var invalidArguments = new Dictionary<string, string>();

      foreach (var parameterInfo in command.CommandMethodInfo.GetParameters())
      {
        // Special parameter of type CommandGroup, just pass the CommandGroup-instance.
        // This is utilized by the HelpCommand to output help for a CommandGroup.
        if (parameterInfo.ParameterType == typeof(CommandGroup))
        {
          parameters.Add(command.CommandGroup);
          continue;
        }

        var isOption = false;
        var isArgument = false;

        var rawValue = options.FirstOrDefault(q => string.Equals(q.Key, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));
        if (!rawValue.Equals(default(KeyValuePair<string, string>)))
        {
          isOption = true;
        }
        else
        {
          rawValue = arguments.FirstOrDefault(q => string.Equals(q.Key, parameterInfo.Name, StringComparison.OrdinalIgnoreCase));
          if (!rawValue.Equals(default(KeyValuePair<string, string>)))
          {
            isArgument = true;
          }
          else
          {
            if (parameterInfo.IsOptional)
            {
              parameters.Add(Type.Missing);
              continue;
            }
            else
            {
              // TODO:  Shouldnt happen but throw exception
            }
          }


        }


        // Boolean Options & Arguments allow the absence of a specific value.
        // If the key is given but no specific value, the value will automatically be set to true
        if (parameterInfo.ParameterType == typeof(bool) && string.IsNullOrWhiteSpace(rawValue.Value))
        {
          parameters.Add(true);
        }
        else
        {
          TypeConverter converter = TypeDescriptor.GetConverter(parameterInfo.ParameterType);
          var isValid = converter.IsValid(rawValue.Value);
          if (isValid)
          {
            var typedValue = converter.ConvertTo(rawValue.Value, parameterInfo.ParameterType);
            parameters.Add(typedValue);
          }
          else
          {
            if (isOption)
            {
              invalidOptions.Add(rawValue.Key, rawValue.Value);
            }
            if (isArgument)
            {
              invalidArguments.Add(rawValue.Key, rawValue.Value);
            }
            // throw exception
          }
        }
      }

      command.CommandMethodInfo.Invoke(instance, parameters.ToArray());
    }
  }
}
