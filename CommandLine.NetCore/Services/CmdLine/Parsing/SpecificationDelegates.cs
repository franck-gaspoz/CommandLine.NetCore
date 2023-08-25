using CommandLine.NetCore.Services.CmdLine.Commands;
using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Parsing;

#if no
/// <summary>
/// command specification delegate
/// </summary>
/// <param name="argSet"></param>
/// <param name="builder"></param>
public delegate void CommandSpecificationDelegate(
    ArgSet argSet,
    CommandBuilder builder
    );
#endif

/// <summary>
/// dynamic command specification delegate
/// </summary>
/// <param name="builder"></param>
/// <param name="context"></param>
public delegate SyntaxMatcherDispatcher DynamicCommandSpecificationDelegate(
    CommandBuilder builder,
    DynamicCommandContext context
    );