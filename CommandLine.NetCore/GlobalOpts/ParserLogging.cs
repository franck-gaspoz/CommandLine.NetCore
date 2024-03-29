﻿
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CommandLine.NetCore.GlobalOpts;

/// <summary>
/// global option: --parser-logging
/// <para>set up the logging level of the parser</para>
/// <para>possibles values from Microsoft.Extensions.Logging.LogLevel</para>
/// <para>for Trace or Debug the parser add detailed informations about the parsed syntaxes</para>
/// </summary>
public class ParserLogging : GlobalOpt<LogLevel>
{
    /// <summary>
    /// global option: --parser-logging
    /// <para>set up the logging level of the parser</para>
    /// <para>possibles values from Microsoft.Extensions.Logging.LogLevel</para>
    /// <para>for Trace or Debug the parser add detailed informations about the parsed syntaxes</para>
    /// </summary>
    /// <param name="config">config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    public ParserLogging(
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter)
            : base(config, texts, valueConverter, 1)
    {
    }
}
