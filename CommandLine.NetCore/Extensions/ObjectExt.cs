namespace CommandLine.NetCore.Extensions;

/// <summary>
/// object extension
/// </summary>
internal static class ObjectExt
{
    /// <summary>
    /// returns the text representation of an object. if object if null returns "?"
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToText(this object? obj)
        => obj is null ? "?" : "'" + obj.ToString()! + "'";
}
