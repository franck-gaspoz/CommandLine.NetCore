﻿using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.Text;

using Microsoft.Extensions.Configuration;

using static CommandLine.NetCore.Services.CmdLine.Globals;

namespace CommandLine.NetCore.Service.CmdLine.Arguments;

/// <summary>
/// a command line option : -name [value1 [.. value n], --name [value1 [.. value n]
/// </summary>
public class Opt : Arg
{
    /// <summary>
    /// values count (expected)
    /// </summary>
    public int ValuesCount { get; private set; }

    /// <summary>
    /// unparsed values
    /// </summary>
    protected readonly List<string> Values = new();

    /// <summary>
    /// argument name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// build a new argument
    /// <para>corresponds to the syntax:</para>
    /// <para>-argName | --argName [value1 [.. valuen]]</para>
    /// </summary>
    /// <param name="name">argument name</param>
    /// <param name="config">app config</param>
    /// <param name="texts">texts</param>
    /// <param name="valueConverter">value converter</param>
    /// <param name="valuesCount">number of expected values</param>
    public Opt(
        string name,
        IConfiguration config,
        Texts texts,
        ValueConverter valueConverter,
        int valuesCount = 0)
        : base(config, texts, valueConverter)
    {
        Name = name;
        ValuesCount = valuesCount;
    }

    /// <summary>
    /// get value at index
    /// </summary>
    /// <param name="index">value index (from 0)</param>
    /// <returns>value at index</returns>
    /// <exception cref="ArgumentException">l'argument n'a pas de valeur pour l'index demandé</exception>
    public string this[int index]
    {
        get
        {
            if (ValuesCount != 0 && index > ValuesCount)
                throw ValueIndexNotAvailaible(index);
            return Values[index];
        }
    }

    /// <summary>
    /// get value (index 0)
    /// </summary>
    /// <returns>value at index 0</returns>
    public string GetValue() => Values[0];

    protected ArgumentException ValueIndexNotAvailaible(int index)
        => new ArgumentException(
            Texts._("NoArgumentValueAtIndex", index));

    /// <summary>
    /// name with prefix
    /// </summary>
    public string PrefixedName => GetPrefixFromArgName(Name) + Name;

    /// <summary>
    /// get the argument description from help settings
    /// </summary>
    /// <param name="description">description or error message if not found</param>
    /// <returns>true if description is found, otherwise false</returns>
    public bool GetDescription(out KeyValuePair<string, string> description)
    {
        var desc = Config.GetSection("GlobalArgs:" + Name);

        if (!desc.Exists() || !desc.GetChildren().Any())
        {
            description = new KeyValuePair<string, string>(
                Texts._("GlobalArgHelpNotFound", Name),
                string.Empty);
            return false;
        }

        var kvp = desc.GetChildren().First();

        description = new KeyValuePair<string, string>(
            kvp.Key,
            kvp.Value ?? string.Empty
            );

        return true;
    }

    #region builders

    /// <summary>
    /// extract values form an arguments lists. try to get the expectd values count
    /// </summary>
    /// <param name="args">arg list</param>
    /// <param name="index">begin index</param>
    /// <param name="position">actual begin index in arguments list</param>
    /// <exception cref="ArgumentException">missing argument value</exception>
    public void ParseValues(List<string> args, int index, int position)
    {
        var expectedCount = ValuesCount;
        args.RemoveAt(index);
        while (expectedCount > 0)
        {
            position++;
            if (!args.Any() || !IsArg(args[index]))
                throw new ArgumentException(Texts._("MissingArgumentValue", position, Name));

            Values.Add(args[index]);
            args.RemoveAt(index);
            expectedCount--;
        }
    }

    #endregion

    #region translaters, helpers

    public string Prefix => GetPrefixFromArgName(Name);

    public static bool IsArg(string text)
        => text.StartsWith(ShortArgNamePrefix)
            || text.StartsWith(LongArgNamePrefix);

    public static string ClassNameToOptName(string name)
        => name[0..^9]
            .ToLower();

    public static string GetPrefixFromClassName(string name)
    {
        var argName = ClassNameToOptName(name);
        return argName.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;
    }

    public static string GetPrefixFromArgName(string name)
        => name.Length == 1
            ? ShortArgNamePrefix
            : LongArgNamePrefix;

    #endregion
}
