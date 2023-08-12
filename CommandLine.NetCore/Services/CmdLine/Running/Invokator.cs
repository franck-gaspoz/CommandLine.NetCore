using System.Reflection;

namespace CommandLine.NetCore.Services.CmdLine.Running;

/// <summary>
/// invoke a method or an action
/// </summary>
sealed class Invokator
{
    readonly MethodInfo _methodInfo;
    readonly Action? _action;
    readonly object? _target;

    public Invokator(
        MethodInfo methodInfo,
        Action? action,
        object? target)
    {
        _methodInfo = methodInfo;
        _action = action;
        _target = target;
        if (action is null && target is null)
            throw new InvalidOperationException();
    }

    public void Invoke(object?[]? parameters)
    {
        if (_action is null)
            _methodInfo.Invoke(_target, parameters);
        else
            _methodInfo.Invoke(_action, parameters);
    }
}
