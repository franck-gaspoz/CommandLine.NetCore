using System.Diagnostics.CodeAnalysis;

using CommandLine.NetCore.Extensions;
using CommandLine.NetCore.Services.CmdLine.Parsing;
using CommandLine.NetCore.Services.CmdLine.Settings;
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

    /// <summary>
    /// options
    /// </summary>
    public IReadOnlyDictionary<string, Type> Opts
        => _opts;

    /// <summary>
    /// set of global options
    /// </summary>
    /// <param name="serviceProvider">service provider</param>
    /// <param name="assemblySet">assembly set</param>
    /// <param name="texts">texts</param>
    /// <param name="parser">parser</param>
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

    /// <summary>
    /// get global options types
    /// </summary>
    /// <param name="assemblySet">assemblies where to look up</param>
    /// <returns>types of global options classes</returns>
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

    internal Dictionary<string, (IOpt, List<string>)> Parse(
        CommandLineArgs commandLineArgs)
    {
        Dictionary<string, (IOpt, List<string>)> res = new();
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

                _parser.ParseOptValues(
                    opt,
                    args,
                    index,
                    position,
                    out var optArgs
                    );
                res.Add(str, (opt, optArgs));
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

