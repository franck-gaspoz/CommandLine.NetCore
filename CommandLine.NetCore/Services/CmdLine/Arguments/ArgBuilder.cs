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
    private readonly ValueConverter _valueConverter;

    public ArgBuilder(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
    {
        _valueConverter = valueConverter;
        _config = config;
        _texts = texts;
    }

    public Opt Arg(
        string name,
        int valueCount = 0
        )
        => new Opt(name, _config, _texts, _valueConverter, valueCount);
}
