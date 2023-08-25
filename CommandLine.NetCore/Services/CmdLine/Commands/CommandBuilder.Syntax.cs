#define Enable_h_Arg

using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command builder
/// </summary>
public sealed partial class CommandBuilder
{
    /// <summary>   
    /// build a syntax from arguments syntaxes set
    /// </summary>
    /// <param name="syntax">arguments syntaxes</param>
    /// <returns>a syntax dispatcher map item</returns>
    public SyntaxExecutionDispatchMapItem For(params Arg[] syntax)
    {
        if (_runMethod is null) ArgumentNullException.ThrowIfNull(_runMethod);

        if (_syntaxMatcherDispatcher is null)
        {
            _syntaxMatcherDispatcher = new(
                _texts,
                _parser,
                _globalSettings,
                _console);
        }

#if Enable_h_Arg
        if (_syntaxMatcherDispatcher.Count == 0)
            AddHelpAboutCommandSyntax(_syntaxMatcherDispatcher);
#endif
        var syntaxExecutionDispatchItem = _syntaxMatcherDispatcher.For(syntax);
        syntaxExecutionDispatchItem.Syntax
            .SetName(_commandName);
        return syntaxExecutionDispatchItem;
    }
}
