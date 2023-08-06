using System.Reflection;

namespace CommandLine.NetCore.Extensions;

/// <summary>
/// reflexion extensions
/// </summary>
static class ReflectionExt
{
    const string NullableAttributePartialName = "NullableAttribute";

    /// <summary>
    /// indicates if the parameter seems to be declared nullable based on type name and parameter attributes
    /// </summary>
    /// <param name="parameter">parameter</param>
    /// <returns>true if nullable, false otherwise</returns>
    public static bool IsNullable(this ParameterInfo parameter)
        => parameter.ParameterType
            .IsExplicitNullable()
                || parameter.CustomAttributes.
                    Any(x => x.AttributeType
                        .Name
                        .Contains(NullableAttributePartialName));

    /// <summary>
    /// returns a description of the parameter
    /// </summary>
    /// <param name="parameter">parameter</param>
    /// <returns>descritpion text</returns>
    public static string ToText(this ParameterInfo parameter)
        => $"{parameter.ParameterType.UnmangledName()}{(parameter.IsNullable() ? "?" : "")} {parameter.Name}";
}
