using System.Diagnostics;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Extensions;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// abstract command
/// </summary>
[DebuggerDisplay("{Name}")]
public abstract class Command
{
    #region properties

    /// <summary>
    /// console service
    /// </summary>
    protected readonly IAnsiVtConsole Console;

    /// <summary>
    /// texts service
    /// </summary>
    protected readonly Texts Texts;

    /// <summary>
    /// parser
    /// </summary>
    protected readonly Parser Parser;

    /// <summary>
    /// setted global options
    /// </summary>
    protected readonly GlobalSettings GlobalSettings;

    readonly CommandBuilder _builder;

    /// <summary>
    /// app config
    /// </summary>
    protected readonly Configuration Config;

    /// <summary>
    /// name of the command
    /// </summary>
    public virtual string Name => ClassNameToCommandName();

    /// <summary>
    /// command specification
    /// </summary>
    protected SyntaxMatcherDispatcher SyntaxMatcherDispatcher { get; set; }

    #endregion

    /// <summary>
    /// builds a new command
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    public Command(Dependencies dependencies)
    {
        Console = dependencies.Console;
        Texts = dependencies.Texts;
        Parser = dependencies.Parser;
        GlobalSettings = dependencies.GlobalSettings;

        _builder = new(
            dependencies,
            ClassNameToCommandName(),
            RunCommand);

        Config = dependencies
                .GlobalSettings
                .Configuration;

        SyntaxMatcherDispatcher = Declare();
    }

    /// <summary>
    /// run the command with the specified arguments
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    public CommandResult Run(ArgSet args)
    {
        var commandResult = SyntaxMatcherDispatcher.With(args);

        if (commandResult.ParseErrors.Any())
        {
            Console.Logger.LogError(
                string.Join(
                    Environment.NewLine,
                    commandResult.ParseErrors)
                + "(br)");
        }

        return commandResult;
    }

#if no
    /// <summary>
    /// command run body to be implemented by subclasses
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    protected abstract CommandResult Execute(ArgSet args);
#endif

    /// <summary>
    /// declares the command syntax and implementation mappings
    /// </summary>
    /// <returns></returns>
    protected abstract SyntaxMatcherDispatcher Declare();

    #region translaters, helpers

    /// <summary>
    /// returns the command name from this class type
    /// </summary>
    /// <returns>command name</returns>
    public virtual string ClassNameToCommandName()
        => ClassNameToCommandName(GetType().Name);

    /// <summary>
    /// transforms a class type name to a commande name
    /// </summary>
    /// <param name="className">command type name</param>
    /// <returns>command name</returns>
    public static string ClassNameToCommandName(string className)
        => className.ToKebabCase()!;

    #endregion

    #region command help

    /// <summary>
    /// short description of the command
    /// </summary>
    /// <param name="text">short description or error text if not found in help settings</param>
    /// <returns>text of the description</returns>
    public bool GetDescription(out string text)
    {
        var desc = Config.Get(
            HelpBuilder.ShortDescriptionKey(
                ClassNameToCommandName()));

        if (desc is not null)
        {
            text = desc;
            return true;
        }
        text = Console.Colors.Error + Texts._(
            "CommandShortHelpNotFound",
            ClassNameToCommandName());
        return false;
    }

    /// <summary>
    /// get descriptions for the command options
    /// </summary>
    /// <param name="texts">founded descriptions or empty list</param>
    /// <returns>true if some options exists</returns>
    public bool GetOptionsDescriptions(out List<KeyValuePair<string, string>> texts)
    {
        var sectionName = $"Commands:{ClassNameToCommandName()}:Options";
        var optDescs = Config.GetSection(sectionName);

        if (!optDescs.Exists() || !optDescs.GetChildren().Any())
        {
            texts = new List<KeyValuePair<string, string>>();
            return false;
        }

        texts = new List<KeyValuePair<string, string>>();
        foreach (var optDesc in optDescs.GetChildren())
        {
            texts.Add(
                new KeyValuePair<string, string>(
                    optDesc.Key,
                    optDesc.Value ?? string.Empty
                ));
        }
        return true;
    }

