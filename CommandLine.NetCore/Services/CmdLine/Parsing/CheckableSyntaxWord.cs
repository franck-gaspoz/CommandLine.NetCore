
using System.Diagnostics;

using CommandLine.NetCore.Services.CmdLine.Arguments;

namespace CommandLine.NetCore.Services.CmdLine.Parsing;

/// <summary>
/// a syntax word that must should be checked by the argument parser
/// </summary>
[DebuggerDisplay("{Arg},IsRemainingOptional={IsRemainingOptional}")]
internal class CheckableSyntaxWord
{
    /// <summary>
    /// is remaining optional
    /// </summary>
    public bool IsRemainingOptional { get; private set; }

    /// <summary>
    /// argument to be checked
    /// </summary>
    public IArg Arg { get; private set; }

    public CheckableSyntaxWord(bool isRemainingOptional, IArg arg)
    {
        IsRemainingOptional = isRemainingOptional;
        Arg = arg;
    }

    public void Deconstruct(
        out bool isRemainingOptional,
        out IArg arg)
    {
        isRemainingOptional = IsRemainingOptional;
        arg = Arg;
    }
}
