using CommandLine.NetCore.Services.AppHost;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// help builder for a dynamic command
/// </summary>
public static class HelpBuilder
{
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration">configuration</param>
    /// <param name="commandName">command name</param>
    /// <param name="argsSyntax">arguments syntax</param>
    /// <param name="description">description of the argument syntax</param>
    /// <param name="culture">culture of the text. if null use the current culture</param>
    internal static void AddSyntaxDescription(
        Configuration configuration,
        string commandName,
        string argsSyntax,
        string description,
        string? culture = null)
        => configuration.Set(
            HelpBuilder.LongDescriptionKey(commandName)
                + ":" + argsSyntax,
            description,
            culture);

    /// <summary>
    /// set a short description for the command syntax and the culture    
    /// </summary>
    /// <param name="configuration">configuration</param>
    /// <param name="commandName">command name</param>
    /// <param name="description">text of the short description</param>
    /// <param name="culture">culture of the text. if null use the current culture</param>
    internal static void SetShortDescription(
        Configuration configuration,
        string commandName,
        string description,
        string? culture = null)
        => configuration.Set(
            HelpBuilder.ShortDescriptionKey(commandName),
            description,
            culture);
}
