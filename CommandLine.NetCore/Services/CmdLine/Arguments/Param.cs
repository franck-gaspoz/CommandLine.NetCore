
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// param of type string
/// </summary>
public class Param : Param<string>
{
    /// <summary>
    /// string parameter
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
        : base(config, texts, valueConverter, value)
            => Value = ConvertValue<string>(value);
}
