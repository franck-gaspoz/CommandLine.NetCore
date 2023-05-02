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
    public InvalidCommandOperationException(string details) : base(details)
    {
    }
}
