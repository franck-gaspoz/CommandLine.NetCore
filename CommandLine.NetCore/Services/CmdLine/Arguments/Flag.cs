using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// a command line flag (optional with no value) : -name | --name
/// </summary>
public class Flag : Opt<bool>
{
    /// <summary>
    /// build a new option
    /// <para>corresponds to the syntax:</para>
    /// <para>-optName | --optName</para>
    /// </summary>
    /// <param name="name">argument name</param>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    /// <param name="isOptional">true if optional</param>
    public Flag(
        string name,
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter,
        bool isOptional)
        : base(name, config, texts, valueConverter, isOptional, 0)
    {
    }

    /// <inheritdoc/>
    public new bool this[int index]
    {
        get
        {
            if (index > ExpectedValuesCount)
                throw ValueIndexNotAvailaible(index);
            return
                IsSet;
        }
    }

    /// <inheritdoc/>
    public override object GetValue() => IsSet;
}
