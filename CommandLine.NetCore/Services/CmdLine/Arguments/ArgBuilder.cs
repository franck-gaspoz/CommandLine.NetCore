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

    public Opt Opt(
        string name,
        bool isOptional,
        int valueCount = 0
        )
        => new Opt(name, _config, _texts, _valueConverter, isOptional, valueCount);

    public Opt<T> Opt<T>(string name, bool isOptional)
        => new Opt<T>(name, _config, _texts, _valueConverter, isOptional, 1);

    public Param<T> Param<T>(string? value = null)
        => new Param<T>(_config, _texts, _valueConverter, value);

    public Param Param(string? value = null)
        => new Param(_config, _texts, _valueConverter, value);
}
