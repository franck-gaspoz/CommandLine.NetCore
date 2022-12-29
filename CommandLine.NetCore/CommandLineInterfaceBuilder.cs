using System.Reflection;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services;
using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static CommandLine.NetCore.Services.CmdLine.Globals;

using cons = AnsiVtConsole.NetCore;

namespace CommandLine.NetCore;

/// <summary>
/// command line interface
/// </summary>
public sealed class CommandLineInterfaceBuilder
{
    private readonly AssemblySet _assemblySet;

    private Action<IConfigurationBuilder>? _configureDelegate;

    private Action<IHostBuilder>? _buildDelegate;

    private AppHostBuilder? _appHostBuilder;

    /// <summary>
    /// creates a new instance builder
    /// </summary>
    public CommandLineInterfaceBuilder()
    {
        _assemblySet = new AssemblySet(Assembly.GetCallingAssembly());
        UseAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// add an assembly where to look up for commands and arguments
    /// <para>this can called multiple times</para>
    /// </summary>
    /// <param name="assembly">assembly</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder UseAssembly(Assembly assembly)
    {
        if (!_assemblySet.Assemblies.Contains(assembly))
            _assemblySet.Assemblies.Add(assembly);
        return this;
    }

    /// <summary>
    /// use assemblies from an assembly set
    /// </summary>
    /// <param name="assemblySet">assembly set</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder UseAssemblySet(AssemblySet assemblySet)
    {
        foreach (var assembly in assemblySet.Assemblies)
            UseAssembly(assembly);
        return this;
    }

    /// <summary>
    /// set up a configure delegate
    /// </summary>
    /// <param name="configureDelegate">configure delegate</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder UseConfigureDelegate(Action<IConfigurationBuilder> configureDelegate)
    {
        _configureDelegate = configureDelegate;
        return this;
    }

    /// <summary>
    /// set up a build delegate
    /// </summary>
    /// <param name="buildDelegate">build delegate</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder UseBuildDelegate(Action<IHostBuilder> buildDelegate)
    {
        _buildDelegate = buildDelegate;
        return this;
    }

    /// <summary>
    /// run the command line
    /// <para>run the command according to the command line arguments</para>
    /// <para>handle errors</para>
    /// <para>output to console command results and any error</para>
    /// </summary>
    /// <returns>return code of the command</returns>
    /// <exception cref="ArgumentException">argument error</exception>
    public int Run()
    {
        try
        {
            var host = _appHostBuilder!.AppHost;
            host.RunAsync();

            var texts = host.Services.GetRequiredService<Texts>();
            var console = host.Services.GetRequiredService<IAnsiVtConsole>();
            var commandSet = host.Services.GetRequiredService<CommandsSet>();
            var args = host.Services.GetRequiredService<CommandLineArgs>();
            var parser = host.Services.GetRequiredService<Parser>();
            var lineBreak = false;

            try
            {
                if (!args.Any())
                    throw new ArgumentException(texts._("MissingArguments"));

                console.Out.WriteLine();
                lineBreak = true;

                var command = commandSet.GetCommand(args.Args.ToArray()[0]);
                var commandResult = command.Run(
                    new ArgSet(
                        args.Args.ToArray()[1..]));

                console.Out.WriteLine();

                return commandResult.ExitCode;
            }
            catch (Exception commandExecutionException)
            {
                return ExitWithError(
                    commandExecutionException,
                    console,
                    lineBreak);
            }
        }
        catch (Exception hostBuilderException)
        {
            return ExitWithError(
                hostBuilderException,
                new cons.AnsiVtConsole(),
                false);
        }
    }

    /// <summary>
    /// build the command line interface async host initialized for the command line arguments
    /// <para>initalize the injectables dependencies</para>
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>this</returns>
    public CommandLineInterfaceBuilder Build(string[] args)
    {
        try
        {
            _appHostBuilder = new AppHostBuilder(
                args.ToList(),
                _assemblySet,
                _configureDelegate,
                _buildDelegate);
        }
        catch (Exception hostBuilderException)
        {
            Environment.Exit(
                ExitWithError(
                    hostBuilderException,
                    new cons.AnsiVtConsole(),
                    false));
        }
        return this;
    }

    private static int ExitWithError(
        Exception ex,
        IAnsiVtConsole console,
        bool lineBreak)
    {
        if (!lineBreak)
            console.Logger.LogError();
        console.Logger.LogError(ex.Message);
        console.Logger.LogError();
        return ExitFail;
    }
}
