using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.GlobalArgs;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine;

internal class GlobalArgsSet
{
    private readonly IServiceProvider _serviceProvider;

    protected readonly Dictionary<string, Type> _args = new();

    public IReadOnlyDictionary<string, Type> Args
        => _args;

    public GlobalArgsSet(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        foreach (var classType in GetGlobalArgTypes())
        {
            var argName = GlobalArg.ClassNameToArgName(
                classType.Name);
            Add(argName, classType);
        }
    }

    public static IEnumerable<Type> GetGlobalArgTypes()
        => Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.InheritsFrom(typeof(GlobalArg)));

    private void Add(
        string name,
        Type argType)
        => _args.Add(name, argType);

    private bool TryBuild(
        IServiceProvider serviceProvider,
        string str,
        [NotNullWhen(true)]
        out Arg? arg)
    {
        arg = null;
        var argName = str;
        while (argName.StartsWith('-'))
            argName = argName[1..];
        if (_args.TryGetValue(argName, out var classType))
        {
            arg = (Arg)serviceProvider.GetRequiredService(classType);
            return true;
        }
        return false;
    }

    public Dictionary<string, Arg> Parse(
        CommandLineArgs commandLineArgs)
    {
        Dictionary<string, Arg> res = new();
        var index = 0;
        var position = 0;
        var args = commandLineArgs.Args.ToList();
        while (index < args.Count)
        {
            var str = args[index];
            if (TryBuild(
                _serviceProvider,
                str,
                out var arg))
            {
                res.Add(str, arg);
                arg.ParseParameters(args, index, position);
            }
            else
            {
                index++;
            }
            position++;
        }
        return res;
    }

    public static bool ExistsInArgList(
        Type argType,
        List<string> args) =>
            argType.InheritsFrom(typeof(GlobalArg))
            && args.Contains(
                GlobalArg.GetPrefixFromClassName(argType.Name)
                + GlobalArg.ClassNameToArgName(argType.Name));
}

