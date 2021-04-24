using NUnit.Framework;
using SH.ConsoleApp.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Tests.Input
{
  [TestFixture]
  public class InputParserTests
  {

    [TestFixture]
    public class ParseInputTests
    {
      private List<string> _availableCommands;

      [SetUp]
      public void Setup()
      {
        _availableCommands = new List<string>()
        {
          "version",
          "install",
          "update",
          "list packages"
        };
      }

      [Test]
      [TestCase("version", new string[] { "version" })]
      [TestCase("install", new string[] { "install", "name:timer", "--version:1.0.0" })]
      [TestCase("list packages", new string[] { "list packages" })]
      [TestCase("list packages", new string[] { "list packages", "--filter:timer" })]
      public void IdentifiesCommand(string desiredCommand, params string[] args)
      {
        var parser = new InputParser();
        var result = parser.ParseInput(args, _availableCommands);
        Assert.AreEqual(desiredCommand, result.Command);
      }

      [TestCase(new object[] { "nonexistingcommand" })]
      [TestCase(new object[] { "typocommand", "name:timer", "--version:1.0.0" })]
      public void ThrowsCommandNotFoundException(params string[] args)
      {
        var parser = new InputParser();
        Assert.Throws<CommandNotFoundException>(() => parser.ParseInput(args, _availableCommands));
      }


      [Test]
      [TestCase("pretty", "", new string[] { "version", "pretty" })]
      [TestCase("name", "timer", new string[] { "install", "name:timer", "--version:1.0.0" })]
      public void IdentifiesSingleOptionWithOrWithoutValue(string desiredOptionKey, string desiredOptionValue, params string[] args)
      {
        var parser = new InputParser();
        var result = parser.ParseInput(args, _availableCommands);
        Assert.IsNotEmpty(result.Options, "Parser has not found any Options");
        Assert.AreEqual(desiredOptionKey, result.Options.ElementAt(0).Key);
        Assert.AreEqual(desiredOptionValue, result.Options.ElementAt(0).Value);
      }

      [Test]
      [TestCase(new string[] { "option1", "option2", "option3" }, new string[] { "version", "option1", "option2", "option3", "--argumentKey1:argumentValue1" })]
      [TestCase(new string[] { "option1", "option2" }, new string[] { "list packages", "option1", "option2", "--filter:time" })]
      [TestCase(new string[] { "option1", "option2" }, new string[] { "list packages", "option1", "option2" })]
      public void IdentifiesMultipleOptionsInCorrectOrder(string[] desiredOptions, params string[] args)
      {
        var parser = new InputParser();
        var result = parser.ParseInput(args, _availableCommands);
        Assert.AreEqual(desiredOptions.Length, result.Options.Count, "Did not identify the correct number of Options.");

        for (int i = 0; i < desiredOptions.Length; i++)
        {
          Assert.AreEqual(desiredOptions[i], result.Options.ElementAt(i).Key);
        }
      }

      [Test]
      [TestCase("ArgumentKey1", "ArgumentValue1", new string[] { "version", "option1", "option2", "--ArgumentKey1:ArgumentValue1" })]
      [TestCase("filter", "time", new string[] { "list packages", "--filter:time" })]
      public void IdentifiesSingleArgumentWithValue(string desiredArgumentKey, string desiredArgumentValue, params string[] args)
      {
        var parser = new InputParser();
        var result = parser.ParseInput(args, _availableCommands);
        Assert.IsNotEmpty(result.Arguments, "Did not identify an Argument");
        Assert.AreEqual(result.Arguments.Count, 1, "Identified more than one Argument");

        var Argument = result.Arguments.ElementAt(0);
        Assert.AreEqual(desiredArgumentKey, Argument.Key);
        Assert.AreEqual(desiredArgumentValue, Argument.Value);
      }

      [Test]
      [TestCase("ArgumentKey1", new string[] { "version", "option1", "option2", "--ArgumentKey1" })]
      [TestCase("pretty", new string[] { "list packages", "--pretty" })]
      public void IdentifiesSingleArgumentWithoutValue(string desiredArgumentKey, params string[] args)
      {
        var parser = new InputParser();
        var result = parser.ParseInput(args, _availableCommands);
        Assert.IsNotEmpty(result.Arguments, "Did not identify an Argument");
        Assert.AreEqual(result.Arguments.Count, 1, "Identified more than one Argument");

        var Argument = result.Arguments.ElementAt(0);
        Assert.AreEqual(desiredArgumentKey, Argument.Key);
        Assert.True(string.IsNullOrWhiteSpace(Argument.Value), "Argument should not have a value");
      }

      [Test]
      [TestCase(new string[] { "ArgumentKey1", "ArgumentKey2" },
        new string[] { "ArgumentValue1", "" },
        new string[] { "version", "option1", "option2", "--ArgumentKey1:ArgumentValue1", "--ArgumentKey2" })]
      [TestCase(new string[] { "ArgumentKey1", "ArgumentKey2", "ArgumentKey3" },
        new string[] { "", "ArgumentValue2", "" },
        new string[] { "version", "option1", "option2", "--ArgumentKey1", "--ArgumentKey2:ArgumentValue2", "--ArgumentKey3" })]
      public void IdentifiesMultipleArgumentsWithOrWithoutValue(string[] desiredArgumentKeys, string[] desiredArgumentValues, params string[] args)
      {
        // Getting a little complex here so just test the test...
        if (desiredArgumentKeys.Length != desiredArgumentValues.Length)
        {
          throw new Exception($"Wrong testcases! desiredArgumentKeys.Length needs to be equal to desiredArgumentValues.");
        }

        var parser = new InputParser();
        var result = parser.ParseInput(args, _availableCommands);

        Assert.AreEqual(desiredArgumentKeys.Length, result.Arguments.Count, "Did not identify the correct number of Arguments.");

        for (int i = 0; i < desiredArgumentKeys.Length; i++)
        {
          var Argument = result.Arguments.ElementAt(i);
          Assert.AreEqual(desiredArgumentKeys[i], Argument.Key, $"Keys for Argument at index {i} are not equal.");
          Assert.AreEqual(desiredArgumentValues[i], Argument.Value, $"Values for Argument at index {i} are not equal.");
        }
      }

      [Test]
      [TestCase(new object[] { "version", "option1:value1", "option2", "--ArgumentKey1:Argument:Value1", "--ArgumentKey2" })]
      [TestCase(new object[] { "version", "option1:value:1", "option2", "--ArgumentKey1:ArgumentValue1" })]
      public void ThrowsFormatExceptionWhenUsingColonOutsideQuotes(params string[] args)
      {

        var parser = new InputParser();
        Assert.Throws<FormatException>(() => parser.ParseInput(args, _availableCommands));
      }

      [Test]
      [TestCase(new string[] { "version", "option1:\"value:1\"" }, new string[] { "value:1" })]
      [TestCase(new string[] { "version", "option1:\"value:with:multiple:colons\"" }, new string[] { "value:with:multiple:colons" })]
      public void AcceptsColonsInOptionValueWhenUsingQuotes(string[] args, params string[] desiredOptionValues)
      {
        var parser = new InputParser();
        var result = parser.ParseInput(args, _availableCommands);

        Assert.Multiple(() =>
       {
         var values = result.Options.Values.ToList();
         for (int i = 0; i < result.Options.Count; i++)
         {
           Assert.AreEqual(values[i], desiredOptionValues[i]);
         }
       });
      }
    }
  }
}
