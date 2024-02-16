using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// abstract command : Args
/// </summary>
public abstract partial class Command
{
    /// <summary>
    /// build a new option
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <param name="valueCount">value count (default 1)</param>
    /// <returns>Opt</returns>
    protected Opt Opt(string name, bool isOptional = false, int valueCount = 1)
        => _builder.Opt(name, isOptional, valueCount);

    /// <summary>
    /// build a new flag (option with no possibilty of values)
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Flag</returns>
    protected Flag Flag(string name, bool isOptional = true)
        => _builder.Flag(name, isOptional);

    /// <summary>
    /// build a new generic option
    /// </summary>
    /// <typeparam name="T">type of options values</typeparam>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <param name="valueCount">value count (default 1)</param>
    /// <returns>Opt{T}</returns>
    protected Opt<T> Opt<T>(string name, bool isOptional = false, int valueCount = 1)
            //where T : struct
            => _builder.Opt<T>(name, isOptional, valueCount);

    /// <summary>
    /// build a new parameter
    /// </summary>
    /// <typeparam name="T">type of parameter value</typeparam>
    /// <param name="value"></param>
    /// <returns>Param{T}</returns>
    protected Param<T> Param<T>(string? value = null)
            //where T : struct
            => _builder.Param<T>(value);

    /// <summary>
    /// build a new string parameter
    /// </summary>
    /// <param name="value">eventual value</param>
    /// <returns>Param</returns>
    protected Param Param(string? value = null)
        => _builder.Param(value);

    /// <summary>
    /// build an option set
    /// </summary>
    /// <param name="options">options</param>
    /// <returns>option set</returns>
    protected static OptSet OptSet(params IOpt[] options)
        => new(options);
}

