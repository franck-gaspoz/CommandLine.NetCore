using CommandLine.NetCore.Services.AppHost;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// help builder for a dynamic command
/// </summary>
public class HelpBuilder
{
    internal readonly Dictionary<string, CommandHelp> _helps = new();

    /// <summary>
    /// creates a new instance of a command help builder
    /// </summary>
    public HelpBuilder() { }

    /// <summary>
    /// creates a new instance of a command help builder that builds a new help for a command
    /// </summary>
    /// <param name="shortDescription">description</param>
    /// <param name="longDescription">long description</param>
    /// <param name="culture">culture. if null uses the current culture</param>
    public HelpBuilder(
        string shortDescription,
        string longDescription,
        string? culture = null) => _helps.Add(
            culture ?? Configuration.Culture,
            new CommandHelp(
                shortDescription,
                longDescription));
}
