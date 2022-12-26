___

# ![CommandLine.NetCore](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/ascii-icon.png "AnsiVtConsole.NetCore") CommandLine.NetCore

___

**CommandLine.NetCore library** provides support to handle command line arguments (parse, validate, command pattern) for .Net Core console applications with ANSI VT support (cursor,colors,screen size) for multi-plateform (windows, linux, osx, arm) console applications using C# and .NET Core 6

[![licence mit](https://img.shields.io/badge/licence-MIT-blue.svg)](license.md) This project is licensed under the terms of the MIT license: [LICENSE.md](LICENSE.md)  
![last commit](https://img.shields.io/github/last-commit/franck-gaspoz/CommandLine.NetCore?style=plastic)
![version](https://img.shields.io/github/v/tag/franck-gaspoz/CommandLine.NetCore?style=plastic)
___

# Features

The library provides functionalities needed to build console applications running in a terminal (WSL/WSL2, cmd.exe, ConEmu, bash, ...) with text interface. That includes:

- parsing of command line arguments

- command pattern helps implementing commands invokable from command line in a simple and regular way

- multi-language commands help configuration files

- automatic **help** command

- compatibile with [**AnsiVtConsole.NetCore**](https://github.com/franck-gaspoz/AnsiVtConsole.NetCore) :

    - **a text printer engine** that supports **print directives** allowing to manage console functionalities from text itself, as html would do but with a simplest grammar (that can be configured). That makes possible colored outputs, cursor control, text scrolling and also dynamic C# execution (scripting), based on **System.Console** and **ANSI VT100 / VT52 (VT100 type Fp or 3Fp, Fs, CSI, SGR)** 

    - The console output can be controlled by:
        - tokens in a string (print directives)
        - as string shortcuts (dynamic ansi vt strings)
        - throught API methods

# Howto

## 1. Run the command line

download the nuget from command line or add it from Visual Studio

``` dos
@rem version 1.0.0 or any new one
dotnet add package CommandLine.NetCore --version 1.0.0
```

link to the library in your console application main class (example: Program.cs):

``` csharp
using CommandLine.NetCore;
```

from your main method, transfer control to the library **CommandLine.NetCore** :

``` csharp
// <summary>
/// command line input
/// <para>commandName (commandArgs|globalArg)*</para>
/// </summary>
/// <param name="args">arguments</param>
/// <returns>status code</returns>
public static int Main(string[] args)
    => CommandLineInterface.Run(args);
```

## 2. Test the integrated **help** command:

Any console application built with the library **ComandLine.NetCore** implements by 
default a command named **help** that dump any available help about commands that are 
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

## 3. Configure the library and a console application built with it

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
        }
    }
  }
```

example of the command **`help`** :

``` json
"Commands": {
    "Help": {
        "Description": "output a of list of all commands and global arguments or output help about a command",
        "Syntax": {
            "help" : "output a of list of all commands and global arguments",
            "help commandName" : "output help for the command with the name 'commandName'"
        }
    }
  }
```

**Description of the global arguments**

``` json
"GlobalOptions": {
    "{ArgumentName}": {
        "{Syntax}" : "Description of the functionality provided by the argument syntax"
    }
  }
```

example of the global argument **s** :

``` json
"GlobalOptions": {
    "s": {
        "-s" : "turn off any output (silent mode)"
    }
  }
```

by convention (POSIX), single letter arguments are prefixed by `-`, whereas arguments with
several letters are prefixed by `--`

## 4. Implement a command



# Version history

1.0.0 init

___

