namespace ImSharp.Table;

/// <summary> Shared base data for <see cref="YesNoColumn{TCacheItem}"/>. </summary>
public static class YesNoColumn
{
    /// <summary> A flag indicating yes or no for wrapping <see cref="TriStateFlagColumn{TEnum,TCacheItem}"/>. </summary>
    [Flags]
    public enum YesNoFlag
    {
        Yes = 0x01,
        No  = 0x02,
    };
}

/// <summary> A column that can display a checkmark or a cross and filter for both options. </summary>
/// <typeparam name="TCacheItem"> The type of the cached transformation of the items to display. </typeparam>
public abstract class YesNoColumn<TCacheItem> : TriStateFlagColumn<YesNoColumn.YesNoFlag, TCacheItem>
{
    /// <summary> The color to use for the checkmark representing true. </summary>
    protected virtual Rgba32 YesColor
        => ImGuiColor.CheckMark.Get();

    /// <summary> The color to use for the x representing false. </summary>
    protected virtual Rgba32 NoColor
        => ImGuiColor.CheckMark.Get();

    /// <summary> The current filter value. </summary>
    protected YesNoColumn.YesNoFlag Filter;

    /// <summary> The label to display for the tri-state checkbox filter. </summary>
    public StringU8 FilterLabel { get; init; } = new("Enabled"u8);

    /// <inheritdoc/>
    public override YesNoColumn.YesNoFlag FilterValue
        => Filter;

    /// <summary> Create a new YesNoColumn. </summary>
    protected YesNoColumn()
    {
        AllFlags = YesNoColumn.YesNoFlag.Yes | YesNoColumn.YesNoFlag.No;
        Filter   = AllFlags;
    }

    /// <summary> Set the current filter value according to <paramref name="enable"/>. The flag parameters are irrelevant. </summary>
    protected override bool SetValue(YesNoColumn.YesNoFlag _1, YesNoColumn.YesNoFlag _2, bool? enable)
    {
        var newFilter = enable switch
        {
            null  => AllFlags,
            true  => YesNoColumn.YesNoFlag.Yes,
            false => YesNoColumn.YesNoFlag.No,
        };
        if (Filter == newFilter)
            return false;

        Filter = newFilter;
        return true;
    }

    /// <inheritdoc/>
    protected override bool SetValue(YesNoColumn.YesNoFlag flags, bool enable)
    {
        var newFilter = Filter = enable ? Filter | flags : Filter & ~flags;
        if (Filter == newFilter)
            return false;

        Filter = newFilter;
        return true;
    }

    /// <summary>
    ///   Get the value of the row as a bool.
    ///   If it is true, it is only displayed if <see cref="YesNoColumn.YesNoFlag.Yes"/> is set, and displays a checkmark.
    ///   If it is false, it is only displayed if <see cref="YesNoColumn.YesNoFlag.No"/> is set, and displays an x.
    /// </summary>
    /// <param name="item"> The row to get the value of. </param>
    /// <returns> The value for this column of the row. </returns>
    protected abstract bool GetBoolValue(in TCacheItem item);

    /// <summary> The width can always be given by twice the frame height. </summary>
    public override float ComputeWidth(IEnumerable<TCacheItem> allItems)
        => 2 * Im.Style.FrameHeight;

    /// <inheritdoc/>
    public override bool FilterFunc(in TCacheItem item)
        => GetBoolValue(item)
            ? FilterValue.HasFlag(YesNoColumn.YesNoFlag.Yes)
            : FilterValue.HasFlag(YesNoColumn.YesNoFlag.No);

    /// <inheritdoc/>
    public override int Compare(in TCacheItem lhs, in TCacheItem rhs)
        => GetBoolValue(lhs).CompareTo(GetBoolValue(rhs));

    /// <summary> Draw a centered checkmark or an x and the tooltip. </summary>
    public override void DrawColumn(in TCacheItem item, int globalIndex)
    {
        var iconSize = Im.Style.TextHeight;
        Im.Cursor.X += Im.ContentRegion.Available.X - iconSize;
        if (GetBoolValue(item))
            Im.Render.Checkmark(Im.Window.DrawList, Im.Cursor.Position, YesColor, iconSize);
        else
            Im.Render.Cross(Im.Window.DrawList, Im.Cursor.Position, NoColor, iconSize);
        Im.Dummy(iconSize, iconSize);
        if (Im.Item.Hovered(HoveredFlags.AllowWhenDisabled))
            DrawTooltip(item, globalIndex);
    }

    /// <inheritdoc/>
    protected override IReadOnlyList<(YesNoColumn.YesNoFlag On, YesNoColumn.YesNoFlag Off, StringU8 Name)> TriEnumData
        => [(YesNoColumn.YesNoFlag.Yes, YesNoColumn.YesNoFlag.No, FilterLabel)];

    /// <summary> Unused. </summary>
    protected sealed override YesNoColumn.YesNoFlag GetValue(in TCacheItem item)
        => throw new NotImplementedException();
}
