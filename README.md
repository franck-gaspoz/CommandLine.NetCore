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

# Usage

``` csharp
using CommandLine.NetCore;
```

# Version history

1.0.0 new library

___

