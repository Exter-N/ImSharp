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

    /// <summary> The label to display for the tri-state checkbox filter. </summary>
    public StringU8 FilterLabel { get; init; } = new("Enabled"u8);

    /// <summary> Create a new YesNoColumn. </summary>
    protected YesNoColumn()
        => Filter = new TriStateFlagFilter(this)
        {
            AllFlags = YesNoColumn.YesNoFlag.Yes | YesNoColumn.YesNoFlag.No,
        };

    /// <summary> The width can always be given by twice the frame height. </summary>
    public override float ComputeWidth(IEnumerable<TCacheItem> allItems)
        => 2 * Im.Style.FrameHeight;

    /// <inheritdoc/>
    public override int Compare(in TCacheItem lhs, int lhsGlobalIndex, in TCacheItem rhs, int rhsGlobalIndex)
        => GetValue(lhs, lhsGlobalIndex, 0).CompareTo(GetValue(rhs, rhsGlobalIndex, 0));

    /// <summary> Draw a centered checkmark or an x and the tooltip. </summary>
    public override void DrawColumn(in TCacheItem item, int globalIndex)
    {
        var iconSize = Im.Style.TextHeight;
        Im.Cursor.X += Im.ContentRegion.Available.X - iconSize;
        if (GetValue(item, globalIndex, 0))
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

    protected class YesNoFilter(TriStateFlagColumn<YesNoColumn.YesNoFlag, TCacheItem> parent) : TriStateFlagFilter(parent)
    {
        /// <inheritdoc/>
        protected override bool SetValue(YesNoColumn.YesNoFlag onValue, YesNoColumn.YesNoFlag offValue, bool? enable)
        {
            var newFilter = enable switch
            {
                null  => AllFlags,
                true  => YesNoColumn.YesNoFlag.Yes,
                false => YesNoColumn.YesNoFlag.No,
            };
            if (FilterValue == newFilter)
                return false;

            FilterValue = newFilter;
            return true;
        }
    }
}
