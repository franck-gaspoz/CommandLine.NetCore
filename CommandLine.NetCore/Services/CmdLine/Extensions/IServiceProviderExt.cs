using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Settings;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine.Extensions;

static class IServiceProviderExt
{
    /// <summary>
    /// configure command line args
    /// </summary>
    /// <param name="serviceProvider">service provider</param>
    /// <param name="args">args</param>
    /// <returns>this</returns>
    public static IServiceProvider ConfigureCommandLineArgs(
        this IServiceProvider serviceProvider,
        string[] args)
    {
        var cmdArgs = serviceProvider.GetRequiredService<CommandLineArgs>();
        cmdArgs.Configure(args.ToList());
        return serviceProvider;
    }

    /// <summary>
    /// configure global arguments
    /// </summary>
    /// <param name="serviceProvider">service provider</param>
    /// <returns>this</returns>
    public static IServiceProvider ConfigureGlobalSettings(
        this IServiceProvider serviceProvider)
    {
        var globalSettings = serviceProvider.GetRequiredService<GlobalSettings>();
        globalSettings
            .SettedGlobalOptsSet
            .Configure(globalSettings);
        return serviceProvider;
    }

    /// <summary>
    /// configure output
    /// </summary>
    /// <param name="serviceProvider">service provider</param>
    /// <returns>this</returns>
    public static IServiceProvider ConfigureOutput(this IServiceProvider serviceProvider)
    {
        new ConsoleFactory(serviceProvider)
            .Configure();
        return serviceProvider;
    }
}
