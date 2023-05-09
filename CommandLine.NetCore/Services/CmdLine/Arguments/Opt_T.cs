using System.Diagnostics;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a command line option : -name [value1 [.. value n], --name [value1 [.. value n] of values of generic type T
/// </summary>
/// <typeparam name="T">option type of values</typeparam>
[DebuggerDisplay("{_debuggerDisplay}")]
public class Opt<T> : Arg, IOpt
{
    string _debuggerDisplay => ToSyntax();

    /// <inheritdoc/>
    public override Type ValueType => typeof(T);

    /// <inheritdoc/>
    public bool IsOptional { get; private set; }

    bool _isSetted;

    /// <inheritdoc/>
    public bool IsSet
    {
        get => _isSetted;

        private set => _isSetted = value;
    }

    /// <inheritdoc/>
    public string ToText()
    {
        var text = PrefixedName;
        var values
            = Values.Where(x => x is not null)
                .Select(x => x.ToText())
                .ToList();

        if (values.Any())
            text += " " + string.Join(" ", values);

        return text;
    }

    /// <inheritdoc/>
    public string[] ToArgs()
    {
        var values
            = Values.Where(x => x is not null)
                .Select(x => x.ToText())
                .ToList();
        values.Insert(0, PrefixedName);
        return values.ToArray();
    }

    /// <inheritdoc/>
    public override string ToSyntax()
    {
        var values = string.Empty;
        object? nullObj = null;
        if (ExpectedValuesCount > 0)
        {
            var valueSet = new List<string>();
            for (var i = 0; i < ExpectedValuesCount; i++)
            {
                valueSet.Add(
                    (i < Values.Count) ?
                    Values[i].ToText()
                    : nullObj.ToText());
            }

            values = $"{{{string.Join(',', valueSet)}}}";
        }

        var isSetted = !IsOptional ? string.Empty :
            IsSet ? "+" : string.Empty;

        return $"Opt{(IsOptional ? "?" : string.Empty)}<{typeof(T).Name}>{PrefixedName}{isSetted}{values}";
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
    public string Name { get; protected set; }

    /// <summary>
    /// build a new option
    /// <para>corresponds to the syntax:</para>
    /// <para>-optName | --optName [value1 [.. valuen]]</para>
    /// </summary>
    /// <param name="name">option name</param>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    /// <param name="isOptional"><see langword="true"/>if is optional</param>
    /// <param name="valuesCount">values count</param>
    public Opt(
        string name,
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter,
        bool isOptional = false,
        int valuesCount = 0
        )
        : base(config, texts, valueConverter)
    {
        Name = name;
        IsOptional = isOptional;
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
    /// <exception cref="ArgumentException">convert error</exception>
    public void AddValue(string value)
    {
        Values.Add(value);
        ConvertValue<T>(Values.Last());
    }

    /// <summary>
    /// build an exception value not available at index
    /// </summary>
    /// <param name="index">index</param>
    /// <returns>ArgumentException</returns>
    protected ArgumentException ValueIndexNotAvailaible(int index)
        => new(
            Texts._("NoArgumentValueAtIndex", index));

    /// <summary>
    /// get value if single (index 0)
    /// </summary>
    /// <returns>value at index 0</returns>
    public T? GetValue() => IsSet ? this[0] : default;

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
        var desc = Config.GetSection("GlobalOptions:" + Name);

        if (!desc.Exists() || !desc.GetChildren().Any())
        {
            description = new KeyValuePair<string, string>(
                Parser.GetPrefixFromOptName(Name) + Name,
                Texts._("GlobalOptHelpNotFound", Name));
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

    /// <inheritdoc/>
    public void SetIsOptional(bool isOptional)
        => IsOptional = isOptional;

    /// <inheritdoc/>
    public void SetIsSetted(bool isSetted)
        => IsSet = isSetted;
}
