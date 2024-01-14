namespace CommandLine.NetCore.Config;

/// <summary>
/// command line interpreter trace settings
///<para>mapped to appSettings.core.json Core:Trace</para>
/// </summary>
public class LogSettings
{
    /// <summary>
    /// add class command
    /// </summary>
    public bool AddClassCommand { get; set; }

    /// <summary>
    /// add dynamic command
    /// </summary>
    public bool AddDynamicCommand { get; set; }

    /// <summary>
    /// syntaxes identification (equivalent to GlobalOpt:ParserLogging)
    /// </summary>
    public bool SyntaxesIdentification { get; set; }
}
