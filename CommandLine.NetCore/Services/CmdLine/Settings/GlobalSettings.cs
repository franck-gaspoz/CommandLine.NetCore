using CommandLine.NetCore.Config;
using CommandLine.NetCore.Services.AppHost;
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
    /// log settings key
    /// </summary>
    public const string LogSettingsKey = "Log";

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
    public Configuration Configuration { get; private set; }

    /// <summary>
    /// configuration
    /// </summary>
    public AppHostConfiguration AppHostConfiguration { get; private set; }

    /// <summary>
    /// command line builder
    /// </summary>
    public CommandLineInterfaceBuilder? CommandLineInterfaceBuilder { get; private set; }

    /// <summary>
    /// log settings
    /// </summary>
    public LogSettings LogSettings { get; private set; }

    /// <summary>
    /// global settings of the command line engine
    /// <para>provides access to:
    /// <list type="bullet">
    /// <item>global options set</item>
    /// <item>setted global options set</item>
    /// <item>command line args and assembly set</item>
    /// <item>app host configuration</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="configuration">configuration</param>
    /// <param name="commandLineArgs">command line args</param>
    /// <param name="globalOptsSet">global options set</param>
    /// <param name="assemblySet">assembly set</param>
    /// <param name="appHostConfiguration">app host configuration</param>
    public GlobalSettings(
        Configuration configuration,
        CommandLineArgs commandLineArgs,
        GlobalOptsSet globalOptsSet,
        AssemblySet assemblySet,
        AppHostConfiguration appHostConfiguration)
    {
        AppHostConfiguration = appHostConfiguration;
        Configuration = configuration;
        AssemblySet = assemblySet;
        CommandLineArgs = commandLineArgs;
        GlobalOptsSet = globalOptsSet;
        SettedGlobalOptsSet = new(globalOptsSet, commandLineArgs);
        LogSettings = new();
        configuration.Bind(LogSettingsKey, LogSettings);
    }

    /// <summary>
    /// indicates if a global option of type T is set or not
    /// </summary>
    /// <typeparam name="T">type of the globation option, inherited from IOpt</typeparam>
    /// <returns>true if the global option is set</returns>
    public bool IsGlobalOptionSet<T>()
        where T : IOpt
        => SettedGlobalOptsSet
            .Contains<T>();

    internal void SetCommandLineBuilder(CommandLineInterfaceBuilder commandLineInterfaceBuilder)
        => CommandLineInterfaceBuilder = commandLineInterfaceBuilder;
}

