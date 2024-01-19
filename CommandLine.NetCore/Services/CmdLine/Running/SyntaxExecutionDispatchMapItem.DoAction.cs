using System.Linq.Expressions;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// a syntax dispatch map : do actions
/// </summary>
public sealed partial class SyntaxExecutionDispatchMapItem
{
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
        if (@delegate.Method.Name.Contains('$'))
            // top level statement case. TODO: check case identification
            return DoAction(@delegate);

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
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1>(Action<T1> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2>(Action<T1, T2> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <typeparam name="T3">T3</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2, T3>(Action<T1, T2, T3> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <typeparam name="T3">T3</typeparam>
    /// <typeparam name="T4">T4</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <typeparam name="T3">T3</typeparam>
    /// <typeparam name="T4">T4</typeparam>
    /// <typeparam name="T5">T5</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <typeparam name="T3">T3</typeparam>
    /// <typeparam name="T4">T4</typeparam>
    /// <typeparam name="T5">T5</typeparam>
    /// <typeparam name="T6">T6</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <typeparam name="T3">T3</typeparam>
    /// <typeparam name="T4">T4</typeparam>
    /// <typeparam name="T5">T5</typeparam>
    /// <typeparam name="T6">T6</typeparam>
    /// <typeparam name="T7">T7</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <typeparam name="T3">T3</typeparam>
    /// <typeparam name="T4">T4</typeparam>
    /// <typeparam name="T5">T5</typeparam>
    /// <typeparam name="T6">T6</typeparam>
    /// <typeparam name="T7">T7</typeparam>
    /// <typeparam name="T8">T8</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <typeparam name="T3">T3</typeparam>
    /// <typeparam name="T4">T4</typeparam>
    /// <typeparam name="T5">T5</typeparam>
    /// <typeparam name="T6">T6</typeparam>
    /// <typeparam name="T7">T7</typeparam>
    /// <typeparam name="T8">T8</typeparam>
    /// <typeparam name="T9">T9</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        => DoAction(action);

    /// <summary>
    /// set up delegate for this syntax execution dispatch map from an action with typed parameters
    /// </summary>
    /// <typeparam name="T1">T1</typeparam>
    /// <typeparam name="T2">T2</typeparam>
    /// <typeparam name="T3">T3</typeparam>
    /// <typeparam name="T4">T4</typeparam>
    /// <typeparam name="T5">T5</typeparam>
    /// <typeparam name="T6">T6</typeparam>
    /// <typeparam name="T7">T7</typeparam>
    /// <typeparam name="T8">T8</typeparam>
    /// <typeparam name="T9">T9</typeparam>
    /// <typeparam name="T10">T10</typeparam>
    /// <param name="action">action</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
        => DoAction(action);

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
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do(LambdaExpression expression)
    {
        var (methodInfo, target) = expression.GetAnyCastDelegate();
        var error = () => GetSyntaxError(expression.ToString());

        if (methodInfo is null)
            throw new MissingOrNotFoundCommandOperationException(
                error(),
                new CommandResult(
                    ExitFail,
                    Syntax));

        if (methodInfo.ReturnType != typeof(void)
            || target is null
            || target is not Command)
            throw new InvalidCommandOperationException(
                error(),
                new CommandResult(
                    ExitFail,
                    Syntax));

        return DoMethod(
            expression.ToString(),
            methodInfo.Name,
            methodInfo,
            target);
    }
}
