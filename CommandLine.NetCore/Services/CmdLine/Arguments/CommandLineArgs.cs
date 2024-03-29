﻿namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// command line arguments
/// </summary>
public sealed class CommandLineArgs
{
    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<string> Args => _args;

    /// <summary>
    /// args count
    /// </summary>
    public int Count => _args.Count;

    /// <summary>
    /// any ?
    /// </summary>
    /// <returns>true if count > 0</returns>
    public bool Any() => Count > 0;

    List<string> _args;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">list of args</param>
    public CommandLineArgs(List<string> args)
        => _args = new List<string>(args);

    /// <summary>
    /// configure
    /// </summary>
    /// <param name="args">args</param>
    public void Configure(List<string> args)
        => _args = new List<string>(args);

    /// <summary>
    /// replace the list of args with new args
    /// </summary>
    /// <param name="args">arguments</param>
    public void Replace(List<string> args)
    {
        _args.Clear();
        _args.AddRange(args);
    }
}
