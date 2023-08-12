using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Error;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CommandLine.NetCore.Services.AppHost;

/// <summary>
/// app host configuration
/// </summary>
public sealed class AppHostConfiguration
{
    /// <summary>
    /// assembly set
    /// </summary>
    public AssemblySet AssemblySet { get; private set; }

    /// <summary>
    /// is global help enabled
    /// </summary>
    public bool IsGlobalHelpEnabled { get; private set; }

    /// <summary>
    /// for unique command type
    /// </summary>
    public Type? ForCommandType { get; private set; }

    /// <summary>
    /// configure delegate
    /// </summary>
    public Action<IConfigurationBuilder>? ConfigureDelegate { get; private set; }

    /// <summary>
    /// build delegate
    /// </summary>
    public Action<IHostBuilder>? BuildDelegate { get; private set; }

    /// <summary>
    /// dynamic commands
    /// </summary>
    public IReadOnlyDictionary<string, DynamicCommandExecuteMethod> DynamicCommands { get; private set; }

    /// <summary>
    /// initialization errors
    /// </summary>
    public IReadOnlyList<ErrorDescriptor> InitializationErrors { get; private set; }

    /// <summary>
    /// build an app host configuration
    /// </summary>
    /// <param name="assemblySet">assembly set</param>
    /// <param name="forCommandType">for command type</param>
    /// <param name="isGlobalHelpEnabled">is global help enabled</param>
    /// <param name="configureDelegate">configure delegate</param>
    /// <param name="buildDelegate">build delegate</param>
    /// <param name="dynamicCommands">dynamic commands</param>
    /// <param name="initializationErrors">initialization errors</param>
    public AppHostConfiguration(
        AssemblySet assemblySet,
        bool isGlobalHelpEnabled,
        Type? forCommandType,
        Action<IConfigurationBuilder>? configureDelegate,
        Action<IHostBuilder>? buildDelegate,
        IReadOnlyDictionary<string, DynamicCommandExecuteMethod> dynamicCommands,
        IReadOnlyList<ErrorDescriptor> initializationErrors)
    {
        AssemblySet = assemblySet;
        IsGlobalHelpEnabled = isGlobalHelpEnabled;
        ForCommandType = forCommandType;
        ConfigureDelegate = configureDelegate;
        BuildDelegate = buildDelegate;
        DynamicCommands = dynamicCommands.ToDictionary(x => x.Key, x => x.Value);
        InitializationErrors = initializationErrors.ToList();
    }
}
