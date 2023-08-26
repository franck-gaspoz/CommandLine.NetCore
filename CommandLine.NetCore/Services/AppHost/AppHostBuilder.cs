using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Extensions;
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
                    .AddSingleton<Configuration>()
                    .AddSingleton<AppHostConfiguration>()
                    .AddSingleton<Texts>()
                    .AddSingleton<ArgBuilder>()
                    .AddSingleton<ValueConverter>()
                    .AddSingleton<Parser>()
                    .AddSingleton<Dependencies>()
                    .AddSingleton(hostConfiguration)
                    .AddSingleton(hostConfiguration.AssemblySet)
                    .AddCommandLineArgs(args)
                    .AddClassCommands(
                        hostConfiguration.AssemblySet,
                        AppHostConfiguration)
                    .AddDynamicCommands()
                    .AddCommandsSet()
                    .AddGlobalArguments(hostConfiguration.AssemblySet)
                    .AddGlobalSettings()
                    .ConfigureOutput());

        hostConfiguration.BuildDelegate?
            .Invoke(hostBuilder);

        AppHost = hostBuilder.Build();

        PostBuildSetup();

        FailIfInitializationErrors();
    }

    void PostBuildSetup()
    {
        var dynComSet = AppHost.Services.GetRequiredService<DynamicCommandsSet>();
        var configuration = AppHost.Services.GetRequiredService<Configuration>();
        //dynComSet.ConfigureHelp(configuration);
    }

    void FailIfInitializationErrors()
    {
        if (!AppHostConfiguration.InitializationErrors.Any())
            return;
        var texts = AppHost.Services.GetRequiredService<Texts>();
        var error = AppHostConfiguration.InitializationErrors[0];
        throw error.ToException(texts);
    }
}
