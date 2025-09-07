using Dalamud.Bindings.ImGui;

namespace ImSharp;

public abstract class SimpleFilterCombo<T> : FilterComboBase<SimpleCacheItem<T>>
{
    public SimpleFilterCombo(SimpleFilterType filterType)
        => Filter = filterType.ToFilter<T>();

    public abstract StringU8 DisplayString(in T value);
    public abstract string   FilterString(in T value);

    public virtual ColorParameter TextColor(in T value)
        => ColorParameter.Default;

    public virtual StringU8 Tooltip(in T value)
        => StringU8.Empty;

    public abstract IEnumerable<T> GetBaseItems();

    protected internal override IEnumerable<SimpleCacheItem<T>> GetItems()
        => GetBaseItems().Select(i => new SimpleCacheItem<T>(i, DisplayString(i), FilterString(i), TextColor(i), Tooltip(i)));

    protected internal override float ItemHeight
        => Im.Style.TextHeightWithSpacing;

    protected internal override bool DrawItem(in SimpleCacheItem<T> item, int globalIndex)
    {
        using var color = Im.Color.Push(ImGuiColor.Text, item.TextColor);
        var       ret   = Im.Selectable(item.DisplayString, false);
        Im.Tooltip.OnHover(item.Tooltip);
        return ret;
    }
}

public abstract class FilterComboBase<TCacheItem>
{
    public    IFilter<TCacheItem> Filter          { get; init; } = NopFilter<TCacheItem>.Instance;
    public    MouseWheelType      AllowMouseWheel { get; init; } = MouseWheelType.None;
    public    ComboFlags          Flags           { get; init; } = ComboFlags.None;
    protected ImGuiId             CurrentId;
    protected bool                SetScroll;
    protected bool                ClosePopup;

    protected internal abstract IEnumerable<TCacheItem> GetItems();

    protected virtual bool DrawFilter()
    {
        if (Filter is NopFilter<TCacheItem>)
            return false;

        PreDrawFilter();
        var ret = Filter.DrawFilter("Filter..."u8, Im.ContentRegion.Available);
        PostDrawFilter();
        return ret;
    }

    protected internal abstract float ItemHeight { get; }

    protected internal abstract bool DrawItem(in TCacheItem item, int globalIndex);

    protected virtual FilterComboBaseCache<TCacheItem> CreateCache()
        => new(this);

    public virtual bool Draw(Utf8LabelHandler label, Utf8TextHandler preview, Utf8HintHandler tooltip, float previewWidth, out TCacheItem? ret)
    {
        using var id = Im.Id.Push(label);
        CurrentId = Im.Id.Current;

        PreDrawCombo(previewWidth);
        Im.Item.SetNextWidth(previewWidth);
        using var combo = Im.Combo.Begin(label, preview, Flags | ComboFlags.HeightLarge);
        if (tooltip.GetSpan(out var tooltipSpan) && !tooltipSpan.IsEmpty)
        {
            using var enabled = Im.Enabled();
            Im.Tooltip.OnHover(tooltipSpan);
        }

        PostDrawCombo(previewWidth);

        if (!combo)
        {
            ret = default;
            return false;
        }

        if (DrawFilter())
            CacheManager.Instance.SetCustomDirty(CurrentId);

        var currentIndex = 0;
        var cache        = CacheManager.Instance.GetOrCreateCache(CurrentId, CreateCache);

        if (!cache.DrawList(out var globalIndex))
        {
            ret = default;
            return false;
        }

        ret = cache.AllItems[globalIndex];
        return true;
    }

    protected internal virtual int FindIndex(ReadOnlySpan<byte> displayText)
        => -1;

    protected internal virtual void PreDrawList()
    { }

    protected internal virtual void PostDrawList()
    { }

    protected internal virtual void PreDrawCombo(float width)
    { }

    protected internal virtual void PostDrawCombo(float width)
    { }

    protected internal virtual void PreDrawFilter()
    { }

    protected internal virtual void PostDrawFilter()
    { }
}

public class FilterComboBaseCache<TCacheItem>(FilterComboBase<TCacheItem> parent)
    : FilterCache<TCacheItem>
{
    public float ComboWidth { get; protected set; }

    public virtual bool DrawList(out int selectedGlobalIndex)
    {
        parent.PreDrawList();
        var ret = false;
        selectedGlobalIndex = -1;
        using (var clipper = new Im.ListClipper(FilteredItems.Count, parent.ItemHeight))
        {
            foreach (var globalIndex in clipper.Iterate(FilteredItems))
            {
                if (!parent.DrawItem(UnfilteredItems[globalIndex], globalIndex))
                    continue;

                ret                 = true;
                selectedGlobalIndex = globalIndex;
            }
        }

        parent.PostDrawList();
        return ret;
    }

    protected override bool WouldBeVisible(in TCacheItem item, int globalIndex)
        => parent.Filter.WouldBeVisible(item, globalIndex);

    protected override IEnumerable<TCacheItem> GetItems()
        => parent.GetItems();

    // Does not handle Enter.
    protected void DrawKeyboardNavigation()
    {
        // Enable keyboard navigation for going up and down,
        // jumping if reaching the end. This also scrolls to the element.
        if (_available.Count > 0)
        {
            if (ImGui.IsKeyPressed(ImGuiKey.DownArrow))
                (_lastSelection, _setScroll) = ((_lastSelection + 1) % _available.Count, true);
            else if (ImGui.IsKeyPressed(ImGuiKey.UpArrow))
                (_lastSelection, _setScroll) = ((_lastSelection - 1 + _available.Count) % _available.Count, true);
        }

        // Escape closes the popup without selection
        _closePopup = ImGui.IsKeyPressed(ImGuiKey.Escape);

        // Enter selects the current selection if any, or the first available item.
        if (ImGui.IsKeyPressed(ImGuiKey.Enter))
        {
            if (_lastSelection >= 0)
                NewSelection = _available[_lastSelection];
            else if (_available.Count > 0)
                NewSelection = _available[0];
            _closePopup = true;
        }
    }

    protected virtual void OnMouseWheel(string preview, ref int currentSelection, int steps)
    { }
}
