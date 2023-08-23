using CommandLine.NetCore.Services.CmdLine.Running;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command result
/// </summary>
public sealed class CommandResult : CommandLineResult
{
    /// <summary>
    /// list of parse errors (if any else empty)
    /// </summary>
    public List<string> ParseErrors { get; set; }

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
        : base(exitCode, result) => ParseErrors = parseErrors;

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code</param>
    public CommandResult(int exitCode)
        : base(exitCode) => ParseErrors = new List<string>();

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code</param>
    /// <param name="result">result</param>
    public CommandResult(int exitCode, object? result = null)
        : base(exitCode)
    {
        ParseErrors = new List<string>();
        Result = result;
    }
}
