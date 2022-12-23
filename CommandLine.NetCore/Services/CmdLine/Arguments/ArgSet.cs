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
        params Arg[] syntax
        )
    {
        var parse_index = 0;
        var syntax_index = 0;

        var args = _args.ToList();
        var error = string.Empty;

        while (parse_index < args.Count && syntax_index < syntax.Length)
        {
            Arg currentSyntax() => syntax[syntax_index];
            string currentArg() => args[parse_index];

            var str = currentArg();
            if (Parser.IsOpt(str))
            {
                // option
            }
            else
            {
                // parameter
                if (currentSyntax() is not IParam param)
                {
                    // type mismatch
                    error = "Expected: ";
                }
                else
                {

                }
            }
        }

        return false;
    }
}
