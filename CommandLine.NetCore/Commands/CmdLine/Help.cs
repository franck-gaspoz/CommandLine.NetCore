using System.Reflection;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Commands.CmdLine;

/// <summary>
/// command line help
/// </summary>
internal sealed class Help : Command
{
    private readonly CommandsSet _commandsSet;
    private readonly GlobalOptsSet _globalOptsSet;
    private readonly IServiceProvider _serviceProvider;

    public Help(
        IConfiguration config,
        CommandsSet commands,
        GlobalOptsSet globalOptsSet,
        IAnsiVtConsole console,
        Texts texts,
        ArgBuilder argBuilder,
        SettedGlobalOptsSet settedGlobalOptsSet,
        Parser parser,
        IServiceProvider serviceProvider) :
            base(config, console, texts, argBuilder, settedGlobalOptsSet, parser)
    {
        _globalOptsSet = globalOptsSet;
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

    protected override CommandResult Execute(ArgSet args)
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

            OutputSectionTitle(Texts._("GlobalOptions"));
            DumpGlobalOptList();
        }
        else
        {
            DumpCommandHelp(args);
        }

        Console.Out.WriteLine();
        DumpInformationalData();
        Sep();

        return new CommandResult(Globals.ExitOk);
    }

    private void DumpCommandHelp(ArgSet args)
    {
        var command = _commandsSet.GetCommand(args[0]);
        if (command.GetLongDescriptions(out var longDescs))
            DumpSyntaxes(command, longDescs);
        else
            Console.Out.WriteLine(command.Name + StOff);
    }

    private void DumpInformationalData()
        => Console.Out.WriteLine(
            InformationalDataMessage +
            Texts._("CurrentCulture", Thread.CurrentThread.CurrentCulture.Name));

    private void DumpGlobalOptList()
    {
        foreach (var kvp in _globalOptsSet.Opts)
        {
            var globalOpt = (IOpt)_serviceProvider.GetRequiredService(kvp.Value);
            var isError = !globalOpt.GetDescription(out var argDesc)
                ? Console.Colors.Error.ToString() : "";
            Console.Out.WriteLine(
                ArgNameColor + argDesc.Key + $"{StOff} : "
                + isError + argDesc.Value + StOff);
        }
    }

    private void DumpCommandList()
    {
        foreach (var kvp in _commandsSet.Commands)
        {
            var command = (Command)_serviceProvider.GetRequiredService(kvp.Value);
            command.GetDescription(out var description);
            Console.Out.WriteLine(CommandNameColor + kvp.Key + $"{StOff} : " + description + StOff);
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

    private void DumpSyntaxes(
        Command command,
        List<KeyValuePair<string, string>> longDescriptions)
    {
        longDescriptions.Insert(
            0,
            new KeyValuePair<string, string>(
                "-h",
                Texts._("HelpAboutThisCommand")));

        foreach (var kvp in longDescriptions)
            DumpSyntax(command, kvp);
    }

    private void DumpSyntax(
        Command command,
        KeyValuePair<string, string> longDescription)
    {
        var desc = longDescription.Value.Trim();
        var descExists = !string.IsNullOrWhiteSpace(desc);

        DumpCommandSyntax(
            (command.Name + " " +
            longDescription.Key.Trim())
                .Trim());

        if (descExists)
            Console.Out.Write(" : " + desc);
        Console.Out.WriteLine();
    }

    private static string StringAt(int i, ref string[] t)
        => i <= t.Length ? t[i] : "";
}
