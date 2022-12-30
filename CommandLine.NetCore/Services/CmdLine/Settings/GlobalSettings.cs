using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Settings;

/// <summary>
/// global settings of the command line engine
/// <para>provides access to:
/// - global options set
/// - setted global options set
/// - command line args and assembly set</para>
/// </summary>
public sealed class GlobalSettings
{
    /// <summary>
    /// assembly set
    /// </summary>
    public AssemblySet AssemblySet { get; private set; }

    /// <summary>
    /// global opts set
    /// </summary>
    public GlobalOptsSet GlobalOptsSet { get; private set; }

    /// <summary>
    /// setted global opts set
    /// </summary>
    public SettedGlobalOptsSet SettedGlobalOptsSet { get; private set; }

    /// <summary>
    /// command line args
    /// </summary>
    public CommandLineArgs CommandLineArgs { get; private set; }

    /// <summary>
    /// configuration
    /// </summary>
    public IConfiguration Configuration { get; private set; }

    /// <summary>
    /// global settings of the command line engine
    /// <para>provides access to:
    /// - global options set
    /// - setted global options set
    /// - command line args and assembly set</para>
    /// </summary>
    /// <param name="configuration">configuration</param>
    /// <param name="commandLineArgs">command line args</param>
    /// <param name="globalOptsSet">global options set</param>
    /// <param name="assemblySet">assembly set</param>
    public GlobalSettings(
        IConfiguration configuration,
        CommandLineArgs commandLineArgs,
        GlobalOptsSet globalOptsSet,
        AssemblySet assemblySet)
    {
        Configuration = configuration;
        AssemblySet = assemblySet;
        CommandLineArgs = commandLineArgs;
        GlobalOptsSet = globalOptsSet;
        SettedGlobalOptsSet = new(globalOptsSet, commandLineArgs);
    }
}

