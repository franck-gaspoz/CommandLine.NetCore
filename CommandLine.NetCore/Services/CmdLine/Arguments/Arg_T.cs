
using CommandLine.NetCore.Service.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// arg with one value of type T
/// </summary>
public class Arg<T> : Arg
{
    public Arg(string name,
        IConfiguration config,
        Texts texts,
        int valuesCount = 0)
        : base(name, config, texts, valuesCount)
    {
    }

    /// <summary>
    /// get value at index
    /// </summary>
    /// <param name="index">value index (from 0)</param>
    /// <returns>value at index</returns>
    /// <exception cref="ArgumentException">l'argument n'a pas de valeur pour l'index demandé</exception>
    public new T this[int index]
    {
        get
        {
            if (ValuesCount != 0 && index > ValuesCount)
                throw ValueIndexNotAvailaible(index);
            return Values[index];
        }
    }

    /// <summary>
    /// get value (index 0)
    /// </summary>
    /// <returns>value at index 0</returns>
    public new T GetValue() => Values[0];
}
