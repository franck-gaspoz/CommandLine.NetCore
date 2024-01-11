
using System.Collections;
using System.Reflection;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Arguments.Parsing;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Running.Exceptions;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// a syntax dispatch map : do methods
/// </summary>
public sealed partial class SyntaxExecutionDispatchMapItem
{
    SyntaxMatcherDispatcher DoAction(Delegate action)
    {
        var methodInfo = action.GetMethodInfo();

        ValidateAction(action);

        return DoMethod(
            action.ToString() ?? string.Empty,
            Syntax.Name ?? string.Empty,
            methodInfo,
            null,
            action);
    }

    SyntaxMatcherDispatcher DoMethod(
        string expression,
        string methodName,
        MethodInfo methodInfo,
        object? target,
        Delegate? action = null)
    {
        var error = () => GetSyntaxError(expression);

        Name = methodName;
        Syntax.SetName(Name);
        Delegate = (CommandContext context) =>
        {
            var callParameters = new List<object?>();
            int argIndex;
            var currentParamIndex = -1;

            foreach (var parameter in methodInfo.GetParameters())
            {
                // anywhere command context argument
                if (parameter.ParameterType == typeof(CommandContext))
                {
                    callParameters.Add(
                        new CommandContext(
                            SyntaxMatcherDispatcher.GlobalSettings,
                            SyntaxMatcherDispatcher.Console,
                            SyntaxMatcherDispatcher.Texts,
                            Syntax,
                            SyntaxMatcherDispatcher.OptSet,
                            SyntaxMatcherDispatcher
                            ));
                }
                else
                {
                    int GetParamIndex(int currentParamIndex, int argSkipCount = 0)
                    {
                        // mapped arguments
                        // auto mapped arguments (index mapping)
                        if (parameter
                            .GetCustomAttributes(false)
                            .Where(x => x.GetType() == typeof(MapArgAttribute))
                            .FirstOrDefault() is not MapArgAttribute mapArg)
                        {
                            return
                                Syntax.GetIndexOfArgWithExpectedValueFromIndex(
                                    currentParamIndex + 1 + argSkipCount);
                        }
                        else
                        {
                            // mapped explicitly
                            return mapArg.ArgIndex;
                        }
                    }

                    currentParamIndex = argIndex = GetParamIndex(currentParamIndex);

                    var arg = Syntax[argIndex];

                    if (arg is not Flag
                        && arg is IOpt opt
                        && opt.ExpectedValuesCount == 0
                        && !opt.IsOptional)
                    // skip arg, parameter not required
                    {
                        currentParamIndex = argIndex = GetParamIndex(currentParamIndex);
                        arg = Syntax[argIndex];
                    }

                    var argType = arg.GetType();
                    var targetIsArg = parameter!.ParameterType.HasInterface(typeof(IArg));
                    var isOptional = arg.GetIsOptional();
                    var isTargetNullable = parameter.IsNullable();

                    var isTargetNullableRequired =
                        isOptional
                        && arg is not Flag
                        && arg is IOpt;

                    var isSet = arg.GetIsSet();
                    var argValueType = arg.ValueType;
                    var useArgIsSetValue = false;
                    var mapToArray = false;

                    if (arg is not Flag
                        && arg is IOpt opt2
                        && opt2.ExpectedValuesCount == 0
                        && isOptional)
                    {
                        // an optional opt with no expected value acts as a flag wathever its type definition
                        isOptional = false;
                        isTargetNullableRequired = false;
                        argValueType = typeof(bool);
                        useArgIsSetValue = true;
                    }

                    var avoidCollection = arg is not Flag
                        && arg is IOpt opt3
                        && opt3.ExpectedValuesCount == 1;

                    #region errors

                    void ThrowInvalidCommandOperationParameterCastException()
                        => throw new InvalidCommandOperationParameterCastException(
                                currentParamIndex,
                                arg,
                                parameter,
                                error());

                    void ThrowInvalidCommandOperationParameterNullabilityExpectedException()
                        => throw new InvalidCommandOperationParameterNullabilityExpectedException(
                                currentParamIndex,
                                arg,
                                parameter,
                                error());

                    void ThrowInvalidCommandOperationParameterNullabilityNotExpectedException()
                        => throw new InvalidCommandOperationParameterNullabilityNotExpectedException(
                                currentParamIndex,
                                arg,
                                parameter,
                                error());

                    #endregion

                    if (targetIsArg && parameter.ParameterType == argType)
                    {
                        // argument type == specification type (Opt,Param,..)
                        callParameters.Add(arg);
                    }
                    else
                    {
                        // concrete argument type mappings

                        #region unmatchable target cases

                        if (targetIsArg)
                            ThrowInvalidCommandOperationParameterCastException();

                        if (!isTargetNullableRequired && isTargetNullable)
                            ThrowInvalidCommandOperationParameterNullabilityNotExpectedException();

                        if (isTargetNullableRequired && !isTargetNullable)
                            ThrowInvalidCommandOperationParameterNullabilityExpectedException();

                        #endregion

                        if (((!isOptional || !isTargetNullableRequired) && parameter.ParameterType == argValueType)
                            || (isOptional && isTargetNullableRequired
                                 && (isTargetNullable
                                    || (parameter.ParameterType.GenericTypeArguments.Any()
                                        && parameter.ParameterType.GenericTypeArguments[0] == argValueType)))
                            || (mapToArray = IsArgumentIsArrayAndParameterIsList(parameter, arg)))
                        {
                            // argument type == value type (direct type mapping: string,bool,...)
                            callParameters.Add(
                                useArgIsSetValue ?
                                    isSet
                                    : avoidCollection ?
                                        isSet ? ((IList)arg.GetValue()!)[0] : null
                                        : !mapToArray ?
                                            arg.GetValue()
                                            : ((IOpt)arg).GetValueArray());
                        }
                        else
                        {
                            if (!avoidCollection
                                && parameter.ParameterType.GenericTypeArguments.Length == 1
                                && parameter.ParameterType.GenericTypeArguments[0] == argValueType)
                            {
                                // generic argument type == value type (direct type mapping for collection)
                                // and Nullable types
                                callParameters.Add(isSet ? arg.GetValue() : null);
                            }
                            else
                                // parameter type mismatch
                                ThrowInvalidCommandOperationParameterCastException();
                        }
                    }
                }
            }

            if (action is null)
                methodInfo.Invoke(target, callParameters.ToArray());
            else
                action!.DynamicInvoke(callParameters.ToArray());

            return new();
        };

        return SyntaxMatcherDispatcher;
    }
}
