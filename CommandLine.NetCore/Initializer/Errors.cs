using System.Runtime.CompilerServices;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.Error;

namespace CommandLine.NetCore.Initializer;

/// <summary>
/// intialisation errors
/// </summary>
static class Errors
{
    /// <summary>
    /// name of the property name
    /// </summary>
    public const string NameProperty = "Name";

    /// <summary>
    /// command already exists
    /// </summary>
    /// <param name="caller">caller</param>
    /// <param name="name">name of the command</param>
    /// <param name="data">data</param>
    /// <param name="callerMemberName"></param>
    /// <returns>error descriptor</returns>
    public static ErrorDescriptor CommandAlreadyExists(
        this object caller,
        string name,
        object? data = null,
        [CallerMemberName] string? callerMemberName = null)
            => caller.CreateErrorDescriptor(
                "CommandAlreadyExists",
                data.Add((NameProperty, name)),
                new(() => NameProperty),
                callerMemberName);

    /// <summary>
    /// command already exists error
    /// </summary>
    /// <param name="caller">caller</param>
    /// <param name="name">name of the command</param>
    /// <param name="data">data</param>
    /// <param name="callerMemberName"></param>
    /// <returns>error descriptor</returns>
    public static ErrorDescriptor MulitpleFor(
        this object caller,
        string name,
        object? data = null,
        [CallerMemberName] string? callerMemberName = null)
            => caller.CreateErrorDescriptor(
                "MulitpleFor",
                data.Add((NameProperty, name)),
                new(() => NameProperty),
                callerMemberName);
}
