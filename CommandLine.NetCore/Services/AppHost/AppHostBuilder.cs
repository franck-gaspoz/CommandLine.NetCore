
using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

namespace CommandLine.NetCore.Services.AppHost;

sealed class AppHostBuilder
{
    /// <summary>
    /// app host
    /// </summary>
    public IHost AppHost { get; private set; }

    /// <summary>
    /// app host configuration
    /// </summary>
    public AppHostConfiguration AppHostConfiguration { get; private set; }

    /// <summary>
    /// build the app host and run it async
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <param name="hostConfiguration">app host configuration</param> 
    public AppHostBuilder(
        List<string> args,
        AppHostConfiguration hostConfiguration)
    {
        AppHostConfiguration = hostConfiguration;
        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder
            .ConfigureAppConfiguration(
                configure =>
                {
                    configure.AddJsonFile(
                        Path.Combine(
                            Environment.CurrentDirectory,
                            ConfigFilePrefix + ConfigFileCoreName + ConfigFilePostfix),
                        optional: false);

                    configure.AddJsonFile(
                        Path.Combine(
                            Environment.CurrentDirectory,
                            ConfigFilePrefix + ConfigFileCoreName + "." + Thread.CurrentThread.CurrentCulture.Name + ConfigFilePostfix),
                        optional: true);

                    configure.AddJsonFile(
                        Path.Combine(
                            Environment.CurrentDirectory, ConfigFilePrefix + ConfigFilePostfix),
                        optional: true);

                    configure.AddJsonFile(
                        Path.Combine(
                            Environment.CurrentDirectory, ConfigFilePrefix + "." + Thread.CurrentThread.CurrentCulture.Name + ConfigFilePostfix),
                        optional: true);
                });

        if (hostConfiguration.ConfigureDelegate is not null)
            hostBuilder.ConfigureAppConfiguration(
                hostConfiguration.ConfigureDelegate);

        hostBuilder
            .ConfigureServices(
                services => services
                    .AddSingleton<AppHostConfiguration>()
                    .AddSingleton<Texts>()
                    .AddSingleton<ArgBuilder>()
                    .AddSingleton<ValueConverter>()
                    .AddSingleton<Parser>()
                    .AddSingleton<Dependencies>()
                    .AddSingleton(hostConfiguration)
                    .AddSingleton(hostConfiguration.AssemblySet)
                    .AddCommandLineArgs(args)
                    .AddCommands(
                        hostConfiguration.AssemblySet,
                        AppHostConfiguration)
                    .AddGlobalArguments(hostConfiguration.AssemblySet)
                    .AddGlobalSettings()
                    .ConfigureOutput());

        hostConfiguration.BuildDelegate?
            .Invoke(hostBuilder);

        AppHost = hostBuilder.Build();
    }
}
