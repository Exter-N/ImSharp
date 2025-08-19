namespace ImSharp.Table;

/// <summary> A table column representing multiple flags that represent some values. </summary>
/// <typeparam name="TEnum"> The type of the enum that defines the flags. </typeparam>
/// <typeparam name="TCacheItem"> The type of the cached transformation of the items to display. </typeparam>
public abstract class FlagColumn<TEnum, TCacheItem> : BasicColumn<TCacheItem>
    where TEnum : struct, Enum
{
    /// <summary> The value used when no filter is enabled. </summary>
    protected TEnum AllFlags { get; init; }

    /// <summary> The flags used to draw the combo of checkboxes. </summary>
    protected ComboFlags ComboFlags { get; init; } = ComboFlags.NoArrowButton;

    /// <summary> The different flag values to filter for and their display labels. </summary>
    protected abstract IReadOnlyList<(TEnum Value, StringU8 Name)> EnumData { get; }

    /// <summary> Get the current filter value. </summary>
    public virtual TEnum FilterValue
        => default;

    /// <summary> An event invoked when the filter for this column changes. </summary>
    public event Action<TEnum>? FilterChanged;

    /// <summary> Enable or disable a specific flag. </summary>
    /// <param name="value"> The flag to change. </param>
    /// <param name="enable"> If true, <paramref name="value"/> is toggled on, if false, it is toggled off. </param>
    /// <returns> True if the current filter value changed. </returns>
    protected abstract bool SetValue(TEnum value, bool enable);

    /// <summary> Get the set of flags for a row. </summary>
    /// <param name="item"> The row to check. </param>
    /// <returns> The metadata flags for this row. </returns>
    protected abstract TEnum GetValue(in TCacheItem item);

    /// <summary> Get the text to display for a row. </summary>
    /// <param name="item"> The row to check. </param>
    /// <returns> The text for this cell. </returns>
    protected abstract StringU8 DisplayString(in TCacheItem item);

    /// <inheritdoc/>
    public override void DrawColumn(in TCacheItem item, int globalIndex)
    {
        Im.Text(DisplayString(item));
        if (Im.Item.Hovered(HoveredFlags.AllowWhenDisabled))
            DrawTooltip(item, globalIndex);
    }

    /// <inheritdoc/>
    public override int Compare(in TCacheItem lhs, in TCacheItem rhs)
        => Comparer<TEnum>.Default.Compare(GetValue(lhs), GetValue(rhs));

    /// <inheritdoc/>
    public override bool FilterFunc(in TCacheItem item)
        => FilterValue.HasFlag(GetValue(item));

    /// <summary> A tooltip to draw when the text is hovered. This is only called when the text is hovered, but does not start a tooltip itself. </summary>
    /// <param name="item"> The drawn row. </param>
    /// <param name="globalIndex"> The global index of the drawn row. </param>
    protected virtual void DrawTooltip(in TCacheItem item, int globalIndex)
    { }

    /// <summary> Draw a filter that expands a combo of multiple checkboxes on click. </summary>
    /// <inheritdoc/>
    public override bool DrawFilter(float arrowWidth)
    {
        using var id    = Im.Id.Push("##Filter"u8);
        using var combo = BeginCombo(arrowWidth, out var changes);
        if (!combo)
            return changes;

        for (var i = 0; i < EnumData.Count; ++i)
            changes |= DrawCheckbox(i);

        if (changes)
            InvokeEvent();

        return changes;
    }

    /// <summary> Begin a combo width the correct width, also handling the right-click to reset filters. </summary>
    /// <param name="arrowWidth"> <inheritdoc cref="DrawFilter"/> </param>
    /// <param name="changes"> Whether any changes to the filter were applied. </param>
    /// <returns> The combo disposable. </returns>
    [MethodImpl(ImSharpConfiguration.Inl)]
    protected Im.ComboDisposable BeginCombo(float arrowWidth, out bool changes)
    {
        var       all   = FilterValue.HasFlag(AllFlags);
        using var style = ImStyleSingle.FrameRounding.Push(0);
        Im.Item.SetNextWidth(-arrowWidth);
        var color = ImGuiColor.FrameBackground.Push(ImEx.Table.ActiveFilterColor, !all);
        var combo = Im.Combo.Begin(""u8, Label, ComboFlags);
        color.Dispose();
        changes = Im.Item.Clicked(MouseButton.Right) && SetValue(AllFlags, true);
        if (!all)
            Im.Tooltip.OnHover("Right-click to clear filters."u8);

        return combo;
    }

    /// <summary> Draw checkboxes for possible filters that also allow to toggle all other values on right-clicks. </summary>
    protected virtual bool DrawCheckbox(int idx)
    {
        var (flag, text) = EnumData[idx];
        var tmp = FilterValue.HasFlag(flag);
        var ret = false;
        if (Im.Checkbox(text, ref tmp))
        {
            ret |= SetValue(flag, tmp);
        }
        else if (Im.Item.Clicked(MouseButton.Right))
        {
            if (tmp)
            {
                ret |= SetValue(AllFlags, false);
                ret |= SetValue(flag,     true);
            }
            else
            {
                ret |= SetValue(AllFlags, true);
                ret |= SetValue(flag,     false);
            }
        }

        Im.Tooltip.OnHover("Right-click to turn all other filters off (when this is on) or on (when this is off)."u8);
        return ret;
    }

    /// <summary> Invoke the <see cref="FilterChanged"/> event. </summary>
    [MethodImpl(ImSharpConfiguration.Inl)]
    protected void InvokeEvent()
        => FilterChanged?.Invoke(FilterValue);
}
