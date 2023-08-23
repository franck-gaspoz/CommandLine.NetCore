using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// commmand running context
/// </summary>
public sealed class CommandContext : DynamicCommandContext
{
    /// <summary>
    /// running syntax
    /// </summary>
    public Syntax Syntax { get; private set; }

    /// <summary>
    /// options set
    /// </summary>
    public OptSet? OptSet { get; private set; }

    /// <summary>
    /// syntax matcher dispatcher
    /// </summary>
    public SyntaxMatcherDispatcher SyntaxMatcherDispatcher { get; private set; }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="globalSettings">global settings</param>
    /// <param name="console">console</param>
    /// <param name="texts">texts</param>
    /// <param name="syntax">running syntax</param>
    /// <param name="optSet">command options set</param>
    /// <param name="syntaxMatcherDispatcher">syntax matcher dispatcher</param>
    internal CommandContext(
        GlobalSettings globalSettings,
        IAnsiVtConsole console,
        Texts texts,
        Syntax syntax,
        OptSet? optSet,
        SyntaxMatcherDispatcher syntaxMatcherDispatcher)
        : base(
            globalSettings,
            console,
            texts
            )
    {
        Syntax = syntax;
        OptSet = optSet;
        SyntaxMatcherDispatcher = syntaxMatcherDispatcher;
    }
}
