
using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// takes grammar definitions and try run the valid one
/// </summary>
public sealed class GrammarMatcherDispatcher
{
    private readonly List<GrammarExecutionDispatchMapItem> _maps = new();

    /// <summary>
    /// build a grammar from arguments grammars set
    /// </summary>
    /// <param name="grammar">arguments grammars</param>
    /// <returns>a grammar object</returns>
    public GrammarExecutionDispatchMapItem For(params Arg[] grammar)
    {
        var dispatchMap = new GrammarExecutionDispatchMapItem(
            this,
            new Grammar(grammar));
        _maps.Add(dispatchMap);
        return dispatchMap;
    }

    /// <summary>
    /// run the grammar matching the given args or produces an error result
    /// </summary>
    /// <param name="args">set of command line arguments</param>
    /// <returns>command execution result</returns>
    public CommandResult Run(ArgSet args) => throw new NotImplementedException();
}
