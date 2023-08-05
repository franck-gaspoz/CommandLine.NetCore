using System.Reflection;

namespace CommandLine.NetCore.Extensions;

/// <summary>
/// object extension
/// </summary>
static class ObjectExt
{
    /// <summary>
    /// indicate if an object is one of the given. compares by reference
    /// </summary>
    /// <param name="value">value to be compared</param>
    /// <param name="values">values to compare to</param>
    /// <returns></returns>
    public static bool IsOneOf(this Type value, params Type[] values)
        => values.Contains(value);

    /// <summary>
    /// returns the text representation of an object. if object if null returns "?"
    /// </summary>
    /// <param name="obj">object to translate to text</param>
    /// <param name="ifNullText">text if value is null</param>
    /// <returns></returns>
    public static string ToText(this object? obj, string ifNullText = "?")
        => obj is null ? ifNullText : "'" + obj.ToString()! + "'";

    /// <summary>
    /// surface clone
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    /// <param name="obj">object to clone</param>
    /// <returns>the clone</returns>
    public static T Clone<T>(this T obj) where T : new()
    {
        var cloneObj = new T();
        foreach (var member in typeof(T).GetMembers())
        {
            if (member is FieldInfo field && !field.IsInitOnly)
                field.SetValue(cloneObj, field.GetValue(obj));
            if (member is PropertyInfo prop && prop.CanWrite)
                prop.SetValue(cloneObj, prop.GetValue(obj));
        }
        return cloneObj;
    }
}
