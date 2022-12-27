using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Service.CmdLine.Arguments;

/// <summary>
/// a command line option : -name [value1 [.. value n], --name [value1 [.. value n] of values of type string
/// </summary>
public class Opt : Opt<string>
{
    /// <summary>
    /// build a new option
    /// <para>corresponds to the syntax:</para>
    /// <para>-optName | --optName [value1 [.. valuen]]</para>
    /// </summary>
    /// <param name="name">argument name</param>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    /// <param name="isOptional">true if optional</param>
    /// <param name="valuesCount">number of expected values</param>
    public Opt(
        string name,
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter,
        bool isOptional,
        int valuesCount = 0)
        : base(name, config, texts, valueConverter, isOptional, valuesCount)
    {
    }

    /// <inheritdoc/>
    public new string? this[int index]
    {
        get
        {
            if (ExpectedValuesCount != 0 && index > ExpectedValuesCount)
                throw ValueIndexNotAvailaible(index);
            return
                Values[index];
        }
    }

    /// <inheritdoc/>
    public new string? GetValue() => this[0];
}
