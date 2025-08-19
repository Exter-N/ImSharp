using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ImSharp;

/// <summary> A manager to handle managed caches for UI display in a single space. </summary>
/// <remarks> Any object of this type will subscribe its update function to <seealso cref="ImSharpPerFrame.Update"/>. </remarks>
public class CacheManager : IDisposable
{
    /// <summary> The default cache manager that internal objects can use. </summary>
    /// <remarks> It is possible to set the logger and service provider of this instance. </remarks>
    public static readonly CacheManager Instance = new(null, null);

    /// <summary> A custom logger to set when the manager should not use the global logger. </summary>
    public ILogger? CustomLogger
    {
        get => field;
        set
        {
            field = value;
            UpdateLogger(ImSharpConfiguration.Logger);
        }
    }

    /// <summary> The logger the internal functions write to. </summary>
    public ILogger Logger { get; private set; }

    /// <summary> A service provider to generate transient cache objects without providing a factory method. </summary>
    public IServiceProvider? ServiceProvider { get; set; }

    private readonly Dictionary<ImGuiId, (IManagedCache Cache, DateTime Time)> _caches     = [];
    private readonly Dictionary<ImGuiId, object>                               _storedData = [];

    /// <summary> Create a manager to handle managed caches for UI display in a single space. </summary>
    /// <param name="logger"> A logger. </param>
    /// <param name="serviceProvider"> A service provider to generate transient cache objects without providing a factory method. </param>
    protected CacheManager(ILogger? logger, IServiceProvider? serviceProvider)
    {
        ServiceProvider                    =  serviceProvider;
        CustomLogger                       =  logger;
        Logger                             =  CustomLogger ?? ImSharpConfiguration.Logger;
        ImSharpPerFrame.Update             += CheckCaches;
        ImSharpConfiguration.LoggerChanged += UpdateLogger;
    }

    /// <summary> Get or create a new cache for a given ID, and update the last access for it. </summary>
    /// <typeparam name="TResult"> The type of the cache. </typeparam>
    /// <param name="id"> The ID to store the cache under. </param>
    /// <param name="factory"> The factory function to create the cache if it does not exist already. </param>
    /// <returns> The existing or newly created cache. </returns>
    /// <remarks>
    ///   If there is a cache for <paramref name="id"/> stored but its type is not compatible with <typeparamref name="TResult"/>
    ///   it will be disposed and replaced by a newly created cache.
    /// </remarks>
    public TResult GetOrCreateCache<TResult>(ImGuiId id, Func<TResult> factory)
        where TResult : class, IManagedCache
    {
        if (!_caches.TryGetValue(id, out var pair))
        {
            var cache = factory();
            if (_storedData.TryGetValue(id, out var data))
                cache.ApplyStoredData(data);
            _caches.Add(id, (cache, NextDeletion(cache.KeepAliveDuration)));
            Logger.LogDebug("Created new cache of type {Type} for ID {ID}.", typeof(TResult), id.Id);
            return cache;
        }

        if (CheckAndUpdateCache<TResult>(id, pair.Item1) is { } existingCache)
            return existingCache;

        if (pair.Item1.SaveStoredData() is { } obj)
            _storedData[id] = obj;
        (pair.Item1 as IDisposable)?.Dispose();
        var newCache = factory();
        if (_storedData.TryGetValue(id, out var newData))
            newCache.ApplyStoredData(newData);
        _caches[id] = (newCache, NextDeletion(newCache.KeepAliveDuration));
        Logger.LogInformation("Replaced existing cache of type {OldType} with new type {NewType} for ID {ID}.", pair.Item1.GetType(),
            typeof(TResult), id.Id);
        return newCache;
    }

    /// <summary> Get or fetch a new, transient cache from the service provider for a given ID, and update the last access for it. </summary>
    /// <typeparam name="TResult"> The type of the cache. </typeparam>
    /// <param name="id"> The ID to store the cache under. </param>
    /// <param name="key"> An optional key for the service provider. </param>
    /// <returns> The existing or newly created cache. </returns>
    /// <remarks>
    ///   If there is a cache for <paramref name="id"/> stored but its type is not compatible with <typeparamref name="TResult"/>
    ///   it will be disposed and replaced by a newly created cache.
    /// </remarks>
    public TResult GetOrCreateCache<TResult>(ImGuiId id, object? key = null)
        where TResult : class, IManagedCache
    {
        if (!_caches.TryGetValue(id, out var pair))
        {
            var cache = GetService<TResult>(key);
            if (_storedData.TryGetValue(id, out var data))
                cache.ApplyStoredData(data);
            _caches.Add(id, (cache, NextDeletion(cache.KeepAliveDuration)));
            Logger.LogDebug("Created new cache of type {Type} for ID {ID}.", typeof(TResult), id.Id);
            return cache;
        }

        if (CheckAndUpdateCache<TResult>(id, pair.Item1) is { } existingCache)
            return existingCache;

        if (pair.Item1.SaveStoredData() is { } obj)
            _storedData[id] = obj;
        (pair.Item1 as IDisposable)?.Dispose();
        var newCache = GetService<TResult>(key);
        if (_storedData.TryGetValue(id, out var newData))
            newCache.ApplyStoredData(newData);
        _caches[id] = (newCache, NextDeletion(newCache.KeepAliveDuration));
        Logger.LogInformation("Replaced existing cache of type {OldType} with new type {NewType} for ID {ID}.", pair.Item1.GetType(),
            typeof(TResult), id.Id);
        return newCache;
    }

