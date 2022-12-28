using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.GlobalOpts;

/// <summary>
/// global argument: -s 
/// <para>turn off output</para>
/// </summary>
public class S : GlobalOpt
{
    public S(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
            : base(config, texts, valueConverter)
    {
    }
}

