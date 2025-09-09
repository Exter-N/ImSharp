namespace ImSharp;

/// <summary> A base class for a combo supporting filtering, cached drawing and clipping. </summary>
/// <typeparam name="TCacheItem"> The type of the cache items to draw. </typeparam>
public abstract class FilterComboBase<TCacheItem>
{
    /// <summary> The filter used. It is drawn at the top of the expanded combo unless it is a <see cref="NopFilter{TCacheItem}"/>, in which case it is ignored. </summary>
    public IFilter<TCacheItem> Filter { get; init; } = NopFilter<TCacheItem>.Instance;

    /// <summary> Whether to allow mouse-wheel scrolling while hovering the unexpanded combo, potentially only with specific key modifiers held. </summary>
    public MouseWheelType AllowMouseWheel { get; init; } = MouseWheelType.Control;

    /// <summary> Additional flags used to draw the combo. </summary>
    public ComboFlags Flags { get; init; } = ComboFlags.None;

    /// <summary> Whether the filter should be cleared whenever the selection is updated. </summary>
    public bool ClearFilterOnSelection { get; init; } = false;

    /// <summary> Whether the filter should be cleared whenever the combo cache is disposed. </summary>
    public bool ClearFilterOnCacheDisposal { get; init; } = true;

    /// <summary> The ID used for the cache. </summary>
    protected ImGuiId CurrentId;

    /// <summary> Obtain the list of all available cache items without filtering. </summary>
    protected internal abstract IEnumerable<TCacheItem> GetItems();

    /// <summary> Obtain the item height used to draw a single cache item. Should include spacing. </summary>
    protected internal abstract float ItemHeight { get; }

    /// <summary> Draw a single cache item, generally as a selectable. </summary>
    /// <param name="item"> The item to draw. </param>
    /// <param name="globalIndex"> The global index of the item. </param>
    /// <param name="selected"> Whether the item should be highlighted as selected </param>
    /// <returns> True if the item was selected. </returns>
    protected internal abstract bool DrawItem(in TCacheItem item, int globalIndex, bool selected);

    /// <summary> Check whether an item should be highlighted as the current selection. </summary>
    /// <param name="item"> The item to query. </param>
    /// <param name="globalIndex"> The global index of the item. </param>
    /// <returns> True if the item is currently selected. </returns>
    /// <remarks> Also used to compute the index of the currently selected item on appearing. </remarks>
    protected internal abstract bool IsSelected(TCacheItem item, int globalIndex);

    /// <summary> Create the cache used to draw the expanded combo list. </summary>
    protected virtual FilterComboBaseCache<TCacheItem> CreateCache()
        => new(this);

    /// <summary> Draw a combo with a given label, the given preview text and an optional tooltip. </summary>
    /// <param name="label"> The label of the combo. If this is a UTF8 string, it HAS to be null-terminated. </param>
    /// <param name="preview"> The preview text displayed in the combo. If this is a UTF8 string, it HAS to be null-terminated. </param>
    /// <param name="tooltip"> The tooltip shown when hovering the combo box. Omitted if this is empty. </param>
    /// <param name="previewWidth"> The width of the preview box for the combo. </param>
    /// <param name="ret"> If true is returned, a newly selected item. </param>
    /// <returns> True if a new item is selected by any means, false otherwise. </returns>
    public virtual bool Draw(Utf8LabelHandler label, Utf8TextHandler preview, Utf8HintHandler tooltip, float previewWidth,
        [NotNullWhen(true)] out TCacheItem? ret)
    {
        // Push the ID and save it for this frame.
        using var id = Im.Id.Push(ref label);
        CurrentId = Im.Id.Current;

        // Draw the combo and additional control handling.
        var exit = DrawCombo(ref label, ref preview, ref tooltip, previewWidth, out ret!);
        if (DrawMouseWheelHandling(out var ret2))
        {
            ret  = ret2;
            exit = true;
        }

        if (exit)
        {
            // Clear the filter on selection change given the setting.
            if (ClearFilterOnSelection)
                Filter.Clear();
            return true;
        }

        ret = default;
        return false;
    }

