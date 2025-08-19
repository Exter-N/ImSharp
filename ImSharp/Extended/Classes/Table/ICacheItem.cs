namespace ImSharp.Table;

/// <summary> An interface for table cache items that demands a factory from the regular item to exist. </summary>
/// <typeparam name="TItem"> The type of the item to display in the table. </typeparam>
/// <typeparam name="TCacheItem"> The type of the cached item to create when the table is active. </typeparam>
public interface ICacheItem<in TItem, out TCacheItem>
{
    /// <summary> Factory method to construct a cache item from a given item. </summary>
    /// <param name="item"> The regular item to create a cache from. </param>
    /// <returns> The cache item created. </returns>
    public abstract static TCacheItem Create(TItem item);
}

/// <summary> A defaulted cache item that just contains and exposes the regular item it was constructed from. </summary>
/// <typeparam name="TItem"> The type of the item to display in the table. </typeparam>
/// <param name="item"> The regular item to create a cache from. </param>
public readonly struct DefaultCacheItem<TItem>(TItem item) : ICacheItem<TItem, DefaultCacheItem<TItem>>
{
    /// <summary> The base item. </summary>
    public readonly TItem Item = item;

    /// <inheritdoc/>
    public static DefaultCacheItem<TItem> Create(TItem item)
        => new(item);
}
