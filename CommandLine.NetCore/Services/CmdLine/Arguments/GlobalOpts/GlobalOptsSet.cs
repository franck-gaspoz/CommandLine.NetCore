﻿using System.Diagnostics.CodeAnalysis;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.NetCore.Services.CmdLine.Arguments.GlobalOpts;

/// <summary>
/// global arguments set
/// </summary>
public sealed class GlobalOptsSet
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _opts = new();
    private readonly Texts _texts;
    private readonly Parser _parser;

    public IReadOnlyDictionary<string, Type> Opts
        => _opts;

    public GlobalOptsSet(
        IServiceProvider serviceProvider,
        AssemblySet assemblySet,
        Texts texts,
        Parser parser)
    {
        _parser = parser;
        _texts = texts;
        _serviceProvider = serviceProvider;
        foreach (var classType in GetGlobalOptTypes(assemblySet))
        {
            var orgName = Parser.ClassNameToOptName(
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
                    .Where(x => x.HasInterface(typeof(IGlobalOpt))
                        && !x.IsAbstract)
                    );
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
        out IOpt? opt)
    {
        opt = null;
        var optName = str;
        while (optName.StartsWith('-'))
            optName = optName[1..];
        if (_opts.TryGetValue(optName, out var classType))
        {
            opt = (IOpt)serviceProvider.GetRequiredService(classType);
            return true;
        }
        return false;
    }

    internal Dictionary<string, IOpt> Parse(
        CommandLineArgs commandLineArgs)
    {
        Dictionary<string, IOpt> res = new();
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
                _parser.ParseOptValues(
                    opt,
                    args,
                    index,
                    position
                    );
            }
            else
            {
                if (res.Any())
                    throw new ArgumentException(_texts._("GlobalOptionsMustBeAtEndOfTheCommandLine", res.First().Key));

                index++;
            }
            position++;
        }

        commandLineArgs.Replace(args);
        return res;
    }
}

