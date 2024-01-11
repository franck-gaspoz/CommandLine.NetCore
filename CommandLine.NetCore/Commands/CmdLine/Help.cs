using System.Reflection;
using System.Text;

using AnsiVtConsole.NetCore.Component.Console;

using CommandLine.NetCore.GlobalOpts;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Commands.Attributes;
using CommandLine.NetCore.Services.CmdLine.Running;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Commands.CmdLine;

/// <summary>
/// command line help
/// </summary>
[Tag(Tags.CmdLine, Tags.Shell, Tags.Help)]
sealed class Help : Command
{
    readonly CommandsSet _commandsSet;
    readonly GlobalOptsSet _globalOptsSet;
    readonly GlobalSettings _globalSettings;
    readonly IServiceProvider _serviceProvider;

    const string SepColor = "(uon,f=cyan,bon)";
    const string SectionTitleColor = "(uon,f=yellow,bon)";
    const string CommandNameColor = "(bon,f=green)";
    const string CommandTagsColor = "(f=blue)";
    const string CommandNamespaceColor = "(f=blue)";
    const string ArgNameColor = "(f=darkyellow)";
    const string InformationalDataMessage = "(f=darkgreen)";
    const string ArgValueColor = "(bon,f=cyan)";
    const string StOff = "(tdoff)";
    const string Br = "(br)";

    /// <inheritdoc/>
    public Help(
        Dependencies dependencies,
        CommandsSet commands,
        IServiceProvider serviceProvider) : base(dependencies)
    {
        _globalOptsSet = dependencies.GlobalSettings.GlobalOptsSet;
        _globalSettings = dependencies.GlobalSettings;
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
    }

    /// <inheritdoc/>
    protected override SyntaxMatcherDispatcher Declare() =>

        For()
            .Do(() => DumpHelpForAllCommands)

        .For(Param())
            .Do(() => DumpCommandHelp)

        .Options(Opt("v"), Opt("info"));

    void DumpHelpForAllCommands(bool verbose, bool info)
    {
        OutputAppTitle();

        OutputSectionTitle(Texts._("Syntax"));

        DumpCommandSyntax(Texts._("GlobalSyntax"));
        Console.Out.WriteLine();
        Console.Out.WriteLine();

        OutputSectionTitle(Texts._("Commands"));
        DumpCommandList(verbose);
        Console.Out.WriteLine();

        OutputSectionTitle(Texts._("GlobalOptions"));
        DumpGlobalOptList();

        CommandEnd(info);
    }

    void DumpCommandHelp(string comandName, bool verbose, bool info)
    {
        OutputAppTitle();

        var command = _commandsSet.GetCommand(comandName);

        DumpCommandDescription(command, false, false, false);

        Console.Out.WriteLine();

        if (command.GetLongDescriptions(out var longDescs))
            DumpSyntaxes(command, longDescs);

        if (verbose)
        {
            var hasTags = DumpCommandTags(command, Br);
            DumpCommandNamespace(command, hasTags ? string.Empty : Br);
        }

        if (command.GetOptionsDescriptions(out var optDescs))
            DumpCommandOptions(optDescs);

        CommandEnd(info);
    }

    string TextBox(List<string> lines)
    {
        var length = lines.Select(line => Console.Out.GetText(line).Length).Max();
        var textColor = "(bon,f=white)";
        var barColor = "(f=white)";
        var boxBackgroundColor = "(bkb,b=darkblue)";

        var clRight = ANSI.RSTXTA + ANSI.EL(ANSI.ELParameter.p0);

        var hBar = string.Empty;
        hBar = string.Empty.PadLeft(length + 2, '─');
        var topBar =
            barColor + boxBackgroundColor
            + "┌" + hBar + "┐" + clRight;
        var bottomBar =
            barColor + boxBackgroundColor +
            "└" + hBar + "┘" + clRight;

        var sb = new StringBuilder();
        sb.AppendLine(topBar);
        foreach (var line in lines)
        {
            sb.AppendLine(
                barColor + boxBackgroundColor + "│ "
                + textColor + line
                + barColor + " │"
                + clRight);
        }

        sb.Append(bottomBar);
        return sb.ToString();
    }

