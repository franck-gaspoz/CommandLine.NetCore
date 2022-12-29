using System.Linq.Expressions;
using System.Reflection;

namespace CommandLine.NetCore.Extensions;

/// <summary>
/// lambda expression extensions
/// </summary>
internal static class LambdaExpressionExt
{
    /// <summary>
    /// get a method info from a lambda unary call expression:
    /// <para>() => methodName</para>
    /// </summary>
    /// <param name="expression">lambda expression</param>
    /// <returns>the method invoked by the lambda expression if any, else null</returns>
    public static MethodInfo? GetIndirectMethodInfo(this LambdaExpression expression)
    {
        if (expression.Body is UnaryExpression unaryExpression)
        {
            var operandObjectField = unaryExpression
                .Operand
                .GetFieldsAndProperties()
                .FirstOrDefault(x => x.Name == "Object");
            if (operandObjectField is null) return null;
            var operandObject = operandObjectField.GetMemberValue(unaryExpression.Operand, false);
            if (operandObject is null) return null;
            var valueField = operandObject.GetFieldsAndProperties()
                .FirstOrDefault(x => x.Name == "Value");
            if (valueField is null) return null;
            var value = valueField.GetMemberValue(operandObject, false);
            if (value is not MethodInfo methodInfo) return null;
            return methodInfo;
        }
        return null;
    }
}
