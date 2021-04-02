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
  }
}