    /// <summary> Draw the combo itself. </summary>
    /// <param name="label"> The label of the combo. If this is a UTF8 string, it HAS to be null-terminated. </param>
    /// <param name="preview"> The preview text displayed in the combo. If this is a UTF8 string, it HAS to be null-terminated. </param>
    /// <param name="tooltip"> The tooltip shown when hovering the combo box. Omitted if this is empty. </param>
    /// <param name="previewWidth"> The width of the preview box for the combo. </param>
    /// <param name="ret"> If true is returned, a newly selected item. </param>
    /// <returns> True if a new item is selected by any means, false otherwise. </returns>
    protected virtual bool DrawCombo(ref Utf8LabelHandler label, ref Utf8TextHandler preview, ref Utf8HintHandler tooltip, float previewWidth,
        [NotNullWhen(true)] out TCacheItem? ret)
    {
        // Draw the combo itself.
        PreDrawCombo(previewWidth);
        Im.Item.SetNextWidth(previewWidth);
        using var combo = Im.Combo.Begin(label, preview, Flags | ComboFlags.HeightLarge);

        // Draw the tooltip if not empty.
        if (tooltip.GetSpan(out var tooltipSpan) && !tooltipSpan.IsEmpty)
        {
            using var enabled = Im.Enabled();
            Im.Tooltip.OnHover(tooltipSpan);
        }

        PostDrawCombo(previewWidth);

        // If the combo is expanded, draw the filter and list.
        if (combo)
            return DrawComboPopup(previewWidth, out ret);

        ret = default;
        return false;
    }

    /// <summary> Draw the expanded combo popup. </summary>
    /// <param name="previewWidth"> The width of the preview box for the combo. </param>
    /// <param name="ret"> If true is returned, a newly selected item. </param>
    /// <returns> True if a new item is selected by any means, false otherwise. </returns>
    protected virtual bool DrawComboPopup(float previewWidth, [NotNullWhen(true)] out TCacheItem? ret)
    {
        var cache = CacheManager.Instance.GetOrCreateCache(CurrentId, CreateCache);
        var width = Math.Max(cache.ComboWidth, previewWidth);
        // If the filter is changed, set it dirty for the next frame.
        if (DrawFilter(width, cache))
            cache.Dirty |= IManagedCache.DirtyFlags.Custom;

        // Draw the list.
        if (cache.DrawList(width, out var globalIndex))
        {
            PostDrawList();
            ret = cache.AllItems[globalIndex]!;
            return true;
        }

        ret = default;
        return false;
    }

    /// <summary> Draw the filter on top of the item list. </summary>
    /// <returns> True if the filter changed. </returns>
    protected virtual bool DrawFilter(float width, FilterComboBaseCache<TCacheItem> cache)
    {
        if (Filter is NopFilter<TCacheItem>)
            return false;

        PreDrawFilter();
        if (Im.Window.Appearing)
            Im.Keyboard.SetFocusHere();

        var ret = Filter.DrawFilter("Filter..."u8, new Vector2(width, Im.Style.FrameHeight));
        PostDrawFilter();
        return ret;
    }

    /// <summary> Handle mouse-wheel interaction when hovering the combo. </summary>
    /// <param name="ret"></param>
    /// <returns></returns>
    protected virtual bool DrawMouseWheelHandling([NotNullWhen(true)] out TCacheItem? ret)
    {
        // Check hovering and mouse-wheel activity.
        if (Im.Item.Hovered() && AllowMouseWheel.CheckMouseWheel())
        {
            // Set the item to consume the mouse wheel.
            Im.Item.SetUsingMouseWheel();
            // Use the mouse wheel delta to select a new item. This may require creating a new cache.
            var delta = (int)Im.Io.MouseWheel;
            if (delta is not 0)
            {
                var cache = CacheManager.Instance.GetOrCreateCache(CurrentId, CreateCache);
                if (cache.HandleMouseWheel(delta, out var newIndex))
                {
                    ret = cache.AllItems[newIndex]!;
                    return true;
                }
            }
        }

        ret = default;
        return false;
    }

    /// <summary> Function invoked before drawing the expanded combo list. </summary>
    protected internal virtual void PreDrawList()
    { }

    /// <summary> Function invoked after drawing the expanded combo list. </summary>
    protected internal virtual void PostDrawList()
    { }

    /// <summary> Function invoked before drawing the combo preview. </summary>
    protected virtual void PreDrawCombo(float width)
    { }

    /// <summary> Function invoked after drawing the combo preview. </summary>
    protected virtual void PostDrawCombo(float width)
    { }

    /// <summary> Function invoked before drawing the filter inside the expanded combo list. </summary>
    protected virtual void PreDrawFilter()
    { }

    /// <summary> Function invoked after drawing the filter inside the expanded combo list. </summary>
    protected virtual void PostDrawFilter()
    { }
}
