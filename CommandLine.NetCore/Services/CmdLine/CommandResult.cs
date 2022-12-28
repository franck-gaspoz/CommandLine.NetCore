namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// command result
/// </summary>
public class CommandResult
{
    /// <summary>
    /// an exit code
    /// </summary>
    public int ExitCode { get; set; }

    /// <summary>
    /// list of parse errors (if any else empty)
    /// </summary>
    public List<string> ParseErrors { get; set; }

    /// <summary>
    /// any result
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code</param>
    /// <param name="parseErrors">parse errors</param>
    /// <param name="result">result</param>
    public CommandResult(
        int exitCode,
        List<string> parseErrors,
        object? result = null)
    {
        ExitCode = exitCode;
        ParseErrors = parseErrors;
        Result = result;
    }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code</param>
    public CommandResult(int exitCode)
    {
        ExitCode = exitCode;
        ParseErrors = new List<string>();
        Result = null;
    }
}
