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
            base(config, console, texts, argBuilder)
    { }

    public override int Execute(ArgSet args)
    {
        // getinfo env [-o fic.txt] varName
        args.MatchSyntax(
            Param("env"),
            Opt("o", true, 1),
            Opt("l")
            );

        /*args.MatchSyntax(
            Param("env"),
            Param()
            );*/

        return ExitOk;
    }
}
