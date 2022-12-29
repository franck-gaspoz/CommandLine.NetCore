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
    /// name of the grammar
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
    /// Set the name of the grammar
    /// </summary>
    /// <param name="name">name</param>
    public void SetName(string name) => Name = name;

    /// <summary>
    /// string representation of the grammar
    /// </summary>
    /// <returns>string representation of the grammar</returns>
    public string ToGrammar()
        => ((Name == null) ? "?" : Name!) +
            ": " +
            string.Join(' ', _args.Select(x => x.ToGrammar()));

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
