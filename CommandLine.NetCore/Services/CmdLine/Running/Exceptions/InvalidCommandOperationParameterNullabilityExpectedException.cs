using System.Reflection;

using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

/// <summary>
/// invalid nullability of a command operation parameter
/// </summary>
sealed class InvalidCommandOperationParameterNullabilityExpectedException
    : InvalidCommandOperationParameterMappingException
{
    /// <summary>
    /// build new instance
    /// </summary>
    /// <param name="index">index in syntax</param>
    /// <param name="sourceArgument">source argument</param>
    /// <param name="parameter">target parameter</param>
    /// <param name="details">details</param>
    public InvalidCommandOperationParameterNullabilityExpectedException(
        int index,
        IArg sourceArgument,
        ParameterInfo parameter,
        string details) : base(
            index,
            sourceArgument,
            parameter,
            details)
    { }
}
