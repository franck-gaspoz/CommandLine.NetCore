using System.Reflection;

using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// a syntax dispatch map
/// </summary>
public sealed partial class SyntaxExecutionDispatchMapItem
{
    #region properties

    /// <summary>
    /// command name
    /// </summary>
    readonly string _commandName;

    /// <summary>
    /// name of the syntax
    /// <para>is the Do method name</para>
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// syntax spec
    /// </summary>
    public Syntax Syntax { get; private set; }

    /// <summary>
    /// execute action delegate
    /// </summary>
    public Func<CommandContext, CommandResult>? Delegate { get; private set; }

    /// <summary>
    /// the syntax matcher dispatcher owner of this
    /// </summary>
    public SyntaxMatcherDispatcher SyntaxMatcherDispatcher { get; private set; }

    #endregion

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <param name="syntaxMatcherDispatcher">the syntax matcher dispatcher owner of this</param>
    /// <param name="syntax">syntax</param>
    public SyntaxExecutionDispatchMapItem(
        string commandName,
        SyntaxMatcherDispatcher syntaxMatcherDispatcher,
        Syntax syntax)
        => (_commandName, SyntaxMatcherDispatcher, Syntax, Name)
            = (commandName, syntaxMatcherDispatcher, syntax, string.Empty);

    const string ArrayTypePostFix = "[]";

    /// <summary>
    /// indicates if the parameter type is an array value type and the argument is a list value type
    /// </summary>
    /// <param name="parameterInfo">parameter info</param>
    /// <param name="argument">argument</param>
    /// <returns>a tuple with the question bool result</returns>
    static bool IsArgumentIsArrayAndParameterIsList(ParameterInfo parameterInfo, object argument)
        => parameterInfo.ParameterType
            .IsArray
            && argument is IOpt opt
            && opt.ExpectedValuesCount > 1
            && opt.ValueType.FullName + ArrayTypePostFix == parameterInfo.ParameterType.FullName;

    void ValidateAction(Delegate action)
    {
        var error = () => GetSyntaxError(action.ToString() ?? string.Empty);
        var methodInfo = action.GetMethodInfo()
            ?? throw new MissingOrNotFoundCommandOperationException(
                error(),
                new CommandResult(
                    ExitFail,
                    Syntax)
                );
        if (methodInfo.ReturnType != typeof(void))
            throw new InvalidCommandOperationException(
                error(),
                new CommandResult(
                    ExitFail,
                    Syntax)
                );
    }

    string GetSyntaxError(string expression)
        => Syntax.ToSyntax()
            + Environment.NewLine
            + expression.ToString();
}
