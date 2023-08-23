#define Enable_h_Arg

using AnsiVtConsole.NetCore;

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
public sealed class CommandBuilder
{
    #region properties

    /// <summary>
    /// the run method of the builded command
    /// </summary>
    Func<string[], CommandLineResult>? _runMethod;

    /// <summary>
    /// console service
    /// </summary>
    readonly IAnsiVtConsole _console;

    /// <summary>
    /// texts service
    /// </summary>
    readonly Texts _texts;

    /// <summary>
    /// parser
    /// </summary>
    readonly Parser _parser;

    /// <summary>
    /// setted global options
    /// </summary>
    readonly GlobalSettings _globalSettings;

    readonly ArgBuilder _argBuilder;

    SyntaxMatcherDispatcher? _syntaxMatcherDispatcher;

    readonly string _commandName;

    #endregion

    /// <summary>
    /// creates a new command builder
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    /// <param name="commandName">command name</param>
    /// <param name="runMethod">a run method for the builded command</param>
    public CommandBuilder(
        Dependencies dependencies,
        string commandName,
        Func<string[], CommandLineResult>? runMethod = null)
    {
        _commandName = commandName;
        _runMethod = runMethod;
        _argBuilder = dependencies.ArgBuilder;
        _globalSettings = dependencies.GlobalSettings;
        _texts = dependencies.Texts;
        _console = dependencies.Console;
        _parser = dependencies.Parser;
    }

    /// <summary>
    /// set the run method
    /// </summary>
    /// <param name="runMethod">run method</param>
    public void SetRunMethod(Func<string[], CommandLineResult> runMethod)
        => _runMethod = runMethod;

    #region args build helpers

    /// <summary>
    /// build a new option
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <param name="valueCount">value count</param>
    /// <returns>Opt</returns>
    public Opt Opt(string name, bool isOptional = false, int valueCount = 0)
        => _argBuilder.Opt(name, isOptional, valueCount);

    /// <summary>
    /// build a new flag (option with no possibilty of values)
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Flag</returns>
    public Flag Flag(string name, bool isOptional = false)
        => _argBuilder.Flag(name, isOptional);

    /// <summary>
    /// build a new generic option
    /// </summary>
    /// <typeparam name="T">type of options values</typeparam>
    /// <param name="name">name</param>
    /// <param name="isOptional">is optional</param>
    /// <returns>Opt{T}</returns>
    public Opt<T> Opt<T>(string name, bool isOptional = false)
        => _argBuilder.Opt<T>(name, isOptional);

    /// <summary>
    /// build a new parameter
    /// </summary>
    /// <typeparam name="T">type of parameter value</typeparam>
    /// <param name="value"></param>
    /// <returns>Param{T}</returns>
    public Param<T> Param<T>(string? value = null)
        => _argBuilder.Param<T>(value);

    /// <summary>
    /// build a new string parameter
    /// </summary>
    /// <param name="value">eventual value</param>
    /// <returns>Param</returns>
    public Param Param(string? value = null)
        => _argBuilder.Param(value);

    /// <summary>
    /// build an option set
    /// </summary>
    /// <param name="options">options</param>
    /// <returns>option set</returns>
    public static OptSet OptSet(params IOpt[] options)
        => new(options);

    #endregion

    #region syntax builders

    /// <summary>   
    /// build a syntax from arguments syntaxes set
    /// </summary>
    /// <param name="syntax">arguments syntaxes</param>
    /// <returns>a syntax dispatcher map item</returns>
    public SyntaxExecutionDispatchMapItem For(params Arg[] syntax)
    {
        if (_runMethod is null) ArgumentNullException.ThrowIfNull(_runMethod);

        if (_syntaxMatcherDispatcher is null)
        {
            _syntaxMatcherDispatcher = new(
                _texts,
                _parser,
                _globalSettings,
                _console);
        }

#if Enable_h_Arg
        if (_syntaxMatcherDispatcher.Count == 0)
            AddHelpAboutCommandSyntax(_syntaxMatcherDispatcher);
#endif
        var syntaxExecutionDispatchItem = _syntaxMatcherDispatcher.For(syntax);
        syntaxExecutionDispatchItem.Syntax
            .SetName(_commandName);
        return syntaxExecutionDispatchItem;
    }

    void AddHelpAboutCommandSyntax(SyntaxMatcherDispatcher syntaxMatcherDispatcher)
        => syntaxMatcherDispatcher
            .For(
                Opt("h"))
                    .Do(HelpAboutCommandSyntax)
            .Options(Opt("v"), Opt("info"));

    CommandLineResult HelpAboutCommandSyntax(CommandContext context)
    {
        var args =
            new List<string>{
                "help" ,
                _commandName };

        if (context.OptSet is not null)
        {
            foreach (var opt in context.OptSet.Opts)
            {
                if (opt.IsSet)
                    args.AddRange(opt.ToArgs());
            }
        }

        foreach (var (_, globalArgSyntax) in _globalSettings
            .SettedGlobalOptsSet
            .OptSpecs)
        {
            args.AddRange(globalArgSyntax);
        }

        return _runMethod!(args.ToArray());
    }

    #endregion
}
