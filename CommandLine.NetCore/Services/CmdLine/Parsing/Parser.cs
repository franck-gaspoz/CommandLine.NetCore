
using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.Text;

using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

namespace CommandLine.NetCore.Services.CmdLine.Parsing;

/// <summary>
/// options and argument syntax parser
/// </summary>
public sealed class Parser
{
    private readonly Texts _texts;

    /// <summary>
    /// build a new arguments parser
    /// </summary>
    /// <param name="texts">texts</param>
    public Parser(Texts texts)
        => _texts = texts;

    /// <summary>
    /// transform a class name to an option or command name
    /// <para>norm is kebab-case</para>
    /// </summary>
    /// <param name="name">name to be normalized</param>
    /// <returns>normalized name</returns>
    public static string ClassNameToOptName(string name)
        => name.ToKebabCase()!;

    /// <summary>
    /// returns the prefix that will be used for an option name
    /// </summary>
    /// <param name="name">class name of the option</param>
    /// <returns>prefix</returns>
    public static string GetPrefixFromClassName(string name)
    {
        var argName = ClassNameToOptName(name);
        return argName.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;
    }

    /// <summary>
    /// returns the prefix that will be used for an option name
    /// </summary>
    /// <param name="name">option name without prefix</param>
    /// <returns>prefix</returns>
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

