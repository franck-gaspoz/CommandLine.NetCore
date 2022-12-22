﻿
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Commands;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

using static CommandLine.NetCore.Services.CmdLine.Globals;

namespace CommandLine.NetCore.Example.Commands;

internal sealed class GetInfoCommand : Command
{
    //private readonly Arg _env;
    /*
     * syntax list:
     * getinfo env varName      : dump environment variable value
     * getinfo env -l           : list of environment variables names and values
     * getinfo console          : dump infos about console
     * getinfo system           : dump infos about system
     * getinfo --all            : list all infos
     */

    public GetInfoCommand(
        IConfiguration config,
        IAnsiVtConsole console,
        ArgBuilder argBuilder,
        Texts texts) :
            base(config, console, texts, argBuilder, 1, 2)
    {
        // env envVarName
        // Value<string> = env , Value<string> = varName

        // env -l
        // Value<string> = env , Arg = l,0
    }

    protected override int Execute(ArgSet args)
    {
        args.MatchSyntax(
            Param("env"),
            Param()
            );

        args.MatchSyntax(
            Param("env"),
            Opt("l")
            );

        return ExitOk;
    }
}
