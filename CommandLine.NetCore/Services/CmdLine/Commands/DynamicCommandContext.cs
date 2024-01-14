using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// dynamic command context
/// </summary>
public class DynamicCommandContext
{
    /// <summary>
    /// global settings
    /// </summary>
    public GlobalSettings GlobalSettings { get; private set; }

    /// <summary>
    /// console
    /// </summary>
    public IAnsiVtConsole Console { get; private set; }

    /// <summary>
    /// texts
    /// </summary>
    public Texts Texts { get; private set; }

    /// <summary>
    /// logger
    /// </summary>
    public CoreLogger Logger { get; private set; }

    /// <summary>
    /// creates a new instance
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    internal DynamicCommandContext(Dependencies dependencies)
    {
        GlobalSettings = dependencies.GlobalSettings;
        Console = dependencies.Console;
        Texts = dependencies.Texts;
        Logger = dependencies.Logger;
    }

    /// <summary>
    /// creates a new instance
    /// </summary>
    /// <param name="globalSettings">global settings</param>
    /// <param name="console">console</param>
    /// <param name="texts">texts</param>
    /// <param name="logger">logger</param>
    internal DynamicCommandContext(
        GlobalSettings globalSettings,
        IAnsiVtConsole console,
        Texts texts,
        CoreLogger logger
        )
    {
        GlobalSettings = globalSettings;
        Console = console;
        Texts = texts;
        Logger = logger;
    }
}
