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
        => CommandLineInterface.Run(args);
}