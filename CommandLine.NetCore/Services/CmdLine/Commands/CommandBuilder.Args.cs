#define Enable_h_Arg

using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command builder
/// </summary>
public sealed partial class CommandBuilder
{
    /// <summary>
    /// build a new option
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <param name="valueCount">value count</param>
    /// <returns>Opt</returns>
    public Opt Opt(string name, bool isOptional = false, int valueCount = 0)
        => _argBuilder.Opt(name, isOptional, valueCount);

    /// <summary>
    /// build a new flag (option with no possibilty of values)
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Flag</returns>
    public Flag Flag(string name, bool isOptional = false)
        => _argBuilder.Flag(name, isOptional);

    /// <summary>
    /// build a new generic option
    /// </summary>
    /// <typeparam name="T">type of options values</typeparam>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Opt{T}</returns>
    public Opt<T> Opt<T>(string name, bool isOptional = false)
        => _argBuilder.Opt<T>(name, isOptional);

    /// <summary>
    /// build a new parameter
    /// </summary>
    /// <typeparam name="T">type of parameter value</typeparam>
    /// <param name="value">text of single value</param>
    /// <returns>Param{T}</returns>
    public Param<T> Param<T>(string? value = null)
        => _argBuilder.Param<T>(value);

    /// <summary>
    /// build a new string parameter
    /// </summary>
    /// <param name="value">text of single value</param>
    /// <returns>Param</returns>
    public Param Param(string? value = null)
        => _argBuilder.Param(value);

    /// <summary>
    /// build an option set
    /// </summary>
    /// <param name="options">options</param>
    /// <returns>option set</returns>
    public static OptSet OptSet(params IOpt[] options)
        => new(options);
}
