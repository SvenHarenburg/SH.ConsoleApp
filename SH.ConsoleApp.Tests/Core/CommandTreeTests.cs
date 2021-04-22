using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SH.ConsoleApp.Core;
using System.Reflection;
using SH.ConsoleApp.Tests.TestCommands;
using Moq;

namespace SH.ConsoleApp.Tests.Core
{
  [TestFixture]
  public class CommandTreeTests
  {
    [TestFixture]
    public class FindCommandTests
    {
      private CommandTree _tree;

      [SetUp]
      public void Setup()
      {
        // The creation of a CommandTree is complicated. To keep the test simple, the CommandTreeBuilder is used.        
        // The tradeoff is that if the CommandTreeBuilder does not work correctly, this test might fail even though
        // the CommandTree itself works correctly. Since the Builder itself is tested, I find this acceptable.

        var mock = new Mock<ICommandGroupAssemblyProvider>();
        mock.Setup(q => q.GetAssemblies())
          .Returns(new Assembly[]
          {
              Assembly.GetExecutingAssembly()
          });

        _tree = new CommandTreeBuilder(mock.Object).BuildBaseTree();
      }

      [Test]
      public void FindsMatchNoOptionsOrArguments()
      {
        var expectedMethod = typeof(CommandTreeTestsCommand).GetMethod(nameof(CommandTreeTestsCommand.NoOptionsOrArguments));
        var match = _tree.FindCommand("CommandTreeTestsCommand NoOptionsOrArguments",
          options: new List<string>(),
          arguments: new List<string>());
        Assert.AreEqual(match.Command.CommandMethodInfo.MethodHandle, expectedMethod.MethodHandle);
      }

      [Test]
      public void FindsMatchWithOptionsNoArguments()
      {
        var expectedMethod = typeof(CommandTreeTestsCommand).GetMethod(nameof(CommandTreeTestsCommand.WithOptionsNoArguments));
        var match = _tree.FindCommand("CommandTreeTestsCommand WithOptionsNoArguments",
          options: new List<string>() { "option1", "option2" },
          arguments: new List<string>());
        Assert.AreEqual(match.Command.CommandMethodInfo.MethodHandle, expectedMethod.MethodHandle);
      }

      [Test]
      public void FindsMatchNoOptionsWithArguments()
      {
        var expectedMethod = typeof(CommandTreeTestsCommand).GetMethod(nameof(CommandTreeTestsCommand.NoOptionsWithArguments));
        var match = _tree.FindCommand("CommandTreeTestsCommand NoOptionsWithArguments",
          options: new List<string>(),
          arguments: new List<string>() { "argument1", "argument2" });
        Assert.AreEqual(match.Command.CommandMethodInfo.MethodHandle, expectedMethod.MethodHandle);
      }

      [Test]
      public void FindsMatchWithOptionsAndArguments()
      {
        var expectedMethod = typeof(CommandTreeTestsCommand).GetMethod(nameof(CommandTreeTestsCommand.WithOptionsAndArguments));
        var match = _tree.FindCommand("CommandTreeTestsCommand WithOptionsAndArguments",
          options: new List<string>() { "option1", "option2" },
          arguments: new List<string>() { "argument1", "argument2" });
        Assert.AreEqual(match.Command.CommandMethodInfo.MethodHandle, expectedMethod.MethodHandle);
      }

      [Test]
      public void FindsBestMatchWithMultiplePotentialCommands()
      {
        var expectedMethod = typeof(CommandTreeTestsCommand).GetMethod(nameof(CommandTreeTestsCommand.MultiplePotentialCommands1));
        var match = _tree.FindCommand("CommandTreeTestsCommand MultiplePotentialCommands",
          options: new List<string>() { "option1" },
          arguments: new List<string>() { "argument1" });
        Assert.AreEqual(match.Command.CommandMethodInfo.MethodHandle, expectedMethod.MethodHandle);
      }

      [Test]
      public void ReturnsNullOnNoMatch()
      {
        var match = _tree.FindCommand("CommandTreeTestsCommand NonExistingCommand",
          options: new List<string>(),
          arguments: new List<string>());
        Assert.IsNull(match);
      }
    }
  }
}
