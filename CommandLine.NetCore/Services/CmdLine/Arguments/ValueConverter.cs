using System.Collections;
using System.Reflection;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.Text;

namespace CommandLine.NetCore.Services.CmdLine.Arguments;

/// <summary>
/// value converter
/// <para>this code is part of my project OrbitalShell https://github.com/OrbitalShell/Orbital-Shell</para>
/// </summary>
public sealed class ValueConverter
{
    private readonly Texts _texts;

    /// <summary>
    /// value converter
    /// <para>this code is part of my project OrbitalShell https://github.com/OrbitalShell/Orbital-Shell</para>
    /// </summary>
    /// <param name="texts">texts</param>
    public ValueConverter(Texts texts)
        => _texts = texts;

    /// <summary>
    /// try to convert a text representation to a typed valued according to a type specification
    /// </summary>
    /// <param name="ovalue">object (null,string) to translate to object real type</param>
    /// <param name="ptype">real type expected</param>
    /// <param name="defaultValue">default value used on type value instantiation default value. not used if null</param>
    /// <param name="convertedValue">value converted to real type expected</param>
    /// <param name="possibleValues">in case of fail, message indicating possible values for the expected type</param>
    /// <param name="fallBackType">if given value if not a string, try to downcast to this type with precision lost if allowed</param>
    /// <param name="defaultReturnIdentityOk">return identity if all conversion failed if true</param>
    /// <param name="allowPrecisionLost">allows numeric values precision lost</param>
    /// <returns>true if success, false otherwise</returns>
    public bool ToTypedValue(
        object ovalue,
        Type ptype,
        object? defaultValue,
        out object? convertedValue,
        out List<object>? possibleValues,
        Type? fallBackType = null,
        bool defaultReturnIdentityOk = false,
        bool allowPrecisionLost = true
        )
    {
        convertedValue = null;
        possibleValues = null;
        if (ovalue == null) return false;

        var result = false;
        var found = false;
        possibleValues = null;
        var interfaces = ptype.GetInterfaces();

        var h = ptype.GetInheritanceChain();

        if (ptype.HasInterface(typeof(ICollection)) && ovalue is string s)
        {
            var genArgs = ptype.GenericTypeArguments;

            if (genArgs.Length > 1)
            {
                throw new ArgumentException(
                    _texts._("GenericTypeWithMoreThanOneArgumentNotSupported", ptype.UnmangledName()));
            }

            var argType = genArgs[0];
            var lst = Activator.CreateInstance(ptype);
            var met = ptype.GetMethod("Add");

            if (met == null)
            {
                throw new ArgumentException(
                    _texts._("CollectionTypeHasNoMethodAdd", ptype.UnmangledName()));
            }

            var values = s.SplitNotUnslashed(Globals.ParameterTypeListValuesSeparator);
            foreach (var val in values)
            {
                if (ToTypedValue(
                    val,
                    argType,
                    null,
                    out var convertedVal,
                    out var valPossibleValues,
                    fallBackType,
                    defaultReturnIdentityOk,
                    allowPrecisionLost))
                {
                    met.Invoke(lst, new object[] { convertedVal! });
                }
                else
                {
                    possibleValues = valPossibleValues;
                    return false;
                }
            }

            convertedValue = lst;
            return true;

        }
        else if (ptype.HasInterface(typeof(ICollection))
            && ovalue.GetType().HasInterface(typeof(ICollection)))
        {
            var genArgs = ptype.GenericTypeArguments;

            if (genArgs.Length > 1)
            {
                throw new ArgumentException(
                    _texts._("GenericTypeWithMoreThanOneArgumentNotSupported", ptype.UnmangledName()));
            }

            var argType = genArgs[0];
            var lst = Activator.CreateInstance(ptype);
            var met = ptype.GetMethod("Add");

            if (met == null)
            {
                throw new ArgumentException(
                    _texts._("CollectionTypeHasNoMethodAdd", ptype.UnmangledName()));
            }

            var values = ((ICollection)ovalue).GetEnumerator();
            while (values.MoveNext())
            {
                var val = values.Current;
                if (ToTypedValue(
                    val,
                    argType,
                    null,
                    out var convertedVal,
                    out var valPossibleValues,
                    fallBackType,
                    defaultReturnIdentityOk,
                    allowPrecisionLost))
                {
                    met.Invoke(lst, new object[] { convertedVal! });
                }
                else
                {
                    possibleValues = valPossibleValues;
                    return false;
                }
            }

            convertedValue = lst;
            return true;
        }
        else if (ptype.IsEnum && ovalue is string str)
        {
            if (ptype.GetCustomAttribute<FlagsAttribute>() != null
                && str.Contains(Globals.ParameterTypeFlagEnumValuePrefixs))
            {
                // flag enum Name
                var fvalues = str.SplitByPrefixsNotUnslashed(
                    Globals.ParameterTypeFlagEnumValuePrefixs);

                var flag = defaultValue ?? Activator.CreateInstance(ptype);

                foreach (var fval in fvalues)
                {
                    var val = fval[1..];
                    var flagEnabling = fval[0] == Globals.ParameterTypeFlagEnumValuePrefixEnabled;
                    if (ToTypedValue(
                        val,
                        ptype,
                        null,
                        out var convertedVal,
                        out var valPossibleValues,
                        fallBackType,
                        defaultReturnIdentityOk,
                        allowPrecisionLost))
                    {
                        if (flagEnabling)
                            flag = (int)flag! + (int)convertedVal!;
                        else
                            flag = (int)flag! & ~((int)convertedVal!);
                    }
                    else
                    {
                        possibleValues = valPossibleValues;
                        return false;
                    }
                }

                convertedValue = flag;
                return true;
            }
            else
            {
                // support for any Enum type (expr = single val)
                // support for flag Enum type and expr = val1,...,valn (result is: Enum val1 | .. | valn) - test with: val.HasFlag(flagval)
                // support for Enum type and expr = val1,...,valn (result is: int val1 + .. + valn) - test with: val.HasFlag(flagval)
                if (Enum.TryParse(ptype, str, false, out convertedValue))
                {
                    return true;
                }
                else
                {
                    possibleValues = Enum.GetNames(ptype).ToList<object>();
                    return false;
                }
            }
        }
        else
        {
            if (ovalue is string value)
            {
                if (ptype == typeof(int))
                {
                    result = int.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(short))
                {
                    result = short.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(int))
                {
                    result = int.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(long))
                {
                    result = long.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(ushort))
                {
                    result = ushort.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(uint))
                {
                    result = uint.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(ulong))
                {
                    result = ulong.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(short))
                {
                    result = short.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(long))
                {
                    result = long.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(double))
                {
                    result = double.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(float))
                {
                    result = float.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(decimal))
                {
                    result = decimal.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(string))
                {
                    result = true;
                    convertedValue = value;
                    found = true;
                }
                if (ptype == typeof(bool))
                {
                    result = bool.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(sbyte))
                {
                    result = sbyte.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(byte))
                {
                    result = byte.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(char))
                {
                    result = char.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(float))
                {
                    result = float.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
                if (ptype == typeof(DateTime))
                {
                    result = DateTime.TryParse(value, out var intv);
                    convertedValue = intv;
                    found = true;
                }
            }

            // unknown type, not CustomParameter: converted value = original value
            if (!found)
            {
                if (allowPrecisionLost)
                {
                    var valType = ovalue.GetType();
                    // try to decrease precision with lost
                    if (valType.IsValueType && valType.IsPrimitive)
                    {
                        if (valType.Name.Contains("64") && ptype.Name.Contains("32"))
                        {
                            if (ptype == typeof(int) && valType == typeof(long))
                            {
                                // this is a common case implied by json deserializer which set int to 64bits as default

                                unchecked
                                {
                                    var newVal = ((long)ovalue) & 0x00000000FFFFFFFFL;
                                    convertedValue = (int)newVal;
                                    if (!allowPrecisionLost && (long)convertedValue != (long)ovalue)
                                        convertedValue = null;
                                    return true;
                                }
                            }
                        }
                    }
                }

                result = defaultReturnIdentityOk;
                if (defaultReturnIdentityOk) convertedValue = ovalue;
            }
        }

        return result;
    }
}
