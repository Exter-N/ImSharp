namespace ImSharp;

/// <summary> Basic filter for flag based checks. </summary>
/// <typeparam name="TCacheItem"> The type of item to filter. </typeparam>
/// <typeparam name="TEnum"> The type of the flags to check against. </typeparam>
public abstract class FlagFilterBase<TCacheItem, TEnum> : IFilter<TCacheItem>
    where TEnum : unmanaged, Enum
{
    /// <summary> The flags used to draw the combo of checkboxes. </summary>
    public ComboFlags ComboFlags { get; init; } = ComboFlags.NoArrowButton;

    /// <summary> The value used when no filter is enabled. </summary>
    public TEnum AllFlags { get; init; }

    /// <summary> The different flag values to filter for and their display labels. </summary>
    public abstract IReadOnlyList<(TEnum Value, StringU8 Name)> EnumData { get; }

    /// <summary> Get the current filter value. </summary>
    public abstract TEnum FilterValue { get; protected set; }

    /// <summary> Get the set of flags for a row. </summary>
    /// <param name="item"> The row to check. </param>
    /// <param name="globalIndex"> The global index of the row. </param>
    /// <returns> The metadata flags for this row. </returns>
    public abstract TEnum GetValue(in TCacheItem item, int globalIndex);

    /// <inheritdoc/>
    public virtual bool WouldBeVisible(in TCacheItem item, int globalIndex)
        => FilterValue.HasFlag(GetValue(item, globalIndex));

    /// <inheritdoc/>
    public event Action? FilterChanged;

    /// <summary> Draw a filter that expands a combo of multiple checkboxes on click. </summary>
    /// <inheritdoc/>
    public virtual bool DrawFilter(ReadOnlySpan<byte> label, Vector2 availableRegion)
    {
        using var id    = Im.Id.Push("##Filter"u8);
        using var combo = BeginCombo(label, availableRegion, out var changes);
        if (!combo)
            return changes;

        for (var i = 0; i < EnumData.Count; ++i)
            changes |= DrawCheckbox(i);

        if (changes)
            InvokeEvent();

        return changes;
    }

    /// <summary> Begin a combo using the available width, also handling the right-click to reset filters. </summary>
    /// <param name="label"> The text used as the preview text for the combo. </param>
    /// <param name="availableRegion"> The available content region, of which the width is used. </param>
    /// <param name="changes"> Whether any changes to the filter were applied. </param>
    /// <returns> The combo disposable. </returns>
    [MethodImpl(ImSharpConfiguration.Inl)]
    protected Im.ComboDisposable BeginCombo(ReadOnlySpan<byte> label, Vector2 availableRegion, out bool changes)
    {
        var       all   = FilterValue.HasFlag(AllFlags);
        using var style = ImStyleSingle.FrameRounding.Push(0);
        Im.Item.SetNextWidth(availableRegion.X);
        var color = ImGuiColor.FrameBackground.Push(ImEx.Table.ActiveFilterColor, !all);
        var combo = Im.Combo.Begin(""u8, label, ComboFlags);
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
    protected void InvokeEvent()
        => FilterChanged?.Invoke();

    /// <summary> Set the flags in <paramref name="flags"/> to <paramref name="value"/>. </summary>
    /// <param name="flags"> The flags to toggle. </param>
    /// <param name="value"> Whether to turn the flags on or off. </param>
    /// <returns> True if the filter value changed. </returns>
    /// <exception cref="InvalidOperationException"> Only thrown for non-standard enum types. </exception>
    protected virtual unsafe bool SetValue(TEnum flags, bool value)
    {
        var newValue = sizeof(TEnum) switch
        {
            1 => Convert<byte>(FilterValue, flags, value),
            2 => Convert<ushort>(FilterValue, flags, value),
            4 => Convert<uint>(FilterValue, flags, value),
            8 => Convert<ulong>(FilterValue, flags, value),
            _ => throw new InvalidOperationException(
                $"Can not set value of type {typeof(TEnum).Name} since its size is {sizeof(TEnum)}. Must be 1, 2, 4 or 8."),
        };
        if (newValue.Equals(FilterValue))
            return false;

        FilterValue = newValue;
        return true;
    }

    /// <summary> Apply bitwise operators on the enum. </summary>
    [MethodImpl(ImSharpConfiguration.OptInl)]
    protected internal static unsafe TEnum Convert<T>(TEnum lhs, TEnum rhs, bool on) where T : unmanaged, IBitwiseOperators<T, T, T>
    {
        if (on)
        {
            var ret = *(T*)&lhs | *(T*)&rhs;
            return *(TEnum*)&ret;
        }
        else
        {
            var ret = *(T*)&lhs & ~(*(T*)&rhs);
            return *(TEnum*)&ret;
        }
    }
}
