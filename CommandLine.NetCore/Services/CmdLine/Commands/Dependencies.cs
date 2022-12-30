
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command base dependencies
/// </summary>
public class Dependencies
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
    /// arg builder
    /// </summary>
    public ArgBuilder ArgBuilder { get; private set; }

    /// <summary>
    /// parser
    /// </summary>
    public Parser Parser { get; private set; }

    /// <summary>
    /// build set of command dependencies
    /// </summary>
    /// <param name="globalSettings">global settings</param>
    /// <param name="console">console</param>
    /// <param name="texts">texts</param>
    /// <param name="argBuilder">arg builder</param>
    /// <param name="parser">parser</param>
    public Dependencies(
        GlobalSettings globalSettings,
        IAnsiVtConsole console,
        Texts texts,
        ArgBuilder argBuilder,
        Parser parser)
    {
        GlobalSettings = globalSettings;
        Console = console;
        Texts = texts;
        ArgBuilder = argBuilder;
        Parser = parser;
    }
}
