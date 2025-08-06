namespace ImSharp;

public static partial class Im
{
    /// <summary> A wrapper class around Combo functions. </summary>
    public static unsafe class Combo
    {
        /// <inheritdoc cref="ComboDisposable(ref Utf8LabelHandler,ref Utf8TextHandler,ComboFlags)"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static ComboDisposable Begin(Utf8LabelHandler label, Utf8TextHandler preview, ComboFlags flags = ComboFlags.None)
            => new(ref label, ref preview, flags);

        /// <summary> Draw a simple combo of items. </summary>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentIndex"> The index of the currently selected item used for the preview string and highlighting the item. </param>
        /// <param name="itemsSeparatedByZeros"> The list of item strings separated by '\0' bytes and terminated by a doubled '\0' byte. </param>
        /// <param name="popupMaxHeight"> The maximum height of the combo popup in items. If non-positive, this is automatically calculated. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentIndex"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool Draw(Utf8LabelHandler label, ref int currentIndex, Utf8TextHandler itemsSeparatedByZeros, int popupMaxHeight = -1)
            => Native.Methods.Widgets.Combo(label.Start(), (int*)Unsafe.AsPointer(ref currentIndex), itemsSeparatedByZeros.Start(),
                popupMaxHeight);

        /// <summary> Draw a simple combo of items. </summary>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentIndex"> The index of the currently selected item used for the preview string and highlighting the item. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <param name="items"> The list of items to display as UTF8 strings. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentIndex"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool Draw(Utf8LabelHandler label, ref int currentIndex, ComboFlags flags, params IReadOnlyList<StringU8> items)
            => Draw(ref label, ref currentIndex, flags, items);

        /// <inheritdoc cref="Draw(Utf8LabelHandler,ref int,ComboFlags,IReadOnlyList{StringU8})"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool Draw(Utf8LabelHandler label, ref int currentIndex, params IReadOnlyList<StringU8> items)
            => Draw(ref label, ref currentIndex, ComboFlags.None, items);

        /// <summary> Draw a simple combo of items. </summary>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentIndex"> The currently selected item used for the preview string and highlighting the item. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <param name="items"> The list of items to display as UTF16 strings. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentIndex"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool Draw(Utf8LabelHandler label, ref int currentIndex, ComboFlags flags, params IReadOnlyList<string> items)
            => Draw(ref label, ref currentIndex, flags, items);

        /// <inheritdoc cref="Draw(Utf8LabelHandler,ref int,ComboFlags,IReadOnlyList{string})"/>
        [MethodImpl(ImSharpConfiguration.OptInl)]
        public static bool Draw(Utf8LabelHandler label, ref int currentIndex, params IReadOnlyList<string> items)
            => Draw(ref label, ref currentIndex, ComboFlags.None, items);

