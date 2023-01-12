using System.Diagnostics;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;

/// <summary>
/// a command options set
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class OptSet
{
    private string DebuggerDisplay => ToSyntax();

    /// <summary>
    /// name of the syntax
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<IOpt> Opts => _opts;

    private readonly List<IOpt> _opts;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="opts">options</param>
    public OptSet(params IOpt[] opts) =>
        _opts = opts
            .Select(x =>
                {
                    x.SetIsOptional(true);
                    return x;
                })
            .ToList();

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="optSet">merged opt set</param>
    /// <param name="opts">options</param>
    public OptSet(OptSet optSet, params IOpt[] opts)
    {
        _opts = opts
            .Select(x =>
            {
                x.SetIsOptional(true);
                return x;
            })
            .ToList();
        _opts.AddRange(optSet.Opts);
    }

    /// <summary>
    /// add an option to the opt set
    /// </summary>
    /// <param name="opt">option to be addded</param>
    public void Add(IOpt opt)
        => _opts.Add(opt);

    /// <summary>
    /// array get accessor
    /// </summary>
    /// <param name="index">argument index (from 0)</param>
    /// <returns>argument value at index</returns>
    public IOpt this[int index] => _opts[index];

    /// <summary>
    /// args count
    /// </summary>
    public int Count => Opts.Count;

    /// <summary>
    /// string representation of the syntax
    /// </summary>
    /// <returns>string representation of the syntax</returns>
    public string ToSyntax()
        => ((Name == null) ? "?" : Name!) +
            ": " +
            string.Join(' ', _opts.Select(x => x.ToSyntax()));
}
