using System.Reflection;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Service.CmdLine.Arguments.GlobalArgs;
using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Commands.CmdLine;

/// <summary>
/// command line help
/// </summary>
internal sealed class HelpCommand : Command
{
    private readonly CommandsSet _commandsSet;
    private readonly GlobalArgsSet _globalArgsSet;
    private readonly IServiceProvider _serviceProvider;

    public HelpCommand(
        IConfiguration config,
        CommandsSet commands,
        GlobalArgsSet globalArgsSet,
        IAnsiVtConsole console,
        Texts texts,
        IServiceProvider serviceProvider) :
            base(config, console, texts, 0, 1)
    {
        _globalArgsSet = globalArgsSet;
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
    }

    private const string TitleColor = "(bon,f=cyan)";
    private const string SectionTitleColor = "(uon,f=yellow,bon)";
    private const string CommandNameColor = "(bon,f=green)";
    private const string ArgNameColor = "(f=darkyellow)";
    private const string InformationalDataMessage = "(f=darkgreen)";
    private const string ArgValueColor = "(bon,f=cyan)";
    private const string StOff = "(tdoff)";

    private void Sep() => Console.Out.WriteLine(TitleColor + "".PadLeft(50, '-'));

    protected override int Execute(ArgSet args)
    {
        OutputAppTitle();

        if (args.Count == 0)
        {
            OutputSectionTitle(Texts._("Syntax"));

            DumpCommandSyntax(Texts._("GlobalSyntax"));
            Console.Out.WriteLine();
            Console.Out.WriteLine();

            OutputSectionTitle(Texts._("Commands"));
            DumpCommandList();
            Console.Out.WriteLine();

            OutputSectionTitle(Texts._("GlobalArgs"));
            DumpGlobalArgList();
        }
        else
        {
            DumpCommandHelp(args);
        }

        Console.Out.WriteLine();
        DumpInformationalData();
        Sep();

        return Globals.ExitOk;
    }

    private void DumpCommandHelp(ArgSet args)
    {
        var command = _commandsSet.GetCommand(args[0]);
        if (command.GetLongDescriptions(out var longDescs))
            DumpLongDescriptions(longDescs);
        else
            Console.Out.WriteLine(longDescs[0].Key + StOff);
    }

    private void DumpInformationalData()
        => Console.Out.WriteLine(
            InformationalDataMessage +
            Texts._("CurrentCulture", Thread.CurrentThread.CurrentCulture.Name));

    private void DumpGlobalArgList()
    {
        foreach (var kvp in _globalArgsSet.Args)
        {
            var globalArg = (GlobalArg)_serviceProvider.GetRequiredService(kvp.Value);
            globalArg.GetDescription(out var argDesc);
            Console.Out.WriteLine(ArgNameColor + argDesc.Key + $"{StOff} : " + argDesc.Value);
        }
    }

    private void DumpCommandList()
    {
        foreach (var kvp in _commandsSet.Commands)
        {
            var command = (Command)_serviceProvider.GetRequiredService(kvp.Value);
            command.GetShortDescription(out var shortDesc);
            Console.Out.WriteLine(CommandNameColor + kvp.Key + $"{StOff} : " + shortDesc + StOff);
        }
    }

    private void OutputAppTitle()
    {
        Sep();
        var date =
            DateOnly.ParseExact(
            Config.GetValue<string>("App:ReleaseDate")!,
            Globals.SettingsDateFormat,
            null);
        Console.Out.WriteLine(TitleColor + Config.GetValue<string>("App:Title")!
            + $" ({Assembly.GetExecutingAssembly().GetName().Version} {date})");
        Sep();
        Console.Out.WriteLine();
    }

    private void OutputSectionTitle(string text)
        => Console.Out.WriteLine(SectionTitleColor + text + StOff);

    private void DumpCommandSyntax(string text)
    {
        var args = text.Split(' ');
        var cmdName = StringAt(0, ref args);

        Console.Out.Write(CommandNameColor + cmdName + StOff);

        for (var i = 1; i < args.Length; i++)
        {
            var arg = args[i];
            arg = arg.StartsWith('-') ?
                ArgNameColor + arg
                : ArgValueColor + arg;

            Console.Out.Write(" " + arg);
        }

        Console.Out.Write(StOff);
    }

    private void DumpLongDescriptions(List<KeyValuePair<string, string>> longDescriptions)
    {
        foreach (var kvp in longDescriptions)
            DumpLongDescription(kvp);
    }

    private void DumpLongDescription(KeyValuePair<string, string> longDescription)
    {
        var desc = longDescription.Value.Trim();
        var descExists = !string.IsNullOrWhiteSpace(desc);

        DumpCommandSyntax(longDescription.Key.Trim());

        if (descExists)
            Console.Out.Write(" : " + desc);
        Console.Out.WriteLine();
    }

    private static string StringAt(int i, ref string[] t)
        => i <= t.Length ? t[i] : "";
}
