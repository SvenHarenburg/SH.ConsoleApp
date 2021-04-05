# SH.ConsoleApp

SH.ConsoleApp is a small framework aiming to help with creating .NET Console Applications by utilizing the power of reflection and the [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host).

[![Build Status](https://sharenburg.visualstudio.com/SH.ConsoleApp/_apis/build/status/SvenHarenburg.SH.ConsoleApp?branchName=main)](https://sharenburg.visualstudio.com/SH.ConsoleApp/_build/latest?definitionId=1&branchName=main)

---

## Features
The framework provides the following features:

- Detection of CommandGroups and Commands by searching the assembly for classes and functions with applied CommandGroup- or Command-Attributes.
- Parsing and passing of options and arguments to function-parameters with applied CommandOption- and CommandArgument-Attributes.
- Automatic creation of help commands.
- Dependency Injection & Logging by utilizing the [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host).
- Console Utilities
  - Formatting for writing dictionaries and tables to the console.
  - Selection Menu: Writes a menu with options and lets the user select one of the options by using the arrow-keys and enter.

---

## Quickstart

- Install NuGet package [SH.ConsoleApp](https://www.nuget.org/packages/SH.ConsoleApp/)
- Bootstrap the [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host) and use the **RunConsoleAppEngineAsync** extension provided by the library.  
Sample Program.cs:
```c#
  class Program
  {
    static async Task Main(string[] args)
    {
      await Host.CreateDefaultBuilder(args).RunConsoleAppEngineAsync(args);
    }
  }
```
- Create a CommandGroup-class anywhere in the Assembly:
```c#
  [CommandGroup("quickstart","Contains Quickstart commands.")]
  public class QuickstartCommand
  {
    [Command("execute","Executes the Quickstart command.")]
    public void Execute()
    {
      Console.WriteLine($"Executing the Quickstart command.");
    }
  }
```
- Run the application providing the arguments "quickstart execute" and the command will be executed. The output should be similar to the following. Near the end it will show that the Command has been executed.
```
info: SH.ConsoleApp.Engine[0]
      EngineService => Register
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Repositories\QuickstartTestApp\QuickstartTestApp\bin\Debug\net5.0
Executed the quickstart command.
info: Microsoft.Hosting.Lifetime[0]
      Application is shutting down...
```
