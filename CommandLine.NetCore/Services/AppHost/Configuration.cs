using CommandLine.NetCore.Services.CmdLine.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace CommandLine.NetCore.Services.AppHost;

/// <summary>
/// culture dependent configuration
/// <para>overloads host configuration</para>
/// </summary>
public class Configuration : IConfiguration
{
    /// <summary>
    /// host configuration
    /// </summary>
    internal IConfiguration HostConfiguration { get; }

    readonly Dictionary<string, Dictionary<string, string?>> _settings
        = new();

    /// <summary>
    /// creates a new dynamic culture dependent configuration
    /// </summary>
    /// <param name="configuration">host configuration</param>
    public Configuration(IConfiguration configuration)
        => HostConfiguration = configuration;

    /// <summary>
    /// current culture
    /// </summary>
    internal static string Culture => Thread.CurrentThread.CurrentCulture.Name;

    /// <summary>
    /// get a value for a culture
    /// <para>fallback to current culture</para>
    /// <para>fallback to independent culture</para>
    /// </summary>
    /// <param name="culture">culture</param>
    /// <param name="key">key</param>
    /// <returns>value</returns>
    public string? Get(string key, string? culture = null)
    {
        culture ??= Culture;
        if (_settings.TryGetValue(culture, out var settings)
                && settings.TryGetValue(key, out var value))
            return value;
        if (_settings.Keys.Count > 0
            && _settings.TryGetValue(_settings.Keys.First(), out settings)
            && settings.TryGetValue(key, out value))
            return value;
        return HostConfiguration[key];
    }

    /// <summary>
    /// determines wheter settings contains key for the culture or not 
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="culture">culture. if null use the current culture</param>
    /// <returns>true if key is found, false otherwise</returns>
    public bool ContainsKey(string key, string? culture = null)
    {
        culture ??= Culture;
        if (!_settings.TryGetValue(culture, out var cultureKeys))
            return false;
        return cultureKeys.ContainsKey(key);
    }

    /// <summary>
    /// build an unique key for the culture. if key exists, a new one is built with a postfix beeing the minimum available index
    /// </summary>
    /// <param name="commandName">command name</param>
    /// <param name="key">key</param>
    /// <param name="culture">culture. if null use the current culture</param>
    /// <returns>key itself or key with a postfix beeing the minimum available index</returns>
    public string BuildUniqueKey(string commandName, string key, string? culture = null)
    {
        culture ??= Culture;
        if (!_settings.TryGetValue(culture, out var cultureKeys))
            return key;
        var index = 2;
        var ckey = key;
        while (cultureKeys.ContainsKey(
            HelpBuilder.LongDescriptionKey(commandName)
                + ":" + ckey))
        {
            ckey = key + " (" + index + ")";
            index++;
        }
        return ckey;
    }

    /// <summary>
    /// set a value for a culture
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <param name="culture">culture. if null use the current culture</param>
    public void Set(string key, string? value, string? culture = null)
    {
        culture ??= Culture;
        if (_settings.ContainsKey(culture))
            _settings[Culture][key] = value;
        else
            _settings.Add(culture,
                new Dictionary<string, string?> { { key, value } });
    }

    /// <summary>
    /// get a value for a culture
    /// <para>fallback to current culture</para>
    /// <para>fallback to independent culture</para>
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="culture">culture</param>
    /// <returns>value</returns>
    public T? GetValue<T>(string key, string? culture = null)
        where T : class
        => Get(key, culture) as T;

    /// <summary>
    /// set a value for a culture
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <param name="culture">culture. if null use the current culture</param>
    public void Set<T>(
        string key,
        T? value,
        string? culture = null)
            => Set(key, value as string, culture);

    /// <inheritdoc/>
    public string? this[string key]
    {
        get => Get(Culture, key);
        set => Set(key, value, Culture);
    }

    /// <inheritdoc/>
    public IEnumerable<IConfigurationSection> GetChildren()
        => HostConfiguration.GetChildren();

    /// <inheritdoc/>
    public IChangeToken GetReloadToken()
        => HostConfiguration.GetReloadToken();

    /// <inheritdoc/>
    public IConfigurationSection GetSection(string key)
        => GetSection(key, null);

    /// <summary>
    /// set a section for a culture
    /// </summary>
    /// <param name="path">section path</param>
    /// <param name="culture">culture. if null use the current culture</param>
    public IConfigurationSection GetSection(
        string path, string? culture = null)
    {
        culture ??= Culture;
        var hostSection = HostConfiguration.GetSection(path);
        if (_settings.ContainsKey(culture))
        {
            static string PathKey(string x) => x.Split(':').Last();
            var sectionKey = PathKey(path);
            _settings[culture].TryGetValue(path, out var value);
            value ??= hostSection.Value;

            var subpaths = _settings[culture]
                .Keys
                .Where(x => x.StartsWith(path) && x != path);

            var subSections = new List<IConfigurationSection>();
            foreach (var subpath in subpaths)
            {
                var subSection = GetSection(
                        subpath,
                        culture
                        );
                subSections.Add(subSection);
                if (!subSection.Exists())
                {
                    var hostSubSection = HostConfiguration.GetSection(path);
                    if (hostSubSection.Exists())
                        subSections.Add(hostSubSection);
                }
            }

            var section = new ConfigurationSection(
                this,
                sectionKey,
                path,
                value,
                subSections);

            return section;
        }
        return hostSection;
    }
}
