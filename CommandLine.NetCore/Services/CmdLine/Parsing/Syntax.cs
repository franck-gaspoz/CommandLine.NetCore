using System.Diagnostics;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;

/// <summary>
/// a command line syntax
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class Syntax
{
    private string DebuggerDisplay => ToSyntax();

    /// <summary>
    /// name of the syntax
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<Arg> Args => _args;

    private readonly List<Arg> _args;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    public Syntax(
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
    /// Set the name of the syntax
    /// </summary>
    /// <param name="name">name</param>
    public void SetName(string name) => Name = name;

    /// <summary>
    /// string representation of the syntax
    /// </summary>
    /// <returns>string representation of the syntax</returns>
    public string ToSyntax()
        => ((Name == null) ? "?" : Name!) +
            ": " +
            string.Join(' ', _args.Select(x => x.ToSyntax()));

    /// <summary>
    /// get index of next arg with expected value from an index
    /// </summary>
    /// <param name="fromIndex">from index</param>
    /// <returns>index if found else -1</returns>
    public int GetIndexOfArgWithExpectedValueFromIndex(int fromIndex)
    {
        for (var i = fromIndex; i < _args.Count; i++)
        {
            var arg = _args[i];
            if ((arg is IOpt opt && opt.ExpectedValuesCount > 0)
                || (arg is IParam param && param.IsExpectingValue))
            {
                return i;
            }
        }
        return -1;
    }
}
