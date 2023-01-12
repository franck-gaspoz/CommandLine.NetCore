using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.GlobalOpts;

/// <summary>
/// global option: --no-color 
/// <para>turn off ansi/vt outputs</para>
/// </summary>
public class NoColor : GlobalOpt
{
    /// <summary>
    /// global option: --no-color 
    /// <para>turn off ansi/vt outputs</para>
    /// </summary>
    /// <param name="config">config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    public NoColor(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
            : base(config, texts, valueConverter)
    {
    }
}

