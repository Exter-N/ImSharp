namespace ImSharp.Containers;

/// <summary> Add a list that invokes events when changes occur. </summary>
/// <typeparam name="T"> The type of item. </typeparam>
public class ObservableList<T> : List<T>
{
    /// <summary> Invoked after the entire list was cleared. </summary>
    public event Action? OnClear;

    /// <summary> Invoked with the new item and its index after a single item was added to the list or inserted into the list. </summary>
    public event Action<T, int>? OnAdd;

    /// <summary> Invoked with the number of added items after multiple items were appended to the list. </summary>
    public event Action<int>? OnAddRange;

    /// <summary> Invoked with the removed item and its prior index after a single item is removed from the list. </summary>
    public event Action<T, int>? OnRemove;

    /// <summary> Invoked with the old item, the new item and their index after the item in an index was replaced with a new one. </summary>
    public event Action<T, T, int>? OnUpdate;

    /// <inheritdoc cref="List{T}.Clear"/>
    public new void Clear()
    {
        base.Clear();
        OnClear?.Invoke();
    }

    /// <inheritdoc cref="List{T}.Add"/>
    public new void Add(T item)
    {
        base.Add(item);
        OnAdd?.Invoke(item, Count - 1);
    }

    /// <inheritdoc cref="List{T}.Insert"/>
    public new void Insert(int index, T item)
    {
        base.Insert(index, item);
        OnAdd?.Invoke(item, index);
    }

    /// <inheritdoc cref="List{T}.Remove"/>
    public new bool Remove(T item)
    {
        var idx = IndexOf(item);
        if (idx < 0)
            return false;

        base.RemoveAt(idx);
        OnRemove?.Invoke(item, idx);
        return true;
    }

    /// <inheritdoc cref="List{T}.RemoveAt"/>
    public new void RemoveAt(int idx)
    {
        if (idx >= Count)
            return;

        var item = this[idx];
        base.RemoveAt(idx);
        OnRemove?.Invoke(item, idx);
    }

    /// <inheritdoc cref="List{T}.AddRange"/>
    public new void AddRange(IEnumerable<T> collection)
    {
        var startIdx = Count;
        base.AddRange(collection);
        OnAddRange?.Invoke(Count - startIdx);
    }

    /// <inheritdoc cref="List{T}.this"/>
    public new T this[int index]
    {
        get => base[index];
        set
        {
            var old = base[index];
            base[index] = value;
            OnUpdate?.Invoke(old, value, index);
        }
    }
}
