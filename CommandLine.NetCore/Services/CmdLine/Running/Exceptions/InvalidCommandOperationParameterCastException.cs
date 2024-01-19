using System.Reflection;

using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;

namespace CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

/// <summary>
/// invalid cast of an argument value to a command operation parameter
/// </summary>
sealed class InvalidCommandOperationParameterCastException
    : InvalidCommandOperationParameterMappingException
{
    /// <summary>
    /// build new instance
    /// </summary>
    /// <param name="index">index in syntax</param>
    /// <param name="sourceArgument">source argument</param>
    /// <param name="parameter">target parameter</param>
    /// <param name="details">details</param>
    /// <param name="commandResult">command result</param>
    public InvalidCommandOperationParameterCastException(
        int index,
        IArg sourceArgument,
        ParameterInfo parameter,
        string details,
        CommandResult commandResult) : base(
            index,
            sourceArgument,
            parameter,
            details,
            commandResult)
    { }
}
