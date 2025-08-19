namespace ImSharp.Table;

/// <summary> Shared base data for <see cref="NumberColumn{TNumber,TCacheItem}"/>. </summary>
public static class NumberColumn
{
    /// <summary> The method used to filter numbers against the filter value. </summary>
    public enum FilterMethod : byte
    {
        /// <summary> Only accept numbers with the same value. </summary>
        Equal = 0,

        /// <summary> Only accept numbers with less or equal value. </summary>
        LessEqual = 1,

        /// <summary> Only accept numbers with greater or equal value. </summary>
        GreaterEqual = 2,

        /// <summary> Do not compare numerical values, only text. </summary>
        TextOnly = 3,
    };
}

/// <summary> A basic column to display and filter integers. </summary>
/// <typeparam name="TNumber"> The type of number to use, mainly for choosing between integrals and floating points. </typeparam>
/// <typeparam name="TCacheItem"> The type of the cached transformation of the items to display. </typeparam>
public abstract class NumberColumn<TNumber, TCacheItem> : TextColumn<TCacheItem>
    where TNumber : unmanaged, INumber<TNumber>
{
    /// <summary> The method used to filter numbers against the filter value. </summary>
    protected NumberColumn.FilterMethod Filter { get; init; }

    /// <summary> The filter value. </summary>
    protected TNumber? FilterNumber;

    /// <summary> Get the numerical value for this column from the item. </summary>
    /// <param name="item"> The row to fetch. </param>
    /// <returns> The numerical value. </returns>
    public abstract TNumber ToValue(in TCacheItem item);

    /// <summary> Compare using numerical values. </summary>
    public override int Compare(in TCacheItem lhs, in TCacheItem rhs)
        => ToValue(lhs).CompareTo(ToValue(rhs));

    /// <summary> Get the number to display for this column as a sized string to draw it right-aligned from the item. </summary>
    /// <param name="item"> The row to fetch. </param>
    /// <returns> The textual representation of the numerical value. </returns>
    protected abstract SizedString DisplayNumber(in TCacheItem item);

    /// <summary> Also try to parse the input into a number. </summary>
    public override bool UpdateFilter(string newValue)
    {
        if (!base.UpdateFilter(newValue))
            return false;

        if (TNumber.TryParse(FilterValue, NumberStyles.Any, null, out var number))
        {
            FilterNumber = number;
            FilterRegex  = null;
        }
        else
        {
            FilterNumber = null;
        }

        return true;
    }

    /// <summary> Use the given filter method on the numerical value or text. </summary>
    public override bool FilterFunc(in TCacheItem item)
    {
        if (!FilterNumber.HasValue || Filter is NumberColumn.FilterMethod.TextOnly)
            return base.FilterFunc(item);

        var value = ToValue(item);
        return Filter switch
        {
            NumberColumn.FilterMethod.Equal        => value == FilterNumber.Value,
            NumberColumn.FilterMethod.LessEqual    => value <= FilterNumber.Value,
            NumberColumn.FilterMethod.GreaterEqual => value >= FilterNumber.Value,
            _                                      => true,
        };
    }

    /// <summary> Draw the display number aligned to the right. </summary>
    public override void DrawColumn(in TCacheItem item, int globalIndex)
    {
        ImEx.TextRightAligned(DisplayNumber(item));
        if (Im.Item.Hovered(HoveredFlags.AllowWhenDisabled))
            DrawTooltip(item, globalIndex);
    }

    /// <summary> Unused. </summary>
    protected sealed override string ComparisonText(in TCacheItem item)
        => throw new NotImplementedException();

    /// <summary> Unused. </summary>
    protected sealed override StringU8 DisplayText(in TCacheItem item)
        => throw new NotImplementedException();
}
