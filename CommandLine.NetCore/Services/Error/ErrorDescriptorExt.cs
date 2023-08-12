using System.Reflection;
using System.Runtime.CompilerServices;

namespace CommandLine.NetCore.Services.Error;

/// <summary>
/// error descriptor extension
/// </summary>
static class ErrorDescriptorExt
{
    /// <summary>
    /// creates a new error descriptor for the caller object and member name, expected it is a method
    /// </summary>
    /// <param name="object">caller object</param>
    /// <param name="code">error code</param>
    /// <param name="data">data at source errpr</param>
    /// <param name="dataTextMap">func that returns data names to parameter number in error text (if any)</param>
    /// <param name="callerMemberName">caller member name, expected to be a method</param>
    /// <returns>error descriptor</returns>
    public static ErrorDescriptor CreateErrorDescriptor(
        this object @object,
        string code,
        object data,
        ErrorDataTextMap? dataTextMap = null,
        [CallerMemberName] string? callerMemberName = null)
    {
        ArgumentNullException.ThrowIfNull(@object);
        ArgumentNullException.ThrowIfNull(callerMemberName);

        return new ErrorDescriptor(
            code,
            data,
            @object.GetType()
                .GetMethod(
                    callerMemberName,
                    BindingFlags.Public | BindingFlags.NonPublic),
            dataTextMap,
            callerMemberName);
    }
}
