﻿using System.Diagnostics;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;

/// <summary>
/// a command line syntax
/// </summary>
[DebuggerDisplay("{_debuggerDisplay}")]
public sealed class Syntax
{
    string _debuggerDisplay => ToSyntax();

    /// <summary>
    /// name of the syntax
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// options set
    /// </summary>
    public OptSet? OptSet { get; private set; }

    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<IArg> Args => _args;

    readonly List<IArg> _args;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    public Syntax(IArg[] args)
        => _args = args.ToList();

    /// <summary>
    /// add command options to the syntax
    /// <para>can be called multiple times, addition is unique</para>
    /// </summary>
    /// <param name="optSet">options set</param>
    public void AddOptions(OptSet? optSet)
    {
        OptSet = optSet;
        if (optSet is null) return;
        foreach (var opt in optSet.Opts)
            if (!_args.Contains(opt))
                _args.Add(opt);
    }

    /// <summary>
    /// array get accessor
    /// </summary>
    /// <param name="index">argument index (from 0)</param>
    /// <returns>argument value at index</returns>
    public IArg this[int index] => _args[index];

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
    /// text of the name of an unamed snytax
    /// </summary>
    public static string UnNamedSyntaxNameReplacement { get; set; } = "{unamed}";

    /// <summary>
    /// string representation of the syntax
    /// </summary>
    /// <param name="unNamedSyntaxNameReplacement">replacement text for the name of an unamed syntax</param>
    /// <returns>string representation of the syntax</returns>
    public string ToSyntax(string? unNamedSyntaxNameReplacement = null)
        => ((Name == null) ? (unNamedSyntaxNameReplacement ?? UnNamedSyntaxNameReplacement) : Name!) +
            ": " +
            string.Join(' ', _args.Select((x, n) => n + ":" + x.ToSyntax()));

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
            if ((arg is IOpt opt)
                || (arg is IParam param && param.IsExpectingValue))
                return i;
        }
        return -1;
    }
}
