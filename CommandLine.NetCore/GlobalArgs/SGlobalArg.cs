using CommandLine.NetCore.Service.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.GlobalArgs;

/// <summary>
/// global argument: -s
/// </summary>
public class SGlobalArg : Opt
{
    public SGlobalArg(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter,
        bool isOptional = false)
            : base("s", config, texts, valueConverter, isOptional)
    {
    }
}

