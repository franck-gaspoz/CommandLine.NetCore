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
}
