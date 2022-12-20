
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Commands;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Example.Commands;

internal sealed class GetInfoCommand : Command
{
    public GetInfoCommand(
        IConfiguration config,
        IAnsiVtConsole console,
        Texts texts) :
            base(config, console, texts)
    {
    }

    protected override int Execute(ArgSet args)
        => throw new NotImplementedException();
}
