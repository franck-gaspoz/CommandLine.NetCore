
using System.Linq.Expressions;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// a syntax dispatch map
/// </summary>
public sealed class SyntaxExecutionDispatchMapItem
{
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
    public Func<CommandContext, OperationResult>? Delegate { get; private set; }

    /// <summary>
    /// the syntax matcher dispatcher owner of this
    /// </summary>
    public SyntaxMatcherDispatcher SyntaxMatcherDispatcher { get; private set; }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="syntaxMatcherDispatcher">the syntax matcher dispatcher owner of this</param>
    /// <param name="syntax">syntax</param>
    public SyntaxExecutionDispatchMapItem(
        SyntaxMatcherDispatcher syntaxMatcherDispatcher,
        Syntax syntax)
        => (SyntaxMatcherDispatcher, Syntax, Name)
            = (syntaxMatcherDispatcher, syntax, string.Empty);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map
    /// </summary>
    /// <param name="delegate">with parameter operation context and OperationResult result delegate</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do(Func<CommandContext, OperationResult> @delegate)
    {
        Delegate = @delegate;
        Name = Delegate.Method.Name;
        Syntax.SetName(Name);
        return SyntaxMatcherDispatcher;
    }

    /// <summary>
    /// set up delegate for this syntax execution dispatch map
    /// <para>takes a method with a default command result (code ok, result null)</para>
    /// </summary>
    /// <param name="delegate">with parameter operation context and void delegate</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do(Action<CommandContext> @delegate)
    {
        Name = @delegate.Method.Name;
        Delegate = (CommandContext context) =>
        {
            @delegate.Invoke(context);
            return new();
        };
        Syntax.SetName(Name);
        return SyntaxMatcherDispatcher;
    }

    /// <summary>
    /// set up delegate for this syntax execution dispatch map
    /// <para>takes a method with no parameter </para>
    /// <para>takes a method with a default command result (code ok, result null)</para>
    /// </summary>
    /// <param name="delegate">with no parameter and void result delegate</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do(Action @delegate)
    {
        Name = @delegate.Method.Name;
        Delegate = (CommandContext context) =>
        {
            @delegate.Invoke();
            return new();
        };
        Syntax.SetName(Name);
        return SyntaxMatcherDispatcher;
    }

    /// <summary>
    /// set up delegate for this syntax execution dispatch map
    /// <para>takes a method with no parameter </para>
    /// </summary>
    /// <param name="delegate">with no parameter and void result delegate</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do(Func<OperationResult> @delegate)
    {
        Name = @delegate.Method.Name;
        Delegate = (CommandContext context) =>
        {
            return @delegate.Invoke();
        };
        Syntax.SetName(Name);
        return SyntaxMatcherDispatcher;
    }

