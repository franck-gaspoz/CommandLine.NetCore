
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CommandLine.NetCore.Services.AppHost;

/// <summary>
/// app host configuration
/// </summary>
public sealed class AppHostConfiguration
{
    /// <summary>
    /// configure delegate
    /// </summary>
    public Action<IConfigurationBuilder>? ConfigureDelegate { get; private set; }

    /// <summary>
    /// build delegate
    /// </summary>
    public Action<IHostBuilder>? BuildDelegate { get; private set; }

    /// <summary>
    /// build an app host configuration
    /// </summary>
    /// <param name="configureDelegate">configure delegate</param>
    /// <param name="buildDelegate">build delegate</param>
    public AppHostConfiguration(
        Action<IConfigurationBuilder>? configureDelegate,
        Action<IHostBuilder>? buildDelegate)
    {
        ConfigureDelegate = configureDelegate;
        BuildDelegate = buildDelegate;
    }
}
