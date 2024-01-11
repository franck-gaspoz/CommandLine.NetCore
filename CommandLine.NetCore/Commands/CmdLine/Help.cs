using System.Reflection;
using System.Text;

using AnsiVtConsole.NetCore.Component.Console;

using CommandLine.NetCore.GlobalOpts;
using CommandLine.NetCore.Services.AppHost;
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
[Package(Packages.cmdLine)]
[Tag(Tags.cmdLine, Tags.shell, Tags.help)]
sealed class Help : Command
{
    readonly CommandsSet _commandsSet;
    readonly GlobalOptsSet _globalOptsSet;
    readonly GlobalSettings _globalSettings;
    readonly IServiceProvider _serviceProvider;
    readonly Configuration _config;

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
        IServiceProvider serviceProvider,
        Configuration config) : base(dependencies)
    {
        _globalOptsSet = dependencies.GlobalSettings.GlobalOptsSet;
        _globalSettings = dependencies.GlobalSettings;
        _serviceProvider = serviceProvider;
        _commandsSet = commands;
        _config = config;
    }

    /// <inheritdoc/>
    protected override SyntaxMatcherDispatcher Declare() =>

        // Opt (isOptional:true) maps to nullable
        For(Opt("n", true, 1), Opt("p", true, 1))
            .Do(() => DumpHelpForAllCommands)

        .For(Param())
            .Do(() => DumpCommandHelp)

        // Flag map to bool and not bool? (for conveniency)
        // Flag is equivalent to Opt<bool>(name,true,0) with a non nullable target type (as for Flag)
        .Options(Flag("v"), Flag("info"));

    void DumpHelpForAllCommands(string? n, string? p, bool verbose, bool info)
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

    void DumpCommandHelp(string commandName, bool verbose, bool info)
    {
        OutputAppTitle();

        var comProps = _commandsSet.GetProperties(commandName);
        DumpCommandDescription(
            comProps,
            false,
            false,
            false,
            false,
            commandName.Length + 3);

        Console.Out.WriteLine();

        if (Command.GetLongDescriptions(
            commandName,
            _config,
            Texts,
            Console,
            out var longDescs))
            DumpSyntaxes(commandName, longDescs);

        if (verbose)
        {
            DumpCommandPackage(comProps, Br);
            var hasTags = DumpCommandTags(comProps, string.Empty);
            DumpCommandNamespace(comProps, string.Empty);
        }

        if (Command.GetOptionsDescriptions(
            commandName,
            Config,
            out var optDescs))
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

    bool DumpCommandPackage(CommandProperties comProps, string prefix = "")
    {
        var tags = comProps.Package;
        Console.Out.WriteLine(
            prefix
            + "(f=darkgray)package: "
            + CommandTagsColor + string.Join(',', tags) + StOff);
        return true;
    }

    bool DumpCommandTags(CommandProperties comProps, string prefix = "")
    {
        var tags = comProps.Tags;
        if (!tags.Any())
            return false;
        Console.Out.WriteLine(
            prefix
            + "(f=darkgray)tags: "
            + CommandTagsColor + string.Join(',', tags) + StOff);
        return true;
    }

    void DumpCommandNamespace(CommandProperties comProps, string prefix = "")
        => Console.Out.WriteLine(
            prefix
            + "(f=darkgray)namespace: "
            + CommandNamespaceColor + comProps.Namespace + StOff);

    void DumpCommandOptions(List<KeyValuePair<string, string>> optDescs)
    {
        Console.Out.WriteLine();
        OutputSectionTitle(Texts._("CommandOptions"));
        var padLeft = optDescs.Any() ?
            optDescs.Max(x => x.Key.Length) : 0;
        foreach (var optDesc in optDescs)
            DumpOpt(optDesc, true, padLeft);
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
        var padLeft = _globalOptsSet.Opts.Any() ?
            _globalOptsSet.Opts.Max(x => x.Key.Length) : 0;
        foreach (var kvp in _globalOptsSet.Opts)
        {
            var globalOpt = (IOpt)_serviceProvider.GetRequiredService(kvp.Value);
            var descriptionAvailable = globalOpt.GetDescription(out var argDesc);
            DumpOpt(
                argDesc,
                descriptionAvailable,
                padLeft + 2
                );
        }
    }

    void DumpOpt(
        KeyValuePair<string, string> optDesc,
        bool descriptionAvailable,
        int padLeft)
    {
        var isError = !descriptionAvailable
            ? Console.Colors.Error.ToString() : "";
        Console.Out.WriteLine(
            ArgNameColor
            + optDesc.Key
            + StOff
            + "".PadLeft(padLeft - optDesc.Key.Length + 3)
            + isError + optDesc.Value + StOff);
    }

    void DumpCommandList(bool v)
    {
        var commands = _commandsSet.GetCommandNames();
        _commandsSet.BuildDynamicCommands();

        var maxCommandNameLength = commands.Any() ? commands.Max(
            x => x.Value.Length) : 0;
        foreach (var command in commands)
        {
            var comProps = _commandsSet.GetProperties(command.Value);
            DumpCommandDescription(
                comProps,
                true,
                v,
                v,
                v,
                maxCommandNameLength + 3);
        }
    }

    void DumpCommandDescription(
        CommandProperties comProps,
        bool withName,
        bool dumpNamespace,
        bool dumpTags,
        bool dumpPackage,
        int padLeft)
    {
        var commandName = comProps.Name;
        if (_commandsSet.IsDynamicCommand(commandName))
            _commandsSet.BuildDynamicCommand(commandName);

        Command.GetDescription(
            commandName,
            _config,
            Texts,
            Console,
            out var description);

        if (withName)
        {
            var margin = "".PadLeft(padLeft - commandName.Length);
            Console.Out.Write(CommandNameColor + commandName + $"{StOff}{margin}");
        }
        Console.Out.WriteLine(description + StOff);

        var sep = (dumpTags | dumpNamespace) ?
            ("".PadLeft(padLeft))
            : string.Empty;

        if (dumpPackage)
            DumpCommandPackage(comProps, sep);
        if (dumpTags)
            DumpCommandTags(comProps, sep);
        if (dumpNamespace)
            DumpCommandNamespace(comProps, sep);
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
        string commandName,
        List<KeyValuePair<string, string>> longDescriptions)
    {
        longDescriptions.Insert(
            0,
            new KeyValuePair<string, string>(
                "-h",
                Texts._("HelpAboutThisCommand")));

        var margin = longDescriptions.Any() ?
            longDescriptions.Max(x => x.Key.Trim().Length) : 0;

        foreach (var kvp in longDescriptions)
            DumpSyntax(commandName, kvp, margin);
    }

    void DumpSyntax(
        string commandName,
        KeyValuePair<string, string> longDescription,
        int margin)
    {
        var desc = longDescription.Value.Trim();
        var descExists = !string.IsNullOrWhiteSpace(desc);
        var args = longDescription.Key.Trim();

        DumpCommandSyntax(
            ((_globalSettings.IsGlobalOptionSet<DisableGlobalHelp>()
                ? string.Empty
                : commandName + " ")
            + args
            + "".PadLeft(margin - args.Length)
            ));

        if (descExists)
            Console.Out.Write("   " + desc);
        Console.Out.WriteLine();
    }

    void Sep() => Console.Out.WriteLine(SepColor + "".PadLeft(50, ' '));

    static string StringAt(int i, ref string[] t)
        => i <= t.Length ? t[i] : "";
}
