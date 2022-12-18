﻿using System.Reflection;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Service.CmdLine.GlobalArgs;
using CommandLine.NetCore.Services.CmdLine;

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
    private const string ArgValueColor = "(bon,f=cyan)";
    private const string StOff = "(tdoff)";

    private void Sep() => Console.Out.WriteLine(TitleColor + "".PadLeft(50, '-'));

    protected override int Execute(string[] args)
    {
        OutputAppTitle();

        if (args.Length == 0)
        {
            OutputSectionTitle(Texts._("Syntax"));
            DumpCommandSyntax(Texts._("GlobalSyntax"));
            Console.Out.WriteLine();
            Console.Out.WriteLine();

            OutputSectionTitle(Texts._("Commands"));
            foreach (var kvp in _commandsSet.Commands)
            {
                var command = (Command)_serviceProvider.GetRequiredService(kvp.Value);
                Console.Out.WriteLine(CommandNameColor + kvp.Key + $"{StOff} : " + command.ShortDescription());
            }
            Console.Out.WriteLine();
            OutputSectionTitle(Texts._("GlobalArgs"));
            foreach (var kvp in _globalArgsSet.Args)
            {
                var globalArg = (GlobalArg)_serviceProvider.GetRequiredService(kvp.Value);
                Console.Out.WriteLine(ArgNameColor + globalArg.Prefix + kvp.Key + $"{StOff} : " + globalArg.Description());
            }
        }
        else
        {
            var command = _commandsSet.GetCommand(args[0]);
            DumpLongDescription(command.LongDescription());
        }
        Console.Out.WriteLine();
        Console.Out.WriteLine(Texts._("CurrentCulture", Thread.CurrentThread.CurrentCulture.Name));
        Sep();

        return Globals.ExitOk;
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

    private void DumpLongDescription(string text)
    {
        var lines = text
            .Replace("\r", "")
            .Split('\n');

        foreach (var line in lines)
        {
            var t = line.Split(':');
            var desc = StringAt(1, ref t).Trim();
            var descExists = !string.IsNullOrWhiteSpace(desc);

            var cmdSyntax = StringAt(0, ref t).Trim();

            DumpCommandSyntax(cmdSyntax);

            if (descExists)
                Console.Out.Write(" : " + desc);
            Console.Out.WriteLine();
        }
    }

    private static string StringAt(int i, ref string[] t)
        => i <= t.Length ? t[i] : "";
}
