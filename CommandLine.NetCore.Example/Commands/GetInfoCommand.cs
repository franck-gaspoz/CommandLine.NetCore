
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

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
        Parser parser,
        Texts texts) :
            base(config, console, texts, argBuilder, parser)
    { }

    /// <inheritdoc/>
    protected override CommandResult Execute(ArgSet args) =>

        // env -l

        For(
            Param("env"),
            Opt("l")
            )
                .Do(DumpAllVars)

        // env varName

        .For(
            Param("env"),
            Param())
                .Do(DumpEnvVar)

        .With(args);

    private OperationResult DumpEnvVar(Grammar grammar) => new(Globals.ExitOk);

    private OperationResult DumpAllVars(Grammar grammar) => new(Globals.ExitOk);
}