        /// <summary> Draw a combo over all valid entries for an Enum type using the enums <seealso cref="Enum.ToString()"/> for names. </summary>
        /// <typeparam name="T"> The Enum type. </typeparam>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentValue"> The currently selected value used for the preview string and highlighting the item. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentValue"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawEnum<T>(Utf8LabelHandler label, ref T currentValue, ComboFlags flags = ComboFlags.None) where T : struct, Enum
        {
            using var combo = new ComboDisposable(ref label, $"{currentValue}", flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in Enum.GetValues<T>())
            {
                var equal = value.Equals(currentValue);
                if (Selectable($"{value}", equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <summary> Draw a combo over all valid entries for an Enum type using a custom string function for names. </summary>
        /// <typeparam name="T"> The Enum type. </typeparam>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentValue"> The currently selected value used for the preview string and highlighting the item. </param>
        /// <param name="toName"> The method converting the enum value to a UTF8 string which has to be null-terminated. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentValue"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawEnum<T>(Utf8LabelHandler label, ref T currentValue, Func<T, ReadOnlySpan<byte>> toName,
            ComboFlags flags = ComboFlags.None) where T : struct, Enum
        {
            var       handler = (Utf8TextHandler)toName(currentValue);
            using var combo   = new ComboDisposable(ref label, ref handler, flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in Enum.GetValues<T>())
            {
                var equal = value.Equals(currentValue);
                if (Selectable(toName(value), equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <summary> Draw a combo over all valid entries for an Enum type using a custom string function for names. </summary>
        /// <typeparam name="T"> The Enum type. </typeparam>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentValue"> The currently selected value used for the preview string and highlighting the item. </param>
        /// <param name="toName"> The method converting the enum value to a UTF8 string. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentValue"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawEnum<T>(Utf8LabelHandler label, ref T currentValue, Func<T, StringU8> toName,
            ComboFlags flags = ComboFlags.None) where T : struct, Enum
        {
            var       handler = (Utf8TextHandler)toName(currentValue);
            using var combo   = new ComboDisposable(ref label, ref handler, flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in Enum.GetValues<T>())
            {
                var equal = value.Equals(currentValue);
                if (Selectable(toName(value), equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <summary> Draw a combo over all valid entries for an Enum type using a custom string function for names. </summary>
        /// <typeparam name="T"> The Enum type. </typeparam>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentValue"> The currently selected value used for the preview string and highlighting the item. </param>
        /// <param name="toName"> The method converting the enum value to a UTF16 string. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentValue"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawEnum<T>(Utf8LabelHandler label, ref T currentValue, Func<T, ReadOnlySpan<char>> toName,
            ComboFlags flags = ComboFlags.None) where T : struct, Enum
        {
            var       handler = (Utf8TextHandler)toName(currentValue);
            using var combo   = new ComboDisposable(ref label, ref handler, flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in Enum.GetValues<T>())
            {
                var equal = value.Equals(currentValue);
                if (Selectable(toName(value), equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <inheritdoc cref="DrawEnum{T}(Utf8LabelHandler,ref T,Func{T,ReadOnlySpan{char}},ComboFlags)"/>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawEnum<T>(Utf8LabelHandler label, ref T currentValue, Func<T, string> toName,
            ComboFlags flags = ComboFlags.None) where T : struct, Enum
        {
            var       handler = (Utf8TextHandler)toName(currentValue);
            using var combo   = new ComboDisposable(ref label, ref handler, flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in Enum.GetValues<T>())
            {
                var equal = value.Equals(currentValue);
                if (Selectable(toName(value), equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <summary> Draw a combo over the given items using <seealso cref="object.ToString()"/> for names. </summary>
        /// <typeparam name="T"> The item type. </typeparam>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentValue"> The currently selected value used for the preview string and highlighting the item. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <param name="items"> The list of items to draw as selectables. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentValue"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawItems<T>(Utf8LabelHandler label, ref T currentValue, ComboFlags flags, params IEnumerable<T> items)
            where T : IEquatable<T>
        {
            using var combo = new ComboDisposable(ref label, $"{currentValue}", flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in items)
            {
                var equal = value.Equals(currentValue);
                if (Selectable($"{value}", equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <inheritdoc cref="DrawItems{T}(Utf8LabelHandler,ref T,ComboFlags,IEnumerable{T})"/>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawItems<T>(Utf8LabelHandler label, ref T currentValue, params IEnumerable<T> items) where T : IEquatable<T>
        {
            using var combo = new ComboDisposable(ref label, $"{currentValue}", ComboFlags.None);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in items)
            {
                var equal = value.Equals(currentValue);
                if (Selectable($"{value}", equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <summary> Draw a combo over the given items using a custom string function for names. </summary>
        /// <typeparam name="T"> The item type. </typeparam>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentValue"> The currently selected value used for the preview string and highlighting the item. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <param name="toName"> The method converting the enum value to a UTF8 string which has to be null-terminated. </param>
        /// <param name="items"> The list of items to draw as selectables. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentValue"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawItems<T>(Utf8LabelHandler label, ref T currentValue, ComboFlags flags, Func<T, ReadOnlySpan<byte>> toName,
            params IEnumerable<T> items) where T : IEquatable<T>
        {
            var       handler = (Utf8TextHandler)toName(currentValue);
            using var combo   = new ComboDisposable(ref label, ref handler, flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in items)
            {
                var equal = value.Equals(currentValue);
                if (Selectable(toName(value), equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <summary> Draw a combo over the given items using a custom string function for names. </summary>
        /// <typeparam name="T"> The item type. </typeparam>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentValue"> The currently selected value used for the preview string and highlighting the item. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <param name="toName"> The method converting the enum value to a UTF8 string. </param>
        /// <param name="items"> The list of items to draw as selectables. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentValue"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawItems<T>(Utf8LabelHandler label, ref T currentValue, ComboFlags flags, Func<T, StringU8> toName,
            params IEnumerable<T> items) where T : IEquatable<T>
        {
            var       handler = (Utf8TextHandler)toName(currentValue);
            using var combo   = new ComboDisposable(ref label, ref handler, flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in items)
            {
                var equal = value.Equals(currentValue);
                if (Selectable(toName(value), equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <summary> Draw a combo over the given items using a custom string function for names. </summary>
        /// <typeparam name="T"> The item type. </typeparam>
        /// <param name="label"> The combo label as text. If this is a UTF8 string, it HAS to be null-terminated. </param>
        /// <param name="currentValue"> The currently selected value used for the preview string and highlighting the item. </param>
        /// <param name="flags"> Flags controlling the behavior of the combo. </param>
        /// <param name="toName"> The method converting the enum value to a UTF16 string. </param>
        /// <param name="items"> The list of items to draw as selectables. </param>
        /// <returns> True if a new item was selected in this frame, in which case <paramref name="currentValue"/> will have changed. </returns>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawItems<T>(Utf8LabelHandler label, ref T currentValue, ComboFlags flags, Func<T, ReadOnlySpan<char>> toName,
            params IEnumerable<T> items) where T : IEquatable<T>
        {
            var       handler = (Utf8TextHandler)toName(currentValue);
            using var combo   = new ComboDisposable(ref label, ref handler, flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in items)
            {
                var equal = value.Equals(currentValue);
                if (Selectable(toName(value), equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        /// <inheritdoc cref="DrawItems{T}(Utf8LabelHandler,ref T,ComboFlags,Func{T,ReadOnlySpan{char}},IEnumerable{T})"/>
        [MethodImpl(ImSharpConfiguration.Opt)]
        public static bool DrawItems<T>(Utf8LabelHandler label, ref T currentValue, ComboFlags flags, Func<T, string> toName,
            params IEnumerable<T> items) where T : IEquatable<T>
        {
            var       handler = (Utf8TextHandler)toName(currentValue);
            using var combo   = new ComboDisposable(ref label, ref handler, flags);
            if (!combo)
                return false;

            var ret = false;

            foreach (var value in items)
            {
                var equal = value.Equals(currentValue);
                if (Selectable(toName(value), equal) && !equal)
                {
                    currentValue = value;
                    ret          = true;
                }
            }

            return ret;
        }

        [MethodImpl(ImSharpConfiguration.Opt)]
        private static bool Draw(scoped ref Utf8LabelHandler label, ref int currentIndex, ComboFlags flags,
            params IReadOnlyList<StringU8> items)
        {
            var       currentItem = (Utf8TextHandler)items[currentIndex];
            using var combo       = new ComboDisposable(ref label, ref currentItem, flags);
            if (!combo)
                return false;

            var ret = false;
            for (var i = 0; i < items.Count; ++i)
            {
                if (Selectable(items[i], i == currentIndex) && i != currentIndex)
                {
                    currentIndex = i;
                    ret          = true;
                }
            }

            return ret;
        }

        [MethodImpl(ImSharpConfiguration.Opt)]
        private static bool Draw(ref Utf8LabelHandler label, ref int currentIndex, ComboFlags flags, params IReadOnlyList<string> items)
        {
            var       currentItem = (Utf8TextHandler)items[currentIndex];
            using var combo       = new ComboDisposable(ref label, ref currentItem, flags);
            if (!combo)
                return false;

            var ret = false;
            for (var i = 0; i < items.Count; ++i)
            {
                if (Selectable(items[i], i == currentIndex) && i != currentIndex)
                {
                    currentIndex = i;
                    ret          = true;
                }
            }

            return ret;
        }
    }
}
