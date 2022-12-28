using System.Diagnostics;

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

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    /// <param name="parser">syntax parser</param>
    public ArgSet(
        IEnumerable<string> args,
        Parser parser
        )
        => (_args, _parser)
            = (new List<string>(args), parser);

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
    /// <returns>true if syntax match, false otherwise and list of errors</returns>
    public CommandResult MatchSyntax(
        params Arg[] grammar
        )
    {
        var (hasErrors, errors) = _parser.MatchSyntax(_args, grammar);
        return new CommandResult(
            hasErrors ? Globals.ExitFail : Globals.ExitOk,
            errors
            );
    }
}
