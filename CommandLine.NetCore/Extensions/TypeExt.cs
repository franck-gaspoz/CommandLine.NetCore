using System.Reflection;

namespace CommandLine.NetCore.Extensions;

/// <summary>
/// Type extensions
/// </summary>
static class TypeExt
{
    const string NullableTypeFullNamePrefix = "System.Nullable`";
    const string NullableAttributePartialName = "NullableAttribute";
    const string NullableContextAttributePartialName = "NullableContextAttribute";

    /// <summary>
    /// indicates if a type inherits from another type. returns false if type is base type
    /// </summary>
    /// <param name="type">type to check</param>
    /// <param name="baseType">expected base type</param>
    /// <returns>true if type is a sub type of base type</returns>
    public static bool InheritsFrom(this Type? type, Type? baseType)
    {
        if (baseType == null)
            return false;
        if (type == baseType)
            return false;
        type = type!.BaseType;
        while (type != null && type != typeof(object))
        {
            if (type == baseType)
                return true;
            type = type.BaseType;
        }
        return false;
    }

    /// <summary>
    /// unmangled name of a type
    /// </summary>
    /// <param name="type">type</param>
    /// <returns>unmangled name of the type</returns>
    public static string UnmangledName(this Type type) => TypesManglingExt.FriendlyName(type);

    public static bool IsOrInheritsFrom(this Type type, Type refType)
        => type == refType || type.InheritsFrom(refType);

    /// <summary>
    /// types an type inherits from
    /// </summary>
    /// <param name="type">type</param>
    /// <param name="includeRoot">if true, root type (should be 'object') is also included in results</param>
    /// <returns>list of types the given type inherits from</returns>
    public static List<Type> GetInheritanceChain(
        this Type? type,
        bool includeRoot = true)
    {
        var r = new List<Type>();
        if (type is null)
            return r;
        if (includeRoot)
            r.Add(type);
        type = type.BaseType;
        // walk inheritance chain
        while (type != null)
        {
            if (type != typeof(object))
                r.Add(type);
            type = type.BaseType;
        }
        return r;
    }

    /// <summary>
    /// indicates if a type implements an interface
    /// </summary>
    /// <param name="type">checked type</param>
    /// <param name="interfaceType">interface type</param>
    /// <returns>true if type implements interface, false otherwise</returns>
    public static bool HasInterface(this Type type, Type interfaceType)
        => type.GetInterface(interfaceType.FullName!) != null;

    /// <summary>
    /// indicates if a type is specifed to be nullable
    /// </summary>
    /// <param name="type">type</param>
    /// <returns>true if nullable, false otherwise</returns>
    public static bool IsExplicitNullable(this Type type)
        => type.FullName?.StartsWith(NullableTypeFullNamePrefix) ?? false;

    /*=> (type.FullName?.StartsWith(NullableTypeFullNamePrefix) ?? false)
    || type.CustomAttributes.Any(x =>
        x.AttributeType.Name.Contains(NullableAttributePartialName)
        || x.AttributeType.Name.Contains(NullableContextAttributePartialName));*/

    /// <summary>
    /// fields and properties of an object
    /// </summary>
    /// <param name="o">object</param>
    /// <returns>list of MemberInfo</returns>
    public static List<MemberInfo> GetFieldsAndProperties(this object o)
    {
        var t = o.GetType();
        var r = new List<MemberInfo>();
        foreach (var f in t.GetFields())
        {
            r.Add(f);
        }
        foreach (var p in t.GetProperties())
        {
            r.Add(p);
        }
        return r;
    }

    public static object? GetMemberValue(this MemberInfo mi, object obj, bool throwException = true)
    {
        if (mi is FieldInfo f)
            return f.GetValue(obj);
        if (mi is PropertyInfo p)
        {
            var getGetMethod = p.GetGetMethod();
            if (getGetMethod is not null && getGetMethod!.GetParameters().Length == 0)
            {
                return p.GetValue(obj);
            }
            else
            {
                if (throwException)
                    // indexed property
                    throw new ArgumentException($"can't get value of indexed property: '{p.Name}'");
            }
        }
        return null;
    }

    /// <summary>
    /// values of object members
    /// </summary>
    /// <param name="o">object</param>
    /// <returns>list of members names,value and MemberInfo</returns>
    public static
        List<(string name, object? value, MemberInfo memberInfo)>
        GetMemberValues(this object o)
    {
        var t = o.GetType();
        var array = new List<(string, object?, MemberInfo)>();
        foreach (var f in t.GetFields())
        {
            array.Add((f.Name, f.GetMemberValue(o), f));
        }
        foreach (var p in t.GetProperties())
        {
            try
            {
                var val = p.GetMemberValue(o, true);
                array.Add((p.Name, p.GetValue(o), p));
            }
            catch (ArgumentException)
            {
                array.Add((p.Name, "indexed property", p));
            }
        }
        array.Sort(new Comparison<(string, object?, MemberInfo)>(
            (a, b) => a.Item1.CompareTo(b.Item1)));
        return array;
    }

    /// <summary>
    /// type of a member value
    /// </summary>
    /// <param name="memberInfo">member info</param>
    /// <returns>type of the member value</returns>
    public static Type GetMemberValueType(this MemberInfo memberInfo)
    {
        if (memberInfo is FieldInfo field) return field.FieldType;
        if (memberInfo is PropertyInfo prop) return prop.PropertyType;
        return memberInfo.DeclaringType!;
    }
}