    /// <summary>
    /// long descriptions of the command
    /// </summary>
    /// <param name="texts">command syntaxes descriptions or error text if not found in help settings</param>
    /// <returns>text of the descriptions of each command syntax. key is the syntax, value is the description</returns>
    public bool GetLongDescriptions(out List<KeyValuePair<string, string>> texts)
    {
        var descs = Config.GetSection(
            HelpBuilder.LongDescriptionKey(
                ClassNameToCommandName()));

        if (!descs.Exists() || !descs.GetChildren().Any())
        {
            texts = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string,string>(
                    Console.Colors.Error +
                    Texts._("CommandLongHelpNotFound",
                        ClassNameToCommandName()),
                    string.Empty)
            };
            return false;
        }

        texts = new List<KeyValuePair<string, string>>();
        foreach (var desc in descs.GetChildren())
        {
            texts.Add(
                new KeyValuePair<string, string>(
                    desc.Key, desc.Value ?? string.Empty));
        }

        return true;
    }

    #endregion

    #region args build helpers

    /// <summary>
    /// build a new option
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <param name="valueCount">value count</param>
    /// <returns>Opt</returns>
    protected Opt Opt(string name, bool isOptional = false, int valueCount = 0)
        => _builder.Opt(name, isOptional, valueCount);

    /// <summary>
    /// build a new flag (option with no possibilty of values)
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Flag</returns>
    protected Flag Flag(string name, bool isOptional = false)
        => _builder.Flag(name, isOptional);

    /// <summary>
    /// build a new generic option
    /// </summary>
    /// <typeparam name="T">type of options values</typeparam>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Opt{T}</returns>
    protected Opt<T> Opt<T>(string name, bool isOptional = false)
        where T : class
            => _builder.Opt<T>(name, isOptional);

    /// <summary>
    /// build a new parameter
    /// </summary>
    /// <typeparam name="T">type of parameter value</typeparam>
    /// <param name="value"></param>
    /// <returns>Param{T}</returns>
    protected Param<T> Param<T>(string? value = null)
        where T : struct
            => _builder.Param<T>(value);

    /// <summary>
    /// build a new string parameter
    /// </summary>
    /// <param name="value">eventual value</param>
    /// <returns>Param</returns>
    protected Param Param(string? value = null)
        => _builder.Param(value);

    /// <summary>
    /// build an option set
    /// </summary>
    /// <param name="options">options</param>
    /// <returns>option set</returns>
    protected static OptSet OptSet(params IOpt[] options)
        => new(options);

    #endregion

    #region syntax builders

    /// <summary>   
    /// build a syntax from arguments syntaxes set
    /// </summary>
    /// <param name="syntax">arguments syntaxes</param>
    /// <returns>a syntax dispatcher map item</returns>
    protected SyntaxExecutionDispatchMapItem For(params Arg[] syntax)
        => _builder.For(syntax);

    #endregion

    #region command run

    /// <summary>
    /// run a command from command line arguments
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>operation result</returns>
    public CommandLineResult RunCommand(params string[] args)
    {
        var commandLineInterfaceBuilder = GlobalSettings
            .CommandLineInterfaceBuilder!;

        var serviceProvider =
            commandLineInterfaceBuilder
            .AppHost!
            .Services!;

        serviceProvider
            .ConfigureCommandLineArgs(args)
            .ConfigureGlobalSettings()
            .ConfigureOutput();

        return new(
            commandLineInterfaceBuilder.Run()
            );
    }

    /// <summary>
    /// run a command from command line arguments in a new host
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>operation result</returns>
    public CommandLineResult RunCommandInSeparateHost(params string[] args) =>
        new(
            new CommandLineInterfaceBuilder()
                .UseAssemblySet(GlobalSettings.AssemblySet)
                .UseConfigureDelegate(GlobalSettings
                    .AppHostConfiguration
                    .ConfigureDelegate)
                .UseBuildDelegate(GlobalSettings
                    .AppHostConfiguration
                    .BuildDelegate)
                .Build(args)
                .Run());

    #endregion
}