    /// <summary>
    /// set up delegate for this syntax execution dispatch map
    /// <para>takes a method in a lambda unary call expression: () => methodName</para>
    /// <para>takes a called method with no parameter</para>
    /// <para>takes a called method with a default command result (code ok, result null)</para>
    /// </summary>
    /// <param name="expression">
    /// a lambda unary call expression:
    /// <para>() => methodName</para>
    /// </param>
    /// <returns>syntax matcher dispatcher</returns>
    /// <exception cref="MissingMethodException">the method was not found or is not suitable</exception>
    /// <exception cref="InvalidOperationException">the method prototype doesn't match the command</exception>
    /// <exception cref="InvalidCastException">the method parameter type doesn't match the command argument value type</exception>
    public SyntaxMatcherDispatcher Do(LambdaExpression expression)
    {
        var (methodInfo, target) = expression.GetAnyCastDelegate();
        var error = () => Syntax.ToSyntax()
                + Environment.NewLine
                + expression.ToString();

        if (methodInfo is null)
            throw new MissingOrNotFoundCommandOperationException(error());

        if (methodInfo.ReturnType != typeof(void)
            || target is null
            || target is not Command)
            throw new InvalidCommandOperationException(error());

        Name = methodInfo.Name;
        Syntax.SetName(Name);
        Delegate = (CommandContext context) =>
        {
            var callParameters = new List<object?>();
            int argIndex;
            var currentParamIndex = -1;

            foreach (var parameter in methodInfo.GetParameters())
            {
                // anywhere command context argument
                if (parameter.ParameterType == typeof(CommandContext))
                {
                    callParameters.Add(
                        new CommandContext(
                            Syntax,
                            SyntaxMatcherDispatcher.OptSet,
                            SyntaxMatcherDispatcher
                            ));
                }
                else
                {
                    int GetParamIndex(int currentParamIndex, int argSkipCount = 0)
                    {
                        // mapped arguments
                        // auto mapped arguments (index mapping)
                        if (parameter
                            .GetCustomAttributes(false)
                            .Where(x => x.GetType() == typeof(MapArgAttribute))
                            .FirstOrDefault() is not MapArgAttribute mapArg)
                        {
                            return
                                Syntax.GetIndexOfArgWithExpectedValueFromIndex(
                                    currentParamIndex + 1 + argSkipCount);
                        }
                        else
                        {
                            // mapped explicitly
                            return mapArg.ArgIndex;
                        }
                    }

                    currentParamIndex = argIndex = GetParamIndex(currentParamIndex);

                    var arg = Syntax[argIndex];

                    if (arg is not Flag && arg is IOpt opt && opt.ExpectedValuesCount == 0)
                    // skip arg, parameter not required
                    {
                        currentParamIndex = argIndex = GetParamIndex(currentParamIndex);
                        arg = Syntax[argIndex];
                    }

                    #region errors

                    void ThrowInvalidCommandOperationParameterCastException()
                        => throw new InvalidCommandOperationParameterCastException(
                                currentParamIndex,
                                arg!.ValueType,
                                parameter!.ParameterType,
                                error!());

                    void ThrowInvalidCommandOperationParameterNullabilityExpectedException()
                        => throw new InvalidCommandOperationParameterNullabilityExpectedException(
                                currentParamIndex,
                                arg!.ValueType,
                                parameter!.ParameterType,
                                error!());

                    void ThrowInvalidCommandOperationParameterNullabilityNotExpectedException()
                        => throw new InvalidCommandOperationParameterNullabilityNotExpectedException(
                                currentParamIndex,
                                arg!.ValueType,
                                parameter!.ParameterType,
                                error!());

                    #endregion

                    var argType = arg.GetType();
                    var targetIsArg = parameter!.ParameterType.HasInterface(typeof(IArg));
                    var isOptional = arg.GetIsOptional();
                    var isTargetNullable = parameter.ParameterType
                            .IsExplicitNullable()
                        || parameter.CustomAttributes.
                                Any(x => x.AttributeType
                                    .Name
                                    .Contains("NullableAttribute"));



                    var isTargetNullableRequired = isOptional && (
                        argType == typeof(Opt<>) || argType == typeof(Opt));
                    var isSet = arg.GetIsSet();

                    if (targetIsArg && parameter.ParameterType == arg.GetType())
                    {
                        // argument type == specification type (Opt,Param,..)
                        callParameters.Add(arg);
                    }
                    else
                    {
                        // concrete argument type mappings

                        if (targetIsArg)
                            ThrowInvalidCommandOperationParameterCastException();

                        if (!isTargetNullableRequired && isTargetNullable)
                            ThrowInvalidCommandOperationParameterNullabilityNotExpectedException();

                        if (isTargetNullableRequired && !isTargetNullable)
                            ThrowInvalidCommandOperationParameterNullabilityExpectedException();

                        if (parameter.ParameterType == arg.ValueType)
                        {
                            // argument type == value type (direct type mapping: string,bool,...)
                            callParameters.Add(arg.GetValue());
                        }
                        else
                        {
                            if (parameter.ParameterType.GenericTypeArguments.Length == 1 &&
                                parameter.ParameterType.GenericTypeArguments[0] == arg.ValueType)
                            {
                                // generic argument type == value type (direct type mapping for collection)
                                // and Nullable types
                                callParameters.Add(isSet ? arg.GetValue() : null);
                            }
                            else
                                // parameter type mismatch
                                ThrowInvalidCommandOperationParameterCastException();
                        }
                    }
                }
            }

            methodInfo.Invoke(target, callParameters.ToArray());

            return new();
        };

        return SyntaxMatcherDispatcher;
    }
}
