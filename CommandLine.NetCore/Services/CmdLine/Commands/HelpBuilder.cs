using CommandLine.NetCore.Services.AppHost;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// help builder for a dynamic command
/// </summary>
public sealed class HelpBuilder
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

    /// <summary>
    /// add help to configuration
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <param name="configuration">configuration</param>
    internal void Configure(
        string commandName,
        Configuration configuration)
    {
        foreach (var kvp in _helps)
            Configure(
                commandName,
                kvp.Key,
                kvp.Value,
                configuration);
    }

    static void Configure(
        string commandName,
        string culture,
        CommandHelp help,
        Configuration configuration) => configuration.Set(
            ShortDescriptionKey(commandName),
            help.ShortDescription,
            culture);

    /// <summary>
    /// configuration key of the short description of a command
    /// </summary>
    /// <param name="commandName">command name (posix)</param>
    /// <returns>configuration key</returns>
    internal static string ShortDescriptionKey(string commandName) => $"Commands:{commandName}:Description";

    /// <summary>
    /// configuration key of the long description of a command
    /// </summary>
    /// <param name="commandName">command name (posix)</param>
    /// <returns>configuration key</returns>
    internal static string LongDescriptionKey(string commandName) => $"Commands:{commandName}:Syntax";

}
