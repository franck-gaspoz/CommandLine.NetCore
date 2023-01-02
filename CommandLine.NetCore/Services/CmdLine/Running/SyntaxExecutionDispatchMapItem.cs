
using System.Linq.Expressions;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Parsing;

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
    public Func<OperationContext, OperationResult>? Delegate { get; private set; }

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
    /// <param name="delegate">with parameter syntax and OperationResult result delegate</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do(Func<OperationContext, OperationResult> @delegate)
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
    public SyntaxMatcherDispatcher Do(Action<OperationContext> @delegate)
    {
        Name = @delegate.Method.Name;
        Delegate = (OperationContext context) =>
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
        Delegate = (OperationContext context) =>
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
        Delegate = (OperationContext context) =>
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
    public SyntaxMatcherDispatcher Do(LambdaExpression expression)
    {
        var (methodInfo, target) = expression.GetAnyCastDelegate();
        var error = () => Syntax.ToSyntax()
                + Environment.NewLine
                + expression.ToString();

        if (methodInfo is null)
            throw new MissingMethodException(error());

        if (methodInfo.ReturnType != typeof(void)
            || target is null
            || target is not Command)
        {
            throw new InvalidOperationException(error());
        }

        Name = methodInfo.Name;
        Syntax.SetName(Name);
        Delegate = (OperationContext context) =>
        {
            var callParameters = new List<object?>();
            int argIndex;
            var currentParamIndex = -1;

            foreach (var parameter in methodInfo.GetParameters())
            {
                if (parameter
                    .GetCustomAttributes(false)
                    .Where(x => x.GetType() == typeof(MapArgAttribute))
                    .FirstOrDefault() is not MapArgAttribute mapArg)
                {
                    currentParamIndex = argIndex =
                        Syntax.GetIndexOfArgWithExpectedValueFromIndex(
                            currentParamIndex + 1);
                }
                else
                {
                    currentParamIndex = argIndex = mapArg.ArgIndex;
                }

                var argValue = Syntax[argIndex];
                callParameters.Add(argValue);
            }

            methodInfo.Invoke(target, callParameters.ToArray());

            return new();
        };

        return SyntaxMatcherDispatcher;
    }
}
