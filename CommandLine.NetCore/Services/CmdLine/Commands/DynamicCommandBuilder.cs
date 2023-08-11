namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// dynamic command builder
/// </summary>
public sealed class DynamicCommandBuilder : CommandBuilder
{
    /// <summary>
    /// creates a new instance
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    public DynamicCommandBuilder(
        Dependencies dependencies,
        string commandName)
        : base(
            dependencies,
            commandName) { }
}
