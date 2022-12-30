using System.Diagnostics;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a single value parameter argument : value
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
public class Param<T> : Arg, IParam
{
    private string DebuggerDisplay => ToSyntax();

    /// <inheritdoc/>
    public override string ToSyntax()
    {
        var val = Value.ToText();
        return $"Param<{typeof(T).Name}>{val}";
    }

    private T? _value;

    /// <summary>
    /// value
    /// </summary>
    public T? Value
    {
        get => _value;
        set
        {
            _value = value;
            StringValue = _value?.ToString();
        }
    }

    /// <inheritdoc/>
    public string? StringValue { get; private set; }

    /// <inheritdoc/>
    public bool IsExpectingValue { get; private set; }

    /// <inheritdoc/>
    public void SetValue(string value) =>
        Value = ConvertValue<T>(value);

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
    {
        StringValue = value;
        IsExpectingValue = value is null;
        Value = ConvertValue<T>(value);
    }
}
