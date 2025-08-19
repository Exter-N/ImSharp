namespace ImSharp.Table;

/// <summary> A basic column to display and filter text. </summary>
/// <typeparam name="TCacheItem"> The type of the cached transformation of the items to display. </typeparam>
public abstract class TextColumn<TCacheItem> : BasicColumn<TCacheItem>
{
    /// <remarks> Text columns should be resizable by default. </remarks>
    protected TextColumn()
        => Flags &= ~TableColumnFlags.NoResize;

    /// <summary> The current filter value of this column as a string. </summary>
    protected string FilterValue = string.Empty;

    /// <summary> If the filter can be parsed as a regular expression, this is set and compiled, and used for filtering. </summary>
    protected Regex? FilterRegex;

    /// <summary> Invoked whenever the filter changes. </summary>
    public event Action<string>? FilterChanged;

    /// <summary> Get the text used for comparison of items according to this column from the item. </summary>
    /// <param name="item"> The row to fetch. </param>
    /// <returns> Text that is used for comparing against other rows. </returns>
    protected abstract string ComparisonText(in TCacheItem item);

    /// <summary> Get the text displayed in the column from the item. </summary>
    /// <param name="item"> The row to fetch. </param>
    /// <returns> Text that is displayed. </returns>
    protected abstract StringU8 DisplayText(in TCacheItem item);

    /// <summary> Compares two rows according to the comparison text. </summary>
    public override int Compare(in TCacheItem lhs, in TCacheItem rhs)
        => string.Compare(ComparisonText(lhs), ComparisonText(rhs), StringComparison.Ordinal);

    /// <summary> Display the row using the display text. </summary>
    /// <inheritdoc/>
    public override void DrawColumn(in TCacheItem item, int globalIndex)
    {
        Im.Text(DisplayText(item));
        if (Im.Item.Hovered(HoveredFlags.AllowWhenDisabled))
            DrawTooltip(item, globalIndex);
    }

    /// <summary> A tooltip to draw when the text is hovered. This is only called when the text is hovered, but does not start a tooltip itself. </summary>
    /// <param name="item"> The drawn row. </param>
    /// <param name="globalIndex"> The global index of the drawn row. </param>
    protected virtual void DrawTooltip(in TCacheItem item, int globalIndex)
    { }

    /// <summary> Draw a textual input filter that uses the <see cref="BasicColumn{TCacheItem}.Label"/> as a hint and converts to regular expression if possible. </summary>
    /// <inheritdoc/>
    public override bool DrawFilter(float arrowWidth)
    {
        using var style = ImStyleSingle.FrameRounding.Push(0);

        Im.Item.SetNextWidth(-arrowWidth);
        var tmp = FilterValue;
        if (!Im.Input.Text("##Filter"u8, ref tmp, Label) || !UpdateFilter(tmp))
            return false;

        FilterChanged?.Invoke(FilterValue);
        return true;
    }

    /// <summary> Update the filter with a new text. </summary>
    public virtual bool UpdateFilter(string newValue)
    {
        if (newValue == FilterValue)
            return false;

        // Try to compile the regular expression and invoke the update event.
        FilterValue = newValue;
        try
        {
            FilterRegex = new Regex(FilterValue, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
        catch
        {
            FilterRegex = null;
        }

        return true;
    }

    /// <summary> Filter rows according to the regular expression, if set, otherwise on whether they contain the text. </summary>
    /// <inheritdoc/>
    public override bool FilterFunc(in TCacheItem item)
    {
        if (FilterValue.Length == 0)
            return true;

        var name = ComparisonText(item);
        return FilterRegex?.IsMatch(name) ?? name.Contains(FilterValue, StringComparison.OrdinalIgnoreCase);
    }
}
