using Luna;

namespace ImSharp.Containers;

/// <summary> Type-less base class for cache list adapters. </summary>
public abstract class CacheListAdapter
{
    /// <summary> A change-counter to keep track of invoked changes. </summary>
    public uint Revision { get; protected set; }

    /// <summary> An additional tracker of invoked changed that can be cleaned up and checked by the parent. </summary>
    public bool Dirty { get; set; }

    /// <summary> The number of items that can be obtained. The items are lazily initialized. </summary>
    public abstract int Count { get; }
}

/// <summary> Source-type-less base class for cache list adapters. </summary>
/// <typeparam name="TCacheItem"> The type of item provided by the adapter. </typeparam>
/// <param name="count"> The initial capacity of the adapter.</param>
public abstract class CacheListAdapter<TCacheItem>(int count) : CacheListAdapter, IReadOnlyList<TCacheItem>
    where TCacheItem : class
{
    /// <summary> The actual storage cache of lazily initialized items. </summary>
    protected readonly List<TCacheItem?> CacheItems = new(count);

    /// <inheritdoc/>
    /// <remarks> Initializes all items that are reached during iteration. </remarks>
    public abstract IEnumerator<TCacheItem> GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary> Get the item at the given index. </summary>
    /// <param name="index"> The index to query. </param>
    /// <returns> The item at the index if the source list is large enough for it. </returns>
    /// <remarks> If the source item is not initialized yet, it will be automatically initialized. </remarks>
    public abstract TCacheItem this[int index] { get; }
}

/// <summary> An adapter binding to another list and supplying transformed items based on that list. </summary>
/// <typeparam name="TSourceItem"> The type of the source items. </typeparam>
/// <typeparam name="TCacheItem"> The type of the transformed items. </typeparam>
public class CacheListAdapter<TSourceItem, TCacheItem> : CacheListAdapter<TCacheItem>, IDisposable
    where TCacheItem : class
{
    /// <summary> The source list. </summary>
    private readonly IReadOnlyList<TSourceItem> _source;

    /// <summary> The transforming function, invoked when new items are initialized. </summary>
    private readonly Func<TSourceItem, TCacheItem> _converter;

    /// <summary> Create a new adapter for the given list using the specified converter. </summary>
    /// <param name="source"> The associated list. If this is a <see cref="ObservableList{T}"/>, the adapter automatically subscribes to its events and updates its own state with changes in it. </param>
    /// <param name="converter"> The transforming function, invoked whenever new items are initialized in the cache. </param>
    public CacheListAdapter(IReadOnlyList<TSourceItem> source, Func<TSourceItem, TCacheItem> converter)
        : base(source.Count)
    {
        _source    = source;
        _converter = converter;
        Subscribe();
    }

    /// <summary> Subscribe to the events of the associated list if it is observable. </summary>
    private void Subscribe()
    {
        if (_source is not ObservableList<TSourceItem> observable)
            return;

        observable.OnClear    += OnClear;
        observable.OnAdd      += OnAdd;
        observable.OnRemove   += OnRemove;
        observable.OnAddRange += OnAddRange;
        observable.OnUpdate   += OnUpdate;
    }

    /// <summary> Unsubscribe from the events of the associated list if it is observable. </summary>
    private void Unsubscribe()
    {
        if (_source is not ObservableList<TSourceItem> observable)
            return;

        observable.OnClear    -= OnClear;
        observable.OnAdd      -= OnAdd;
        observable.OnRemove   -= OnRemove;
        observable.OnAddRange -= OnAddRange;
        observable.OnUpdate   -= OnUpdate;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }

    ~CacheListAdapter()
        => Unsubscribe();

    /// <inheritdoc/>
    public sealed override IEnumerator<TCacheItem> GetEnumerator()
    {
        var count = _source.Count;
        CacheItems.EnsureCount(count);
        for (var i = 0; i < count; ++i)
            yield return CacheItems[i] ??= _converter(_source[i]);
    }

    /// <inheritdoc/>
    public sealed override int Count
        => _source.Count;

    /// <inheritdoc/>
    public sealed override TCacheItem this[int index]
    {
        get
        {
            if (index >= _source.Count)
                throw new IndexOutOfRangeException();

            CacheItems.EnsureCount(_source.Count);
            return CacheItems[index] ??= _converter(_source[index]);
        }
    }

    private void OnClear()
    {
        CacheItems.Clear();
        SetDirty();
    }

    private void OnAdd(TSourceItem _, int index)
    {
        if (index <= CacheItems.Count)
            CacheItems.Insert(index, null);
        SetDirty();
    }

    private void OnRemove(TSourceItem _, int index)
    {
        if (index <= CacheItems.Count)
            CacheItems.RemoveAt(index);
        SetDirty();
    }

    private void OnAddRange(int addedCount)
        => SetDirty();

    private void OnUpdate(TSourceItem _, TSourceItem _2, int index)
    {
        CacheItems[index] = null;
        SetDirty();
    }

    /// <summary> Ensure that the actual size, not just the capacity of the list is large enough. </summary>
    private static int EnsureCount<T>(ICollection<T> list, int count)
    {
        if (list.Count >= count)
            return 0;

        var toAdd = count - list.Count;
        for (var i = 0; i < toAdd; i++)
            list.Add(default!);
        return toAdd;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void SetDirty()
    {
        Dirty = true;
        ++Revision;
    }
}
