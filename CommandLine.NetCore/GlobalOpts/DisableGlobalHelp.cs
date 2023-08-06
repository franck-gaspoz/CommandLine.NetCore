using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.GlobalOpts;

/// <summary>
/// global option: --disable-global-help
/// <para>disable the global help command</para>
/// </summary>
public class DisableGlobalHelp : GlobalOpt
{
    /// <summary>
    /// global option: --disable-global-help
    /// <para>disable the global help command</para>
    /// </summary>
    /// <param name="config">config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    public DisableGlobalHelp(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
            : base(config, texts, valueConverter)
    {
    }
}
