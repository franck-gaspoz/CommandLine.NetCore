using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.Text;

/// <summary>
/// texts service
/// </summary>
public sealed class Texts
{
    private readonly IConfiguration _config;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="config">app config</param>
    public Texts(IConfiguration config)
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
        => T("texts:" + textId, false, parameters);

    /// <summary>
    /// returns text from text id
    /// </summary>
    /// <param name="textId">text id</param>
    /// <param name="noRecurse">if false do not perform a search of unknown text</param>
    /// <param name="parameters">parmetrized text parameters (like string.Format)</param>
    /// <returns>text from settings</returns>
    private string T(
        string textId,
        bool noRecurse,
        params object?[] parameters)
    {
        var txt = _config.GetValue<string>(textId);
        return txt is null
            ? !noRecurse
                ? T("texts:UnknownText", true, textId)
                : $"Unknown text: {textId}"
            : string.Format(txt, parameters);
    }
}
