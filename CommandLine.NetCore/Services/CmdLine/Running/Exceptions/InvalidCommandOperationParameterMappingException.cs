using System.Reflection;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;

namespace CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

/// <summary>
/// invalid command operation parameter mapping
/// </summary>
class InvalidCommandOperationParameterMappingException
    : SyntaxMatcherDispatcherException
{
    /// <summary>
    /// index in syntax
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// source argument type
    /// </summary>
    public IArg SourceArgument { get; }

    /// <summary>
    /// target parameter type
    /// </summary>
    public ParameterInfo TargetParameter { get; }

    /// <summary>
    /// build new instance
    /// </summary>
    /// <param name="index">index in syntax</param>
    /// <param name="sourceArgument">source argument</param>
    /// <param name="targetParameter">target parameter type</param>
    /// <param name="details">details</param>
    /// <param name="commandResult">command result</param>
    public InvalidCommandOperationParameterMappingException(
        int index,
        IArg sourceArgument,
        ParameterInfo targetParameter,
        string details,
        CommandResult commandResult) : base(
            $"parameter index={index}{Environment.NewLine}argument source={sourceArgument.ToSyntax()}{Environment.NewLine}target parameter={targetParameter.ToText()}{Environment.NewLine}{details}",
            commandResult)
    {
        Index = index;
        SourceArgument = sourceArgument;
        TargetParameter = targetParameter;
    }
}
