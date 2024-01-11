using CommandLine.NetCore.Services.CmdLine.Commands.Attributes;

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
    /// tags
    /// </summary>
    public List<string> Tags { get; private set; } = new();

    /// <summary>
    /// package
    /// </summary>
    public string Package { get; set; } = PackageAttribute.DefaultPackage;

    /// <summary>
    /// execute
    /// </summary>
    public DynamicCommandSpecificationDelegate SpecificationDelegate { get; private set; }

    /// <summary>
    /// builds a new instance
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <param name="specificationDelegate">specification delegate</param>
    public DynamicCommandSpecification(
        string commandName,
        DynamicCommandSpecificationDelegate specificationDelegate)
            => (CommandName, SpecificationDelegate)
                = (commandName, specificationDelegate);
}
