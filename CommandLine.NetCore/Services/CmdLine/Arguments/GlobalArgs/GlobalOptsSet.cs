using System.Diagnostics.CodeAnalysis;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Service.CmdLine.Arguments.GlobalArgs;

/// <summary>
/// global arguments set
/// </summary>
public sealed class GlobalOptsSet
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _opts = new();
    private readonly Texts _texts;

    public IReadOnlyDictionary<string, Type> Opts
        => _opts;

    public GlobalOptsSet(
        IServiceProvider serviceProvider,
        AssemblySet assemblySet,
        Texts texts)
    {
        _texts = texts;
        _serviceProvider = serviceProvider;
        foreach (var classType in GetGlobalOptTypes(assemblySet))
        {
            var orgName = Opt.ClassNameToOptName(
                classType.Name);
            Add(orgName, classType);
        }
    }

    public static IEnumerable<Type> GetGlobalOptTypes(AssemblySet assemblySet)
    {
        var globalOptTypes = new List<Type>();
        foreach (var assembly in assemblySet.Assemblies)
        {
            globalOptTypes
                .AddRange(
                    assembly
                    .GetTypes()
                    .Where(x => x.InheritsFrom(typeof(Opt))
                        && x.Name.EndsWith(Globals.GlobalArgPostFix)
                    ));
        }
        return globalOptTypes;
    }

    private void Add(
        string name,
        Type optType)
        => _opts.Add(name, optType);

    private bool TryBuild(
        IServiceProvider serviceProvider,
        string str,
        [NotNullWhen(true)]
        out Opt? opt)
    {
        opt = null;
        var optName = str;
        while (optName.StartsWith('-'))
            optName = optName[1..];
        if (_opts.TryGetValue(optName, out var classType))
        {
            opt = (Opt)serviceProvider.GetRequiredService(classType);
            return true;
        }
        return false;
    }

    public Dictionary<string, Opt> Parse(
        CommandLineArgs commandLineArgs)
    {
        Dictionary<string, Opt> res = new();
        var index = 0;
        var position = 0;
        var args = commandLineArgs.Args.ToList();
        while (index < args.Count)
        {
            var str = args[index];
            if (TryBuild(
                _serviceProvider,
                str,
                out var opt))
            {
                if (res.ContainsKey(str))
                    throw new ArgumentException(_texts._("DuplicatedOption", str));

                res.Add(str, opt);
                opt.ParseValues(args, index, position);
            }
            else
            {
                index++;
            }
            position++;
        }

        if (position != commandLineArgs.Count)
            throw new ArgumentException(_texts._("GlobalOptionsMustBeAtEndOfTheCommandLine"));

        commandLineArgs.Replace(args);
        return res;
    }
}

