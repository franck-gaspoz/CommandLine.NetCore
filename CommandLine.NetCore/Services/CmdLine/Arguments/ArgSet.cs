namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// set of arguments of a command invokation
/// </summary>
public sealed class ArgSet
{
    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<string> Args => _args;

    private readonly List<string> _args;

    private readonly Parser _parser;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    /// <param name="parser">syntax parser</param>
    public ArgSet(IEnumerable<string> args, Parser parser)
        => (_args, _parser) = (new List<string>(args), parser);

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
        params Arg[] syntax
        )
    {
        var syntax_index = 0;
        var position = 0;

        var args = _args.ToList();
        var error = string.Empty;

        string SyntaxMismatchError(IArg ewpectedGrammar, int atIndex, string foundSyntax)
            => $"Expected '{ewpectedGrammar.ToGrammar()}' at position '{atIndex}' but found '{foundSyntax}'";
        string UnknownGrammar(IArg arg, int atIndex) => $"unknown grammar: '{arg.ToGrammar()}' at position {atIndex}";
        string BuildError(Exception ex, string message) => ex.Message + Environment.NewLine + message;

        while (args.Count > 0 && syntax_index < syntax.Length)
        {
            Arg currentSyntax() => syntax[syntax_index];
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
                syntax_index++;
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
                    syntax_index++;
                }
                else
                {
                    // type mismatch
                    error = UnknownGrammar(gram, syntax_index);
                }
            }
        }

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
