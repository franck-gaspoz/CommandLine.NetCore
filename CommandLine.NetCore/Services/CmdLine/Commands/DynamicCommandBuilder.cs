namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// dynamic command builder
/// </summary>
class DynamicCommandBuilder : CommandBuilder
{
    /// <summary>
    /// creates a new instance
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    public DynamicCommandBuilder(Dependencies dependencies)
        : base(dependencies) { }
}
