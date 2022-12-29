
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

/// <summary>
/// global option
/// </summary>
public abstract class GlobalOpt : Opt, IGlobalOpt
{
    /// <summary>
    /// global option
    /// </summary>
    /// <param name="config">config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    public GlobalOpt(
            IConfiguration config,
            Texts texts,
            ValueConverter valueConverter)
                : base(
                    string.Empty,
                    config,
                    texts,
                    valueConverter,
                    true)
        => Name = Parser.ClassNameToOptName(
                GetType().Name);
}
