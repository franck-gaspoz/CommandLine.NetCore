using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine;

internal static class IServiceProviderExt
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
    public static IServiceProvider ConfigureSettedGlobalArguments(
        this IServiceProvider serviceProvider)
    {
        var settedGlobalOptsSet = serviceProvider.GetRequiredService<SettedGlobalOptsSet>();
        var globalOptsSet = serviceProvider.GetRequiredService<GlobalOptsSet>();
        var cmdArgs = serviceProvider.GetRequiredService<CommandLineArgs>();
        settedGlobalOptsSet.Configure(globalOptsSet, cmdArgs);
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
