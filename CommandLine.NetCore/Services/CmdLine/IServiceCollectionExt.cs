﻿using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Settings;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine;

static class IServiceCollectionExt
{
    /// <summary>
    /// add commands founded in the executing assembly as injectable dependencies
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="assemblySet">assembly set where lookup for commands and arguments</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddCommands(
        this IServiceCollection services,
        AssemblySet assemblySet
        )
    {
        services.AddSingleton<CommandsSet>();

        foreach (var classType in CommandsSet.GetCommandTypes(assemblySet))
            services.AddTransient(classType);

        return services;
    }

    /// <summary>
    /// add command line arguments as a injectable dependcy
    /// </summary>
    /// <param name="services">service collection</param>
    /// <param name="args">command line args</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddCommandLineArgs(
        this IServiceCollection services,
        List<string> args)
    {
        services.AddSingleton(
            serviceProvider =>
                new CommandLineArgs(args));

        return services;
    }

    /// <summary>
    /// add global arguments founded in the executing assembly as injectable dependencies
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    /// <param name="assemblySet">assembly set where lookup for commands and arguments</param>
    public static IServiceCollection AddGlobalArguments(
        this IServiceCollection services,
        AssemblySet assemblySet)
    {
        services.AddSingleton<GlobalOptsSet>();

        foreach (var classType in GlobalOptsSet.GetGlobalOptTypes(assemblySet))
            services.AddSingleton(classType);

        return services;
    }

    /// <summary>
    /// configure output. creates and configure a console
    /// </summary>
    /// <param name="services">services</param>
    /// <returns>services</returns>
    public static IServiceCollection ConfigureOutput(this IServiceCollection services)
    {
        services.AddSingleton(
            serviceProvider =>
            {
                return new ConsoleFactory(serviceProvider)
                    .Create();
            });
        return services;
    }

    /// <summary>
    /// <para>initialize global settings</para>
    /// <para>add global arguments founded in command line arguments as injectable dependencies</para>
    /// </summary>
    /// <param name="services">service collection</param>
    /// <returns>service collection</returns>
    public static IServiceCollection AddGlobalSettings(
        this IServiceCollection services)
    {
        services.AddSingleton<GlobalSettings>();

        return services;
    }
}
