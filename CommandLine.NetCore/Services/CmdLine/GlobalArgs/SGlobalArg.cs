using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Service.CmdLine.GlobalArgs;

internal class SGlobalArg : GlobalArg
{
    public SGlobalArg(
        IConfiguration config,
        Texts texts) : base("s", config, texts)
    {
    }
}

