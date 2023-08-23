using static CommandLine.NetCore.Services.CmdLine.Settings.Globals;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// result of an operation done by a command successfully parsed
/// </summary>
public class CommandLineResult
{
    /// <summary>
    /// an exit code
    /// </summary>
    public int ExitCode { get; set; }

    /// <summary>
    /// any result
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code (default ExitOk)</param>
    /// <param name="result">eventual result (default null)</param>
    public CommandLineResult(int exitCode = ExitOk, object? result = null)
    {
        ExitCode = exitCode;
        Result = result;
    }
}
