﻿
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
    #region properties

    readonly List<SyntaxExecutionDispatchMapItem> _maps = new();

    readonly Parser _parser;
    internal Texts Texts { get; private set; }
    internal GlobalSettings GlobalSettings { get; private set; }
    internal IAnsiVtConsole Console { get; private set; }

    /// <summary>
    /// command options
    /// </summary>
    public OptSet? OptSet { get; private set; }

    /// <summary>
    /// count of items in map
    /// </summary>
    public int Count => _maps.Count;

    readonly string _commandName;

    /// <summary>
    /// logger
    /// </summary>
    public CoreLogger Logger { get; private set; }

    #endregion

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <param name="texts">texts service</param>
    /// <param name="parser">parser</param>
    /// <param name="globalSettings">setted global options</param>
    /// <param name="console">console</param>
    /// <param name="logger">logger</param>
    public SyntaxMatcherDispatcher(
        string commandName,
        Texts texts,
        Parser parser,
        GlobalSettings globalSettings,
        IAnsiVtConsole console,
        CoreLogger logger)
        => (_commandName, Texts, _parser, GlobalSettings, Console, Logger)
            = (commandName, texts, parser, globalSettings, console, logger);

    /// <summary>
    /// build a syntax from arguments syntaxes set
    /// </summary>
    /// <param name="syntaxArgs">syntaxe arguments</param>
    /// <returns>a syntax object</returns>
    public SyntaxExecutionDispatchMapItem For(params Arg[] syntaxArgs)
    {
        var syntax = new Syntax(syntaxArgs);
        var dispatchMap = new SyntaxExecutionDispatchMapItem(
            _commandName,
            this,
            syntax);

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
        OptSet =
            OptSet is null ?
                new OptSet(options)
                : new OptSet(OptSet, options);
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
        var logLevel = GlobalSettings
            .SettedGlobalOptsSet
            .TryGetByType<ParserLogging>(out var parserLogging) ?
                parserLogging.Value() : LogLevel.Error;

        if (GlobalSettings
            .LogSettings
            .SyntaxesIdentification)
            logLevel = LogLevel.Trace;

        var logTrace = logLevel == LogLevel.Trace
            || logLevel == LogLevel.Debug;

        void Trace(string action = "", string? text = "")
            => Logger.Log(
                    action,
                    text
                );

        List<CommandResult> tryCommandsResults = new();
        List<MatchingSyntax> matchingSyntaxes = new();
        List<string> parseErrors = new();

        foreach (var syntaxMatcherDispatcher in _maps)
        {
            if (syntaxMatcherDispatcher.Delegate is null)
                throw new InvalidOperationException(
                    Texts._("SyntaxExecutionDispatchMapItemDelegateNotDefined",
                        syntaxMatcherDispatcher.Syntax.ToSyntax()));

            var (hasErrors, errors) = _parser.MatchSyntax(
                args,
                syntaxMatcherDispatcher.Syntax,
                OptSet,
                out var settedOptions
                );

            if (logTrace)
                Trace(
                    "syntax",
                    syntaxMatcherDispatcher
                    .Syntax
                    .ToSyntax() +
                    $" : match={!hasErrors}"
                    );

            if (!hasErrors)
                matchingSyntaxes.Add(
                    new(syntaxMatcherDispatcher, settedOptions));
            else
            {
                if (parseErrors.Any())
                    parseErrors.Add(string.Empty);
                parseErrors.AddRange(errors);
            }
        }

        if (logTrace) Trace();

        if (!matchingSyntaxes.Any())
            return new CommandResult(
                Globals.ExitFail,
                parseErrors,
                null);

        if (matchingSyntaxes.Count > 1
            && GlobalSettings
                .SettedGlobalOptsSet
                .Contains<ExcludeAmbiguousSyntax>())
        {
            parseErrors.Add(
                Texts._(
                    "AmbiguousSyntaxes",
                    args.ToText()));

            foreach (var syntaxExecutionDispatchMapItem in matchingSyntaxes)
                parseErrors.Add(syntaxExecutionDispatchMapItem
                    .SyntaxExecutionDispatchMapItem
                    .Syntax
                    .ToSyntax());

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
                new CommandContext(
                    GlobalSettings,
                    Console,
                    Texts,
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
