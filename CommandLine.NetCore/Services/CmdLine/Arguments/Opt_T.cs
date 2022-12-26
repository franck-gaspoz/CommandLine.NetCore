using System.Diagnostics;

using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a command line option : -name [value1 [.. value n], --name [value1 [.. value n] of values of generic type T
/// </summary>
/// <typeparam name="T">option type of values</typeparam>
[DebuggerDisplay("{DebuggerDisplay}")]
public class Opt<T> : Arg, IOpt
{
    private string DebuggerDisplay => ToGrammar();

    /// <inheritdoc/>
    public override string ToGrammar()
    {
        var values = string.Empty;
        if (ExpectedValuesCount > 0)
            values = $" = {string.Join(',', Values)}";
        return $"Opt<{typeof(T).Name}> {PrefixedName}{values}";
    }

    /// <summary>
    /// values count (expected)
    /// </summary>
    public int ExpectedValuesCount { get; private set; }

    /// <summary>
    /// unparsed values
    /// </summary>
    protected readonly List<string> Values = new();

    /// <summary>
    /// argument name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// build a new option
    /// <para>corresponds to the syntax:</para>
    /// <para>-optName | --optName [value1 [.. valuen]]</para>
    /// </summary>
    /// <param name="name">option name</param>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    /// <param name="valuesCount">values count</param>
    public Opt(
        string name,
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter,
        int valuesCount = 0)
        : base(config, texts, valueConverter)
    {
        Name = name;
        ExpectedValuesCount = valuesCount;
    }

    /// <summary>
    /// get value at index
    /// </summary>
    /// <param name="index">value index (from 0)</param>
    /// <returns>value at index</returns>
    /// <exception cref="ArgumentException">the option has no value at the index</exception>
    public T? this[int index]
    {
        get
        {
            if (ExpectedValuesCount != 0 && index > ExpectedValuesCount)
                throw ValueIndexNotAvailaible(index);
            return
                ConvertValue<T>(Values[index]);
        }
    }

    /// <summary>
    /// add a value to the option
    /// </summary>
    /// <param name="value">string representation of the value</param>
    public void AddValue(string value)
        => Values.Add(value);

    protected ArgumentException ValueIndexNotAvailaible(int index)
        => new ArgumentException(
            Texts._("NoArgumentValueAtIndex", index));

    /// <summary>
    /// get value if single (index 0)
    /// </summary>
    /// <returns>value at index 0</returns>
    public T? GetValue() => this[0];

    /// <summary>
    /// name with prefix
    /// </summary>
    public string PrefixedName => Prefix + Name;

    /// <summary>
    /// get the argument description from help settings
    /// </summary>
    /// <param name="description">description or error message if not found</param>
    /// <returns>true if description is found, otherwise false</returns>
    public bool GetDescription(out KeyValuePair<string, string> description)
    {
        var desc = Config.GetSection("GlobalArgs:" + Name);

        if (!desc.Exists() || !desc.GetChildren().Any())
        {
            description = new KeyValuePair<string, string>(
                Texts._("GlobalArgHelpNotFound", Name),
                string.Empty);
            return false;
        }

        var kvp = desc.GetChildren().First();

        description = new KeyValuePair<string, string>(
            kvp.Key,
            kvp.Value ?? string.Empty
            );

        return true;
    }

    /// <summary>
    /// returns the prefix allowed for this option
    /// </summary>
    public string Prefix => Parser.GetPrefixFromOptName(Name);
}
