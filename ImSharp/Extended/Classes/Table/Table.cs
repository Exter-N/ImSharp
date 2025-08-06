namespace ImSharp;

public static partial class ImEx
{

    public static class Table
    {
        public const float ArrowWidth = 10;
    }

    public class Table<TItem, TCacheItem>
    {
        protected class TableCache(Table<TItem, TCacheItem> table) : BasicCache
        {
            public readonly Table<TItem, TCacheItem> Table = table;

            public bool FilterDirty
            {
                get => field;
                set
                {
                    field = value;
                    if (value)
                        Dirty |= IManagedCache.DirtyFlags.CustomDirty;
                }
            }

            public bool SortDirty
            {
                get => field;
                set
                {
                    field = value;
                    if (value)
                        Dirty |= IManagedCache.DirtyFlags.CustomDirty;
                }
            }

            public int VisibleColumns;

            public List<TCacheItem> AllItems      = [];
            public List<int>        FilteredItems = [];

            public override void Update()
            {
                if (Dirty is IManagedCache.DirtyFlags.Clean)
                    return;


                UpdateFilter();
                SortInternal();
            }

            public bool WouldBeVisible(TCacheItem value)
                => Table.Headers.All(header => header.FilterFunc(value));

            protected virtual void UpdateFilter()
            {
                if (!FilterDirty)
                    return;

                FilteredItems.Clear();
                foreach (var (idx, item) in AllItems.Index())
                {
                    if (WouldBeVisible(item))
                        FilteredItems.Add(idx);
                }

                FilterDirty = false;
                SortDirty   = true;
            }

            protected virtual void SortInternal(ref Im.TableDisposable table)
            {
                if (!Table.Sortable)
                    return;

                var sortSpecs = table.SortSpecifications;
                SortDirty |= sortSpecs.Dirty;

                if (!SortDirty || sortSpecs.Count is 0)
                    return;

                var specs = sortSpecs[0];
                Table.SortIndex = specs.ColumnIndex;

                if (Table.Headers.Length <= Table.SortIndex)
                    Table.SortIndex = 0;

                var header = Table.Headers[Table.SortIndex];
                switch (specs.SortDirection)
                {
                    case SortDirection.Ascending:
                        header.PreSort();
                        FilteredItems.StableSort((a, b) => header.Compare(a.Item1, b.Item1));
                        header.PostSort();
                        break;
                    case SortDirection.Descending:
                        header.PreSort();
                        FilteredItems.StableSort((a, b) => header.CompareInverse(a.Item1, b.Item1));
                        header.PostSort();
                        break;
                    default: Table.SortIndex = -1; break;
                }

                SortDirty       = false;
                sortSpecs.Dirty = false;
            }
        }

        protected          StringU8                   Label { get; }
        protected readonly ITableColumn<TCacheItem>[] Headers;

        protected bool  FilterDirty = true;
        protected bool  SortDirty   = true;
        protected float ItemHeight  { get; set; }
        public    float ExtraHeight { get; set; } = 0;
        private   int   _currentIdx = 0;
        protected int   SortIndex   = -1;

        public bool Sortable
        {
            get => Flags.HasFlag(TableFlags.Sortable);
            protected set => Flags = value ? Flags | TableFlags.Sortable : Flags & ~TableFlags.Sortable;
        }

        public TableFlags Flags = TableFlags.RowBackground
          | TableFlags.Sortable
          | TableFlags.BordersOuter
          | TableFlags.ScrollY
          | TableFlags.ScrollX
          | TableFlags.PreciseWidths
          | TableFlags.SizingFixedFit
          | TableFlags.BordersInnerVertical
          | TableFlags.NoBordersInBodyUntilResize;

        public int TotalColumns
            => Headers.Length;

        public int VisibleColumns { get; private set; }

        public Table(StringU8 label, IReadOnlyCollection<TItem> items, params ITableColumn<TCacheItem>[] headers)
        {
            Label          = label;
            Headers        = headers;
            VisibleColumns = Headers.Length;
        }

        public void Draw(float itemHeight)
        {
            ItemHeight = itemHeight;
            using var idPush = Im.Id.Push(Label);
            DrawTableInternal();
        }

        protected virtual TableCache CreateCache()
            => new(this);



        protected virtual void DrawFilters()
            => throw new NotImplementedException();

        protected virtual void PreDraw()
        { }




        private void DrawItem((T, int) pair)
        {
            var       column = 0;
            using var id     = ImRaii.PushId(_currentIdx);
            _currentIdx = pair.Item2;
            foreach (var header in Headers)
            {
                id.Push(column++);
                if (ImGui.TableNextColumn())
                    header.DrawColumn(pair.Item1, pair.Item2);
                id.Pop();
            }
        }

        private void DrawTableInternal()
        {
            using var table = Im.Table.Begin("Table"u8, Headers.Length, Flags,
                Im.ContentRegion.Available - new Vector2(0, ExtraHeight * Im.Style.GlobalScale));
            if (!table)
                return;

            var tableId = Im.Id.Current;
            var cache   = CacheManager.Instance.GetOrCreateCache(tableId, CreateCache);
            PreDraw();
            ImGui.TableSetupScrollFreeze(1, 1);

            foreach (var header in Headers)
                ImGui.TableSetupColumn(header.Label, header.Flags, header.Width);

            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
            var i = 0;
            VisibleColumns = 0;
            foreach (var header in Headers)
            {
                using var id = ImRaii.PushId(i);
                if (ImGui.TableGetColumnFlags(i).HasFlag(ImGuiTableColumnFlags.IsEnabled))
                    ++VisibleColumns;
                if (!ImGui.TableSetColumnIndex(i++))
                    continue;

                using var style = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
                ImGui.TableHeader(string.Empty);
                ImGui.SameLine();
                style.Pop();
                if (header.DrawFilter())
                    FilterDirty = true;
            }

            SortInternal();
            _currentIdx = 0;
            ImGuiClip.ClippedDraw(FilteredItems, DrawItem, ItemHeight);
        }
    }
}
