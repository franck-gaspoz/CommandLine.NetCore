using System.Diagnostics;

using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a single value parameter argument : value
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
public class Param<T> : Arg, IParam
{
    private string DebuggerDisplay => ToGrammar();

    /// <inheritdoc/>
    public override string ToGrammar()
    {
        var val = Value is null ? "?" : Value?.ToString();
        return $"Param<{typeof(T).Name}> = {val}";
    }

    /// <summary>
    /// value
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// generic type parameter
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
