using System.Reflection;

namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// assemblies where lookup for commands and arguments
/// </summary>
public sealed class AssemblySet
{
    /// <summary>
    /// assemblies
    /// </summary>
    public List<Assembly> Assemblies { get; private set; }
        = new();

    /// <summary>
    /// assemblies where lookup for commands and arguments
    /// </summary>
    public AssemblySet() { }

    /// <summary>
    /// assemblies where lookup for commands and arguments
    /// </summary>
    /// <param name="assembly">asembly to be added</param>
    public AssemblySet(Assembly assembly) => Assemblies.Add(assembly);
}
