
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

/// <summary>
/// global option
/// </summary>
public abstract class GlobalOpt<T> : Opt<T>, IGlobalOpt
{
    /// <summary>
    /// global option
    /// </summary>
    /// <param name="config">config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    /// <param name="valuesCount">values count</param>
    public GlobalOpt(
            IConfiguration config,
            Texts texts,
            ValueConverter valueConverter,
            int valuesCount = 0)
                : base(
                    string.Empty,
                    config,
                    texts,
                    valueConverter,
                    true,
                    valuesCount)
        => Name = Parser.ClassNameToOptName(
                GetType().Name);
}
