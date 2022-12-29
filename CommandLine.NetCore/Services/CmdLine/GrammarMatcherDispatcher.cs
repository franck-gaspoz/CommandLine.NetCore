
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.GlobalOpts;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Logging;

namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// takes grammar definitions and try run the valid one
/// </summary>
public sealed class GrammarMatcherDispatcher
{
    private readonly List<GrammarExecutionDispatchMapItem> _maps = new();
    private readonly Texts _texts;
    private readonly Parser _parser;
    private readonly SettedGlobalOptsSet _settedGlobalOptsSet;
    private readonly IAnsiVtConsole _console;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="texts">texts service</param>
    /// <param name="parser">parser</param>
    /// <param name="settedGlobalOptsSet">setted global options</param>
    /// <param name="console">console</param>
    public GrammarMatcherDispatcher(
        Texts texts,
        Parser parser,
        SettedGlobalOptsSet settedGlobalOptsSet,
        IAnsiVtConsole console)
        => (_texts, _parser, _settedGlobalOptsSet, _console)
            = (texts, parser, settedGlobalOptsSet, console);

    /// <summary>
    /// build a grammar from arguments grammars set
    /// </summary>
    /// <param name="grammar">arguments grammars</param>
    /// <returns>a grammar object</returns>
    public GrammarExecutionDispatchMapItem For(params Arg[] grammar)
    {
        var dispatchMap = new GrammarExecutionDispatchMapItem(
            this,
            new Grammar(grammar));
        _maps.Add(dispatchMap);
        return dispatchMap;
    }

    /// <summary>
    /// run the grammar matching the given args or produces an error result
    /// </summary>
    /// <param name="args">set of command line arguments</param>
    /// <returns>command execution result</returns>
    /// <exception cref="InvalidOperationException">the grammar matcher dispatcher delegate action is not defined</exception>
    public CommandResult With(ArgSet args)
    {
        var logLevel = _settedGlobalOptsSet.TryGetByType<ParserLogging>(out var parserLogging) ?
            parserLogging.GetValue() : LogLevel.Error;
        var logTrace = logLevel == LogLevel.Trace
            || logLevel == LogLevel.Debug;
        void Trace(string? text = "")
            => _console.Logger.Log(
                    _console.Colors.Debug + text
                );

        List<CommandResult> tryCommandsResults = new();
        List<GrammarExecutionDispatchMapItem> matchingGrammars = new();
        List<string> parseErrors = new();

        foreach (var grammarMatcherDispatcher in _maps)
        {
            if (grammarMatcherDispatcher.Delegate is null)
            {
                throw new InvalidOperationException(
                    _texts._("GrammarExecutionDispatchMapItemDelegateNotDefined",
                        grammarMatcherDispatcher.Grammar.ToGrammar()));
            }

            var (hasErrors, errors) = _parser.MatchSyntax(
                args,
                grammarMatcherDispatcher.Grammar);

            if (logTrace)
            {
                Trace(
                    grammarMatcherDispatcher.Grammar.ToGrammar() +
                    $" : match={!hasErrors}"
                    );
            }

            if (!hasErrors)
            {
                matchingGrammars.Add(grammarMatcherDispatcher);
            }
            else
            {
                if (parseErrors.Any())
                    parseErrors.Add(string.Empty);
                parseErrors.AddRange(errors);
            }
        }

        if (logTrace) Trace();

        if (!matchingGrammars.Any())
        {
            return new CommandResult(
                Globals.ExitFail,
                parseErrors,
                null);
        }

        if (matchingGrammars.Count > 1
            && _settedGlobalOptsSet.Contains<ExcludeAmbiguousGrammar>())
        {
            parseErrors.Add(
                _texts._(
                    "AmbiguousGrammars",
                    args.ToText()));

            foreach (var grammarExecutionDispatchMapItem in matchingGrammars)
            {
                parseErrors.Add(grammarExecutionDispatchMapItem
                    .Grammar
                    .ToGrammar());
            }

            return new CommandResult(
                Globals.ExitFail,
                parseErrors,
                null);
        }

        var selectedGrammarExecutionDispatchMapItem = matchingGrammars
            .First();
        var operationResult = selectedGrammarExecutionDispatchMapItem
            .Delegate!
            .Invoke(selectedGrammarExecutionDispatchMapItem.Grammar);
        return new CommandResult(
            operationResult.ExitCode,
            operationResult.Result
            );
    }
}
