using System.Reflection;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;

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
    public InvalidCommandOperationParameterMappingException(
        int index,
        IArg sourceArgument,
        ParameterInfo targetParameter,
        string details) : base(
            $"parameter index={index}{Environment.NewLine}argument source={sourceArgument.ToSyntax()}{Environment.NewLine}target parameter={targetParameter.ToText()}{Environment.NewLine}{details}")
    {
        Index = index;
        SourceArgument = sourceArgument;
        TargetParameter = targetParameter;
    }
}
