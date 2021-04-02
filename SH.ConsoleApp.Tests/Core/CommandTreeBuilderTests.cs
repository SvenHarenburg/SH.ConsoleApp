using NUnit.Framework;
using SH.ConsoleApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Reflection;
using SH.ConsoleApp.Tests.TestCommands;

namespace SH.ConsoleApp.Tests.Core
{
  [TestFixture]
  public class CommandTreeBuilderTests
  {

    [TestFixture]
    public class BuildBaseTreeTests
    {
      // The builder can search for commands in either a single assembly or in multiple assemblies. 
      // To make sure both work, I create _singleAssembly which returns all TestCommands and 
      // _multipleAssemblies which contains multiple that contain different Commands.
      private Assembly _singleAssembly;
      private Assembly[] _multipleAssemblies;

      [SetUp]
      public void Setup()
      {
        // Create mock for _singleAssembly:
        {
          var mock = new Mock<Assembly>();
          mock.Setup(q => q.GetTypes())
            .Returns(
            new Type[]
            {
            typeof(TestCommands.MathCommand),
            typeof(TestCommands.WeatherCommand)
            });
          _singleAssembly = mock.Object;
        }

        // Create mocks for _multipleAssemblies:
        {
          _multipleAssemblies = new Assembly[2];
          var mock = new Mock<Assembly>();
          mock.Setup(q => q.GetTypes())
            .Returns(
            new Type[]
            {
            typeof(TestCommands.MathCommand)
            });
          _multipleAssemblies[0] = mock.Object;

          mock = new Mock<Assembly>();
          mock.Setup(q => q.GetTypes())
            .Returns(
            new Type[]
            {
            typeof(TestCommands.WeatherCommand)
            });
          _multipleAssemblies[1] = mock.Object;
        }
      }

      [Test]
      [TestCase(typeof(MathCommand))]
      [TestCase(typeof(WeatherCommand))]
      public void FindsCommandGroupInSingleAssembly(Type commandGroupType)
      {
        var builder = new CommandTreeBuilder(_singleAssembly);
        var tree = builder.BuildBaseTree();

        var found = tree.CommandGroups.Any(q => q.CommandGroupType == commandGroupType);
        Assert.True(found, $"CommandGroup of type {commandGroupType.Name} has not been found.");
      }

      [Test]
      [TestCase(typeof(MathCommand))]
      [TestCase(typeof(WeatherCommand))]
      public void FindsCommandGroupInMultipleAssemblies(Type commandGroupType)
      {
        var builder = new CommandTreeBuilder(_multipleAssemblies);
        var tree = builder.BuildBaseTree();

        var found = tree.CommandGroups.Any(q => q.CommandGroupType == commandGroupType);
        Assert.True(found, $"CommandGroup of type {commandGroupType.Name} has not been found.");
      }

      [Test]
      [TestCase(typeof(MathCommand), new string[] { nameof(MathCommand.Add), nameof(MathCommand.Subtract) })]
      [TestCase(typeof(WeatherCommand), new string[] { nameof(WeatherCommand.Today), nameof(WeatherCommand.Tomorrow), nameof(WeatherCommand.Weekly) })]
      public void FindsCommandsInCommandGroups(Type commandGroupType, string[] commandMethodNames)
      {
        var builder = new CommandTreeBuilder(_singleAssembly);
        var tree = builder.BuildBaseTree();

        var commandGroup = tree.CommandGroups.First(q => q.CommandGroupType == commandGroupType);
        Assert.Multiple(() =>
        {
          foreach (var methodName in commandMethodNames)
          {
            var methodInfo = commandGroupType.GetMethod(methodName);
            var found = commandGroup.Commands.Any(q => q.CommandMethodInfo.MethodHandle == methodInfo.MethodHandle);
            Assert.True(found, $"Command with MethodName {methodName} has not been found in CommandGroup for Type {commandGroupType.Name}");
          }
        });
      }

      /// <summary>
      /// Filling Options and Arguments is only necessary for the specific command that the user wants to execute.
      /// Analyzing all commands would therefore be unnecessary and a waste of performance. 
      /// If this logic will ever change this test will fail and I am forced to reconsider.
      /// </summary>
      [Test]
      public void DoesNotFillOptionsAndArguments()
      {
        var builder = new CommandTreeBuilder(_singleAssembly);
        var tree = builder.BuildBaseTree();
        var commands = from commandGroup in tree.CommandGroups
                       from command in commandGroup.Commands
                       where command.Arguments.Any() || command.Options.Any()
                       select new { commandGroup, command };

        Assert.IsEmpty(commands,
          $"Found {commands.Count()} Commands with filled Options and/or Arguments. These should not be filled when building the base tree.");
      }



    }


    [TestFixture]
    public class FillOptionsAndArgumentsTests
    {
      private Dictionary<MethodInfo, ExpectedCommandParameter[]> _expectedOptionsPerCommand;
      private Dictionary<MethodInfo, ExpectedCommandParameter[]> _expectedArgumentsPerCommand;

