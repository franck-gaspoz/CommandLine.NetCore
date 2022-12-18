namespace CommandLine.NetCore.Extensions;

/// <summary>
/// String extensions
/// </summary>
public static class StringExt
{
    /// <summary>
    /// returns a string with first letter upper case
    /// </summary>
    /// <param name="text">text</param>
    /// <returns>text with first letter upper case</returns>
    public static string? ToFirstUpper(this string? text)
        => (text is null || text.Length == 0)
            ? text
            : char.ToUpperInvariant(text[0]) + text[1..];

}
