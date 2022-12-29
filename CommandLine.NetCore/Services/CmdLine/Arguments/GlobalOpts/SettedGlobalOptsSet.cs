using System.Diagnostics.CodeAnalysis;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

/// <summary>
/// setted global arguments set
/// <para>add access to global options set, command line args and assembly set</para>
/// </summary>
public sealed class SettedGlobalOptsSet
{
    private readonly Dictionary<string, IOpt> _opts = new();

    public IReadOnlyDictionary<string, IOpt> Opts
        => _opts;

    /// <summary>
    /// assembly set
    /// </summary>
    public AssemblySet AssemblySet { get; private set; }

    /// <summary>
    /// global opts set
    /// </summary>
    public GlobalOptsSet GlobalOptsSet { get; private set; }

    /// <summary>
    /// command line args
    /// </summary>
    public CommandLineArgs CommandLineArgs { get; private set; }

    public SettedGlobalOptsSet(
        CommandLineArgs commandLineArgs,
        GlobalOptsSet globalOptsSet,
        AssemblySet assemblySet)
    {
        AssemblySet = assemblySet;
        CommandLineArgs = commandLineArgs;
        GlobalOptsSet = globalOptsSet;
        foreach (var kvp in globalOptsSet.Parse(commandLineArgs))
            Add(kvp.Value);
    }

    public void Add(IOpt opt)
        => _opts.Add(opt.Name, opt);

    public bool TryGetByType<T>([NotNullWhen(true)] out T? opt)
        where T : IOpt
    {
        opt = (T?)_opts
            .Values
            .Where(x => x.GetType() == typeof(T))
            .FirstOrDefault();

        return opt != null;
    }

    public bool Contains<T>()
        where T : IOpt
        => TryGetByType<T>(out _);
}

