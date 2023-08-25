using CommandLine.NetCore.Services.CmdLine.Parsing;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// an execute method of a command
/// </summary>
public sealed class DynamicCommandSpecification
{
    /// <summary>
    /// command name owning the execute method
    /// </summary>
    public string CommandName { get; private set; }

    /// <summary>
    /// execute
    /// </summary>
    public DynamicCommandSpecificationDelegate SpecificationDelegate { get; private set; }

    /// <summary>
    /// help builder
    /// </summary>
    public HelpBuilder? HelpBuilder { get; private set; }

    /// <summary>
    /// builds a new instance
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <param name="specificationDelegate">specification delegate</param>
    /// <param name="helpBuilder">eventual command help builder</param>
    public DynamicCommandSpecification(
        string commandName,
        DynamicCommandSpecificationDelegate specificationDelegate,
        HelpBuilder? helpBuilder)
            => (CommandName, SpecificationDelegate, HelpBuilder)
                = (commandName, specificationDelegate, helpBuilder);
}
