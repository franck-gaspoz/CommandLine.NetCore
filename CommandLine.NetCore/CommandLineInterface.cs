using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.CmdLine;
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
public static class CommandLineInterface
{
    /// <summary>
    /// run the command line arguments
    /// <para>initalize the injectables dependencies</para>
    /// <para>run the command according to the command line arguments</para>
    /// <para>handle errors</para>
    /// <para>output to console command results and any error</para>
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <param name="configureDelegate">optional configure delegate</param>
    /// <param name="buildDelegate">optional build delegate</param>
    /// <returns>exit code</returns>
    public static int Run(
        string[] args,
        Action<IConfigurationBuilder>? configureDelegate = null,
        Action<IHostBuilder>? buildDelegate = null
        )
    {
        var argList = args.ToList();

        try
        {
            var host = new AppHostBuilder(
                argList,
                configureDelegate,
                buildDelegate).AppHost;
            var texts = host.Services.GetRequiredService<Texts>();
            var console = host.Services.GetRequiredService<IAnsiVtConsole>();
            var commandSet = host.Services.GetRequiredService<CommandsSet>();
            var lineBreak = false;
            try
            {
                if (args.Length == 0)
                    throw new ArgumentException(texts._("MissingArguments"));

                console.Out.WriteLine();
                lineBreak = true;

                var command = commandSet.GetCommand(args[0]);
                var exitCode = command.Run(argList.ToArray()[1..]);

                console.Out.WriteLine();

                return exitCode;
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
