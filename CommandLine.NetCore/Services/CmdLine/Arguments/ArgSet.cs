using System.Diagnostics;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// set of arguments of a command invokation
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class ArgSet
{
    private string DebuggerDisplay => string.Join(' ', _args);

    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<string> Args => _args;

    private readonly List<string> _args;

    private readonly Parser _parser;
    private readonly IAnsiVtConsole _console;
    private readonly Texts _texts;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    /// <param name="parser">syntax parser</param>
    /// <param name="console">console</param>
    /// <param name="texts">texts</param>
    public ArgSet(
        IEnumerable<string> args,
        Parser parser,
        IAnsiVtConsole console,
        Texts texts
        )
        => (_args, _parser, _console, _texts)
            = (new List<string>(args), parser, console, texts);

    /// <summary>
    /// args count
    /// </summary>
    public int Count => Args.Count;

    /// <summary>
    /// array get accessor
    /// </summary>
    /// <param name="index">argument index (from 0)</param>
    /// <returns>argument value at index</returns>
    public string this[int index] => _args[index];

    /// <summary>
    /// check if the arg set match the syntax described by the parameters from left to right
    /// </summary>
    /// <returns>true if syntax match, false otherwise</returns>
    public bool MatchSyntax(
        params Arg[] grammar
        )
        => _parser.MatchSyntax(_args, grammar);
}
