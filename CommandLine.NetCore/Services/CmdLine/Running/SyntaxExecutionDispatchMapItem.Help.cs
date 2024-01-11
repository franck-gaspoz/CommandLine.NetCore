using CommandLine.NetCore.Services.CmdLine.Commands;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// a syntax dispatch map
/// </summary>
public sealed partial class SyntaxExecutionDispatchMapItem
{
    /// <summary>
    /// adds help of a command syntax
    /// </summary>
    /// <param name="argsSyntax">arguments syntax</param>
    /// <param name="description">description of the argument syntax</param>
    /// <param name="culture">culture of the text. if null use the current culture</param>
    /// <returns>this object</returns>
    public SyntaxExecutionDispatchMapItem Help(
        string argsSyntax,
        string description,
        string? culture = null)
    {
        var conf = SyntaxMatcherDispatcher
            .GlobalSettings
            .Configuration;
        HelpBuilder.AddSyntaxDescription(
            conf,
            _commandName,
            conf.BuildUniqueKey(_commandName, argsSyntax),
            description,
            culture
            );
        return this;
    }
}