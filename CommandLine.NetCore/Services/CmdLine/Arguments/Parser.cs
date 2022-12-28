
using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.Text;

using static CommandLine.NetCore.Services.CmdLine.Globals;
namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// options and argument syntax parser
/// </summary>
public sealed class Parser
{
    private readonly Texts _texts;
    private readonly IAnsiVtConsole _console;

    /// <summary>
    /// build a new arguments parser
    /// </summary>
    /// <param name="texts">texts</param>
    /// <param name="console">console</param>
    public Parser(
        Texts texts,
        IAnsiVtConsole console)
        => (_texts, _console) = (texts, console);

    public static string ClassNameToOptName(string name)
        => name[0..^9]
            .ToLower();

    public static string GetPrefixFromClassName(string name)
    {
        var argName = ClassNameToOptName(name);
        return argName.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;
    }

    public static string GetPrefixFromOptName(string name)
        => name.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;

    /// <summary>
    /// return true if the text is the syntax of an opt name (starst with - ot --)
    /// </summary>
    /// <param name="text">text to be checked</param>
    /// <returns>true if syntax max an opt name</returns>
    public static bool IsOpt(string text)
        => text.StartsWith(ShortArgNamePrefix)
            || text.StartsWith(LongArgNamePrefix);

    /// <summary>
    /// extract values from an arguments lists. try to get the expectd values count
    /// </summary>
    /// <param name="opt">parsed option</param>
    /// <param name="args">arg list. the list is consumed (elements are removed)</param>
    /// <param name="index">begin index</param>
    /// <param name="position">actual begin index in arguments list</param>
    /// <exception cref="ArgumentException">missing argument value</exception>
    public void ParseOptValues(
        IOpt opt,
        List<string> args,
        int index,
        int position)
    {
        var expectedCount = opt.ExpectedValuesCount;
        args.RemoveAt(index);
        while (expectedCount > 0)
        {
            position++;
            if (!args.Any() || IsOpt(args[index]))
            {
                throw new ArgumentException(
                    _texts._("MissingOptionValue", position, opt.Name));
            }

            opt.AddValue(args[index]);
            args.RemoveAt(index);
            expectedCount--;
        }
    }

    /// <summary>
    /// parse value of a parameter
    /// </summary>
    /// <param name="param">parsed parameter</param>
    /// <param name="args">arg list. the list is consumed (elements are removed)</param>
    /// <param name="index">begin index</param>
    /// <param name="position">actual begin index in arguments list</param>
    /// <exception cref="ArgumentException"></exception>
    public void ParseParamValue(
        IParam param,
        List<string> args,
        int index,
        int position
        )
    {
        var arg = args[index];
        if (param.StringValue is not null)
        {
            // expect value
            if (arg != param.StringValue)
            {
                throw new ArgumentException(
                    _texts._("ExpectedParameterValue",
                        param.StringValue, position, arg));
            }
        }
        else
        {
            try
            {
                // assign value
                param.SetValue(arg);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(
                    ex.Message
                    + _texts._("ParameterValueConvertError", position));
            }
        }
        args.RemoveAt(index);
    }

    private bool ParseArg(
        ref int grammar_index,
        ref int position,
        List<string> args,
        List<IOpt> optionals,
        string arg,
        IArg gram,
        List<string> errors)
    {
        var parseBreaked = false;
        var currentPosition = position;

        if (gram is IOpt opt)
        {
            // option
            if (arg == opt.PrefixedName)
            {
                if (!TryCatch(
                    () => ParseOptValues(opt, args, 0, currentPosition),
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
                    () => ParseParamValue(param, args, 0, currentPosition),
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

    public bool MatchSyntax(
        List<string> arguments,
        params Arg[] grammar
        )
    {
        var grammar_index = 0;
        var position = 0;
        var args = arguments.ToList();
        var grammarText = string.Join(' ',
            grammar.Select(
                x => x.ToGrammar()));
        var optionals = new List<IOpt>();
        var parseBreaked = false;
        var errors = new List<string>();
        var isParsingOptions = false;
        var grammars = new List<IArg>();

        while (args.Count > 0
            && (grammar_index < grammar.Length || isParsingOptions)
            && !parseBreaked)
        {
            Arg currentSyntax() => grammar[grammar_index];
            string currentArg() => args[0];

            var arg = currentArg();

            grammars.Clear();
            if (!isParsingOptions)
                grammars.Add(currentSyntax());
            else
                grammars.AddRange(optionals);

            var hasError = false;
            foreach (var gram in grammars)
            {
                hasError |= ParseArg(
                    ref grammar_index,
                    ref position,
                    args,
                    optionals,
                    arg,
                    gram,
                    errors);
            }

            isParsingOptions = grammar_index == grammar.Length
                && optionals.Any();

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

}
