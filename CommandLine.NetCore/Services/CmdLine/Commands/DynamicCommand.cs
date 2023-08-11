using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// a command that is declared and implemented dynamically
/// </summary>
sealed class DynamicCommand : Command
{
    readonly ExecuteMethod _method;

    /// <summary>
    /// builds a new dynamic command from an execute method
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    /// <param name="method">execute method that specify and implements the command</param>
    public DynamicCommand(
        Dependencies dependencies,
        ExecuteMethod method)
            : base(dependencies) => _method = method;

    /// <inheritdoc/>
    protected override CommandResult Execute(ArgSet args)
        => _method.Execute(args);
}
