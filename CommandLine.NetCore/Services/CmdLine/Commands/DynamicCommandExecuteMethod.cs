using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// an execute method of a command
/// </summary>
sealed class DynamicCommandExecuteMethod
{
    /// <summary>
    /// execute 
    /// </summary>
    public Func<ArgSet, CommandBuilder, DynamicCommandContext, CommandResult> Execute { get; private set; }

    /// <summary>
    /// builds a new instance
    /// </summary>
    /// <param name="execute">execute delegate</param>
    public DynamicCommandExecuteMethod(Func<ArgSet, CommandBuilder, DynamicCommandContext, CommandResult> execute)
        => Execute = execute;
}
