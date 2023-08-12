using System.Diagnostics;
using System.Reflection;

namespace CommandLine.NetCore.Services.Error;

/// <summary>
/// an error occuring at initialization time before the app host is built
/// </summary>
[DebuggerDisplay("{Code} ({CallerMemberName})")]
public sealed class ErrorDescriptor
{
    #region properties

    /// <summary>
    /// error code
    /// <para>match a Text key</para>
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// data related to the error
    /// </summary>
    public object Data { get; private set; }

    /// <summary>
    /// method at the origin of error
    /// </summary>
    public MethodInfo? Method { get; private set; }

    /// <summary>
    /// funcs that returns data names of parameter number in error text (if any)
    /// </summary>
    public ErrorDataTextMap? DataTextMap { get; private set; }

    /// <summary>
    /// caller member name
    /// </summary>
    public string? CallerMemberName { get; private set; }

    #endregion

    /// <summary>
    /// creates a new initialization error
    /// </summary>
    /// <param name="code">code</param>
    /// <param name="data">data</param>
    /// <param name="method">method</param>
    /// <param name="dataTextMap">funcs that returns data names to parameter number in error text (if any)</param>
    /// <param name="callerMemberName">caller member name</param>
    public ErrorDescriptor(
        string code,
        object data,
        MethodInfo? method = null,
        ErrorDataTextMap? dataTextMap = null,
        string? callerMemberName = null)
    {
        Code = code;
        Data = data;
        Method = method;
        DataTextMap = dataTextMap;
        CallerMemberName = callerMemberName;
    }
}
