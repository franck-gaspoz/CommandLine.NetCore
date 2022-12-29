
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

namespace CommandLine.NetCore.GlobalOpts;

/// <summary>
/// global option: --exclude-ambiguous-grammar
/// <para>exclude ambiguous grammar</para>
/// <para>by default, the first matching grammar is selected in the command line arguments parser</para>
/// </summary>
public class ExcludeAmbiguousGrammar : GlobalOpt<string>
{
    public ExcludeAmbiguousGrammar(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
            : base(config, texts, valueConverter)
    {
    }
}