      [SetUp]
      public void Setup()
      {
        _expectedOptionsPerCommand = new Dictionary<MethodInfo, ExpectedCommandParameter[]>();
        _expectedArgumentsPerCommand = new Dictionary<MethodInfo, ExpectedCommandParameter[]>();

        var commandMethodInfos = new MethodInfo[]
        {
          typeof(MathCommand).GetMethod(nameof(MathCommand.Add)),
          typeof(MathCommand).GetMethod(nameof(MathCommand.Subtract)),
          typeof(WeatherCommand).GetMethod(nameof(WeatherCommand.Today)),
          typeof(WeatherCommand).GetMethod(nameof(WeatherCommand.Tomorrow)),
          typeof(WeatherCommand).GetMethod(nameof(WeatherCommand.Weekly))
        };

        foreach (var methodInfo in commandMethodInfos)
        {
          _expectedOptionsPerCommand.Add(methodInfo, GetExpectedCommandOptions(methodInfo));
          _expectedArgumentsPerCommand.Add(methodInfo, GetExpectedCommandArguments(methodInfo));
        }
      }

      private ExpectedCommandParameter[] GetExpectedCommandOptions(MethodInfo methodInfo)
      {
        return methodInfo.GetParameters()
             .Where(q => q.GetCustomAttribute<CommandOptionAttribute>() != null)
             .Select(q => new ExpectedCommandParameter()
             {
               Name = q.GetCustomAttribute<CommandOptionAttribute>().Name,
               ParameterType = q.ParameterType
             })
             .ToArray();
      }

      private ExpectedCommandParameter[] GetExpectedCommandArguments(MethodInfo methodInfo)
      {
        return methodInfo.GetParameters()
             .Where(q => q.GetCustomAttribute<CommandArgumentAttribute>() != null)
             .Select(q => new ExpectedCommandParameter()
             {
               Name = q.GetCustomAttribute<CommandArgumentAttribute>().Name,
               ParameterType = q.ParameterType
             })
             .ToArray();
      }


      [Test]
      [TestCase(typeof(MathCommand), nameof(MathCommand.Add))]
      [TestCase(typeof(MathCommand), nameof(MathCommand.Subtract))]
      [TestCase(typeof(WeatherCommand), nameof(WeatherCommand.Today))]
      [TestCase(typeof(WeatherCommand), nameof(WeatherCommand.Tomorrow))]
      [TestCase(typeof(WeatherCommand), nameof(WeatherCommand.Weekly))]
      public void FillsOptions(Type commandGroupType, string commandMethodName)
      {
        var methodInfo = commandGroupType.GetMethod(commandMethodName);
        _expectedOptionsPerCommand.TryGetValue(methodInfo, out var expectedOptions);
        Assert.NotNull(expectedOptions, $"Test setup not correct. Missing expectedOptions for command of method {commandGroupType.Name}.{commandMethodName}");

        var command = new Command()
        {
          CommandMethodInfo = methodInfo
        };
        CommandTreeBuilder.FillOptionsAndArguments(command);

        Assert.Multiple(() =>
        {
          foreach (var expected in expectedOptions)
          {
            var found = command.Options.Any(
              q => q.CommandOptionAttribute.Name == expected.Name && q.ParameterInfo.ParameterType == expected.ParameterType);
            Assert.True(found, $"Option {expected.Name} with parameter type {expected.ParameterType.Name} for command of method {commandGroupType.Name}.{commandMethodName} was not found.");
          }
        });
      }

      [Test]
      [TestCase(typeof(MathCommand), nameof(MathCommand.Add))]
      [TestCase(typeof(MathCommand), nameof(MathCommand.Subtract))]
      [TestCase(typeof(WeatherCommand), nameof(WeatherCommand.Today))]
      [TestCase(typeof(WeatherCommand), nameof(WeatherCommand.Tomorrow))]
      [TestCase(typeof(WeatherCommand), nameof(WeatherCommand.Weekly))]
      public void FillsArguments(Type commandGroupType, string commandMethodName)
      {
        var methodInfo = commandGroupType.GetMethod(commandMethodName);
        _expectedArgumentsPerCommand.TryGetValue(methodInfo, out var expectedArguments);
        Assert.NotNull(expectedArguments, $"Test setup not correct. Missing expectedArguments for command of method {commandGroupType.Name}.{commandMethodName}");

        var command = new Command()
        {
          CommandMethodInfo = methodInfo
        };
        CommandTreeBuilder.FillOptionsAndArguments(command);

        Assert.Multiple(() =>
        {
          foreach (var expected in expectedArguments)
          {
            var found = command.Arguments.Any(
              q => q.CommandArgumentAttribute.Name == expected.Name && q.ParameterInfo.ParameterType == expected.ParameterType);
            Assert.True(found, $"Argument {expected.Name} with parameter type {expected.ParameterType.Name} for command of method {commandGroupType.Name}.{commandMethodName} was not found.");
          }
        });
      }

    }
  }
}
