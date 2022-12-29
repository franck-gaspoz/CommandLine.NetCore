
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
    /// <param name="delegate"></param>
    /// <returns></returns>
    public GrammarMatcherDispatcher Do(Func<Grammar, OperationResult> @delegate)
    {
        Delegate = @delegate;
        Name = Delegate.Method.Name;
        Grammar.SetName(Name);
        return GrammarMatcherDispatcher;
    }
}
