namespace CommandLine.NetCore.Services.CmdLine;

/// <summary>
/// result of an operation done by a command successfully parsed
/// </summary>
public class OperationResult
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
    /// <param name="exitCode">exit code</param>
    /// <param name="result">eventual result</param>
    public OperationResult(int exitCode, object? result = null)
    {
        ExitCode = exitCode;
        Result = result;
    }
}
