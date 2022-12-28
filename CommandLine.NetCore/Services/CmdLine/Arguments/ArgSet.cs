using System.Diagnostics;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// set of arguments of a command invokation
/// </summary>
[DebuggerDisplay("{DebuggerDisplay}")]
public sealed class ArgSet
{
    private string DebuggerDisplay => string.Join(' ', _args);

    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<string> Args => _args;

    private readonly List<string> _args;

    private readonly Parser _parser;
    private readonly IAnsiVtConsole _console;
    private readonly Texts _texts;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    /// <param name="parser">syntax parser</param>
    /// <param name="console">console</param>
    /// <param name="texts">texts</param>
    public ArgSet(
        IEnumerable<string> args,
        Parser parser,
        IAnsiVtConsole console,
        Texts texts
        )
        => (_args, _parser, _console, _texts)
            = (new List<string>(args), parser, console, texts);

    /// <summary>
    /// args count
    /// </summary>
    public int Count => Args.Count;

    /// <summary>
    /// array get accessor
    /// </summary>
    /// <param name="index">argument index (from 0)</param>
    /// <returns>argument value at index</returns>
    public string this[int index] => _args[index];

    #region errors texts

    private static string TooManyArguments(List<string> args, int atIndex)
        => $"to many arguments: '{string.Join(' ', args)}' from position {atIndex}";

    private static string MissingArguments(Arg[] args, int atIndex)
        => $"missing arguments, expected '{string.Join(' ', args.Select(x => x.ToGrammar()))}' at position {atIndex}";

    private static string SyntaxMismatchError(IArg expectedGrammar)
        => $"in grammar '{expectedGrammar.ToGrammar()}'";

    private static string ParamValueError(IArg expectedGrammar, int atIndex, string foundSyntax)
        => $"in grammar '{expectedGrammar.ToGrammar()}' at position '{atIndex}' but found '{foundSyntax}'";

    private static string UnknownGrammar(IArg arg, int atIndex) => $"unknown grammar: '{arg.ToGrammar()}' at position {atIndex}";

    private static string BuildError(Exception ex, string message) => ex.Message + Environment.NewLine + message;

    private static string GetError(string error, string grammarText) => error + Environment.NewLine + "for grammar: " + grammarText;

    #endregion

    /// <summary>
    /// check if the arg set match the syntax described by the parameters from left to right
    /// </summary>
    /// <returns>true if syntax match, false otherwise</returns>
    public bool MatchSyntax(
        params Arg[] grammar
        )
    {
        var grammar_index = 0;
        var position = 0;
        var args = _args.ToList();
        var grammarText = string.Join(' ',
            grammar.Select(
                x => x.ToGrammar()));
        var optionals = new List<IOpt>();
        var parseBreaked = false;
        var errors = new List<string>();

        while (args.Count > 0 && grammar_index < grammar.Length && !parseBreaked)
        {
            Arg currentSyntax() => grammar[grammar_index];
            string currentArg() => args[0];

            var arg = currentArg();
            var gram = currentSyntax();

            var hasError = ParseArg(
                ref grammar_index,
                ref position,
                args,
                optionals,
                arg,
                gram,
                errors);

            parseBreaked = hasError;
        }

        if (!errors.Any())
        {
            if (grammar_index < grammar.Length)
            {
                errors.Add(
                    MissingArguments(grammar[grammar_index..], position));
            }

            if (args.Count > 0)
            {
                errors.Add(
                    TooManyArguments(args, position));
            }
        }

        var error = string.Join(Environment.NewLine, errors);

        if (!string.IsNullOrWhiteSpace(error))
            _console.Logger.LogError(GetError(error, grammarText) + "(br)");

        return false;
    }

    private bool ParseArg(
        ref int grammar_index,
        ref int position,
        List<string> args,
        List<IOpt> optionals,
        string arg,
        Arg gram,
        List<string> errors)
    {
        var parseBreaked = false;
        var currentPosition = position;

        if (gram is IOpt opt)
        {
            // option
            if (arg == opt.PrefixedName)
            {
                args.RemoveAt(0);

                if (!TryCatch(
                    () => _parser.ParseOptValues(opt, args, 0, currentPosition),
                    (ex) => errors.Add(BuildError(
                        ex, SyntaxMismatchError(opt)))
                    ))
                {
                    parseBreaked = true;
                }

                position += 1 + opt.ExpectedValuesCount;
                grammar_index++;
            }
            else
            {
                if (!opt.IsOptional)
                {
                    errors.Add(
                        _texts._("ExpectedOption", opt.PrefixedName, position, arg));
                    parseBreaked = true;
                }
                else
                {
                    optionals.Add(opt);
                    grammar_index++;
                }
            }
        }
        else
        {
            // parameter
            if (gram is IParam param)
            {
                if (!TryCatch(
                    () => _parser.ParseParamValue(param, args, 0, currentPosition),
                    (ex) => errors.Add(
                        param.StringValue is not null ?
                            BuildError(
                                ex, SyntaxMismatchError(param))
                            : BuildError(
                                ex, ParamValueError(param, currentPosition, arg)
                    ))))
                {
                    parseBreaked = true;
                }

                position++;
                grammar_index++;
            }
            else
            {
                // type mismatch
                errors.Add(UnknownGrammar(gram, grammar_index));
                parseBreaked = true;
            }
        }

        return parseBreaked;
    }

    private static bool TryCatch(Action tryDelegate, Action<Exception> elseDelegate)
    {
        try
        {
            tryDelegate();
            return true;
        }
        catch (Exception ex)
        {
            elseDelegate(ex);
            return false;
        }
    }
}
