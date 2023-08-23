using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// commmand running context
/// </summary>
public sealed class CommandContext
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
    /// <param name="syntax">running syntax</param>
    /// <param name="optSet">command options set</param>
    /// <param name="syntaxMatcherDispatcher">syntax matcher dispatcher</param>
    public CommandContext(
        Syntax syntax,
        OptSet? optSet,
        SyntaxMatcherDispatcher syntaxMatcherDispatcher)
    {
        Syntax = syntax;
        OptSet = optSet;
        SyntaxMatcherDispatcher = syntaxMatcherDispatcher;
    }
}
