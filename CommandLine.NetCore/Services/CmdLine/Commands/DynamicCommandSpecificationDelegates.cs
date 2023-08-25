using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// dynamic command specification delegate
/// </summary>
/// <param name="builder"></param>
/// <param name="context"></param>
public delegate SyntaxMatcherDispatcher DynamicCommandSpecificationDelegate(
    CommandBuilder builder,
    DynamicCommandContext context
    );