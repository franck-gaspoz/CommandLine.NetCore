#define Enable_h_Arg

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command builder
/// </summary>
public abstract class CommandBuilder
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

    readonly ArgBuilder _argBuilder;
    SyntaxMatcherDispatcher? _syntaxMatcherDispatcher;

    #endregion

    /// <summary>
    /// creates a new command builder
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    public CommandBuilder(Dependencies dependencies)
    {
        _argBuilder = dependencies.ArgBuilder;
        GlobalSettings = dependencies.GlobalSettings;
        Texts = dependencies.Texts;
        Console = dependencies.Console;
        Parser = dependencies.Parser;
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
    /// build a new flag (option with no possibilty of values)
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Flag</returns>
    protected Flag Flag(string name, bool isOptional = false)
        => _argBuilder.Flag(name, isOptional);

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

    #region syntax builders

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

    void AddHelpAboutCommandSyntax(SyntaxMatcherDispatcher syntaxMatcherDispatcher)
        => syntaxMatcherDispatcher
            .For(
                Opt("h"))
                    .Do(HelpAboutCommandSyntax)
            .Options(Opt("v"), Opt("info"));

    OperationResult HelpAboutCommandSyntax(CommandContext context)
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

        foreach (var (_, globalArgSyntax) in GlobalSettings
            .SettedGlobalOptsSet
            .OptSpecs)
        {
            args.AddRange(globalArgSyntax);
        }

        return RunCommand(args.ToArray());
    }

    #endregion

    #region command run

    /// <summary>
    /// run a command from command line arguments
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <returns>operation result</returns>
    public abstract OperationResult RunCommand(params string[] args);

    #endregion
}
