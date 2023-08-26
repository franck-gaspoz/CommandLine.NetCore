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
    /// <returns>text of the description</returns>
    public bool GetDescription(out string text)
    {
        var desc = Config.Get(
            HelpBuilder.ShortDescriptionKey(
                ClassNameToCommandName()));

        if (desc is not null)
        {
            text = desc;
            return true;
        }
        text = Console.Colors.Error + Texts._(
            "CommandShortHelpNotFound",
            ClassNameToCommandName());
        return false;
    }

    /// <summary>
    /// get descriptions for the command options
    /// </summary>
    /// <param name="texts">founded descriptions or empty list</param>
    /// <returns>true if some options exists</returns>
    public bool GetOptionsDescriptions(out List<KeyValuePair<string, string>> texts)
    {
        var sectionName = $"Commands:{ClassNameToCommandName()}:Options";
        var optDescs = Config.GetSection(sectionName);

        if (!optDescs.Exists() || !optDescs.GetChildren().Any())
        {
            texts = new List<KeyValuePair<string, string>>();
            return false;
        }

        texts = new List<KeyValuePair<string, string>>();
        foreach (var optDesc in optDescs.GetChildren())
        {
            texts.Add(
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
    /// <returns>text of the descriptions of each command syntax. key is the syntax, value is the description</returns>
    public bool GetLongDescriptions(out List<KeyValuePair<string, string>> texts)
    {
        var descs = Config.GetSection(
            HelpBuilder.LongDescriptionKey(
                ClassNameToCommandName()));

        if (!descs.Exists() || !descs.GetChildren().Any())
        {
            texts = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string,string>(
                    Console.Colors.Error +
                    Texts._("CommandLongHelpNotFound",
                        ClassNameToCommandName()),
                    string.Empty)
            };
            return false;
        }

        texts = new List<KeyValuePair<string, string>>();
        foreach (var desc in descs.GetChildren())
        {
            texts.Add(
                new KeyValuePair<string, string>(
                    desc.Key, desc.Value ?? string.Empty));
        }

        return true;
    }
}

