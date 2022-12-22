using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a single value parameter argument : value
/// </summary>
public class Param<T> : Arg
{
    /// <summary>
    /// value
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// generic argument
    /// </summary>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    /// <param name="value">value from command line</param>
    public Param(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter,
        string? value)
        : base(config, texts, valueConverter)
            => Value = ConvertValue<T>(value);
}
