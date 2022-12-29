﻿
using System.Linq.Expressions;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// a grammar dispatch map
/// </summary>
public sealed class GrammarExecutionDispatchMapItem
{
    /// <summary>
    /// name of the grammar
    /// <para>is the Do method name</para>
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// grammar spec
    /// </summary>
    public Grammar Grammar { get; private set; }

    /// <summary>
    /// execute action delegate
    /// </summary>
    public Func<Grammar, OperationResult>? Delegate { get; private set; }

    /// <summary>
    /// the grammar matcher dispatcher owner of this
    /// </summary>
    public GrammarMatcherDispatcher GrammarMatcherDispatcher { get; private set; }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="grammarMatcherDispatcher">the grammar matcher dispatcher owner of this</param>
    /// <param name="grammar">grammar</param>
    public GrammarExecutionDispatchMapItem(
        GrammarMatcherDispatcher grammarMatcherDispatcher,
        Grammar grammar)
        => (GrammarMatcherDispatcher, Grammar, Name)
            = (grammarMatcherDispatcher, grammar, string.Empty);

    /// <summary>
    /// set up delegate for this grammar execution dispatch map
    /// </summary>
    /// <param name="delegate">with parameter grammar and OperationResult result delegate</param>
    /// <returns>grammar matcher dispatcher</returns>
    public GrammarMatcherDispatcher Do(Func<Grammar, OperationResult> @delegate)
    {
        Delegate = @delegate;
        Name = Delegate.Method.Name;
        Grammar.SetName(Name);
        return GrammarMatcherDispatcher;
    }

    /// <summary>
    /// set up delegate for this grammar execution dispatch map
    /// <para>takes a method with a default command result (code ok, result null)</para>
    /// </summary>
    /// <param name="delegate">with parameter grammar and void delegate</param>
    /// <returns>grammar matcher dispatcher</returns>
    public GrammarMatcherDispatcher Do(Action<Grammar> @delegate)
    {
        Name = @delegate.Method.Name;
        Delegate = (Grammar grammar) =>
        {
            @delegate.Invoke(grammar);
            return new();
        };
        Grammar.SetName(Name);
        return GrammarMatcherDispatcher;
    }

    /// <summary>
    /// set up delegate for this grammar execution dispatch map
    /// <para>takes a method with no parameter </para>
    /// <para>takes a method with a default command result (code ok, result null)</para>
    /// </summary>
    /// <param name="delegate">with no parameter and void result delegate</param>
    /// <returns>grammar matcher dispatcher</returns>
    public GrammarMatcherDispatcher Do(Action @delegate)
    {
        Name = @delegate.Method.Name;
        Delegate = (Grammar grammar) =>
        {
            @delegate.Invoke();
            return new();
        };
        Grammar.SetName(Name);
        return GrammarMatcherDispatcher;
    }

    /// <summary>
    /// set up delegate for this grammar execution dispatch map
    /// <para>takes a method in a lambda unary call expression: () => methodName</para>
    /// <para>takes a called method with no parameter</para>
    /// <para>takes a called method with a default command result (code ok, result null)</para>
    /// </summary>
    /// <param name="expression">
    /// a lambda unary call expression:
    /// <para>() => methodName</para>
    /// </param>
    /// <returns>grammar matcher dispatcher</returns>
    public GrammarMatcherDispatcher Do(LambdaExpression expression)
    {
        var methodInfo = expression.GetIndirectMethodInfo();
        return GrammarMatcherDispatcher;
    }
}
