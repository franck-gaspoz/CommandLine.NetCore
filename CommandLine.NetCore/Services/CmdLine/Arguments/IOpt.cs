
namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// an option in a set of command line arguments
/// </summary>
public interface IOpt : IArg
{
    /// <summary>
    /// name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// prefix
    /// </summary>
    string Prefix { get; }

    /// <summary>
    /// name with prefix
    /// </summary>
    string PrefixedName { get; }

    /// <summary>
    /// count of values
    /// </summary>
    int ExpectedValuesCount { get; }

    /// <summary>
    /// add a value (in order) to the option
    /// </summary>
    /// <param name="value">text representation of the value</param>
    void AddValue(string value);

    /// <summary>
    /// get description of the option from help settings
    /// </summary>
    /// <param name="description">returned description</param>
    /// <returns>true if found, false otherwise</returns>
    bool GetDescription(out KeyValuePair<string, string> description);
}