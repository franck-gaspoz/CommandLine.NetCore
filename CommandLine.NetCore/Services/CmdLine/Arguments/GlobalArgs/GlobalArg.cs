using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Service.CmdLine.Arguments.GlobalArgs;

/// <summary>
/// global argument abstraction
/// </summary>
public abstract class GlobalArg : Arg
{
    /// <summary>
    /// build a new global argument
    /// </summary>
    /// <param name="name">argument name</param>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="console">console</param>
    /// <param name="valuesCount">number of expected values</param>
    protected GlobalArg(
        string name,
        IConfiguration config,
        IAnsiVtConsole console,
        Texts texts,
        int valuesCount = 0) : base(
            name, config, texts, console, valuesCount)
    {
    }
}

