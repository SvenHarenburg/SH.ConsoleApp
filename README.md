# SH.ConsoleApp

SH.ConsoleApp is a small framework aiming to help with creating .NET Console Applications by utilizing the power of reflection and the [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host).

[![Build Status](https://sharenburg.visualstudio.com/SH.ConsoleApp/_apis/build/status/SvenHarenburg.SH.ConsoleApp?branchName=main)](https://sharenburg.visualstudio.com/SH.ConsoleApp/_build/latest?definitionId=1&branchName=main)

## Features
The framework provides the following features:

- Parsing of Console args and routing to specific command-functions matching the parsed signature including parameters.
- Automatic creation of help commands.
- Dependency Injection & Logging by utilizing the [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host).
- Console Utilities
  - Formatting for writing dictionaries and tables to the console.
  - Selection Menu: Writes a menu with options and lets the user select one of the options by using the arrow-keys and enter.

## Requirements

- .NET Standard 2.1

## Getting started

Please refer to the [Quickstart-page](https://github.com/SvenHarenburg/SH.ConsoleApp/wiki/Quickstart) on the wiki on how to get started.
