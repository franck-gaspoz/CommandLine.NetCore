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
        string? error = null;
        var grammarText = string.Join(' ',
            grammar.Select(
                x => x.ToGrammar()));

        string TooManyArguments(List<string> args, int atIndex)
            => $"to many arguments: '{string.Join(' ', args)}' from position {atIndex}";

        string MissingArguments(Arg[] args, int atIndex)
            => $"missing arguments, expeceted '{string.Join(' ', args.Select(x => x.ToGrammar()))}' from position {atIndex}";

        string SyntaxMismatchError(IArg ewpectedGrammar, int atIndex, string foundSyntax)
            => $"expected '{ewpectedGrammar.ToGrammar()}' at position '{atIndex}' but found '{foundSyntax}'";

        string UnknownGrammar(IArg arg, int atIndex) => $"unknown grammar: '{arg.ToGrammar()}' at position {atIndex}";

        string BuildError(Exception ex, string message) => ex.Message + Environment.NewLine + message;

        string GetError(string error) => error + Environment.NewLine + "for grammar: " + grammarText;

        while (args.Count > 0 && grammar_index < grammar.Length)
        {
            Arg currentSyntax() => grammar[grammar_index];
            string currentArg() => args[0];

            var arg = currentArg();
            var gram = currentSyntax();
            if (gram is IOpt opt)
            {
                // option
                if (!TryElse(
                    () => _parser.ParseOptValues(opt, args, 0, position),
                    (ex) => error = BuildError(
                        ex, SyntaxMismatchError(opt, position, arg))
                    ))
                {
                    break;
                }

                position += 1 + opt.ExpectedValuesCount;
                grammar_index++;
            }
            else
            {
                // parameter
                if (gram is IParam param)
                {
                    if (!TryElse(
                        () => _parser.ParseParamValue(param, args, 0, position),
                        (ex) => error = BuildError(
                            ex, SyntaxMismatchError(param, position, arg))
                        ))
                    {
                        break;
                    }

                    position++;
                    grammar_index++;
                }
                else
                {
                    // type mismatch
                    error = UnknownGrammar(gram, grammar_index);
                }
            }
        }

        if (error is null)
        {
            if (grammar_index < grammar.Length)
                error = MissingArguments(grammar[grammar_index..], position);

            if (args.Count > 0)
                error = TooManyArguments(args, position);
        }

        if (error is not null) _console.Logger.LogError(GetError(error));

        return false;
    }

    private static bool TryElse(Action tryDelegate, Action<Exception> elseDelegate)
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
