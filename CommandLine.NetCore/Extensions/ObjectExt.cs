namespace CommandLine.NetCore.Extensions;

/// <summary>
/// object extension
/// </summary>
internal static class ObjectExt
{
    /// <summary>
    /// returns the text representation of an object. if object if null returns "?"
    /// </summary>
    /// <param name="obj">object to translate to text</param>
    /// <param name="ifNullText">text if value is null</param>
    /// <returns></returns>
    public static string ToText(this object? obj, string ifNullText = "?")
        => obj is null ? ifNullText : "'" + obj.ToString()! + "'";
}
