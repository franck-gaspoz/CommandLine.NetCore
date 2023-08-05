namespace CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

/// <summary>
/// exception from the syntax matcher dispatcher
/// </summary>
class SyntaxMatcherDispatcherException :
    Exception,
    INotExplicitMessageException
{
    /// <summary>
    /// details about error context, data
    /// </summary>
    public string Details { get; }

    /// <summary>
    /// build new instance
    /// </summary>
    /// <param name="details">details</param>
    public SyntaxMatcherDispatcherException(string details)
        : base(details)
        => Details = details;
}
