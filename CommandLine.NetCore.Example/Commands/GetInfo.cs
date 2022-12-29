using System.Collections;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

using static CommandLine.NetCore.Services.CmdLine.Globals;

namespace CommandLine.NetCore.Example.Commands;

internal sealed class GetInfo : Command
{
    //private readonly Arg _env;
    /*
     * syntax list:
     * getinfo env varName      : dump environment variable value
     * getinfo env -l           : list of environment variables names and values
     * getinfo console          : dump infos about console
     * getinfo system           : dump infos about system
     * getinfo --all            : list all infos
     */

    public GetInfo(
        IConfiguration config,
        IAnsiVtConsole console,
        ArgBuilder argBuilder,
        SettedGlobalOptsSet settedGlobalOptsSet,
        Parser parser,
        Texts texts) :
            base(config, console, texts, argBuilder, settedGlobalOptsSet, parser)
    { }

    /// <inheritdoc/>
    protected override CommandResult Execute(ArgSet args) =>

        // env -l

        For(
            Param("env"),
            Opt("l")
            )
                .Do(DumpAllVars)

        // env varName

        .For(
            Param("env"),
            Param())
                .Do(DumpEnvVar)

        // console

        .For(
            Param("console"))
                .Do(DumpConsole)

        // system

        .For(
            Param("system"))
                .Do(DumpSystem)

        // --all

        .For(
            Opt("all"))
                .Do(DumpAll)

        .With(args);

    private OperationResult DumpAll(Grammar grammar)
    {
        var err = new OperationResult(ExitFail);

        DumpSystem(grammar);

        Console.Out.WriteLine();

        DumpConsole(grammar);

        Console.Out.WriteLine();

        DumpAllVars(grammar);

        return new(ExitOk);
    }

    private OperationResult DumpSystem(Grammar grammar)
    {
        OutputSectionTitle("system informations");

        Dictionary<string, string> keyvalues = new();

        keyvalues.Add("Operation System", Environment.OSVersion.ToString());
        keyvalues.Add("Processor Architecture",
            ToText(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")));
        keyvalues.Add("Processor Model",
            ToText(Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")));
        keyvalues.Add("Processor Level",
            ToText(Environment.GetEnvironmentVariable("PROCESSOR_LEVEL")));
        keyvalues.Add("SystemDirectory", Environment.SystemDirectory);
        keyvalues.Add("ProcessorCount", Environment.ProcessorCount.ToString());
        keyvalues.Add("UserDomainName", Environment.UserDomainName);
        keyvalues.Add("UserName", Environment.UserName);
        keyvalues.Add("Version", Environment.Version.ToString());

        foreach (var driveInfo in DriveInfo.GetDrives())
        {
            try
            {
                var volumeLabel = driveInfo.VolumeLabel;
                var name = driveInfo.Name;
                keyvalues.Add(name, string.Empty);
                keyvalues.Add(name + " VolumeLabel", volumeLabel);
                keyvalues.Add(name + " DriveType", driveInfo.DriveType.ToString());
                keyvalues.Add(name + " DriveFormat", driveInfo.DriveFormat);
                keyvalues.Add(name + " TotalSize", driveInfo.TotalSize.ToString());
                keyvalues.Add(name + " AvailableFreeSpace", driveInfo.AvailableFreeSpace.ToString());
            }
            catch
            {
            }
        }

        foreach (var kvp in keyvalues)
            OutputKeyValue(kvp.Key, kvp.Value);

        return new(ExitOk);
    }

    private OperationResult DumpConsole(Grammar grammar)
    {
        OutputSectionTitle("console informations");
        Console.Infos();
        return new(ExitOk);
    }

    private OperationResult DumpEnvVar(Grammar grammar)
    {
        var varName = ((Param<string>)grammar[1]).Value!;
        var value = Environment.GetEnvironmentVariable(varName!);
        if (value is null)
            throw new ArgumentException("variable is not defined");
        OutputKeyValue(varName!, value);
        return new(ExitOk);
    }

    private OperationResult DumpAllVars(Grammar grammar)
    {
        OutputSectionTitle("environment variables");
        var vars = Environment.GetEnvironmentVariables();
        foreach (var obj in vars)
        {
            if (obj is DictionaryEntry kvp)
            {
                OutputKeyValue(
                    kvp.Key.ToString()!,
                    ToText(kvp.Value, "{null}"));
            }
        }

        return new(ExitOk);
    }

    #region utils

    private void OutputKeyValue(string key, string value)
        => Console.Out.WriteLine($"(f=cyan){key} (f=gray)= (f=green){value}");

    public static string ToText(object? obj, string ifNullText = "?")
        => obj is null ? ifNullText : obj.ToString()!;

    private void OutputSectionTitle(string text)
        => Console.Out.WriteLine(SectionTitleColor + text + ":" + StOff + "(br)");

    private const string SectionTitleColor = "(uon,f=yellow,bon)";
    private const string StOff = "(tdoff)";

    #endregion
}
