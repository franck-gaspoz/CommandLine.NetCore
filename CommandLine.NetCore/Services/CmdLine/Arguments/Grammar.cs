using System.Diagnostics;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a command line grammar
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class Grammar
{
    private string DebuggerDisplay => ToGrammar();

    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<Arg> Args => _args;

    private readonly List<Arg> _args;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    public Grammar(
        Arg[] args
        )
        => _args = args.ToList();

    /// <summary>
    /// array get accessor
    /// </summary>
    /// <param name="index">argument index (from 0)</param>
    /// <returns>argument value at index</returns>
    public Arg this[int index] => _args[index];

    /// <summary>
    /// args count
    /// </summary>
    public int Count => Args.Count;

    /// <summary>
    /// string representation of the grammar
    /// </summary>
    /// <returns>string representation of the grammar</returns>
    public string ToGrammar()
        => string.Join(' ', _args.Select(x => x.ToGrammar()));
}
