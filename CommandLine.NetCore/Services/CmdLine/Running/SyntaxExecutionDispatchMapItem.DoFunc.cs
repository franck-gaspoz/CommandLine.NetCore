using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// a syntax dispatch map
/// </summary>
public sealed partial class SyntaxExecutionDispatchMapItem
{
    /// <summary>
    /// set up delegate for this syntax execution dispatch map
    /// </summary>
    /// <param name="delegate">with parameter operation context and OperationResult result delegate</param>
    /// <returns>syntax matcher dispatcher</returns>
    public SyntaxMatcherDispatcher Do(Func<CommandContext, CommandLineResult> @delegate)
    {
        Delegate = @delegate;
        Name = Delegate.Method.Name;
        Syntax.SetName(Name);
        return SyntaxMatcherDispatcher;
    }


    // TODO: check this
#if no
    /// <summary>
    /// set up delegate for this syntax execution dispatch map
    /// <para>takes a method with no parameter </para>
    /// </summary>
    /// <param name="delegate">with no parameter and void result delegate</param>
    /// <returns>syntax matcher dispatcher</returns>
    SyntaxMatcherDispatcher Do(Func<CommandLineResult> @delegate)
    {
        Name = @delegate.Method.Name;
        Delegate = (CommandContext context) =>
        {
            return @delegate.Invoke();
        };
        Syntax.SetName(Name);
        return SyntaxMatcherDispatcher;
    }
#endif
}
