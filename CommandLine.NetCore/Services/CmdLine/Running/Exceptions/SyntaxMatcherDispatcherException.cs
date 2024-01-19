using CommandLine.NetCore.Services.CmdLine.Commands;

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
    /// matching syntax if available
    /// </summary>
    public CommandResult CommandResult { get; }

    /// <summary>
    /// build new instance
    /// </summary>
    /// <param name="details">details</param>
    /// <param name="commandResult">command result</param>
    public SyntaxMatcherDispatcherException(
        string details,
        CommandResult commandResult)
        : base(details)
    {
        Details = details;
        CommandResult = commandResult;
    }
}
