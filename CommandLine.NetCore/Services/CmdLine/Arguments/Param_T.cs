﻿using System.Diagnostics;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a single (mandatory) value parameter argument, that is expected to match a syntax (value provided in syntax definition) or that is free (value not provided in syntax definition)
/// </summary>
[DebuggerDisplay("{_debuggerDisplay}")]
public class Param<T> : Arg, IParam
{
    string _debuggerDisplay => ToSyntax();

    /// <inheritdoc/>
    public override string ToSyntax()
    {
        var val = IsNull ? ObjectExt.ToText(null)
            : Value.ToText();
        return $"Param<{typeof(T).Name}>{val}";
    }

    /// <inheritdoc/>
    public override Type ValueType => typeof(T);

    T? _value;

    /// <summary>
    /// indicates if Value is null
    /// </summary>
    public bool IsNull { get; private set; }

    /// <summary>
    /// value
    /// </summary>
    public T? Value
    {
        get => _value;
        set
        {
            IsNull = false;
            _value = value;
            StringValue = _value?.ToString();
        }
    }

    /// <inheritdoc/>
    public string? StringValue { get; private set; }

    /// <inheritdoc/>
    public bool IsExpectingValue { get; private set; }

    /// <inheritdoc/>
    public override bool GetIsOptional() => false;

    /// <inheritdoc/>
    public override bool GetIsSet() => !IsNull;

    /// <inheritdoc/>
    public override object? GetValue()
        => IsNull ? null : _value;

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
        if (value is not null)
            Value = ConvertValue<T>(value);
        else
            IsNull = true;
    }
}
