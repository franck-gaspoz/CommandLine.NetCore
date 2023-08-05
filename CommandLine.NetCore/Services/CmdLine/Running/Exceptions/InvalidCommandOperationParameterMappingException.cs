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
    public Type SourceArgumentType { get; }

    /// <summary>
    /// target parameter type
    /// </summary>
    public Type TargetParameterType { get; }

    /// <summary>
    /// build new instance
    /// </summary>
    /// <param name="index">index in syntax</param>
    /// <param name="sourceArgumentType">source argument type</param>
    /// <param name="targetParameterType">target parameter type</param>
    /// <param name="details">details</param>
    public InvalidCommandOperationParameterMappingException(
        int index,
        Type sourceArgumentType,
        Type targetParameterType,
        string details) : base(details)
    {
        Index = index;
        SourceArgumentType = sourceArgumentType;
        TargetParameterType = targetParameterType;
    }
}
