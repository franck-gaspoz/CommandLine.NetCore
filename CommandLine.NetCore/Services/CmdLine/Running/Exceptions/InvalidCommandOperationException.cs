using CommandLine.NetCore.Services.CmdLine.Commands;

namespace CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

/// <summary>
/// operation implementing the command is invalid due to his prototype
/// </summary>
sealed class InvalidCommandOperationException
    : SyntaxMatcherDispatcherException
{
    /// <summary>
    /// build new instance
    /// </summary>
    /// <param name="details">details</param>
    /// <param name="commandResult">command result</param>
    public InvalidCommandOperationException(
        string details,
        CommandResult commandResult)
        : base(details, commandResult)
    {
    }
}
