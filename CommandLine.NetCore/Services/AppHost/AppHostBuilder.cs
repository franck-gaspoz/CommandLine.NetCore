
using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

namespace CommandLine.NetCore.Services.AppHost;

internal sealed class AppHostBuilder
{
    public IHost AppHost { get; private set; }

    /// <summary>
    /// build the app host and run it async
    /// </summary>
    /// <param name="args">command line args</param>
    /// <param name="assemblySet">assembly set where lookup for commands and arguments</param>
    /// <param name="configureDelegate">optional configure delegate</param>
    /// <param name="buildDelegate">optional build delegate</param>
    public AppHostBuilder(
        List<string> args,
        AssemblySet assemblySet,
        Action<IConfigurationBuilder>? configureDelegate = null,
        Action<IHostBuilder>? buildDelegate = null)
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        var hostConfiguration = new AppHostConfiguration(
            configureDelegate,
            buildDelegate
            );

        hostBuilder
            .ConfigureAppConfiguration(
                configure =>
                {
                    configure.AddJsonFile(
                        ConfigFilePrefix + ConfigFileCoreName + ConfigFilePostfix,
                        optional: false);

                    configure.AddJsonFile(
                        ConfigFilePrefix + ConfigFileCoreName + "." + Thread.CurrentThread.CurrentCulture.Name + ConfigFilePostfix,
                        optional: true);

                    configure.AddJsonFile(
                        ConfigFilePrefix + ConfigFilePostfix,
                        optional: true);

                    configure.AddJsonFile(
                        ConfigFilePrefix + "." + Thread.CurrentThread.CurrentCulture.Name + ConfigFilePostfix,
                        optional: true);
                });

        if (configureDelegate is not null)
            hostBuilder.ConfigureAppConfiguration(configureDelegate);

        hostBuilder
            .ConfigureServices(
                services => services
                    .AddSingleton<Texts>()
                    .AddSingleton<ArgBuilder>()
                    .AddSingleton<ValueConverter>()
                    .AddSingleton<Parser>()
                    .AddSingleton<Dependencies>()
                    .AddSingleton(hostConfiguration)
                    .AddSingleton(assemblySet)
                    .AddCommandLineArgs(args)
                    .AddCommands(assemblySet)
                    .AddGlobalArguments(assemblySet)
                    .AddGlobalSettings()
                    .ConfigureOutput());

        buildDelegate?.Invoke(hostBuilder);

        AppHost = hostBuilder.Build();
    }
}
