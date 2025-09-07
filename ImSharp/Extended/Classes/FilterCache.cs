namespace ImSharp;

/// <summary> A base cache for items that are transformed for a cache and then can be filtered.  </summary>
/// <typeparam name="TCacheItem"> The transformed, cached item type. </typeparam>
public abstract class FilterCache<TCacheItem> : BasicCache
{
    /// <summary> Whether the next update should re-filter the global data. </summary>
    protected bool FilterDirty { get; set; } = true;

    /// <summary> The pre-processed list of all available items to display. </summary>
    protected IReadOnlyList<TCacheItem> UnfilteredItems = [];

    /// <summary> The global indices of items that are currently visible according to the filters. </summary>
    /// <remarks> Indices refer to <see cref="UnfilteredItems"/>. </remarks>
    protected readonly List<int> FilteredItems = [];

    /// <inheritdoc cref="UnfilteredItems"/>
    public IReadOnlyList<TCacheItem> AllItems
        => UnfilteredItems;

    /// <summary> Update the actual item data if <see cref="BasicCache.CustomDirty"/>. </summary>
    protected virtual void UpdateData()
    {
        if (!CustomDirty)
            return;

        // Update all items and notify that we need to re-filter and re-sort.
        var items = GetItems();
        UnfilteredItems = items as IReadOnlyList<TCacheItem> ?? items.ToList();
        FilterDirty     = true;
        OnDataUpdate();
    }

    /// <summary> Update the cache. Called whenever it is fetched. </summary>
    public override void Update()
    {
        if (Dirty is IManagedCache.DirtyFlags.Clean)
            return;

        UpdateData();
        UpdateFilter();
        Dirty = IManagedCache.DirtyFlags.Clean;
    }

    /// <summary> Update the filtered items. </summary>
    protected virtual void UpdateFilter()
    {
        if (!FilterDirty)
            return;

        // Add all items that are visible according to all filters.
        FilteredItems.Clear();
        foreach (var (idx, item) in UnfilteredItems.Index())
        {
            if (WouldBeVisible(item, idx))
                FilteredItems.Add(idx);
        }

        // Notify that we have filtered.
        FilterDirty = false;
        OnFilterUpdate();
    }

    /// <summary> Check whether a specific item should be visible or is filtered out. </summary>
    /// <param name="item"> The item to check. </param>
    /// <param name="globalIndex"> The global index of the item to check. </param>
    /// <returns> True if the item is not filtered out. </returns>
    protected abstract bool WouldBeVisible(in TCacheItem item, int globalIndex);

    /// <summary> Get an enumeration of all available items before filtering. </summary>
    protected abstract IEnumerable<TCacheItem> GetItems();

    /// <summary> Invoked when the local data cache has been updated. </summary>
    protected virtual void OnDataUpdate()
    { }

    /// <summary> Invoked when the global items have been freshly filtered. </summary>
    protected virtual void OnFilterUpdate()
    { }
}
