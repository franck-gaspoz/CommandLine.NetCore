using System.Collections;

using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Commands.Attributes;
using CommandLine.NetCore.Services.CmdLine.Running;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Example.Commands;

/// <summary>
/// get-info command
/// </summary>
[Package(Packages.example)]
[Tag(Tags.dev, Tags.test)]
[Tag(Tags.shell, Tags.console)]
sealed class GetInfo : Command
{
    /*
     * syntax list:
     * getinfo env varName      : dump environment variable value
     * getinfo env -l           : list of environment variables names and values
     * getinfo console          : dump infos about console
     * getinfo system           : dump infos about system
     * getinfo --all            : list all infos
     */

    /// <inheritdoc/>
    public GetInfo(Dependencies dependencies) : base(dependencies) { }

    /// <inheritdoc/>
    protected override SyntaxMatcherDispatcher Declare() =>

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
                .Do(() => DumpEnvVar)

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
            Flag("all"))
                .Do(DumpAll);

    void DumpAll()
    {
        DumpSystem();

        Console.Out.WriteLine();

        DumpConsole();

        Console.Out.WriteLine();

        DumpAllVars();
    }

    void DumpSystem()
    {
        OutputSectionTitle(Texts._("SystemInformations"));

        Dictionary<string, string> keyvalues = new()
        {
            {
                Texts._("OS"),
                Environment.OSVersion.ToString()
            },
            {
                "ProcArch",
                ToText(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"))
            },
            {
                Texts._("ProcModel"),
                ToText(Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"))
            },
            {
                Texts._("ProcLevel"),
                ToText(Environment.GetEnvironmentVariable("PROCESSOR_LEVEL"))
            },
            {
                Texts._("SysDir"),
                Environment.SystemDirectory
            },
            {
                Texts._("ProcCount"),
                Environment.ProcessorCount.ToString()
            },
            {
                Texts._("UserDomainName"),
                Environment.UserDomainName
            },
            {
                Texts._("UserName"),
                Environment.UserName
            },
            {
                Texts._("OSVersion"),
                Environment.Version.ToString()
            }
        };

        foreach (var driveInfo in DriveInfo.GetDrives())
        {
            try
            {
                var volumeLabel = driveInfo.VolumeLabel;
                var name = driveInfo.Name;
                keyvalues.Add(name, string.Empty);
                keyvalues.Add(name + " " +
                    Texts._("VolumeLabel"),
                    volumeLabel);
                keyvalues.Add(name + " " +
                    Texts._("DriveType"),
                    driveInfo.DriveType.ToString());
                keyvalues.Add(name + " " +
                    Texts._("DriveFormat"),
                    driveInfo.DriveFormat);
                keyvalues.Add(name + " " +
                    Texts._("TotalSize"),
                    driveInfo.TotalSize.ToString());
                keyvalues.Add(name + " " +
                    Texts._("AvailableFreeSpace"),
                    driveInfo.AvailableFreeSpace.ToString());
            }
            catch
            {
            }
        }

        foreach (var kvp in keyvalues)
            OutputKeyValue(kvp.Key, kvp.Value);
    }

    void DumpConsole()
    {
        OutputSectionTitle(Texts._("ConsoleInformations"));
        Console.Infos();
    }

    void DumpEnvVar(string envVarName)
    {
        var value = Environment.GetEnvironmentVariable(envVarName) ?? throw new ArgumentException(
                Texts._("VariableIsNotDefined"));
        OutputKeyValue(envVarName, value);
    }

    void DumpAllVars()
    {
        OutputSectionTitle(Texts._("EnvironmentVariables"));
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
    }

    #region utils

    void OutputKeyValue(string key, string value)
        => Console.Out.WriteLine($"(f=cyan){key} (f=gray)= (f=green){value}");

    public static string ToText(object? obj, string ifNullText = "?")
        => obj is null ? ifNullText : obj.ToString()!;

    void OutputSectionTitle(string text)
        => Console.Out.WriteLine(SectionTitleColor + text + ":" + StOff + "(br)");

    const string SectionTitleColor = "(uon,f=yellow,bon)";
    const string StOff = "(tdoff)";

    #endregion
}
