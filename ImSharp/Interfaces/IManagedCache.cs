namespace ImSharp;

/// <summary> An interface to represent managed caches used by <seealso cref="CacheManager"/>. </summary>
public interface IManagedCache
{
    /// <summary> Flags to denote different types of dirtiness for caches. </summary>
    [Flags]
    public enum DirtyFlags : byte
    {
        /// <summary> The cache does not need any updates. </summary>
        Clean = 0,

        /// <summary> The cache's underlying data or custom attributes changed and it needs updates. </summary>
        CustomDirty = 1,

        /// <summary> The global font changed and text sizes need recalculation. </summary>
        FontDirty = 2,

        /// <summary> Global style variables changed and item sizes and positions need recalculation. </summary>
        StyleDirty = 4,

        /// <summary> Global colors changed and stored color values need recalculation. </summary>
        ColorsDirty = 8,

        /// <summary> Everything should be updated. </summary>
        Dirty = CustomDirty | FontDirty | StyleDirty | ColorsDirty,
    }

    /// <summary> The dirty state of the cache. </summary>
    public DirtyFlags Dirty { get; set; }

    /// <summary> The duration of not being seen a cache should be kept alive. </summary>
    /// <remarks> Set this to <seealso cref="TimeSpan.MaxValue"/> for persistent caches. Set it to non-positive values for caches that should get removed immediately. </remarks>
    public TimeSpan KeepAliveDuration
        => TimeSpan.FromSeconds(5);

    /// <summary> Update the caches state. This should handle the individual <seealso cref="DirtyFlags"/> sensibly and should set <seealso cref="Dirty"/> to <seealso cref="DirtyFlags.Clean"/> when finished. </summary>
    public void Update();
}

/// <summary> A basic disposable cache implementation. </summary>
public abstract class BasicCache : IManagedCache, IDisposable
{
    /// <inheritdoc/>
    public IManagedCache.DirtyFlags Dirty { get; set; } = IManagedCache.DirtyFlags.Dirty;

    /// <inheritdoc/>
    public abstract void Update();

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BasicCache()
        => Dispose(false);

    /// <summary> Custom disposal. </summary>
    /// <param name="disposing"> Whether the disposal comes from a finalizer or a <seealso cref="Dispose()"/>. </param>
    protected virtual void Dispose(bool disposing)
    { }
}
