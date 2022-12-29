using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace CommandLine.NetCore.Extensions;

/// <summary>
/// lambda expression extensions
/// </summary>
internal static class LambdaExpressionExt
{
    /// <summary>
    /// get a method info and the target object of a method call from a lambda unary call expression:
    /// <para>() => methodName</para>
    /// </summary>
    /// <param name="expression">lambda expression</param>
    /// <returns>the method invoked by the lambda expression if any, else null and the object target of the call</returns>
    public static (MethodInfo? methodInfo, object? target) GetAnyCastDelegate(this LambdaExpression expression)
    {
        (MethodInfo?, object?) nullResult = (null, null);

        if (expression.Body is not UnaryExpression unaryExpression) return nullResult;

        var operandArgumentsField = unaryExpression
            .Operand
            .GetFieldsAndProperties()
            .FirstOrDefault(x => x.Name == "Arguments");

        if (operandArgumentsField is null) return nullResult;

        if (operandArgumentsField
            .GetMemberValue(unaryExpression.Operand, false)
                is not ReadOnlyCollection<Expression> operandArguments
                    || operandArguments.Count < 2)
        {
            return nullResult;
        }

        if (operandArguments[1] is not ConstantExpression targetExpression) return nullResult;

        var target = targetExpression.Value;

        var operandObjectField = unaryExpression
            .Operand
            .GetFieldsAndProperties()
            .FirstOrDefault(x => x.Name == "Object");

        if (operandObjectField is null) return nullResult;

        var operandObject = operandObjectField
            .GetMemberValue(unaryExpression.Operand, false);

        if (operandObject is null) return nullResult;

        var valueField = operandObject
            .GetFieldsAndProperties()
            .FirstOrDefault(x => x.Name == "Value");

        if (valueField is null) return nullResult;

        var value = valueField
            .GetMemberValue(operandObject, false);

        if (value is not MethodInfo methodInfo) return nullResult;

        return (methodInfo, target);
    }
}
