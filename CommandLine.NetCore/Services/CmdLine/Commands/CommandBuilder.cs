using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.AppHost;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command builder
/// </summary>
public sealed partial class CommandBuilder
{
    #region properties

    /// <summary>
    /// dynamic command specification only if the builder is related to a dynamic command
    /// </summary>
    public DynamicCommandSpecification? DynamicCommandSpecification { get; }

    readonly Configuration _configuration;

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
        _configuration = dependencies.GlobalSettings.Configuration;
        _commandName = commandName;
        _runMethod = runMethod;
        _argBuilder = dependencies.ArgBuilder;
        _globalSettings = dependencies.GlobalSettings;
        _texts = dependencies.Texts;
        _console = dependencies.Console;
        _parser = dependencies.Parser;
    }

    /// <summary>
    /// creates a new command builder
    /// </summary>
    /// <param name="dependencies">command dependencies</param>
    /// <param name="commandName">command name</param>
    /// <param name="specification">specification</param>
    /// <param name="runMethod">a run method for the builded command</param>
    public CommandBuilder(
        Dependencies dependencies,
        string commandName,
        DynamicCommandSpecification specification,
        Func<string[], CommandLineResult>? runMethod = null)
    {
        DynamicCommandSpecification = specification;
        _configuration = dependencies.GlobalSettings.Configuration;
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
}
