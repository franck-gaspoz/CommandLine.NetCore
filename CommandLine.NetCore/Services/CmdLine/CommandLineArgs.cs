using System.Collections.ObjectModel;

namespace CommandLine.NetCore.Services.CmdLine;

internal sealed class CommandLineArgs
{
    public ReadOnlyCollection<string> Args { get; private set; }

    public CommandLineArgs(List<string> args)
        => Args = new ReadOnlyCollection<string>(args);
}