    bool DumpCommandTags(Command command, string prefix = "")
    {
        var tags = _commandsSet.GetTags(command.Name);
        if (!tags.Any())
            return false;
        Console.Out.WriteLine(
            prefix
            + "(f=darkgray)tags: "
            + CommandTagsColor + string.Join(',', tags) + StOff);
        return true;
    }

    void DumpCommandNamespace(Command command, string prefix = "")
        => Console.Out.WriteLine(
            prefix
            + "(f=darkgray)namespace: "
            + CommandNamespaceColor + command.GetType().Namespace + StOff);

    void DumpCommandOptions(List<KeyValuePair<string, string>> optDescs)
    {
        Console.Out.WriteLine();
        OutputSectionTitle(Texts._("CommandOptions"));
        foreach (var optDesc in optDescs)
            DumpOpt(optDesc, true);
    }

    void CommandEnd(bool info)
    {
        if (!info) return;
        Console.Out.WriteLine();
        DumpInformationalData();
        Sep();
    }

    void DumpInformationalData()
        => Console.Out.WriteLine(
            InformationalDataMessage +
            Texts._("CurrentCulture", Thread.CurrentThread.CurrentCulture.Name));

    void DumpGlobalOptList()
    {
        foreach (var kvp in _globalOptsSet.Opts)
        {
            var globalOpt = (IOpt)_serviceProvider.GetRequiredService(kvp.Value);
            var descriptionAvailable = globalOpt.GetDescription(out var argDesc);
            DumpOpt(
                argDesc,
                descriptionAvailable
                );
        }
    }

    void DumpOpt(
        KeyValuePair<string, string> optDesc,
        bool descriptionAvailable)
    {
        var isError = !descriptionAvailable
            ? Console.Colors.Error.ToString() : "";
        Console.Out.WriteLine(
            ArgNameColor + optDesc.Key + $"{StOff} :{Br}"
            + isError + optDesc.Value + StOff);
    }

    void DumpCommandList(bool v)
    {
        foreach (var command in _commandsSet.GetCommands())
            DumpCommandDescription(command, true, v, v);
    }

    void DumpCommandDescription(
        Command command,
        bool withName,
        bool dumpNamespace,
        bool dumpTags)
    {
        command.GetDescription(out var description);
        if (withName)
            Console.Out.Write(CommandNameColor + command.Name + $"{StOff} : ");
        Console.Out.WriteLine(description + StOff);

        var sep = (dumpTags | dumpNamespace) ?
            ("".PadLeft(command.Name.Length + 3))
            : string.Empty;

        if (dumpTags)
            DumpCommandTags(command, sep);
        if (dumpNamespace)
            DumpCommandNamespace(command, sep);
    }

    void OutputAppTitle()
    {
        var date =
            DateOnly.ParseExact(
                Config.Get("App:ReleaseDate")!,
                Globals.SettingsDateFormat,
                null);
        var lines = new List<string>
        {
            Config.Get("App:Title")!
            + $" ({Assembly.GetExecutingAssembly().GetName().Version} {date})"
        };
        Console.Out.WriteLine(TextBox(lines));
        Console.Out.WriteLine();
    }

    void OutputSectionTitle(string text)
        => Console.Out.WriteLine(SectionTitleColor + text + StOff + Br);

    void DumpCommandSyntax(string text)
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

    void DumpSyntaxes(
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

    void DumpSyntax(
        Command command,
        KeyValuePair<string, string> longDescription)
    {
        var desc = longDescription.Value.Trim();
        var descExists = !string.IsNullOrWhiteSpace(desc);

        DumpCommandSyntax(
            ((_globalSettings.IsGlobalOptionSet<DisableGlobalHelp>()
                ? string.Empty
                : command.Name + " ")
            + longDescription.Key.Trim())
                .Trim());

        if (descExists)
            Console.Out.Write(" : " + desc);
        Console.Out.WriteLine();
    }

    void Sep() => Console.Out.WriteLine(SepColor + "".PadLeft(50, ' '));

    static string StringAt(int i, ref string[] t)
        => i <= t.Length ? t[i] : "";
}
