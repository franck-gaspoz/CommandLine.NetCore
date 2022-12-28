
using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// a grammar dispatch map
/// </summary>
public sealed class GrammarExecutionDispatchMapItem
{
    /// <summary>
    /// grammar spec
    /// </summary>
    public Grammar Grammar { get; private set; }

    /// <summary>
    /// execute action delegate
    /// </summary>
    public Action? Delegate { get; private set; }

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
        => (GrammarMatcherDispatcher, Grammar)
            = (grammarMatcherDispatcher, grammar);

    /// <summary>
    /// set up delegate for this grammar execution dispatch map
    /// </summary>
    /// <param name="delegate"></param>
    /// <returns></returns>
    public GrammarMatcherDispatcher Execute(Action @delegate)
    {
        Delegate = @delegate;
        return GrammarMatcherDispatcher;
    }
}
