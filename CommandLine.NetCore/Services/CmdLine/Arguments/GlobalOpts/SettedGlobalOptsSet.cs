using System.Diagnostics.CodeAnalysis;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

/// <summary>
/// setted global arguments set
/// </summary>
public sealed class SettedGlobalOptsSet
{
    private readonly Dictionary<string, IOpt> _opts = new();

    public IReadOnlyDictionary<string, IOpt> Opts
        => _opts;

    public SettedGlobalOptsSet(
        CommandLineArgs commandLineArgs,
        GlobalOptsSet globalOptsSet)
    {
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

