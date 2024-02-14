___

# ![CommandLine.NetCore](https://raw.githubusercontent.com/franck-gaspoz/CommandLine.NetCore/main/CommandLine.NetCore/assets/ascii-icon.png "CommandLine.NetCore") CommandLine.NetCore

___

**CommandLine.NetCore library** provides support to handle command line arguments (parse, validate, command pattern) for .Net Core console applications with ANSI VT support (cursor,colors,screen size) for multi-plateform (windows, linux, osx, arm) console applications using C# and .NET Core 6

[![licence mit](https://img.shields.io/badge/licence-MIT-blue.svg)](license.md) This project is licensed under the terms of the MIT license: [LICENSE.md](LICENSE.md)  
![last commit](https://img.shields.io/github/last-commit/franck-gaspoz/CommandLine.NetCore?style=plastic)
![version](https://img.shields.io/github/v/tag/franck-gaspoz/CommandLine.NetCore?style=plastic)
___

# Index

- [Features](#features)
- [How to](#howto)
    - [1. Running the command line](#1-running-the-command-line)
    - [2. Testing the integrated **help** command](#2-testing-the-integrated-help-command)
    - [3. Configuring the library and a console application built with it](#3-configuring-the-library-and-a-console-application-built-with-it)
    - [4. Implementing a command](#4-implementing-a-command)

        - [4.1 Implementing a command with a <b>class</b>](#4-1-implementing-a-command-with-a-class)
            - [Arguments to concrete types mapping of Do(LambdaExpression expression) expression parameters](#arguments-to-concrete-types-mapping-of-dolambdaexpression-expression-expression-parameters)
            - [Exemple of the command `help` defined in `CommandLine.NetCore.Commands.CmdLine`](#exemple-of-the-command-help-defined-in-commandlinenetcorecommandscmdline)
            - [Exemple of the command `get-info` defined in `CommandLine.NetCore.Example.Commands.GetInfo`](#exemple-of-the-command-get-info-defined-in-commandlinenetcoreexamplecommandsgetinfo)
        - [4.2 Implementing a <b>classless</b> command with a lambda expression](#4-2-implementing-a-classless-command-with-a-lambda-expression) 
    
    - [5. Setup an unique command console app (without command argument)](#5-setup-an-unique-command-console-app-without-command-argument)
    - [6. Command classes attributes](#6-command-classes-attributes)
    - [7. Debug and troobleshoot](#7-debug-and-troobleshoot)

- [Versions history](#versions-history)

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
>
> the file **Config/appSettings.core.json** is mandatory since it contains the CommandLine.NetCore parser root configuration


link to the library in your console application main class (example: Program.cs):

``` csharp
using CommandLine.NetCore.Services.CmdLine;
```

from your **main** method of your app or using **top level statements**, transfer control to the library **CommandLine.NetCore** :

`Program.cs`

```csharp
namespace MyConsoleApp;

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

or using a top level statement:

```csharp
new CommandLineInterfaceBuilder()
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

### 4.1. Implementing a command with a class

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
            - options can have from 0 to n values
            - can have from 0 to n values of a type `T`, where T can be any scalar type, a collection of scalar types (with `,` as separator) or an Enum            
            - an option can be defined with values, values are always expected
            - `Opt("x")` builds the option `x` with no expected value: `-x`
            - by convention (posix), if the length of the name of the option is greater than 1, the prefix becomes: `--`. For instance, `Opt("xy")` defines the syntax: `--xy`
            - `Opt("x",true)` builds the option `x` wich is optional in the syntax
            - `Opt<T>("value")` builds the option `value` having one expected value that must be convertible to type `T`. For instance, `Opt<int>("value")` defines an option that expect an int, like in syntax: `--value 123`
            - **Flag** is a construct of an **Opt** without value

        - `Parameter ::= parameterValue?`
            - parameters have exactly one value
            - parameters are always expected
            - have a value of a type `T`, where T can be any scalar type, a collection of scalar types (with `,` as separator) or an Enum            
            - if a parameter if defined with a value, it is an expected word in the syntax
            - if a parameter is defined without a value, a value is expected in the syntax
            - `Param()` builds a parameter that expect a value of type `string` like in syntax: `iamastring`
            - `Param<T>()` builds a parameter that expect a value that must be convertible to type `T`. For instance, `Param<int>()` builds a parameter that expect a value of type `int`, like in syntax: `123`
            - `Param("color")` builds a parameter that is expected and being the syntax: `color`
            
* the method **`Do`** chained to a **For** indicates the method that must be executed if the syntax match the command line args:
    
    - the most common way to define the operation method si the lambda expression, since it allows to use a standard method with concrete typed parameters (not Opt,Param,.. but the values types inside it) :

    ```csharp
    // takes a method in a lambda unary call expression: () => methodName, takes a called method with no parameter, takes a called method with a default command result (code ok, result null).
    // Allows to map command arguments to method parameters and operation context
    Do(LambdaExpression expression)
    ```
    
    - others operation methods prototypes that are accepted:

    ```csharp
    // with no parameter and void result delegate
    Do(Action @delegate)

    // with no parameter and void result delegate
    Do(Func<OperationResult> @delegate)

    // with parameter operation context and void delegate
    Do(Action<CommandContext> @delegate)

    // with parameter operation context and OperationResult result delegate
    Do(Func<CommandContext, OperationResult> @delegate)
    ```

    - the lambda expression in the method style `Do(LambdaExpression expression)` can have one of these profiles:

        - the most practical is the use of concrete values types (not Opt,Param,.. but the values types inside it):

    ```csharp
    // arguments mapping to concrete types
    // also accepts an CommandContext parameter placed anywhere
    // also accepts explicit mapping of arguments, with positional references in syntax, and no result
    // avoid repeating the command arguments declarations (Param, Opt)
    void MyOperation( string arg0, bool arg1 , ..)
    ```

        - others lambda expressions prototypes that are accepted:

    ```csharp
    // no parameter and no result
    void MyOperation()

    // explicit mapping of arguments, with positional references in syntax, and no result
    void MyOperation([MapArg(1) Param<string> arg0,[MapArg(5)] Opt<bool> arg1)

    // implicit mapping of arguments and no result
    // expected arguments (arguments having expected valie(s)) are mapped according to their declaring order
    void MyOperation(Param<string> arg0,Opt<bool> arg1)

    // can also have an auto-mapped parameter to the operation context:
    // a parameter of type CommandContext can be placed anywhere in the parameters list
    void MyOperation(...,CommandContext context,..)    
    ```

* methods **`For`** can be chained
* the method **`Options`** can be chained to a **For**. This method allows to declare the command global options (avalaible for any syntax of the command) :
    ```csharp
    Options(params IOpt[] options)
    ```
* the method **`With`** launch the command executing process. First command line parsing, then syntax matching, then operation dispatch:
    ```csharp
    With(ArgSet args)
    ```

### Arguments to concrete types mapping of `Do(LambdaExpression expression)` expression parameters:

#### Flags

| argument constructor | possible corresponding type(s) |
|---|---|
| `Flag("argName")` | `bool` |
| `Flag("argName",isOptional: true)` | `bool` |

#### Options

| argument constructor | possible corresponding type(s) |
|---|---|
| `Opt("argName")` <br> `Opt("argName",valueCount:0)`  | as it is expected to exactly match the syntax **argName** (expected values count = 0), this arg must not be mapped |
| `Opt("argName",isOptional: true)` <br> `Opt("argName",isOptional: true,valueCount:0)` | `bool` (because expected values count = 0, acts as `Flag` in that case) |
| | |
| `Opt("argName",valueCount:1)` | `string` |
| `Opt("argName",isOptional: true,valueCount:1)` | `string?` or null |
| | |
| `Opt("argName",valueCount:2..n)` | `List<string>` |
| `Opt("argName",isOptional: true,valueCount:2..n)` | `List<string>?` or null |
| | |
| `Opt<T>("argName",valueCount:0)` | as it is expected to exactly match the syntax **argName** (expected values count = 0), this arg must not be mapped |
| `Opt<T>("argName",isOptional: true,valueCount:0)` | `bool` (because expected values count = 0, acts as `Flag` in that case) |
| | |
| `Opt<T>("argName")` <br> `Opt<T>("argName",valueCount:1)` | `T` |
| `Opt<T>("argName",isOptional: true)` <br> `Opt<T>("argName",isOptional: true,valueCount:1)` | `T?` or default |
| `Opt<T>("argName",valueCount:2..n)` | `List<T>` |
| `Opt<T>("argName",isOptional: true,valueCount:2..n)` | `List<T>?` or null |

#### Parameters

| argument constructor | possible corresponding type(s) |
|---|---|
| `Param()` | `string` |
| `Param_T()` | `T` |
| `Param("keyWord")` | as it is expected to exactly match the syntax **keyWord**, this arg must not be mapped |

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

    
    private void DumpCommandHelp(string commandName, bool verbose, bool info)
    {
        // ...
    }

    private void DumpHelpForAllCommands(bool verbose, bool info)
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

    private void DumpEnvVar(string envVarName)
    {
        // ...
    }
}
```

### 4.2. Implementing a classless command with a lambda expression

You can dynamically declares and specify a command directly with `CommandLineInterfaceBuilder` fluent methods:

* **`AddCommand`** adds a new dynamic command named *commandName* that is specified by the delegate type `DynamicCommandSpecificationDelegate` 

```csharp
CommandInterfaceBuilder AddCommand( commandName , DynamicCommandSpecificationDelegate )
```

* the delegate `DynamicCommandSpecificationDelegate` provides a `CommandBuilder` and a `DynamicCommandContext` that provides methods for specify and implements the dynamic command
    
    ```chsarp
    (builder, ctx) => ...
    ```

The `CommandBuilder` exposes the following methods:

* the **`For`** method is the way to specify a command arguments, help and body (it is the same than in the abstract class `Command`)

```csharp
SyntaxExecutionDispatchMapItem For(params Arg[] syntax)
```

* methods for building arguments: **`Opt`**, **`Flag`**, **`Param`**, **`OptSet`**

* **`Help`** for dynamically (outside of appsettings) declares the command syntax help:

```csharp
CommandBuilder Help(string text, string? culture = null)
```

* **`Tag`** associates one or several tags to the command specification
* **`Package`** specify a package the command belongs to

The `SyntaxExecutionDispatchMapItem` has now specific methods for building dynamic commands:

* new `Do` methods with generic types that takes an **Action delegate** as the command implementation

```csharp
SyntaxMatcherDispatcher Do<T1>(Action<T1> action)
SyntaxMatcherDispatcher Do<T1, T2>(Action<T1, T2> action)
...
SyntaxMatcherDispatcher Do<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
```

* new `Do` for commands with returns (pending impl.)

```csharp
SyntaxMatcherDispatcher Do(Func<CommandContext, CommandLineResult> @delegate)
```

* **`Help`** for dynamically (outside of appsettings) declares the command syntax help:

```csharp
SyntaxExecutionDispatchMapItem Help(
        string argsSyntax,
        string description,
        string? culture = null)
```

### Example of the command `add` defined in `CommandLine.NetCore.Example.Program`:

```csharp
// the command line interface builder still loads commands defined in classes
new CommandLineInterfaceBuilder()

    // we add a command dynamically specified
    .AddCommand("add", (builder, ctx) => builder

        .Help("add operator")
        .Tag(Tags.Math, Tags.Text)

        .For(builder.Param<int>(), builder.Param<int>(), builder.Param<int>())
            .Help("x y z", "returns x+y+z")
            .Do((int x, int y, int z) =>
            {
                ctx.Console.Out.WriteLine($"{x}+{y}+{z}={x + y + z}");
            })

        .For(builder.Param<int>(), builder.Param<int>())
            .Help("x y", "returns x+y")
            .Do((int x, int y) =>
            {
                ctx.Console.Out.WriteLine($"{x}+{y}={x + y}");
            })

        // ...
```

## 5. Setup an unique command console app (without command argument)
0
You can prepare a console application that run immediately a specific command at launch and that doesn't requires a command name argument,
by activating this option in the **main** of your app or using **top level statements** as shown below:

`Program.cs`

```csharp
using CommandLine.NetCore.Services.CmdLine;

new CommandLineInterfaceBuilder()

    // add this for single command mode (here: only get-info, no global help)
    // case of a class command
    .ForCommand<GetInfo>()
    // case of a dynamic command (here: only add, no global help)
    //.ForCommand("add")

    // add this to avoid global help of the command line parser
    .DisableGlobalHelp()
    
    .Build(args)
    .Run();
```

This example will produce an executable that do not accept a **`command name parameter`** neither the **`help`** global command.
If the program is compiled as `MyConsoleApp.exe` the following command lines are accepted:

- get help about the get-info command:

```dos
MyConsoleApp.exe -h
```

```dos
┌──────────────────────────────────────────────────┐
│ CommandLine.NetCore.Example (1.0.9.0 05/08/2023) │
└──────────────────────────────────────────────────┘

sample command that output informations about system and console

-h : help about this command
--all : output all infos
console : dump infos about console
env -l : list of environment variables names and values
env varName : dump environment variable value with name varName
system : dump infos about system
```

- run the get-info command:

```dos
MyConsoleApp.exe system
```

```dos
system informations:

Operanting System = Microsoft Windows NT 10.0.22621.0
ProcArch = AMD64
Processor Model = Intel64 Family 6 Model 165 Stepping 2, GenuineIntel
Processor Level = 6
System Directory = C:\WINDOWS\system32
Processor Count = 12
User Domain Name = LAPTOP-R3538U70
User Name = franc
Version = 6.0.20
C:\ =
C:\ Volume Label = Windows
C:\ Drive Type = Fixed
C:\ Drive Format = NTFS
C:\ Total Size = 511280410624
C:\ Available FreeSpace = 98612314112
```

## 6. Command classes attributes

- **`[IgnoreCommand]`** : if placed above a command class declaration, the command will be ignored by the command classes loader
- **`[Tag(tag1,..,tagn)]`** : when placed above a command class declaration, this associates one or several tags to the command specification
- **`[Package(name)]`** : when placed above a command class declaration, specify that the command belongs to a named package. When not specified, command package is `global`

## 7. Debug and troobleshoot

### Integrated options

Integrated command line parser options may help the command developer to fix issues:

#### parser traces

`--parser-logging logLevel` enable display of parser syntaxes analysis detailed informations. Possibles values from Microsoft.Extensions.Logging.LogLevel. 
If `Trace` or `Debug` the parser add detailed informations about the parsed syntaxes

```csharp
CommandLine.NetCore.Example.exe help --parser-logging Debug

HelpAboutCommandSyntax: 0:Opt<String>-h 1:Opt?<String>-v 2:Opt?<String>--info 3:Opt?<String>-v 4:Opt?<String>--info : match=False
DumpHelpForAllCommands: 0:Opt?<String>-v 1:Opt?<String>--info 2:Opt?<String>-v 3:Opt?<String>--info : match=True
DumpCommandHelp: 0:Param<String>? 1:Opt?<String>-v 2:Opt?<String>--info 3:Opt?<String>-v 4:Opt?<String>--info : match=False
```

#### parser setup

`--exclude-ambiguous-syntax` exclude any ambiguous syntax when parsing command line arguments. By default, the first matching syntax is selected in the command line arguments parser. 
If this option is set syntaxes of a command can't be ambiguous

# Versions history

`1.0.22`,`1.0.23` - 02/14/2024
- fix version number of app in help title box

`1.0.21` - 02/12/2024		
- add support for environment variables. switch config by environment from DOTNET_ENVIRONMENT
- fix AppHostConfiguration service registering

`1.0.20` - 02/12/2024		
- add support for settings with Environment name

`1.0.19` - 02/01/2024		
- fix command get-info --all syntax was not recognized

`1.0.18` - 02/01/2024
- fix bug command without tag not displayed in help command list

`1.0.12,1.0.13,1.0.14,1.0.15,1.0.16,1.0.17` - 02/01/2024
- set Opt default value count to 1 (distinguish from Flag)
- fix nullable parameter identification in case of classes (string, ..)
- add command class attribute Tag,Namespace,Package + dynamic commands specification method Tag,Namespace,Package
- fix dynamic commands set injection scope
- /!\ breaking change: removed class CommandLineResult, use CommandResult instead
- add tests project
- improve help display
- fix nuget
- fix help -t behavior
- doc update

`1.0.11` - 10/01/2024 (since 09/12/2023)
- add possiblity to declare and implement a command using uniquely a fluent syntax and no class (dynamic commands)
- add support for actions with typed parameters in SyntaxExecutionDispatchMapItem
- fix value was not nullable in Param_T when T is not a class, for instance, Param{int} always had value 0 (=default(T))
- add ignore command attribute
- more properties in CommandContext
- add initialization errors collect and display
- renamings (eg. OperationResult)
- update as consts of null syntax name and null value text
- add support of dynamic and localized configuration
- fix help support of dynamic polymorphic commands syntaxes
- doc update

`1.0.10` - 08/11/2023
- add support of mapping to array parameters when possible (instead of List) in command lambda operations
- add global option --disable-global-help
- doc update

`1.0.9` - 08/06/2023
- add support of mapping for parameters having arguments concrete values types in command lambda operation (not Opt,Param,.. but the values types inside it)
- fix bug GetValue when not setted option
- migrate help,test and get-info commands operations methods with concrete type mapping
- improve mapping errors feedback
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
- rename CommandContext by CommandContext
- fix bug command options were always set in delegate for -h

`1.0.0` - 03/01/2023
- init

___

