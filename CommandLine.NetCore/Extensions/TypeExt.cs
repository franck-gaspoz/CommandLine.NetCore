namespace CommandLine.NetCore.Extensions;

/// <summary>
/// Type extensions
/// </summary>
internal static class TypeExt
{
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
}

