using System.Runtime.CompilerServices;

using AnsiVtConsole.NetCore;

using CommandLine.NetCore.Services.CmdLine.Settings;

using Microsoft.Extensions.Logging;

namespace CommandLine.NetCore.Services;

/// <summary>
/// command line core logger
/// </summary>
public sealed class CoreLogger
{
    /// <summary>
    /// console
    /// </summary>
    readonly IAnsiVtConsole _console;


    /// <summary>
    /// global settings
    /// </summary>
    readonly GlobalSettings _globalSettings;

    /// <summary>
    /// build a new instance of the core logger
    /// </summary>
    /// <param name="console">console</param>
    /// <param name="globalSettings">global settings</param>
    public CoreLogger(
        IAnsiVtConsole console,
        GlobalSettings globalSettings)
        => (_console, _globalSettings)
            = (console, globalSettings);

    /// <summary>
    /// log
    /// </summary>
    /// <param name="message">message</param>
    /// <param name="callerMemberName">caller member name</param>
    /// <param name="callerFilePath">caller file path</param>
    /// <param name="callerLineNumber">caller line number</param>
    /// <param name="messageOnly">if true only log message, ignore meta data</param>
    public void Log(
        string message,
        [CallerMemberName] string? callerMemberName = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int? callerLineNumber = null,
        bool messageOnly = true)
            => OutputLog(
                LogLevel.Information,
                message,
                callerMemberName ?? string.Empty,
                callerFilePath ?? string.Empty,
                callerLineNumber ?? -1,
                messageOnly);

    void OutputLog(
        LogLevel logLevel,
        string message,
        string callerMemberName,
        string callerFilePath,
        int callerLineNumber,
        bool messageOnly)
    {
        var txt =
            messageOnly ? message
            : $"[{logLevel}][{callerFilePath}:{callerLineNumber}][{callerMemberName}][{message}]";
        switch (logLevel)
        {
            case LogLevel.Information:
            case LogLevel.Debug:
            case LogLevel.Trace:
            case LogLevel.None:
                _console.Logger.Log(txt);
                break;
            case LogLevel.Warning:
                _console.Logger.LogWarning(txt);
                break;
            case LogLevel.Error:
            case LogLevel.Critical:
                _console.Logger.LogError(txt);
                break;
        }
    }
}
