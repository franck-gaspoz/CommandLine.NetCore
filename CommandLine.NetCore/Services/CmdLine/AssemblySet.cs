using System.Reflection;

namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// assemblies where lookup for commands and arguments
/// </summary>
public sealed class AssemblySet
{
    public List<Assembly> Assemblies { get; private set; }
        = new();

    public AssemblySet() { }

    public AssemblySet(Assembly assembly) => Assemblies.Add(assembly);

    public AssemblySet Merge(AssemblySet? set)
    {
        if (set is null) return this;
        foreach (var assembly in set.Assemblies)
            Assemblies.Add(assembly);
        return this;
    }
}
