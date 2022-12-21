using CommandLine.NetCore.Service.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.GlobalArgs;

/// <summary>
/// global argument: -s
/// </summary>
public class SGlobalArg : Arg
{
    public SGlobalArg(
        IConfiguration config,
        Texts texts) : base("s", config, texts)
    {
    }
}

