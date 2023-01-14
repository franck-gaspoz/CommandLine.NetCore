namespace CommandLine.NetCore.Extensions;

/// <summary>
/// String extensions
/// </summary>
static class StringExt
{
    /// <summary>
    /// indicates if the string contains at least one of the characters
    /// </summary>
    /// <param name="s">string</param>
    /// <param name="chars">chars list</param>
    /// <returns>true or false</returns>
    public static bool Contains(this string s, List<char> chars)
    {
        foreach (var c in chars)
            if (s.Contains(c)) return true;
        return false;
    }

    /// <summary>
    /// returns a string with first letter upper case
    /// </summary>
    /// <param name="text">text</param>
    /// <returns>text with first letter upper case</returns>
    public static string? ToFirstUpper(this string? text)
        => (text is null || text.Length == 0)
            ? text
            : char.ToUpperInvariant(text[0]) + text[1..];

    /// <summary>
    /// transform a PascalCase or a camelCase text to a kebab-case text
    /// </summary>
    /// <param name="text">text to be kebabized</param>
    /// <returns>kebabized text</returns>
    public static string? ToKebabCase(this string? text)
    {
        if (text == null) return null;
        var arr = text.ToCharArray();
        List<char> chars = new();
        for (var i = 0; i < arr.Length; i++)
        {
            var c = arr[i];
            if (char.IsUpper(c) && i > 0)
                chars.Add('-');
            chars.Add(char.ToLowerInvariant(c));
        }
        return new string(chars.ToArray());
    }

    /// <summary>
    /// transforms \ to \\
    /// </summary>
    /// <param name="s">string</param>
    /// <returns></returns>
    public static string Unslash(this string s)
        => s.Replace(@"\", @"\\");

    /// <summary>
    /// split a text that is not unslashed
    /// </summary>
    /// <param name="s">text to split</param>
    /// <param name="c">split character</param>
    /// <returns>text parts</returns>
    public static List<string> SplitNotUnslashed(this string s, char c)
    {
        var r = new List<string>();
        var j = 0;
        for (var i = 0; i < s.Length; i++)
        {
            if ((s[i] == c) && i > 0 && s[i] - 1 != '\\')
            {
                r.Add(new string(s.AsSpan()[j..i]));
                j = i + 1;
            }
            else if ((s[i] == c) && i == 0)
            {
                r.Add("");
                j = i + 1;
            }
        }

        if (j < s.Length) r.Add(new string(s.AsSpan()[j..]));
        return r;
    }

    /// <summary>
    /// split a text that is not unslashed with a set of characters
    /// </summary>
    /// <param name="s">text to be splited</param>
    /// <param name="chars">set of split characters</param>
    /// <returns>text parts</returns>
    public static List<string> SplitByPrefixsNotUnslashed(this string s, List<char> chars)
    {
        var r = new List<string>();
        var j = 0;
        bool matchsep;
        s = new string(s.Reverse().ToArray());
        for (var i = 0; i < s.Length; i++)
        {
            if ((matchsep = chars.Contains(s[i])) && i > 0 && s[i] - 1 != '\\')
            {
                r.Add(new string(s.Substring(j, i - j + 1)));
                j = i + 1;
            }
            else if ((matchsep = chars.Contains(s[i])) && i == 0)
            {
                r.Add("");
                j = i + 1;
            }
        }

        if (j < s.Length) r.Add(new string(s.AsSpan()[j..]));
        return r.Select(x => new string(x.Reverse().ToArray())).ToList();
    }
}
