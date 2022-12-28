
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CommandLine.NetCore.GlobalOpts;

internal class ParserLogging : GlobalOpt<LogLevel>
{
    public ParserLogging(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
            : base(config, texts, valueConverter, 1)
    {
    }
}