            try
            {
                opt.AddValue(args[index]);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(
                    _texts._(
                        "InvalidOptionValue",
                        position,
                        opt.Name,
                        ex.Message,
                        opt.ToSyntax()));
            }
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
    /// <exception cref="ArgumentException">convert value error</exception>
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
                    _texts._(
                        "InvalidParameterValue",
                        position,
                        param.StringValue,
                        ex.Message,
                        param.ToSyntax()));
            }
        }
        args.RemoveAt(index);
    }

    private bool ParseArg(
        ref int syntax_index,
        ref int position,
        List<string> args,
        List<IOpt> optionals,
        bool isParsingRemainingOptions,
        string arg,
        IArg gram,
        List<string> errors,
        out bool founded)
    {
        founded = false;
        var parseBreaked = false;
        var currentPosition = position;

        if (gram is IOpt opt)
        {
            // option
            if (arg == opt.PrefixedName)
            {
                if (!TryCatch(
                    () => ParseOptValues(opt, args, 0, currentPosition),
                    (ex) => errors.Add(
                        BuildError(
                            ex, SyntaxMismatch(opt)))
                    ))
                {
                    parseBreaked = true;
                }
                else
                {
                    opt.SetIsSetted(true);
                    position += 1 + opt.ExpectedValuesCount;
                    if (!isParsingRemainingOptions)
                        syntax_index++;
                    optionals.Remove(opt);
                    founded = true;
                }
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
                    if (!optionals.Contains(opt))
                        optionals.Add(opt);
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
                                ex, SyntaxMismatch(param))
                            : BuildError(
                                ex, ParamValueError(param, currentPosition, arg)
                    ))))
                {
                    parseBreaked = true;
                }
                else
                {
                    founded = true;
                    position++;
                }
            }
            else
            {
                // type mismatch
                errors.Add(UnknownSyntax(gram, syntax_index));
                parseBreaked = true;
            }
        }

        return parseBreaked;
    }

    /// <summary>
    /// check if a syntax match a set of arguments
    /// <para>produces errors in any</para>
    /// </summary>
    /// <param name="arguments">arguments to be checked</param>
    /// <param name="syntax">syntax reference</param>
    /// <param name="options">command options</param>
    /// <param name="settedOptions">command options that have been setttd in command line args</param>
    /// <returns>true if args match the syntax, false otherwise</returns>
    internal (bool, List<string> errors) MatchSyntax(
        ArgSet arguments,
        Syntax syntax,
        OptSet? options,
        out OptSet settedOptions
        )
    {
        var syntax_index = 0;
        var position = 0;
        var args = arguments.Args.ToList();
        var remainingOptionals = new List<IOpt>();
        var parseBreaked = false;
        var errors = new List<string>();
        var isParsingRemainingOptions = false;
        var toBeCheckedSyntaxWords = new List<IArg>();
        var cmdOptions = options?.Opts.ToList() ?? new List<IOpt>();
        settedOptions = new OptSet();

        syntax.AddOptions(options);

        var syntaxText = syntax.ToSyntax();

        while (args.Count > 0
            && (syntax_index < syntax.Count || isParsingRemainingOptions)
            && !parseBreaked)
        {
            IArg currentSyntax() => syntax[syntax_index];
            string currentArg() => args[0];

            toBeCheckedSyntaxWords.Clear();
            if (!isParsingRemainingOptions)
            {
                toBeCheckedSyntaxWords.Add(currentSyntax());
            }

            foreach (var optional in remainingOptionals.Where(x => !x.IsSet))
            {
                if (!toBeCheckedSyntaxWords.
                        Any(x => x == optional))
                {
                    toBeCheckedSyntaxWords.Add(optional);
                }
            }

            var hasError = false;
            var founded = false;
            var remainingOptionalsHasChanged = false;

            foreach (var gram in toBeCheckedSyntaxWords)
            {
                if (args.Count > 0)
                {
                    var remainingOptionalCount = remainingOptionals.Count;

                    hasError |= ParseArg(
                        ref syntax_index,
                        ref position,
                        args,
                        remainingOptionals,
                        isParsingRemainingOptions,
                        currentArg(),
                        gram,
                        errors,
                        out founded);

                    remainingOptionalsHasChanged |= remainingOptionals.Count != remainingOptionalCount;

                    if (founded &&
                        options is not null
                        && !hasError
                        && gram is IOpt opt
                        && options.Opts.Contains(opt))
                    {
                        settedOptions.Add(opt);
                    }

                    if (founded)
                        break;
                }
            }

            if (!founded)
            {
                if (!remainingOptionalsHasChanged && isParsingRemainingOptions)
                {
                    foreach (var opt in remainingOptionals)
                    {
                        errors.Add(
                            _texts._("ExpectedOption", opt.PrefixedName, position, currentArg()));
                    }
                    hasError = true;
                }
                else
                {
                    syntax_index++;
                }
            }

            isParsingRemainingOptions = syntax_index >= syntax.Count
                && remainingOptionals.Any();

            parseBreaked = hasError;
        }

        bool RemainingSyntaxIsOnlyOptional()
        {
            var remainingSyntaxIsOnlyOptional = true;
            for (var i = syntax_index; i < syntax.Count; i++)
            {
                remainingSyntaxIsOnlyOptional &=
                    syntax[syntax_index] is IOpt opt
                    && opt.IsOptional;
            }

            return remainingSyntaxIsOnlyOptional;
        }

        if (!errors.Any())
        {
            if (syntax_index < syntax.Count
                && !RemainingSyntaxIsOnlyOptional())
            {
                errors.Add(
                    MissingArguments(
                        syntax
                            .Args
                            .ToArray()[syntax_index..],
                        position));
            }

            if (args.Count > 0)
            {
                errors.Add(
                    UnexpectedArguments(args, position));
            }
        }

        var error = string.Join(Environment.NewLine, errors);

        if (!string.IsNullOrWhiteSpace(error))
        {
            errors.Add(
                _texts._("ForSyntax") + " "
                + syntaxText);
        }

        return (!string.IsNullOrWhiteSpace(error), errors);
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

    private string UnexpectedArguments(List<string> args, int atIndex)
        => _texts._(
                args.Count == 1 ?
                    "UnexpectedArgument"
                    : "UnexpectedArguments",
            string.Join(' ', args),
            atIndex);

    private string MissingArguments(IArg[] args, int atIndex)
        => _texts._("MissingArgumentsAtPosition",
            string.Join(' ', args.Select(x => x.ToSyntax())),
            atIndex);

    private string SyntaxMismatch(IArg expectedSyntax)
        => _texts._("SyntaxMismatch", expectedSyntax.ToSyntax());

    private string ParamValueError(IArg expectedSyntax, int atIndex, string foundSyntax)
        => _texts._("ParamValueError",
            expectedSyntax.ToSyntax(),
            atIndex,
            foundSyntax);

    private string UnknownSyntax(IArg arg, int atIndex)
        => _texts._("UnknownSyntax",
            arg.ToSyntax(),
            atIndex);

    private static string BuildError(Exception ex, string message) => ex.Message + Environment.NewLine + message;

    #endregion

}
