using CommandLine.NetCore.Services.AppHost;

namespace CommandLine.NetCore.Services.Text;

/// <summary>
/// texts service
/// </summary>
public sealed class Texts
{
    /// <summary>
    /// name of the section 'texts' in the text settings file
    /// </summary>
    public static string TextsSectionName { get; private set; } = "Texts:";

    /// <summary>
    /// text not found result
    /// </summary>
    public static string TextNotFoundResult { get; private set; } = "???";

    readonly Configuration _config;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="config">app config</param>
    public Texts(Configuration config)
        => _config = config;

    /// <summary>
    /// returns text from text id
    /// </summary>
    /// <param name="textId">text id</param>
    /// <param name="parameters">parmetrized text parameters (like string.Format)</param>
    /// <returns>text from settings</returns>
    public string _(
        string textId,
        params object?[] parameters
        )
        => T(TextsSectionName + textId, false, parameters);

    /// <summary>
    /// returns text from text id
    /// </summary>
    /// <param name="textId">text id</param>
    /// <param name="noRecurse">if false do not perform a search of unknown text</param>
    /// <param name="parameters">parmetrized text parameters (like string.Format)</param>
    /// <returns>text from settings</returns>
    string T(
        string textId,
        bool noRecurse,
        params object?[] parameters)
    {
        var txt = _config.Get(textId);
        try
        {
            return txt is null
                ? !noRecurse
                    ? T(TextsSectionName + "UnknownText", true, textId)
                    : $"Unknown text: {textId}"
                : string.Format(txt, parameters);
        }
        catch (Exception)
        {
            return txt ?? TextNotFoundResult;
        }
    }

    /// <summary>
    /// indicates if the text with textId is defined in settings
    /// </summary>
    /// <param name="textId">text id</param>
    /// <returns>true if defined, false otherwise</returns>
    public bool IsDefined(string textId)
        => _config.Get(TextsSectionName + textId) is not null;
}
