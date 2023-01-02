
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.GlobalOpts;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Settings;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Logging;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// takes syntax definitions and try run the valid one
/// </summary>
public sealed class SyntaxMatcherDispatcher
{
    private readonly List<SyntaxExecutionDispatchMapItem> _maps = new();
    private readonly Texts _texts;
    private readonly Parser _parser;
    private readonly GlobalSettings _globalSettings;
    private readonly IAnsiVtConsole _console;

    /// <summary>
    /// command options
    /// </summary>
    public OptSet? OptSet { get; private set; }

    /// <summary>
    /// count of items in map
    /// </summary>
    public int Count => _maps.Count;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="texts">texts service</param>
    /// <param name="parser">parser</param>
    /// <param name="globalSettings">setted global options</param>
    /// <param name="console">console</param>
    public SyntaxMatcherDispatcher(
        Texts texts,
        Parser parser,
        GlobalSettings globalSettings,
        IAnsiVtConsole console)
        => (_texts, _parser, _globalSettings, _console)
            = (texts, parser, globalSettings, console);

    /// <summary>
    /// build a syntax from arguments syntaxes set
    /// </summary>
    /// <param name="syntax">arguments syntaxes</param>
    /// <returns>a syntax object</returns>
    public SyntaxExecutionDispatchMapItem For(params Arg[] syntax)
    {
        var dispatchMap = new SyntaxExecutionDispatchMapItem(
            this,
            new Syntax(syntax));
        _maps.Add(dispatchMap);
        return dispatchMap;
    }

    /// <summary>
    /// add options to command
    /// </summary>
    /// <param name="options">options set</param>
    /// <returns>this</returns>
    public SyntaxMatcherDispatcher Options(params IOpt[] options)
    {
        OptSet = new OptSet(options);
        return this;
    }

    /// <summary>
    /// add options to command
    /// </summary>
    /// <param name="optSet">options set</param>
    /// <returns>this</returns>
    public SyntaxMatcherDispatcher Options(OptSet? optSet)
    {
        OptSet = optSet;
        return this;
    }

    /// <summary>
    /// run the syntax matching the given args or produces an error result
    /// </summary>
    /// <param name="args">set of command line arguments</param>
    /// <returns>command execution result</returns>
    /// <exception cref="InvalidOperationException">the syntax matcher dispatcher delegate action is not defined</exception>
    public CommandResult With(ArgSet args)
    {
        var logLevel = _globalSettings
            .SettedGlobalOptsSet
            .TryGetByType<ParserLogging>(out var parserLogging) ?
                parserLogging.GetValue() : LogLevel.Error;

        var logTrace = logLevel == LogLevel.Trace
            || logLevel == LogLevel.Debug;

        void Trace(string? text = "")
            => _console.Logger.Log(
                    _console.Colors.Debug + text
                );

        List<CommandResult> tryCommandsResults = new();
        List<MatchingSyntax> matchingSyntaxes = new();
        List<string> parseErrors = new();

        foreach (var syntaxMatcherDispatcher in _maps)
        {
            if (syntaxMatcherDispatcher.Delegate is null)
            {
                throw new InvalidOperationException(
                    _texts._("SyntaxExecutionDispatchMapItemDelegateNotDefined",
                        syntaxMatcherDispatcher.Syntax.ToSyntax()));
            }

            var (hasErrors, errors) = _parser.MatchSyntax(
                args,
                syntaxMatcherDispatcher.Syntax,
                OptSet,
                out var settedOptions
                );

            if (logTrace)
            {
                Trace(
                    syntaxMatcherDispatcher
                    .Syntax
                    .ToSyntax() +
                    $" : match={!hasErrors}"
                    );
            }

            if (!hasErrors)
            {
                matchingSyntaxes.Add(
                    new(syntaxMatcherDispatcher, settedOptions));
            }
            else
            {
                if (parseErrors.Any())
                    parseErrors.Add(string.Empty);
                parseErrors.AddRange(errors);
            }
        }

        if (logTrace) Trace();

        if (!matchingSyntaxes.Any())
        {
            return new CommandResult(
                Globals.ExitFail,
                parseErrors,
                null);
        }

        if (matchingSyntaxes.Count > 1
            && _globalSettings
                .SettedGlobalOptsSet
                .Contains<ExcludeAmbiguousSyntax>())
        {
            parseErrors.Add(
                _texts._(
                    "AmbiguousSyntaxes",
                    args.ToText()));

            foreach (var syntaxExecutionDispatchMapItem in matchingSyntaxes)
            {
                parseErrors.Add(syntaxExecutionDispatchMapItem
                    .SyntaxExecutionDispatchMapItem
                    .Syntax
                    .ToSyntax());
            }

            return new CommandResult(
                Globals.ExitFail,
                parseErrors,
                null);
        }

        var selectedSyntaxExecutionDispatchMapItem = matchingSyntaxes
            .First();

        var operationResult = selectedSyntaxExecutionDispatchMapItem
            .SyntaxExecutionDispatchMapItem
            .Delegate!
            .Invoke(
                new OperationContext(
                    selectedSyntaxExecutionDispatchMapItem
                        .SyntaxExecutionDispatchMapItem
                        .Syntax,
                    OptSet,
                    this));

        return new CommandResult(
            operationResult.ExitCode,
            operationResult.Result
            );
    }
}
