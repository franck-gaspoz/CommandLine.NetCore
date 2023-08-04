___

# ![CommandLine.NetCore](https://raw.githubusercontent.com/franck-gaspoz/CommandLine.NetCore/main/CommandLine.NetCore/assets/ascii-icon.png "CommandLine.NetCore") CommandLine.NetCore

___

**CommandLine.NetCore library** provides support to handle command line arguments (parse, validate, command pattern) for .Net Core console applications with ANSI VT support (cursor,colors,screen size) for multi-plateform (windows, linux, osx, arm) console applications using C# and .NET Core 6

[![licence mit](https://img.shields.io/badge/licence-MIT-blue.svg)](license.md) This project is licensed under the terms of the MIT license: [LICENSE.md](LICENSE.md)  
![last commit](https://img.shields.io/github/last-commit/franck-gaspoz/CommandLine.NetCore?style=plastic)
![version](https://img.shields.io/github/v/tag/franck-gaspoz/CommandLine.NetCore?style=plastic)
___

# Features

The library provides functionalities needed to build console applications running in a terminal (WSL/WSL2, cmd.exe, ConEmu, bash, ...) with text interface. That includes:

- parsing command line arguments

- command pattern helps implementing commands binded to methods from command line in a simple and regular way

- multi-language commands help configuration files

- automatic **help** command

- can compile a .exe for a a single command command, showing only the help for a specific command, or for several commands, showing a help for all commands as a shell would do

- compatible with [**AnsiVtConsole.NetCore**](https://github.com/franck-gaspoz/AnsiVtConsole.NetCore) :

    - **a text printer engine** that supports **print directives** (markup) allowing to manage console functionalities from text itself, as html would do but with a simplest syntax (that can be configured). That makes possible colored outputs, cursor control, text scrolling and also dynamic C# execution (scripting), based on **System.Console** and **ANSI VT100 / VT52 (VT100 type Fp or 3Fp, Fs, CSI, SGR)** 

    - A Ansi Parser that can identify/remove escape sequences in a text

    - The console output can be controlled by:
        - tokens in a string (print directives)
        - as string shortcuts (dynamic ansi vt strings)
        - throught API methods

# Howto

## 1. Running the command line

download the nuget from command line or add it from Visual Studio

``` dos
dotnet add package CommandLine.NetCore
```

> **Notice**
>
> When installing the package, the following files are copied into your project:
> - Config/appSettings.core.json
> - LICENSE.md
> - README.md
> - assets/ascii-icon.png
>
> you can delete any of these files **EXCEPT Config/appSettings.core.json** wich is mandatory since it contains the CommandLine.NetCore parser root configuration
>
> these files are set as `Content` and are copied to output folder on build

link to the library in your console application main class (example: Program.cs):

``` csharp
using CommandLine.NetCore;
```

from your main method, transfer control to the library **CommandLine.NetCore** :

``` csharp
/// <summary>
/// command line input
/// <para>commandName ( option (optionValue)* | parameter )* globalOption*</para>
/// </summary>
/// <param name="args">arguments</param>
/// <returns>status code</returns>
public static int Main(string[] args)
    => new CommandLineInterfaceBuilder()
        .Build(args)
        .Run();
```

That leads to the loading of any command line components like global arguments, commands and help settings
from both the library core and your own console app and parsing of the declared syntaxes and eventualy execution of the method corresponding to the matching syntax.

## 2. Testing the integrated **help** command:

Any console application built with the library **ComandLine.NetCore** implements by 
default a command named **`help`** that dump any available help about commands that are 
implemented in the software that uses the library and in the library itself. 

As an example, you can build the example application console, provided in the project `CommandLine.NetCore.Example`, 
Just execute in your favorite shell this command (available in the folder `bin/Release/net6.0`):

``` dos
./CommandLine.NetCore.Example.exe help
```

To get the help for a particular command, the syntax is `help {commandName}`. In this example you
get help about the command help:

``` dos
./CommandLine.NetCore.Example.exe help help
```

## 3. Configuring the library and a console application built with it

The library settings provides the description of the application and of the commands, and also the translation of texts.
You should override these settings according to your needs.

Every settings are pushed throught `IHostBuilder.ConfigureAppConfiguration`. 
Settings are looked up by this way, in the specified order:

provided by the library CommandLine.NetCore:

- `appSettings.core.json` : this file contains the settings needed by the core functionalities
of the library: decription of the library, texts and description of the integrated command, in
the default language (en-us)

- `appSettings.core.{culture}.json` : same as above, any of these files provides translations for the
culture specified by the tag `{culture}` according to available cultures specified in `Microsoft.`.
The settings file that matches the current platform culture is loaded if it exists.

provided by your application;

- `appSettings.json` : dscription of the commands provided by your application, the texts, and any
settings in the default language (en-us)

- `appSettings.{culture}.json` : same as above for the translations of the culture specified by the tag
`{culture}`

The settings must conform with the following conventions:

**Informations about application**

``` json
"App": {
    "Title": "CommandLine.NetCore",
    "ReleaseDate": "10/12/2022"
  }
```

**Texts**

``` json
"Texts": {
    "{TextId}": "Text"
  }
```

**Description of the commands**

``` json
"Commands": {
    "{CommandName}": {
        "Description": "short description of the command",
        "Syntax": {
            "{Syntax 1}" : "Description of the functionality provided by the syntax 1",
            ...
            "{Syntax n}" : "Description of the functionality provided by the syntax n",
        },
        "Options": {
            "{Option 1}" : "Description of the command option 1",
            ...
            "{Option n}" : "Description of the command option n",
        }
    }
  }
```

example of the command **`help`** :

``` json
"Commands": {
    "help": {
      "Description": "output a of list commands and global arguments or output help about a command",
      "Syntax": {
        "": "list all commands",
        "commandName": "help about the command with name commandName"
      },
      "Options": {
        "-v": "enable verbose: add details to normal output",
        "--info" : "output additional informations about the command line context"
      }
    }
  },
```

These settings are describing the following syntaxes for the command `help`:

``` dos
; help for a command
help {commandName} [-v] [--info]
; global help (all commands)
help [-v] [--info]
```

Command options are optionals and are available for any syntax of the command (here -v and --info). They can appears from
the position they are declared in the command syntax

**Description of the global arguments**

``` json
"GlobalOptions": {
    "{ArgumentName}": {
        "{Syntax}" : "Description of the functionality provided by the argument syntax"
    }
  }
```

Global arguments are optional and availables for any command. They must appear from the end of the command arguments

example of the global argument **`s`** :

``` json
"GlobalOptions": {
    "s": {
        "-s" : "turn off any output (silent mode)"
    }
  }
```

by convention (POSIX), single letter arguments are prefixed by `-`, whereas arguments with
several letters are prefixed by `--`

## 4. Implementing a command

A command specification and implementation is defined in a class that inherits from `CommandLine.NetCore.Services.CmdLine.Commands.Command`.

* the name of the command is `kebab case` from the name of the class (in this case **GetInfo** declares the **get-info** command)
* the command class msut have a constructor with parameter `Dependencies`. These classes are instantiated by the **dependency injector**,
thus any registered dependency can be added as a constructor parameter
* the command class must implements the method:
    ```csharp
    CommandResult Execute(ArgSet args)
    ```
* the method `Execute` declares the syntaxes of the command and the related implementations
* the method **`For`** declares a command syntax:
    ```csharp
    For(params Arg[] syntax)
    ```
- the list of arguments are specifing the command syntax
    - an `Arg` is either an `option` or a `parameter`. Their grammar is defined as this:
        - `Option ::= [-|--]{optionName}[value0..valuen]`
            - options can be expected or optionnal
            - can have from 0 to n values of a type `T`, where T can be any scalar type, a collection of scalar types (with `,` as separator) or an Enum            
            - an option can be defined with values, values are always expected
            - `Opt("x")` builds the option `x` with no expected value: `-x`
            - by convention (posix), if the length of the name of the option is greater than 1, the prefix becomes: `--`. For instance, `Opt("xy")` defines the syntax: `--xy`
            - `Opt("x",true)` builds the option `x` wich is optional in the syntax
            - `Opt<T>("value")` builds the option `value` having one expected value that must be convertible to type `T`. For instance, `Opt<int>("value")` defines an option that expect an int, like in syntax: `--value 123`
            
        - `Parameter ::= parameterValue?`
            - parameters are always expected
            - have a value of a type `T`, where T can be any scalar type, a collection of scalar types (with `,` as separator) or an Enum            
            - if a parameter if defined with a value, it is an expected word in the syntax
            - if a parameter is defined without a value, a value is expected in the syntax
            - `Param()` builds a parameter that expect a value of type `string` like in syntax: `iamastring`
            - `Param<T>()` builds a parameter that expect a value that must be convertible to type `T`. For instance, `Param<int>()` builds a parameter that expect a value of type `int`, like in syntax: `123`
            - `Param("color")` builds a parameter that is expected and being the syntax: `color`
            
* the method **`Do`** chained to a **For** indicates the method that must be executed if the syntax match the command line args:
    ```csharp
    // with no parameter and void result delegate
    Do(Action @delegate)

    // with no parameter and void result delegate
    Do(Func<OperationResult> @delegate)

    // with parameter operation context and void delegate
    Do(Action<OperationContext> @delegate)

    // with parameter operation context and OperationResult result delegate
    Do(Func<OperationContext, OperationResult> @delegate)

    // takes a method in a lambda unary call expression: () => methodName, takes a called method with no parameter, takes a called method with a default command result (code ok, result null).
    // Allows to map command arguments to method parameters and operation context
    Do(LambdaExpression expression)
    ```

    the lambda expression in the method style `Do(LambdaExpression expression)` can have these profiles:

    ```csharp
    // no parameter and no result
    void MyOperation()

    // explicit mapping of argument and no result
    void MyOperation([MapArg(1) Param<string> arg0,[MapArg(5)] Opt<bool> arg1)

    // implicit mapping of arguments and no result
    // expected arguments (arguments having expected valie(s)) are mapped according to their declaring order
    void MyOperation(Param<string> arg0,Opt<bool> arg1)

    // can also have an auto-mapped parameter to the operation context:
    // a parameter of type OperationContext can be placed anywhere in the parameters list
    void MyOperation(...,OperationContext context,..)
    ```

* methods **`For`** can be chained
* the method **`Options`** can be chained to a **For**. This method allows to declare the command options:
    ```csharp
    Options(params IOpt[] options)
    ```
* the method **`With`** launch the command executing process. First command line parsing, then syntax matching, then operation dispatch:
    ```csharp
    With(ArgSet args)
    ```

### Exemple of the command `help` defined in `CommandLine.NetCore.Commands.CmdLine`:

```csharp
// command syntax: help [commandName] [-v] [--info]
internal sealed class Help : Command
{
    protected override CommandResult Execute(ArgSet args) =>

        // syntax: help
        For()
            .Do(() => DumpHelpForAllCommands)

        // syntax: help {commandName}
        .For(Param())
            .Do(() => DumpCommandHelp)

        // any syntax accepts -v and/or --info
        .Options(Opt("v"), Opt("info"))

        // parse and run
        .With(args);

    
    private void DumpCommandHelp(Param commandName, Opt v, Opt info)
    {
        // ...
    }

    private void DumpHelpForAllCommands(Opt v, Opt info)
    {
        // ...
    }
}
```

### Exemple of the command `get-info` defined in `CommandLine.NetCore.Example.Commands.GetInfo`:

```csharp
// syntax: get-info (env -l) | (env {varName}) | console | system | --all
internal sealed class GetInfo : Command
{
    protected override CommandResult Execute(ArgSet args) =>

        // syntax: get-info env -l
        For(
            Param("env"),
            Opt("l")
            )
                .Do(DumpAllVars)

        // syntax: get-info env {varName}
        .For(
            Param("env"),
            Param())
                .Do(() => DumpEnvVar)

        // syntax: get-info console
        .For(
            Param("console"))
                .Do(DumpConsole)

        // syntax: get-info system
        .For(
            Param("system"))
                .Do(DumpSystem)

        // syntax: get-info --all
        .For(
            Opt("all"))
                .Do(DumpAll)

        // parse and run
        .With(args);

    private void DumpEnvVar(Param envVarName)
    {
        // ...
    }
}
```

# Versions history

`1.0.9` - 05/02/2023
- fix bug GetValue when not setted option
- add support of mapping for parameters having arguments values types of command lambda operation
- add SyntaxMatcherDispatcherException and subclasses

`1.0.8` - 01/14/2023
- add single command mode allowing to build an executable for only one command and eventually without the global help
- change editor config and code cleanup
- packages update

`1.0.7` - 01/13/2023
- add global option `--no-color` that turn off ansi/vt outputs
- fix auto syntax -h was not passing global arguments
- new RunCommand in current host
- improve help output
- embed symbols and sources

`1.0.6` - 01/11/2023
- fix MAJOR bug in command options parsing. Were not recongnized correctly
- `AppHostBuilder` moved to namespace `CommandLine.NetCore.Services.AppHost`
- fix auto command -h (Command.RunCommand) didn't call back configure and build delegates
- fix support of -v and --info in -h auto syntax
- doc update

`1.0.5` - 01/05/2023
- fix nupkg: adding the package to a project now deploy files Config/appSettings.core.json, LICENSE.md, README.md, assets/ascii-icon.png in your project. These files are configured as 'Content' and are deployed in the `bin` folder. 
You can remove any of these files **EXCEPT Config/appSettings.core.json** wich is mandatory since it contains the CommandLine.NetCore parser root configuration
- fix doc

`1.0.4` - 01/04/2023
- fix nupkg

`1.0.3` - 01/04/2023
- fix nupkg

`1.0.2` - 01/04/2023
- fix doc

`1.0.1` - 01/04/2023
- add CommandContext to lambda operations method
- add support of abstract classes that inherits from command
- rename OperationContext by CommandContext
- fix bug command options were always set in delegate for -h

`1.0.0` - 03/01/2023
- init

___

