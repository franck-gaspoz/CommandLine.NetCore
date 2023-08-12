using System.Diagnostics;
using System.Reflection;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.Text;

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

    /// <summary>
    /// build an exception from the error
    /// </summary>
    /// <param name="texts">texts provider</param>
    /// <returns>exception</returns>
    public Exception ToException(Texts texts)
    {
        var data = Data.ToExpando();
        List<object?> datas = new();
        if (DataTextMap is not null)
            foreach (var dataName in DataTextMap.DataName)
                datas.Add(
                    data.TryGet(dataName));

        var message =
            texts.IsDefined(Code) ?
                texts._(
                    Code,
                    datas.ToArray()) : Code;

        var exception = new Exception(message);
        return exception;
    }
}
