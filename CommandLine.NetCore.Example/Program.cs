using CommandLine.NetCore.Services.CmdLine;

namespace CommandLine.NetCore.Example;

public class Program
{
    // <summary>
    /// command line input
    /// <para>commandName (commandArgs|globalArg)*</para>
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>status code</returns>
    public static int Main(string[] args)
        => new CommandLineInterfaceBuilder()
            /*
            // enable this for single command mode (here: only get-info, no global help)
            .ForCommand<GetInfo>()
            .DisableGlobalHelp()
            */
            .Build(args)
            .Run();
}