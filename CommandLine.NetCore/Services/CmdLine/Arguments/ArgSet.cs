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

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    public ArgSet(
        IEnumerable<string> args
        )
        => _args
            = new List<string>(args);

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
}
