using CommandLine.NetCore.Services.CmdLine.Commands;

namespace CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

/// <summary>
/// operation implementing the command is missing or not found
/// </summary>
sealed class MissingOrNotFoundCommandOperationException
    : SyntaxMatcherDispatcherException
{
    /// <summary>
    /// build new instance
    /// </summary>
    /// <param name="details">details</param>
    /// <param name="commandResult">command result</param>
    public MissingOrNotFoundCommandOperationException(
        string details,
        CommandResult commandResult)
        : base(details, commandResult)
    {
    }
}
