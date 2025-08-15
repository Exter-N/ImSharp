namespace ImSharp;

public partial class Im
{
    /// <summary> A clipper utility that cleans up after itself. </summary>
    public unsafe struct ListClipper(Native.ListClipper* pointer) : IEnumerable<int>, IDisposable
    {
        /// <summary> The address of the native object. </summary>
        public Native.ListClipper* Pointer { get; private set; } = pointer;

        [MethodImpl(ImSharpConfiguration.Inl)]
        public static implicit operator ListClipper(Native.ListClipper* pointer)
            => new(pointer);

        /// <summary> The index of the first displayed element. </summary>
        public readonly ref int DisplayStart
        {
            [MethodImpl(ImSharpConfiguration.Inl)]
            get => ref Pointer->DisplayStart;
        }

        /// <summary> The index of the last displayed element. </summary>
        public readonly ref int DisplayEnd
        {
            [MethodImpl(ImSharpConfiguration.Inl)]
            get => ref Pointer->DisplayEnd;
        }

        /// <summary> The number of displayed items. </summary>
        public readonly ref int ItemsCount
        {
            [MethodImpl(ImSharpConfiguration.Inl)]
            get => ref Pointer->ItemsCount;
        }

        /// <summary> The total height of the items. </summary>
        public readonly ref float ItemsHeight
        {
            [MethodImpl(ImSharpConfiguration.Inl)]
            get => ref Pointer->ItemsHeight;
        }

        /// <summary> The height offset of the skipped height. </summary>
        public readonly ref float StartPosY
        {
            [MethodImpl(ImSharpConfiguration.Inl)]
            get => ref Pointer->StartPosY;
        }

        /// <summary> Create a new ListClipper. </summary>
        [MethodImpl(ImSharpConfiguration.Inl)]
        public ListClipper()
            : this(Native.ListClipper.Constructor())
        { }

        /// <summary> Create a new ListClipper and begin it with the given number of items and uniform height. </summary>
        /// <param name="itemsCount"> The number of items of uniform height that are in the list. </param>
        /// <param name="itemsHeight"> The uniform height of each item in the list. </param>
        [MethodImpl(ImSharpConfiguration.Inl)]
        public ListClipper(int itemsCount, float itemsHeight)
            : this(Native.ListClipper.Constructor())
            => Native.ListClipper.Begin(Pointer, itemsCount, itemsHeight);

        /// <summary> Execute a step in the clipper. </summary>
        [MethodImpl(ImSharpConfiguration.Inl)]
        public readonly bool Step()
            => Native.ListClipper.Step(Pointer);

        /// <summary> Force the current display range using a pair of indices. </summary>
        [MethodImpl(ImSharpConfiguration.Inl)]
        public readonly void ForceDisplayRangeByIndices(int itemMin, int itemMax)
            => Native.ListClipper.ForceDisplayRangeByIndices(Pointer, itemMin, itemMax);

        /// <summary> Dispose of the list clipper. </summary>
        [MethodImpl(ImSharpConfiguration.Inl)]
        public void Dispose()
        {
            if (Pointer is null)
                return;

            Native.ListClipper.End(Pointer);
            Native.ListClipper.Destructor(Pointer);
            Pointer = null;
        }

        /// <summary> Iterate over all indices to be drawn with the current ListClipper settings. </summary>
        [MethodImpl(ImSharpConfiguration.Inl)]
        public readonly IEnumerator<int> GetEnumerator()
        {
            while (Step())
            {
                for (var i = DisplayStart; i < DisplayEnd; ++i)
                    yield return i;
            }
        }

        /// <summary> Iterate over all items in the list to be drawn with the current ListClipper settings. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public readonly IEnumerable<T> Iterate<T>(IReadOnlyList<T> list)
        {
            while (Step())
            {
                for (var i = DisplayStart; i < DisplayEnd; ++i)
                    yield return list[i];
            }
        }

        /// <inheritdoc cref="Iterate{T}(IReadOnlyList{T})"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public readonly IEnumerable<T> Iterate<T>(IList<T> list)
        {
            while (Step())
            {
                for (var i = DisplayStart; i < DisplayEnd; ++i)
                    yield return list[i];
            }
        }

        /// <summary> Iterate over all items in the enumerable to be drawn with the current ListClipper settings. </summary>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public readonly IEnumerable<T> Iterate<T>(IEnumerable<T> list)
        {
            // Shortcut for random access.
            switch (list)
            {
                case IReadOnlyList<T> l:
                {
                    foreach (var i in Iterate(l))
                        yield return i;

                    break;
                }
                case IList<T> l2:
                {
                    foreach (var i in Iterate(l2))
                        yield return i;

                    break;
                }
            }


            using var enumerator = list.GetEnumerator();
            while (Step())
            {
                var currentIndex = 0;
                var skips        = DisplayStart - currentIndex;
                if (skips < 0)
                    continue;

                for (var i = 0; i < skips; ++i)
                {
                    if (!enumerator.MoveNext())
                        yield break;
                }

                currentIndex += skips;
                var takes = DisplayEnd - currentIndex;
                if (takes <= 0)
                    continue;

                for (var i = 0; i < takes; ++i)
                {
                    if (!enumerator.MoveNext())
                        yield break;

                    ++currentIndex;
                    yield return enumerator.Current;
                }
            }
        }

        [MethodImpl(ImSharpConfiguration.Inl)]
        readonly IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
