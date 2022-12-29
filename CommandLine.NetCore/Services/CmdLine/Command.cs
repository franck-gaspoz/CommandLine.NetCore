
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine;

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
    protected readonly SettedGlobalOptsSet SettedGlobalOptsSet;

    /// <summary>
    /// name of the command
    /// </summary>
    public string Name => ClassNameToCommandName();

    private readonly ArgBuilder _argBuilder;

    private GrammarMatcherDispatcher? _grammarMatcherDispatcher;

    /// <summary>
    /// construit une instance de commande
    /// </summary>
    /// <param name="config">app config</param>
    /// <param name="console">console service</param>
    /// <param name="texts">texts service</param>
    /// <param name="argBuilder">args builder</param>
    /// <param name="settedGlobalOptsSet">setted global options set</param>
    /// <param name="parser">parser</param>
    public Command(
        IConfiguration config,
        IAnsiVtConsole console,
        Texts texts,
        ArgBuilder argBuilder,
        SettedGlobalOptsSet settedGlobalOptsSet,
        Parser parser
        )
    {
        _argBuilder = argBuilder;
        SettedGlobalOptsSet = settedGlobalOptsSet;
        Config = config;
        Texts = texts;
        Console = console;
        Parser = parser;
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

    #endregion

    #region grammar helpers

    /// <summary>   
    /// build a grammar from arguments grammars set
    /// </summary>
    /// <param name="grammar">arguments grammars</param>
    /// <returns>a grammar dispatcher map item</returns>
    protected GrammarExecutionDispatchMapItem For(params Arg[] grammar)
    {
        if (_grammarMatcherDispatcher is null)
        {
            _grammarMatcherDispatcher = new(
                Texts,
                Parser,
                SettedGlobalOptsSet,
                Console);
        }

        if (_grammarMatcherDispatcher.Count == 0)
            AddHelpAboutCommandGrammar(_grammarMatcherDispatcher);

        return _grammarMatcherDispatcher.For(grammar);
    }

    private void AddHelpAboutCommandGrammar(GrammarMatcherDispatcher grammarMatcherDispatcher)
        => grammarMatcherDispatcher
            .For(
                Opt("h"))
                    .Do(HelpAboutCommandGrammar);

    private OperationResult HelpAboutCommandGrammar(Grammar grammar)
        => new(
            new CommandLineInterfaceBuilder()
                .UseAssemblySet(SettedGlobalOptsSet.AssemblySet)
                .Build(new string[] {
                    "help" ,
                    ClassNameToCommandName() })
                .Run());

    #endregion
}

