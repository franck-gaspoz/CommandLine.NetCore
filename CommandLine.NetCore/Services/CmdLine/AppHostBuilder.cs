
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static CommandLine.NetCore.Services.CmdLine.Globals;

namespace CommandLine.NetCore.Services.CmdLine;

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
                    .AddSingleton(assemblySet)
                    .AddCommandLineArgs(args)
                    .AddCommands(assemblySet)
                    .AddGlobalArguments()
                    .AddSettedGlobalArguments()
                    .ConfigureOutput());

        buildDelegate?.Invoke(hostBuilder);

        AppHost = hostBuilder.Build();
        //AppHost.RunAsync();
    }
}
