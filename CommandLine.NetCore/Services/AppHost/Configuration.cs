using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace CommandLine.NetCore.Services.AppHost;

/// <summary>
/// culture dependent configuration
/// </summary>
public sealed class Configuration : IConfiguration
{
    readonly IConfiguration _configuration;

    readonly Dictionary<string, Dictionary<string, string?>> _settings
        = new();

    /// <summary>
    /// creates a new dynamic culture dependent configuration
    /// </summary>
    /// <param name="configuration">host configuration</param>
    public Configuration(IConfiguration configuration)
        => _configuration = configuration;

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
        return _configuration[key];
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
        => _configuration.GetChildren();

    /// <inheritdoc/>
    public IChangeToken GetReloadToken()
        => _configuration.GetReloadToken();

    /// <inheritdoc/>
    public IConfigurationSection GetSection(string key)
        => _configuration.GetSection(key);
}
