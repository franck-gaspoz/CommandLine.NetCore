﻿namespace CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

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
    /// <param name="sourceArgumentType">source argument type</param>
    /// <param name="targetParameterType">target parameter type</param>
    /// <param name="details">details</param>
    public InvalidCommandOperationParameterCastException(
        int index,
        Type sourceArgumentType,
        Type targetParameterType,
        string details) : base(
            index,
            sourceArgumentType,
            targetParameterType,
            details)
    { }
}
