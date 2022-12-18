
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
    /// <param name="buildAction">eventually custom build action</param>
    public AppHostBuilder(
        List<string> args,
        Action<IHostBuilder>? buildAction = null)
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder
            .ConfigureAppConfiguration(
                configure =>
                {
                    configure.AddJsonFile(
                        ConfigFilePath,
                        optional: false);
                    var cultureConfigFileName = $"{ConfigFilePrefix}{Thread.CurrentThread.CurrentCulture.Name}{ConfigFilePostfix}";
                    configure.AddJsonFile(cultureConfigFileName, optional: false);
                })
            .ConfigureServices(
                services => services
                    .AddSingleton<Texts>()
                    .AddCommandLineArgs(args)
                    .AddCommands()
                    .AddGlobalArguments()
                    .AddSettedGlobalArguments()
                    .ConfigureOutput());

        buildAction?.Invoke(hostBuilder);

        AppHost = hostBuilder.Build();
        AppHost.RunAsync();
    }
}
