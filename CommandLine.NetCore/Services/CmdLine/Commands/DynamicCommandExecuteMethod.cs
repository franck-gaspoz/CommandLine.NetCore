using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// an execute method of a command
/// </summary>
public sealed class DynamicCommandExecuteMethod
{
    /// <summary>
    /// command name owning the execute method
    /// </summary>
    public string CommandName { get; private set; }

    /// <summary>
    /// execute
    /// </summary>
    public Func<ArgSet, CommandBuilder, DynamicCommandContext, CommandResult> Execute { get; private set; }

    /// <summary>
    /// builds a new instance
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <param name="execute">execute delegate</param>
    public DynamicCommandExecuteMethod(
        string commandName,
        Func<ArgSet, CommandBuilder, DynamicCommandContext, CommandResult> execute)
            => (CommandName, Execute) = (commandName, execute);
}
