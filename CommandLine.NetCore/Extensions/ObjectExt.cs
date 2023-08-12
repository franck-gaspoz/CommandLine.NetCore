using System.Dynamic;
using System.Reflection;

namespace CommandLine.NetCore.Extensions;

/// <summary>
/// object extensions
/// </summary>
static class ObjectExt
{
    /// <summary>
    /// transform an object (such an anonymous type) to an expando object
    /// <para>copy public properties of the source in the target</para>
    /// </summary>
    /// <param name="obj">source object</param>
    /// <returns>expando</returns>
    public static ExpandoObject ToExpando(this object? obj)
    {
        var r = new ExpandoObject();
        if (obj is not null)
            foreach (var prop in obj.GetType()
                .GetProperties())
                r.TryAdd(
                    prop.Name,
                    prop.GetValue(obj, null));
        return r;
    }

    /// <summary>
    /// transform an object (such an anonymous type) to an expando object, and add to its the given properties
    /// </summary>
    /// <param name="obj">source object</param>
    /// <param name="properties">properties to be added</param>
    /// <returns></returns>
    public static ExpandoObject Add(
        this object? obj,
        params (string Name, object Value)[] properties)
    {
        var r = obj?.ToExpando() ?? new ExpandoObject();
        foreach (var (Name, Value) in properties)
            r.TryAdd(Name, Value);
        return r;
    }

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
