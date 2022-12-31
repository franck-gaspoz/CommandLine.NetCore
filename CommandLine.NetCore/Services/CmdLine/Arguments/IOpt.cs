
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

    /// <summary>
    /// true if optinal
    /// </summary>
    bool IsOptional { get; }

    /// <summary>
    /// true if option is optional and present in the parsed syntax
    /// </summary>
    bool IsSetted { get; }

    /// <summary>
    /// change the is optional value
    /// </summary>
    /// <param name="isOptional">is optionnal</param>
    void SetIsOptional(bool isOptional);

    /// <summary>
    /// change the is setted value
    /// </summary>
    /// <param name="isSetted">is setted</param>
    void SetIsSetted(bool isSetted);

    /// <summary>
    /// syntax as text
    /// </summary>
    /// <returns>text</returns>
    string ToText();

    /// <summary>
    /// syntax as string arguments
    /// </summary>
    /// <returns>string arguments array</returns>
    public string[] ToArgs();
}