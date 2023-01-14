using System.Diagnostics.CodeAnalysis;

using CommandLine.NetCore.Services.CmdLine.Settings;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

/// <summary>
/// setted global arguments set
/// </summary>
public class SettedGlobalOptsSet
{
    /// <summary>
    /// setted global options
    /// </summary>
    public IReadOnlyDictionary<string, IOpt> Opts
        => _opts;

    /// <summary>
    /// setted global options syntaxes
    /// </summary>
    public IReadOnlyDictionary<IOpt, List<string>> OptSpecs
        => _optSpecs;

    readonly Dictionary<string, IOpt> _opts = new();
    readonly Dictionary<IOpt, List<string>> _optSpecs = new();

    /// <summary>
    /// setted global arguments set
    /// </summary>
    /// <param name="globalOptsSet">global options set</param>
    /// <param name="commandLineArgs">command line args</param>
    public SettedGlobalOptsSet(
        GlobalOptsSet globalOptsSet,
        CommandLineArgs commandLineArgs)
    {
        foreach (var kvp in globalOptsSet.Parse(commandLineArgs))
            Add(kvp.Value);
    }

    /// <summary>
    /// configure from global settings
    /// </summary>
    /// <param name="globalSettings">global settings</param>
    public void Configure(GlobalSettings globalSettings)
    {
        _opts.Clear();
        _optSpecs.Clear();
        var globalOptsSet = globalSettings.GlobalOptsSet;
        var commandLineArgs = globalSettings.CommandLineArgs;
        foreach (var kvp in globalOptsSet.Parse(commandLineArgs))
            Add(kvp.Value);
    }

    /// <summary>
    /// add an option to the set
    /// </summary>
    /// <param name="optSpec">option spec</param>
    void Add((IOpt opt, List<string> optArgs) optSpec)
    {
        _opts.Add(
            optSpec.opt.Name,
            optSpec.opt);
        _optSpecs.Add(
            optSpec.opt,
            optSpec.optArgs
            );
    }

    /// <summary>
    /// try get an option from its type
    /// </summary>
    /// <typeparam name="T">type looked up</typeparam>
    /// <param name="opt">option if found</param>
    /// <returns>true if found</returns>
    public bool TryGetByType<T>([NotNullWhen(true)] out T? opt)
        where T : IOpt
    {
        opt = (T?)_opts
            .Values
            .Where(x => x.GetType() == typeof(T))
            .FirstOrDefault();

        return opt != null;
    }

    /// <summary>
    /// indicates if contains an option of type T
    /// </summary>
    /// <typeparam name="T">looked up type</typeparam>
    /// <returns>true if found, false otherwise</returns>
    public bool Contains<T>()
        where T : IOpt
        => TryGetByType<T>(out _);
}
