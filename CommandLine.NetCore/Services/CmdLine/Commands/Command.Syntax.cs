using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// abstract command : Syntax
/// </summary>
public abstract partial class Command
{
    /// <summary>   
    /// build a syntax from arguments syntaxes set
    /// </summary>
    /// <param name="syntax">arguments syntaxes</param>
    /// <returns>a syntax dispatcher map item</returns>
    protected SyntaxExecutionDispatchMapItem For(params Arg[] syntax)
        => _builder.For(syntax);
}