    /// <summary> Check all caches for disposal. </summary>
    /// <remarks> Any cache that has not been retrieved for at least its <seealso cref="IManagedCache.KeepAliveDuration"/> frames will be disposed and removed. </remarks>
    protected void CheckCaches()
    {
        var now = DateTime.UtcNow;
        foreach (var (id, (cache, time)) in _caches)
        {
            if (time < now)
            {
                if (cache.SaveStoredData() is { } obj)
                    _storedData[id] = obj;
                (cache as IDisposable)?.Dispose();
                _caches.Remove(id);
                Logger.LogTrace("Removed cache for ID {ID}.", id.Id);
            }
        }
    }

    /// <summary> Set the custom dirty flag for a specific cache by its ID. </summary>
    /// <param name="id"> The ID of the cache to set the flag for. </param>
    /// <remarks> If no cache for this ID exists, this does nothing. </remarks>
    public void SetCustomDirty(ImGuiId id)
    {
        if (_caches.TryGetValue(id, out var pair))
        {
            pair.Item1.Dirty |= IManagedCache.DirtyFlags.Custom;
            Logger.LogTrace("Set custom dirty flag for ID {ID}.", id.Id);
        }
    }

    /// <summary> Set the full dirty flag for a specific cache by its ID. </summary>
    /// <param name="id"> The ID of the cache to set the flag for. </param>
    /// <remarks> If no cache for this ID exists, this does nothing. </remarks>
    public void SetDirty(ImGuiId id)
    {
        if (_caches.TryGetValue(id, out var pair))
        {
            pair.Item1.Dirty |= IManagedCache.DirtyFlags.Dirty;
            Logger.LogTrace("Set full dirty flag for ID {ID}.", id.Id);
        }
    }

    /// <summary> Set the font dirty flag for all caches. </summary>
    public void SetFontDirty()
    {
        Logger.LogTrace("Set font size dirty flag for all caches.");
        foreach (var (cache, _) in _caches.Values)
            cache.Dirty |= IManagedCache.DirtyFlags.Font;
    }

    /// <summary> Set the style dirty flag for all caches. </summary>
    public void SetStyleDirty()
    {
        Logger.LogTrace("Set style dirty flag for all caches.");
        foreach (var (cache, _) in _caches.Values)
            cache.Dirty |= IManagedCache.DirtyFlags.Style;
    }

    /// <summary> Set the colors dirty flag for all caches. </summary>
    public void SetColorsDirty()
    {
        Logger.LogTrace("Set colors dirty flag for all caches.");
        foreach (var (cache, _) in _caches.Values)
            cache.Dirty |= IManagedCache.DirtyFlags.Style;
    }

    /// <summary> Dispose and remove all stored caches. </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary> Dispose and remove all stored caches. </summary>
    protected virtual void Dispose(bool disposing)
    {
        foreach (var (cache, _) in _caches.Values)
            (cache as IDisposable)?.Dispose();
        _caches.Clear();
        ImSharpPerFrame.Update             -= CheckCaches;
        ImSharpConfiguration.LoggerChanged -= UpdateLogger;
    }

    /// <summary> Update the logger if it changes in the global configuration. </summary>
    private void UpdateLogger(ILogger obj)
        => Logger = CustomLogger ?? obj;

    ~CacheManager()
        => Dispose(false);

    /// <summary> Safely add the current time count and the keep alive duration without overflow. </summary>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    private static DateTime NextDeletion(TimeSpan keepAliveDuration)
    {
        if (keepAliveDuration == TimeSpan.MaxValue)
            return DateTime.MaxValue;
        if (keepAliveDuration < TimeSpan.Zero)
            return DateTime.MinValue;

        return DateTime.UtcNow + keepAliveDuration;
    }

    /// <summary> Try to get a scoped transient cache object from the service provider. </summary>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    private T GetService<T>(object? key = null) where T : class, IManagedCache
    {
        if (ServiceProvider is null)
            throw new Exception("No service provider initialized for this cache manager.");

        var cache = key is not null ? ServiceProvider.GetRequiredKeyedService<T>(key) : ServiceProvider.GetRequiredService<T>();
        return cache;
    }

    /// <summary> Check a pre-existing cache to be the correct type and update it if it is. </summary>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    private TResult? CheckAndUpdateCache<TResult>(ImGuiId id, IManagedCache cache)
        where TResult : class, IManagedCache
    {
        if (cache is not TResult res)
            return null;

        res.Update();
        _caches[id] = (res, NextDeletion(res.KeepAliveDuration));
        return res;
    }
}
