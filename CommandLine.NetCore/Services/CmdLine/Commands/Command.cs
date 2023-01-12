#define Enable_h_Arg

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// abstract command
/// </summary>
public abstract class Command
{
    /// <summary>
    /// console service
    /// </summary>
    protected readonly IAnsiVtConsole Console;

    /// <summary>
    /// texts service
    /// </summary>
    protected readonly Texts Texts;

    /// <summary>
    /// app config
    /// </summary>
    protected readonly IConfiguration Config;

    /// <summary>
    /// parser
    /// </summary>
    protected readonly Parser Parser;

    /// <summary>
    /// setted global options
    /// </summary>
    protected readonly GlobalSettings GlobalSettings;

    /// <summary>
    /// name of the command
    /// </summary>
    public string Name => ClassNameToCommandName();

    private readonly ArgBuilder _argBuilder;

    private SyntaxMatcherDispatcher? _syntaxMatcherDispatcher;

    /// <summary>
    /// construit une instance de commande
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    public Command(Dependencies dependencies)
    {
        _argBuilder = dependencies.ArgBuilder;
        GlobalSettings = dependencies.GlobalSettings;
        Config = dependencies.GlobalSettings.Configuration;
        Texts = dependencies.Texts;
        Console = dependencies.Console;
        Parser = dependencies.Parser;
    }

    /// <summary>
    /// run the command with the specified arguments
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    public CommandResult Run(ArgSet args)
    {
        var commandResult = Execute(args);

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

    /// <summary>
    /// command run body to be implemented by subclasses
    /// </summary>
    /// <param name="args">args</param>
    /// <returns>return code</returns>
    /// <exception cref="NotImplementedException">not implemented</exception>
    protected abstract CommandResult Execute(ArgSet args);

    /// <summary>
    /// short description of the command
    /// </summary>
    /// <param name="text">short description or error text if not found in help settings</param>
    /// <returns>text of the description</returns>
    public bool GetDescription(out string text)
    {
        var desc = Config.GetValue<string>($"Commands:{ClassNameToCommandName()}:Description");
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
        var descs = Config.GetSection($"Commands:{ClassNameToCommandName()}:Syntax");

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

    #region translaters, helpers

    /// <summary>
    /// returns the command name from this class type
    /// </summary>
    /// <returns>command name</returns>
    public string ClassNameToCommandName()
        => ClassNameToCommandName(GetType().Name);

    /// <summary>
    /// transforms a class type name to a commande name
    /// </summary>
    /// <param name="className">command type name</param>
    /// <returns>command name</returns>
    public static string ClassNameToCommandName(string className)
        => className.ToKebabCase()!;

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
        => _argBuilder.Opt(name, isOptional, valueCount);

    /// <summary>
    /// build a new generic option
    /// </summary>
    /// <typeparam name="T">type of options values</typeparam>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Opt{T}</returns>
    protected Opt<T> Opt<T>(string name, bool isOptional = false)
        => _argBuilder.Opt<T>(name, isOptional);

    /// <summary>
    /// build a new parameter
    /// </summary>
    /// <typeparam name="T">type of parameter value</typeparam>
    /// <param name="value"></param>
    /// <returns>Param{T}</returns>
    protected Param<T> Param<T>(string? value = null)
        => _argBuilder.Param<T>(value);

    /// <summary>
    /// build a new string parameter
    /// </summary>
    /// <param name="value">eventual value</param>
    /// <returns>Param</returns>
    protected Param Param(string? value = null)
        => _argBuilder.Param(value);

    /// <summary>
    /// build an option set
    /// </summary>
    /// <param name="options">options</param>
    /// <returns>option set</returns>
    protected static OptSet OptSet(params IOpt[] options)
        => new(options);

    #endregion

    #region syntax helpers

    /// <summary>   
    /// build a syntax from arguments syntaxes set
    /// </summary>
    /// <param name="syntax">arguments syntaxes</param>
    /// <returns>a syntax dispatcher map item</returns>
    protected SyntaxExecutionDispatchMapItem For(params Arg[] syntax)
    {
        if (_syntaxMatcherDispatcher is null)
        {
            _syntaxMatcherDispatcher = new(
                Texts,
                Parser,
                GlobalSettings,
                Console);
        }

#if Enable_h_Arg
        if (_syntaxMatcherDispatcher.Count == 0)
            AddHelpAboutCommandSyntax(_syntaxMatcherDispatcher);
#endif
        return _syntaxMatcherDispatcher.For(syntax);
    }

    private void AddHelpAboutCommandSyntax(SyntaxMatcherDispatcher syntaxMatcherDispatcher)
        => syntaxMatcherDispatcher
            .For(
                Opt("h"))
                    .Do(HelpAboutCommandSyntax);

    private OperationResult HelpAboutCommandSyntax(CommandContext context)
    {
        var args =
            new List<string>{
                "help" ,
                ClassNameToCommandName() };

        if (context.OptSet is not null)
        {
            foreach (var opt in context.OptSet.Opts)
            {
                if (opt.IsSet)
                    args.AddRange(opt.ToArgs());
            }
        }

        return RunCommand(args.ToArray());
    }

    /// <summary>
    /// run a command from command line arguments
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>operation result</returns>
    public OperationResult RunCommand(params string[] args) =>
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

