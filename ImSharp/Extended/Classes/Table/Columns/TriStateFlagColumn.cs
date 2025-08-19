namespace ImSharp.Table;

/// <summary> A table column representing a check on multiple pairs of flags that represent off- and on-states for some values. </summary>
/// <typeparam name="TEnum"> The type of the enum that defines the flags. </typeparam>
/// <typeparam name="TCacheItem"> The type of the cached transformation of the items to display. </typeparam>
public abstract class TriStateFlagColumn<TEnum, TCacheItem> : FlagColumn<TEnum, TCacheItem>
    where TEnum : struct, Enum
{
    /// <summary> Pairs of flags that represent the on and off state for a specific value, and the name to display next to their tri-state checkbox. </summary>
    protected abstract IReadOnlyList<(TEnum On, TEnum Off, StringU8 Name)> TriEnumData { get; }

    /// <summary> Set a value according to the optional bool. </summary>
    /// <param name="onValue"> The flag representing the on state. </param>
    /// <param name="offValue"> The flag representing the off state. </param>
    /// <param name="enable">
    ///   The value to use for setting.
    ///   If true, <paramref name="onValue"/> should be set and <paramref name="offValue"/> should be unset, conversely for false.
    ///   If null, both should be set. </param>
    /// <returns> True if the filter value has changed. </returns>
    protected abstract bool SetValue(TEnum onValue, TEnum offValue, bool? enable);

    /// <summary> Draw a filter that expands a combo of multiple tri-state checkboxes on click. </summary>
    /// <inheritdoc/>
    public override bool DrawFilter(float arrowWidth)
    {
        using var id    = Im.Id.Push("##Filter"u8);
        using var combo = BeginCombo(arrowWidth, out var changes);
        if (!combo)
            return changes;

        for (var i = 0; i < TriEnumData.Count; ++i)
            changes |= DrawCheckbox(i);

        if (changes)
            InvokeEvent();

        return changes;
    }

    /// <summary> Draw a tri-state checkbox for the given pair of flags. </summary>
    /// <inheritdoc/>
    protected override bool DrawCheckbox(int idx)
    {
        var (on, off, name) = TriEnumData[idx];
        bool? current = FilterValue.HasFlag(on)
            ? FilterValue.HasFlag(off)
                ? null
                : true
            : false;

        return TriStateCheckbox.Instance.Draw(name, current, out var tmp) && SetValue(on, off, tmp);
    }

    /// <summary> Unused. </summary>
    protected sealed override IReadOnlyList<(TEnum Value, StringU8 Name)> EnumData
        => throw new NotImplementedException();
}
