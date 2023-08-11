using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// an execute method of a dynamic command
/// </summary>
sealed class ExecuteMethod
{
    /// <summary>
    /// execute 
    /// </summary>
    public Func<ArgSet, CommandResult> Execute { get; private set; }

    /// <summary>
    /// builds a new instance
    /// </summary>
    /// <param name="execute">execute delegate</param>
    public ExecuteMethod(Func<ArgSet, CommandResult> execute)
        => Execute = execute;
}
