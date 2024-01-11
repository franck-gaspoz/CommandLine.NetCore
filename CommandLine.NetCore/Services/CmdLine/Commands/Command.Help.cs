using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// abstract command : Help
/// </summary>
public abstract partial class Command
{
    /// <summary>
    /// short description of the command
    /// </summary>
    /// <param name="text">short description or error text if not found in help settings</param>
    /// <returns>true if found, false if missing</returns>
    public bool GetDescription(out string text)
        => GetDescription(
            ClassNameToCommandName(),
            Config,
            Texts,
            Console,
            out text);

    /// <summary>
    /// short description of the command with provided name
    /// </summary>
    /// <param name="name">command name</param>
    /// <param name="config">configuration</param>
    /// <param name="texts">texts</param>
    /// <param name="console">console</param>
    /// <param name="text">short description</param>
    /// <returns>true if found, false if missing</returns>
    public static bool GetDescription(
        string name,
        Configuration config,
        Texts texts,
        IAnsiVtConsole console,
        out string text)
    {
        var desc = config.Get(
            HelpBuilder.ShortDescriptionKey(name));

        if (desc is not null)
        {
            text = desc;
            return true;
        }
        text = console.Colors.Error + texts._(
            "CommandShortHelpNotFound",
           name);
        return false;
    }

    /// <summary>
    /// get descriptions for the command options
    /// </summary>
    /// <param name="texts">founded descriptions or empty list</param>
    /// <returns>true if some options exists</returns>
    public bool GetOptionsDescriptions(out List<KeyValuePair<string, string>> texts)
        => GetOptionsDescriptions(
            ClassNameToCommandName(),
            Config,
            out texts
            );

    /// <summary>
    /// get descriptions for the command options
    /// </summary>
    /// <param name="name">command name</param>
    /// <param name="config">configuration</param>
    /// <param name="lines">founded descriptions or empty list</param>
    /// <returns>true if some options exists</returns>
    public static bool GetOptionsDescriptions(
        string name,
        Configuration config,
        out List<KeyValuePair<string, string>> lines)
    {
        var sectionName = $"Commands:{name}:Options";
        var optDescs = config.GetSection(sectionName);

        if (!optDescs.Exists() || !optDescs.GetChildren().Any())
        {
            lines = new List<KeyValuePair<string, string>>();
            return false;
        }

        lines = new List<KeyValuePair<string, string>>();
        foreach (var optDesc in optDescs.GetChildren())
        {
            lines.Add(
                new KeyValuePair<string, string>(
                    optDesc.Key,
                    optDesc.Value ?? string.Empty
                ));
        }
        return true;
    }

    /// <summary>
    /// long descriptions of the command
    /// </summary>
    /// <param name="texts">command syntaxes descriptions or error text if not found in help settings</param>
    /// <returns>true if found, false if missing</returns>
    public bool GetLongDescriptions(out List<KeyValuePair<string, string>> texts)
        => GetLongDescriptions(
            ClassNameToCommandName(),
            Config,
            Texts,
            Console,
            out texts
            );

    /// <summary>
    /// long descriptions of the command
    /// </summary>
    /// <param name="name">command name</param>
    /// <param name="config">configuration</param>
    /// <param name="texts">texts</param>
    /// <param name="console">console</param>
    /// <param name="lines">long descriptions</param>
    /// <returns>true if found, false if missing</returns>
    public static bool GetLongDescriptions(
        string name,
        Configuration config,
        Texts texts,
        IAnsiVtConsole console,
        out List<KeyValuePair<string, string>> lines)
    {
        var descs = config.GetSection(
            HelpBuilder.LongDescriptionKey(name));

        var childrens = descs.GetChildren();
        if (!descs.Exists() || !childrens.Any())
        {
            lines = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string,string>(
                    console.Colors.Error +
                    texts._("CommandLongHelpNotFound",name),
                    string.Empty)
            };
            return false;
        }

        lines = new List<KeyValuePair<string, string>>();
        foreach (var desc in childrens)
        {
            lines.Add(
                new KeyValuePair<string, string>(
                    desc.Key, desc.Value ?? string.Empty));
        }

        return true;
    }
}

