using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Parsing;

class MatchingSyntax
{
    public SyntaxExecutionDispatchMapItem SyntaxExecutionDispatchMapItem { get; private set; }

    public OptSet SettedOptions { get; private set; }

    public MatchingSyntax(
        SyntaxExecutionDispatchMapItem syntaxExecutionDispatchMapItem,
        OptSet settedOptions)
    {
        SyntaxExecutionDispatchMapItem = syntaxExecutionDispatchMapItem;
        SettedOptions = settedOptions;
    }
}