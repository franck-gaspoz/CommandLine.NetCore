using System.Collections.ObjectModel;

namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// command line arguments
/// </summary>
public sealed class CommandLineArgs
{
    public ReadOnlyCollection<string> Args { get; private set; }

    public CommandLineArgs(List<string> args)
        => Args = new ReadOnlyCollection<string>(args);
}
