﻿using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;

namespace CommandLine.NetCore.Services.CmdLine.Commands;

/// <summary>
/// command result
/// <para>can implicitly cast to int exit code</para>
/// </summary>
public sealed class CommandResult
{
    /// <summary>
    /// list of parse errors (if any else empty)
    /// </summary>
    public List<string> ParseErrors { get; set; }

    /// <summary>
    /// any exception
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// related syntax
    /// </summary>
    public Syntax? Syntax { get; }

    /// <summary>
    /// résult
    /// </summary>
    public object? Result { get; }

    /// <summary>
    /// an exit code
    /// </summary>
    public int ExitCode { get; set; }

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
        ParseErrors = parseErrors;
        ExitCode = exitCode;
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
    }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code</param>
    /// <param name="exception">exception</param>
    public CommandResult(int exitCode, Exception exception)
    {
        ExitCode = exitCode;
        ParseErrors = new List<string>();
        Exception = exception;
    }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code</param>
    /// <param name="syntax">syntax of the command</param>
    public CommandResult(int exitCode, Syntax syntax)
    {
        ExitCode = exitCode;
        ParseErrors = new List<string>();
        Syntax = syntax;
    }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code</param>
    /// <param name="exception">exception</param>
    /// <param name="syntax">syntax of the command</param>
    public CommandResult(int exitCode, Exception exception, Syntax syntax)
    {
        ExitCode = exitCode;
        ParseErrors = new List<string>();
        Exception = exception;
        Syntax = syntax;
    }

    /// <summary>
    /// build a new instance
    /// </summary>
    /// <param name="exitCode">exit code</param>
    /// <param name="result">result</param>
    public CommandResult(int exitCode, object? result = null)
    {
        ExitCode = exitCode;
        ParseErrors = new List<string>();
        Result = result;
    }

    /// <summary>
    /// convert the CommandResult to the int exit code
    /// </summary>
    /// <param name="commandResult">command result</param>
    /// <returns>exit code</returns>
    public static implicit operator int(CommandResult commandResult)
        => commandResult.ExitCode;
}
