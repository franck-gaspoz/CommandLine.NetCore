namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// set of arguments of a command invokation
/// </summary>
public class ArgSet
{
    /// <summary>
    /// arguments
    /// </summary>
    public IReadOnlyCollection<string> Args => _args;

    private readonly List<string> _args;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="args">arguments</param>
    public ArgSet(IEnumerable<string> args)
        => _args = new List<string>(args);

    /// <summary>
    /// args count
    /// </summary>
    public int Count => Args.Count;

    /// <summary>
    /// array get accessor
    /// </summary>
    /// <param name="index">argument index (from 0)</param>
    /// <returns>argument value at index</returns>
    public string this[int index] => _args[index];

    /// <summary>
    /// check if the arg set match the syntax described by the parameters from left to right
    /// </summary>
    /// <returns>true if syntax match, false otherwise</returns>
    public bool MatchSyntax(
        params Arg[] args
        )
    {
        foreach (var arg in args)
        {

        }
        return false;
    }
}
