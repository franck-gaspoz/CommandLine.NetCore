using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// arg builder
/// </summary>
public sealed class ArgBuilder
{
    readonly IConfiguration _config;
    readonly Texts _texts;
    readonly ValueConverter _valueConverter;

    /// <summary>
    /// arg builder
    /// </summary>
    /// <param name="config">config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    public ArgBuilder(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
    {
        _valueConverter = valueConverter;
        _config = config;
        _texts = texts;
    }

    /// <summary>
    /// build a new option
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <param name="valueCount">value count</param>
    /// <returns>Opt</returns>
    public Opt Opt(
        string name,
        bool isOptional,
        int valueCount = 0
        )
        => new(name, _config, _texts, _valueConverter, isOptional, valueCount);

    /// <summary>
    /// build a new flag
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Opt</returns>
    public Flag Flag(
        string name,
        bool isOptional)
        => new(name, _config, _texts, _valueConverter, isOptional);

    /// <summary>
    /// build a new generic option
    /// </summary>
    /// <typeparam name="T">type of options values</typeparam>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Opt{T}</returns>
    public Opt<T> Opt<T>(string name, bool isOptional)
        => new(name, _config, _texts, _valueConverter, isOptional, 1);

    /// <summary>
    /// build a new parameter
    /// </summary>
    /// <typeparam name="T">type of parameter value</typeparam>
    /// <param name="value"></param>
    /// <returns>Param{T}</returns>
    public Param<T> Param<T>(string? value = null)
        => new(_config, _texts, _valueConverter, value);

    /// <summary>
    /// build a new string parameter
    /// </summary>
    /// <param name="value">eventual value</param>
    /// <returns>Param</returns>
    public Param Param(string? value = null)
        => new(_config, _texts, _valueConverter, value);
}
