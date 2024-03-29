﻿using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a command line argument : value | -name [value1 [.. value n], --name [value1 [.. value n]
/// <para>is an option or a parameter</para>
/// </summary>
public abstract class Arg : IArg
{
    /// <summary>
    /// app config
    /// </summary>
    protected readonly IConfiguration Config;

    /// <summary>
    /// texts
    /// </summary>
    protected readonly Texts Texts;

    /// <summary>
    /// Value converter
    /// </summary>
    protected readonly ValueConverter ValueConverter;

    /// <summary>
    /// type of the argument value
    /// </summary>
    public abstract Type ValueType { get; }

    /// <summary>
    /// generic argument
    /// </summary>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    public Arg(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
    {
        Config = config;
        Texts = texts;
        ValueConverter = valueConverter;
    }

    /// <summary>
    /// returns a syntax representation of this param
    /// </summary>
    /// <returns></returns>
    public abstract string ToSyntax();

    /// <summary>
    /// convert the value with string representation to a value of the expected type
    /// </summary>
    /// <typeparam name="T">expected type</typeparam>
    /// <param name="value">text representation of the value</param>
    /// <returns>a value of type T or null</returns>
    /// <exception cref="ArgumentException">convert error</exception>
    public T ConvertValue<T>(string value)
    {
        var convertOk = ValueConverter.ToTypedValue(
            value,
            typeof(T),
            default(T),
            out var convertedValue,
            out var possibleValues,
            null,
            false,
            true
            );

        if (!convertOk || convertedValue is null)
        {
            var values = possibleValues == null ? string.Empty :
                Texts._("PossibleValues")
                    + string.Join(',', possibleValues);
            throw new ArgumentException(
                Texts._("UnableToConvertValue", value, typeof(T).UnmangledName())
                + ", " + values);
        }
        return (T)convertedValue;
    }

    /// <inheritdoc/>
    public abstract object? GetValue();

    /// <inheritdoc/>
    public abstract bool GetIsOptional();

    /// <inheritdoc/>
    public abstract bool GetIsSet();
}
