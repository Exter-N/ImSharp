namespace ImSharp;

public interface ITableColumn<TCacheItem>
{
    public ReadOnlySpan<byte> Label { get; }
    public float              Width { get; }
    public TableColumnFlags   Flags { get; }

    public int  Compare(in TCacheItem lhs, in TCacheItem rhs);
    public bool DrawFilter();
    public void DrawColumn(TCacheItem item, int itemIndex);
    public void PreSort();
    public void PostSort();

    public bool FilterFunc(TCacheItem item);

    public int CompareInverse(in TCacheItem lhs, in TCacheItem rhs)
        => Compare(rhs, lhs);
}
