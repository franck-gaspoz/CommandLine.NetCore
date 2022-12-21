using CommandLine.NetCore.Service.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// arg builder
/// </summary>
public sealed class ArgBuilder
{
    private readonly IConfiguration _config;
    private readonly Texts _texts;

    public ArgBuilder(
        IConfiguration config,
        Texts texts)
    {
        _config = config;
        _texts = texts;
    }

    public Arg Arg(
        string name,
        int valueCount = 0
        )
        => new Arg(name, _config, _texts, valueCount);
}
